using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KillSwitchAPI.TinyTcp;

public class TinyTcpClient : IDisposable
{
	public Action<string> Logger = null;

	private readonly string _header = "[TinyTcp.Client] ";

	private TinyTcpClientSettings _settings = new TinyTcpClientSettings();

	private string _serverIp = null;

	private int _serverPort = 0;

	private readonly IPAddress _ipAddress = null;

	private TcpClient _client = null;

	private NetworkStream _networkStream = null;

	private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);

	private bool _isConnected = false;

	private Task _dataReceiver = null;

	private Task _idleServerMonitor = null;

	private Task _connectionMonitor = null;

	private CancellationTokenSource _tokenSource = new CancellationTokenSource();

	private CancellationToken _token;

	private DateTime _lastActivity = DateTime.Now;

	private bool _isTimeout = false;

	private bool exit = false;

	private static readonly object obj = new object();

	public bool IsConnected
	{
		get
		{
			return _isConnected;
		}
		set
		{
			_isConnected = value;
		}
	}

	public IPEndPoint LocalEndpoint
	{
		get
		{
			if (_client != null && _isConnected)
			{
				return (IPEndPoint)_client.Client.LocalEndPoint;
			}
			return null;
		}
	}

	public TinyTcpClientSettings Settings
	{
		get
		{
			return _settings;
		}
		set
		{
			if (value == null)
			{
				_settings = new TinyTcpClientSettings();
			}
			else
			{
				_settings = value;
			}
		}
	}

	public string ServerIpPort => $"{_serverIp}:{_serverPort}";

	public event EventHandler<ConnectionEventArgs> Connected;

	public event EventHandler<ConnectionEventArgs> Disconnected;

	public event EventHandler<DataReceivedEventArgs> DataReceived;

	public event EventHandler<DataSentEventArgs> DataSent;

	internal void OnConnected(object sender, ConnectionEventArgs args)
	{
		this.Connected?.Invoke(sender, args);
	}

	internal void OnDisconnected(object sender, ConnectionEventArgs args)
	{
		this.Disconnected?.Invoke(sender, args);
	}

	internal void OnDataReceived(object sender, DataReceivedEventArgs args)
	{
		this.DataReceived?.Invoke(sender, args);
	}

	internal void OnDataSent(object sender, DataSentEventArgs args)
	{
		this.DataSent?.Invoke(sender, args);
	}

	public TinyTcpClient(string ipPort)
	{
		if (string.IsNullOrEmpty(ipPort))
		{
			throw new ArgumentNullException("ipPort");
		}
		TinyTcpCommon.ParseIpPort(ipPort, out _serverIp, out _serverPort);
		if (_serverPort < 0)
		{
			throw new ArgumentException("Port must be zero or greater.");
		}
		if (string.IsNullOrEmpty(_serverIp))
		{
			throw new ArgumentNullException("Server IP or hostname must not be null.");
		}
		if (!IPAddress.TryParse(_serverIp, out _ipAddress))
		{
			_ipAddress = Dns.GetHostEntry(_serverIp).AddressList[0];
			_serverIp = _ipAddress.ToString();
		}
	}

	public TinyTcpClient(string serverIpOrHostname, int port)
	{
		if (string.IsNullOrEmpty(serverIpOrHostname))
		{
			throw new ArgumentNullException("serverIpOrHostname");
		}
		if (port < 0)
		{
			throw new ArgumentException("Port must be zero or greater.");
		}
		_serverIp = serverIpOrHostname;
		_serverPort = port;
		if (!IPAddress.TryParse(_serverIp, out _ipAddress))
		{
			_ipAddress = Dns.GetHostEntry(serverIpOrHostname).AddressList[0];
			_serverIp = _ipAddress.ToString();
		}
	}

	public TinyTcpClient(IPAddress serverIpAddress, int port)
		: this(new IPEndPoint(serverIpAddress, port))
	{
	}

	public TinyTcpClient(IPEndPoint serverIpEndPoint)
	{
		if (serverIpEndPoint == null)
		{
			throw new ArgumentNullException("serverIpEndPoint");
		}
		if (serverIpEndPoint.Port < 0)
		{
			throw new ArgumentException("Port must be zero or greater.");
		}
		_ipAddress = serverIpEndPoint.Address;
		_serverIp = serverIpEndPoint.Address.ToString();
		_serverPort = serverIpEndPoint.Port;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public bool Connect()
	{
		if (IsConnected)
		{
			Logger?.Invoke(_header + "already connected");
			return true;
		}
		_client = new TcpClient();
		_tokenSource = new CancellationTokenSource();
		_token = _tokenSource.Token;
		try
		{
			if (_connectionMonitor == null)
			{
				_connectionMonitor = Task.Run((Func<Task?>)ConnectedMonitor, _token);
			}
			_client.Connect(_serverIp, _serverPort);
			_networkStream = _client.GetStream();
			_networkStream.ReadTimeout = _settings.ReadTimeoutMs;
			_isConnected = true;
			_lastActivity = DateTime.Now;
			_isTimeout = false;
			OnConnected(this, new ConnectionEventArgs(ServerIpPort));
			_dataReceiver = Task.Run(() => DataReceiver(_token), _token);
			Logger?.Invoke(_header + " connected");
			return true;
		}
		catch (Exception ex)
		{
			Logger?.Invoke(_header + ex.ToString());
			return false;
		}
	}

	public void Disconnect()
	{
		if (!IsConnected)
		{
			Logger?.Invoke(_header + "already disconnected");
			return;
		}
		Logger?.Invoke(_header + "disconnecting from " + ServerIpPort);
		_tokenSource.Cancel();
		WaitCompletion();
		_client.Close();
		_isConnected = false;
	}

	public void Send(string data)
	{
		if (string.IsNullOrEmpty(data))
		{
			throw new ArgumentNullException("data");
		}
		if (!_isConnected)
		{
			throw new IOException("Not connected to the server; use Connect() first.");
		}
		byte[] bytes = Encoding.UTF8.GetBytes(data);
		Send(bytes);
	}

	public void Send(byte[] data)
	{
		if (data == null || data.Length < 1)
		{
			throw new ArgumentNullException("data");
		}
		if (!_isConnected)
		{
			throw new IOException("Not connected to the server; use Connect() first.");
		}
		using MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(data, 0, data.Length);
		memoryStream.Seek(0L, SeekOrigin.Begin);
		SendInternal(data.Length, memoryStream);
	}

	public void Send(long contentLength, Stream stream)
	{
		if (contentLength >= 1)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new InvalidOperationException("Cannot read from supplied stream.");
			}
			if (!_isConnected)
			{
				throw new IOException("Not connected to the server; use Connect() first.");
			}
			SendInternal(contentLength, stream);
		}
	}

	public async Task SendAsync(string data, CancellationToken token = default(CancellationToken))
	{
		if (string.IsNullOrEmpty(data))
		{
			throw new ArgumentNullException("data");
		}
		if (!_isConnected)
		{
			throw new IOException("Not connected to the server; use Connect() first.");
		}
		if (token == default(CancellationToken))
		{
			token = _token;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(data);
		using MemoryStream ms = new MemoryStream();
		await ms.WriteAsync(bytes, 0, bytes.Length, token).ConfigureAwait(continueOnCapturedContext: false);
		ms.Seek(0L, SeekOrigin.Begin);
		await SendInternalAsync(bytes.Length, ms, token).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task SendAsync(byte[] data, CancellationToken token = default(CancellationToken))
	{
		if (data == null || data.Length < 1)
		{
			throw new ArgumentNullException("data");
		}
		if (!_isConnected)
		{
			throw new IOException("Not connected to the server; use Connect() first.");
		}
		if (token == default(CancellationToken))
		{
			token = _token;
		}
		using MemoryStream ms = new MemoryStream();
		await ms.WriteAsync(data, 0, data.Length, token).ConfigureAwait(continueOnCapturedContext: false);
		ms.Seek(0L, SeekOrigin.Begin);
		await SendInternalAsync(data.Length, ms, token).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task SendAsync(long contentLength, Stream stream, CancellationToken token = default(CancellationToken))
	{
		if (contentLength >= 1)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new InvalidOperationException("Cannot read from supplied stream.");
			}
			if (!_isConnected)
			{
				throw new IOException("Not connected to the server; use Connect() first.");
			}
			if (token == default(CancellationToken))
			{
				token = _token;
			}
			await SendInternalAsync(contentLength, stream, token).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			exit = true;
			_isConnected = false;
			if (_tokenSource != null && !_tokenSource.IsCancellationRequested)
			{
				_tokenSource.Cancel();
				_tokenSource.Dispose();
			}
			if (_networkStream != null)
			{
				_networkStream.Close();
				_networkStream.Dispose();
			}
			if (_client != null)
			{
				_client.Close();
			}
			Logger?.Invoke(_header + "dispose complete");
		}
	}

	private async Task DataReceiver(CancellationToken token)
	{
		_ = _networkStream;
		while (!token.IsCancellationRequested && _client != null && _client.Connected)
		{
			try
			{
				await DataReadAsync(token).ContinueWith(async delegate(Task<ArraySegment<byte>> task)
				{
					if (task.IsCanceled)
					{
						return default(ArraySegment<byte>);
					}
					ArraySegment<byte> data = task.Result;
					if (data != default(ArraySegment<byte>))
					{
						_lastActivity = DateTime.Now;
						Action action = delegate
						{
							OnDataReceived(this, new DataReceivedEventArgs(ServerIpPort, data));
						};
						if (_settings.UseAsyncDataReceivedEvents)
						{
							Task.Run(action, token);
						}
						else
						{
							action();
						}
						return data;
					}
					await Task.Delay(100).ConfigureAwait(continueOnCapturedContext: false);
					return default(ArraySegment<byte>);
				}, token).ContinueWith(delegate
				{
				}).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (AggregateException)
			{
				Logger?.Invoke(_header + "data receiver canceled, disconnected");
				break;
			}
			catch (IOException)
			{
				Logger?.Invoke(_header + "data receiver canceled, disconnected");
				break;
			}
			catch (SocketException)
			{
				Logger?.Invoke(_header + "data receiver canceled, disconnected");
				break;
			}
			catch (TaskCanceledException)
			{
				Logger?.Invoke(_header + "data receiver task canceled, disconnected");
				break;
			}
			catch (OperationCanceledException)
			{
				Logger?.Invoke(_header + "data receiver operation canceled, disconnected");
				break;
			}
			catch (ObjectDisposedException)
			{
				Logger?.Invoke(_header + "data receiver canceled due to disposal, disconnected");
				break;
			}
			catch (Exception e)
			{
				Logger?.Invoke($"{_header}data receiver exception:{Environment.NewLine}{e}{Environment.NewLine}");
				break;
			}
		}
		Logger?.Invoke(_header + "disconnection detected");
		_isConnected = false;
		if (!_isTimeout)
		{
			OnDisconnected(this, new ConnectionEventArgs(ServerIpPort, DisconnectReason.Normal));
		}
		else
		{
			OnDisconnected(this, new ConnectionEventArgs(ServerIpPort, DisconnectReason.Timeout));
		}
	}

	private async Task<ArraySegment<byte>> DataReadAsync(CancellationToken token)
	{
		byte[] buffer = new byte[_settings.StreamBufferSize];
		try
		{
			int read = await _networkStream.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(continueOnCapturedContext: false);
			if (read > 0)
			{
				using (MemoryStream ms = new MemoryStream())
				{
					ms.Write(buffer, 0, read);
					return new ArraySegment<byte>(ms.GetBuffer(), 0, (int)ms.Length);
				}
			}
			return default(ArraySegment<byte>);
		}
		catch (IOException)
		{
			return default(ArraySegment<byte>);
		}
	}

	private void SendInternal(long contentLength, Stream stream)
	{
		byte[] array = new byte[_settings.StreamBufferSize];
		try
		{
			_sendLock.Wait();
			SendDataStream(contentLength, stream);
		}
		finally
		{
			_sendLock.Release();
		}
	}

	private void SendDataStream(long contentLength, Stream stream)
	{
		if (contentLength <= 0)
		{
			return;
		}
		long num = contentLength;
		int num2 = 0;
		byte[] array = new byte[_settings.StreamBufferSize];
		while (num > 0)
		{
			num2 = stream.Read(array, 0, array.Length);
			if (num2 > 0)
			{
				_networkStream.Write(array, 0, num2);
				num -= num2;
			}
		}
		_networkStream.Flush();
		OnDataSent(this, new DataSentEventArgs(ServerIpPort, contentLength));
	}

	private async Task SendInternalAsync(long contentLength, Stream stream, CancellationToken token)
	{
		try
		{
			long bytesRemaining = contentLength;
			byte[] buffer = new byte[_settings.StreamBufferSize];
			await _sendLock.WaitAsync(token).ConfigureAwait(continueOnCapturedContext: false);
			while (bytesRemaining > 0)
			{
				int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(continueOnCapturedContext: false);
				if (bytesRead > 0)
				{
					await _networkStream.WriteAsync(buffer, 0, bytesRead, token).ConfigureAwait(continueOnCapturedContext: false);
					bytesRemaining -= bytesRead;
				}
			}
			await _networkStream.FlushAsync(token).ConfigureAwait(continueOnCapturedContext: false);
			OnDataSent(this, new DataSentEventArgs(ServerIpPort, contentLength));
		}
		catch (TaskCanceledException)
		{
		}
		catch (OperationCanceledException)
		{
		}
		finally
		{
			_sendLock.Release();
		}
	}

	private void WaitCompletion()
	{
		try
		{
			_dataReceiver.Wait();
		}
		catch (AggregateException ex) when (ex.InnerException is TaskCanceledException)
		{
			Logger?.Invoke("Awaiting a canceled task");
		}
	}

	private async Task WaitCompletionAsync()
	{
		try
		{
			await _dataReceiver;
		}
		catch (TaskCanceledException)
		{
			Logger?.Invoke("Awaiting a canceled task");
		}
	}

	private async Task ConnectedMonitor()
	{
		while (!exit)
		{
			await Task.Delay(_settings.ConnectionLostEvaluationIntervalMs).ConfigureAwait(continueOnCapturedContext: false);
			lock (obj)
			{
				if (!PollSocket())
				{
					Logger?.Invoke(_header + "检测到断线 " + ServerIpPort + " ");
					Disconnect();
					Logger?.Invoke(_header + "重连 " + ServerIpPort + " ");
					Connect();
				}
			}
		}
	}

	private bool PollSocket()
	{
		try
		{
			if (_client.Client == null || !_client.Client.Connected)
			{
				return false;
			}
			if (!_client.Client.Poll(0, SelectMode.SelectRead))
			{
				return true;
			}
			byte[] buffer = new byte[1];
			return _client.Client.Receive(buffer, SocketFlags.Peek) != 0;
		}
		catch (SocketException ex)
		{
			Logger?.Invoke($"{_header}poll socket from {ServerIpPort} failed with ex = {ex}");
			return ex.SocketErrorCode == SocketError.TimedOut;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private byte[] SendAndWaitInternal(int timeoutMs, long contentLength, Stream stream)
	{
		if (!IsConnected)
		{
			throw new InvalidOperationException("Client is not connected to the server.");
		}
		if (contentLength > 0 && (stream == null || !stream.CanRead))
		{
			throw new ArgumentException("Cannot read from supplied stream.");
		}
		bool flag = false;
		if (_client == null || !_client.Connected)
		{
			flag = true;
			throw new InvalidOperationException("Client is not connected to the server.");
		}
		_sendLock.Wait();
		byte[] ret = null;
		AutoResetEvent responded = new AutoResetEvent(initialState: false);
		EventHandler<DataReceivedEventArgs> value = delegate(object sender, DataReceivedEventArgs e)
		{
			ret = Enumerable.ToArray(e.Data);
			responded.Set();
		};
		DataReceived += value;
		try
		{
			SendDataStream(contentLength, stream);
			Logger?.Invoke("synchronous request sent: ");
		}
		catch (TaskCanceledException)
		{
			return ret;
		}
		catch (OperationCanceledException)
		{
			return ret;
		}
		catch (Exception ex3)
		{
			Logger?.Invoke("failed to write message to " + ServerIpPort + ex3.Message);
			DataReceived -= value;
			flag = true;
			throw;
		}
		finally
		{
			_sendLock.Release();
			if (flag)
			{
				IsConnected = false;
			}
		}
		responded.WaitOne(new TimeSpan(0, 0, 0, 0, timeoutMs));
		DataReceived -= value;
		if (ret != null)
		{
			return ret;
		}
		Logger?.Invoke("synchronous response not received within the timeout window");
		throw new TimeoutException("A response to a synchronous request was not received within the timeout window.");
	}

	public byte[] SendAndWait(int timeoutMs, byte[] data)
	{
		if (timeoutMs < 1000)
		{
			throw new ArgumentException("Timeout milliseconds must be 1000 or greater.");
		}
		if (data == null)
		{
			data = new byte[0];
		}
		TinyTcpCommon.BytesToStream(data, 0, out int contentLength, out Stream stream);
		if (contentLength < 0)
		{
			throw new ArgumentException("Content length must be zero or greater.");
		}
		try
		{
			return SendAndWaitInternal(timeoutMs, contentLength, stream);
		}
		catch (Exception ex)
		{
			Logger?.Invoke("Failed to sendAndWait,Exception Message is [" + ex.Message + "]");
			return new byte[0];
		}
	}

	public string SendAndWait(string data, int timeoutMs = 5000)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(data);
		byte[] bytes2 = SendAndWait(timeoutMs, bytes);
		return Encoding.UTF8.GetString(bytes2);
	}

	public async Task<string> SendAndWaitAsync(string data, int timeoutMs = 5000)
	{
		string data2 = data;
		return await Task.Run(() => SendAndWait(data2, timeoutMs)).ConfigureAwait(continueOnCapturedContext: true);
	}
}
