using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KillSwitchAPI.TinyTcp;

public class TinyTcpServer : IDisposable
{
	public Action<string> Logger = null;

	private readonly string _header = "[TinyTcp.Server] ";

	private TinyTcpServerSettings _settings = new TinyTcpServerSettings();

	private TinyTcpKeepaliveSettings _keepalive = new TinyTcpKeepaliveSettings();

	private readonly string _listenerIp = null;

	private readonly IPAddress _ipAddress = null;

	private readonly int _port = 0;

	private readonly ConcurrentDictionary<string, ClientMetadata> _clients = new ConcurrentDictionary<string, ClientMetadata>();

	private readonly ConcurrentDictionary<string, DateTime> _clientsLastSeen = new ConcurrentDictionary<string, DateTime>();

	private readonly ConcurrentDictionary<string, DateTime> _clientsKicked = new ConcurrentDictionary<string, DateTime>();

	private readonly ConcurrentDictionary<string, DateTime> _clientsTimedout = new ConcurrentDictionary<string, DateTime>();

	private TcpListener _listener = null;

	private bool _isListening = false;

	private CancellationTokenSource _tokenSource = new CancellationTokenSource();

	private CancellationToken _token;

	private CancellationTokenSource _listenerTokenSource = new CancellationTokenSource();

	private CancellationToken _listenerToken;

	private Task _acceptConnections = null;

	private Task _idleClientMonitor = null;

	public bool IsListening => _isListening;

	public TinyTcpServerSettings Settings
	{
		get
		{
			return _settings;
		}
		set
		{
			if (value == null)
			{
				_settings = new TinyTcpServerSettings();
			}
			else
			{
				_settings = value;
			}
		}
	}

	public TinyTcpKeepaliveSettings Keepalive
	{
		get
		{
			return _keepalive;
		}
		set
		{
			if (value == null)
			{
				_keepalive = new TinyTcpKeepaliveSettings();
			}
			else
			{
				_keepalive = value;
			}
		}
	}

	public int ConnectionCounts => _clients.Count;

	public event EventHandler<ConnectionEventArgs> ClientConnected;

	public event EventHandler<ConnectionEventArgs> ClientDisconnected;

	public event EventHandler<DataReceivedEventArgs> DataReceived;

	public event EventHandler<DataSentEventArgs> DataSent;

	internal void OnClientConnected(object sender, ConnectionEventArgs args)
	{
		this.ClientConnected?.Invoke(sender, args);
	}

	internal void OnClientDisconnected(object sender, ConnectionEventArgs args)
	{
		this.ClientDisconnected?.Invoke(sender, args);
	}

	internal void OnDataReceived(object sender, DataReceivedEventArgs args)
	{
		this.DataReceived?.Invoke(sender, args);
	}

	internal void OnDataSent(object sender, DataSentEventArgs args)
	{
		this.DataSent?.Invoke(sender, args);
	}

	public TinyTcpServer(string ipPort)
	{
		if (string.IsNullOrEmpty(ipPort))
		{
			throw new ArgumentNullException("ipPort");
		}
		TinyTcpCommon.ParseIpPort(ipPort, out _listenerIp, out _port);
		if (_port < 0)
		{
			throw new ArgumentException("Port must be zero or greater.");
		}
		if (string.IsNullOrEmpty(_listenerIp))
		{
			_ipAddress = IPAddress.Loopback;
			_listenerIp = _ipAddress.ToString();
		}
		else if (_listenerIp == "*" || _listenerIp == "+")
		{
			_ipAddress = IPAddress.Any;
		}
		else if (!IPAddress.TryParse(_listenerIp, out _ipAddress))
		{
			_ipAddress = Dns.GetHostEntry(_listenerIp).AddressList[0];
			_listenerIp = _ipAddress.ToString();
		}
		_isListening = false;
		_token = _tokenSource.Token;
	}

	public TinyTcpServer(string listenerIp, int port)
	{
		if (port < 0)
		{
			throw new ArgumentException("Port must be zero or greater.");
		}
		_listenerIp = listenerIp;
		_port = port;
		if (string.IsNullOrEmpty(_listenerIp))
		{
			_ipAddress = IPAddress.Loopback;
			_listenerIp = _ipAddress.ToString();
		}
		else if (_listenerIp == "*" || _listenerIp == "+")
		{
			_ipAddress = IPAddress.Any;
			_listenerIp = listenerIp;
		}
		else if (!IPAddress.TryParse(_listenerIp, out _ipAddress))
		{
			_ipAddress = Dns.GetHostEntry(listenerIp).AddressList[0];
			_listenerIp = _ipAddress.ToString();
		}
		_isListening = false;
		_token = _tokenSource.Token;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	public void Start()
	{
		if (_isListening)
		{
			throw new InvalidOperationException("TinyTcpServer is already running.");
		}
		_listener = new TcpListener(_ipAddress, _port);
		_listener.Server.NoDelay = _settings.NoDelay;
		_listener.Start();
		_isListening = true;
		_tokenSource = new CancellationTokenSource();
		_token = _tokenSource.Token;
		_listenerTokenSource = new CancellationTokenSource();
		_listenerToken = _listenerTokenSource.Token;
		if (_idleClientMonitor == null)
		{
			_idleClientMonitor = Task.Run(() => IdleClientMonitor(), _token);
		}
		_acceptConnections = Task.Run(() => AcceptConnections(), _listenerToken);
	}

	public Task StartAsync()
	{
		if (_isListening)
		{
			throw new InvalidOperationException("SimpleTcpServer is already running.");
		}
		_listener = new TcpListener(_ipAddress, _port);
		if (_keepalive.EnableTcpKeepAlives)
		{
			EnableKeepalives();
		}
		_listener.Start();
		_isListening = true;
		_tokenSource = new CancellationTokenSource();
		_token = _tokenSource.Token;
		_listenerTokenSource = new CancellationTokenSource();
		_listenerToken = _listenerTokenSource.Token;
		if (_idleClientMonitor == null)
		{
			_idleClientMonitor = Task.Run(() => IdleClientMonitor(), _token);
		}
		_acceptConnections = Task.Run(() => AcceptConnections(), _listenerToken);
		return _acceptConnections;
	}

	public void Stop()
	{
		if (!_isListening)
		{
			throw new InvalidOperationException("SimpleTcpServer is not running.");
		}
		_isListening = false;
		_listener.Stop();
		_listenerTokenSource.Cancel();
		_acceptConnections.Wait();
		_acceptConnections = null;
		Logger?.Invoke(_header + "stopped");
	}

	public IEnumerable<string> GetClients()
	{
		return new List<string>(_clients.Keys);
	}

	public Dictionary<string, string> GetClientsInfo()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (KeyValuePair<string, ClientMetadata> client in _clients)
		{
			dictionary.Add(client.Key, client.Value.Name);
		}
		return dictionary;
	}

	public void SetClientName(string ipPort, string Name)
	{
		if (_clients.TryGetValue(ipPort, out ClientMetadata value))
		{
			value.Name = Name;
		}
	}

	public bool IsConnected(string ipPort)
	{
		if (string.IsNullOrEmpty(ipPort))
		{
			throw new ArgumentNullException("ipPort");
		}
		ClientMetadata value;
		return _clients.TryGetValue(ipPort, out value);
	}

	public void SendByName(string Name, string data)
	{
		string Name2 = Name;
		if (_clients.Any<KeyValuePair<string, ClientMetadata>>((KeyValuePair<string, ClientMetadata> c) => c.Value.Name == Name2))
		{
			string key = _clients.First<KeyValuePair<string, ClientMetadata>>((KeyValuePair<string, ClientMetadata> c) => c.Value.Name == Name2).Key;
			Send(key, data);
		}
		else
		{
			SendToAllClient(data);
		}
	}

	public void Send(string ipPort, string data)
	{
		if (string.IsNullOrEmpty(ipPort))
		{
			throw new ArgumentNullException("ipPort");
		}
		if (string.IsNullOrEmpty(data))
		{
			throw new ArgumentNullException("data");
		}
		byte[] bytes = Encoding.UTF8.GetBytes(data);
		using MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(bytes, 0, bytes.Length);
		memoryStream.Seek(0L, SeekOrigin.Begin);
		SendInternal(ipPort, bytes.Length, memoryStream);
	}

	public void Send(string ipPort, byte[] data)
	{
		if (string.IsNullOrEmpty(ipPort))
		{
			throw new ArgumentNullException("ipPort");
		}
		if (data == null || data.Length < 1)
		{
			throw new ArgumentNullException("data");
		}
		using MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(data, 0, data.Length);
		memoryStream.Seek(0L, SeekOrigin.Begin);
		SendInternal(ipPort, data.Length, memoryStream);
	}

	public void Send(string ipPort, long contentLength, Stream stream)
	{
		if (string.IsNullOrEmpty(ipPort))
		{
			throw new ArgumentNullException("ipPort");
		}
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
			SendInternal(ipPort, contentLength, stream);
		}
	}

	public void SendToAllClient(string data)
	{
		if (string.IsNullOrEmpty(data))
		{
			throw new ArgumentNullException("data");
		}
		Logger?.Invoke("SendToAllClient:" + data);
		foreach (KeyValuePair<string, ClientMetadata> client in _clients)
		{
			Send(client.Key, data);
		}
	}

	public async Task SendAsync(string ipPort, string data, CancellationToken token = default(CancellationToken))
	{
		if (string.IsNullOrEmpty(ipPort))
		{
			throw new ArgumentNullException("ipPort");
		}
		if (string.IsNullOrEmpty(data))
		{
			throw new ArgumentNullException("data");
		}
		if (token == default(CancellationToken))
		{
			token = _token;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(data);
		using MemoryStream ms = new MemoryStream();
		await ms.WriteAsync(bytes, 0, bytes.Length, token).ConfigureAwait(continueOnCapturedContext: false);
		ms.Seek(0L, SeekOrigin.Begin);
		await SendInternalAsync(ipPort, bytes.Length, ms, token).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task SendAsync(string ipPort, byte[] data, CancellationToken token = default(CancellationToken))
	{
		if (string.IsNullOrEmpty(ipPort))
		{
			throw new ArgumentNullException("ipPort");
		}
		if (data == null || data.Length < 1)
		{
			throw new ArgumentNullException("data");
		}
		if (token == default(CancellationToken))
		{
			token = _token;
		}
		using MemoryStream ms = new MemoryStream();
		await ms.WriteAsync(data, 0, data.Length, token).ConfigureAwait(continueOnCapturedContext: false);
		ms.Seek(0L, SeekOrigin.Begin);
		await SendInternalAsync(ipPort, data.Length, ms, token).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task SendAsync(string ipPort, long contentLength, Stream stream, CancellationToken token = default(CancellationToken))
	{
		if (string.IsNullOrEmpty(ipPort))
		{
			throw new ArgumentNullException("ipPort");
		}
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
			if (token == default(CancellationToken))
			{
				token = _token;
			}
			await SendInternalAsync(ipPort, contentLength, stream, token).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	public void DisconnectClient(string ipPort)
	{
		if (string.IsNullOrEmpty(ipPort))
		{
			throw new ArgumentNullException("ipPort");
		}
		if (!_clients.TryGetValue(ipPort, out ClientMetadata value))
		{
			Logger?.Invoke(_header + "unable to find client: " + ipPort);
		}
		else if (!_clientsTimedout.ContainsKey(ipPort))
		{
			Logger?.Invoke(_header + "kicking: " + ipPort);
			_clientsKicked.TryAdd(ipPort, DateTime.Now);
		}
		if (value != null)
		{
			if (!value.TokenSource.IsCancellationRequested)
			{
				value.TokenSource.Cancel();
				Logger?.Invoke(_header + "requesting disposal of: " + ipPort);
			}
			value.Dispose();
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposing)
		{
			return;
		}
		try
		{
			if (_clients != null && _clients.Count > 0)
			{
				foreach (KeyValuePair<string, ClientMetadata> client in _clients)
				{
					client.Value.Dispose();
					Logger?.Invoke(_header + "disconnected client: " + client.Key);
				}
			}
			if (_tokenSource != null)
			{
				if (!_tokenSource.IsCancellationRequested)
				{
					_tokenSource.Cancel();
				}
				_tokenSource.Dispose();
			}
			if (_listener != null && _listener.Server != null)
			{
				_listener.Server.Close();
				_listener.Server.Dispose();
			}
			if (_listener != null)
			{
				_listener.Stop();
			}
		}
		catch (Exception ex)
		{
			Logger?.Invoke($"{_header}dispose exception:{Environment.NewLine}{ex}{Environment.NewLine}");
		}
		_isListening = false;
		Logger?.Invoke(_header + "disposed");
	}

	private bool IsClientConnected(TcpClient client)
	{
		if (!client.Connected)
		{
			return false;
		}
		if (client.Client.Poll(0, SelectMode.SelectWrite) && !client.Client.Poll(0, SelectMode.SelectError))
		{
			byte[] buffer = new byte[1];
			if (client.Client.Receive(buffer, SocketFlags.Peek) == 0)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	private async Task AcceptConnections()
	{
		while (!_listenerToken.IsCancellationRequested)
		{
			ClientMetadata client = null;
			try
			{
				if (!_isListening && _clients.Count >= _settings.MaxConnections)
				{
					Task.Delay(100).Wait();
					continue;
				}
				if (!_isListening)
				{
					_listener.Start();
					_isListening = true;
				}
				TcpClient tcpClient = await _listener.AcceptTcpClientAsync().ConfigureAwait(continueOnCapturedContext: false);
				string clientIpPort = tcpClient.Client.RemoteEndPoint.ToString();
				string clientIp = null;
				int clientPort = 0;
				TinyTcpCommon.ParseIpPort(clientIpPort, out clientIp, out clientPort);
				if (_settings.PermittedIPs.Count > 0 && !_settings.PermittedIPs.Contains(clientIp))
				{
					Logger?.Invoke(_header + "rejecting connection from " + clientIp + " (not permitted)");
					tcpClient.Close();
					continue;
				}
				if (_settings.BlockedIPs.Count > 0 && _settings.BlockedIPs.Contains(clientIp))
				{
					Logger?.Invoke(_header + "rejecting connection from " + clientIp + " (blocked)");
					tcpClient.Close();
					continue;
				}
				client = new ClientMetadata(tcpClient);
				_clients.TryAdd(clientIpPort, client);
				_clientsLastSeen.TryAdd(clientIpPort, DateTime.Now);
				Logger?.Invoke(_header + "starting data receiver for: " + clientIpPort);
				OnClientConnected(this, new ConnectionEventArgs(clientIpPort));
				if (_keepalive.EnableTcpKeepAlives)
				{
					EnableKeepalives(tcpClient);
				}
				Task.Run(cancellationToken: CancellationTokenSource.CreateLinkedTokenSource(client.Token, _token).Token, function: () => DataReceiver(client));
				if (_clients.Count >= _settings.MaxConnections)
				{
					Logger?.Invoke($"{_header}maximum connections {_settings.MaxConnections} met (currently {_clients.Count} connections), pausing");
					_isListening = false;
					_listener.Stop();
				}
			}
			catch (Exception ex)
			{
				if (ex is TaskCanceledException || ex is OperationCanceledException || ex is ObjectDisposedException || ex is InvalidOperationException)
				{
					_isListening = false;
					if (client != null)
					{
						client.Dispose();
					}
					Logger?.Invoke(_header + "stopped listening");
					break;
				}
				if (client != null)
				{
					client.Dispose();
				}
				Logger?.Invoke($"{_header}exception while awaiting connections: {ex}");
			}
		}
		_isListening = false;
	}

	private async Task DataReceiver(ClientMetadata client)
	{
		string ipPort = client.IpPort;
		Logger?.Invoke(_header + "data receiver started for client " + ipPort);
		CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_token, client.Token);
		while (true)
		{
			try
			{
				if (!IsClientConnected(client.Client))
				{
					Logger?.Invoke(_header + "client " + ipPort + " disconnected");
					break;
				}
				if (client.Token.IsCancellationRequested)
				{
					Logger?.Invoke(_header + "cancellation requested (data receiver for client " + ipPort + ")");
					break;
				}
				ArraySegment<byte> data = await DataReadAsync(client, linkedCts.Token).ConfigureAwait(continueOnCapturedContext: false);
				Action action = delegate
				{
					OnDataReceived(this, new DataReceivedEventArgs(ipPort, data));
				};
				if (_settings.UseAsyncDataReceivedEvents)
				{
					Task.Run(action, linkedCts.Token);
				}
				else
				{
					action();
				}
				UpdateClientLastSeen(client.IpPort);
			}
			catch (IOException)
			{
				Logger?.Invoke(_header + "data receiver canceled, peer disconnected [" + ipPort + "]");
				break;
			}
			catch (SocketException)
			{
				Logger?.Invoke(_header + "data receiver canceled, peer disconnected [" + ipPort + "]");
				break;
			}
			catch (TaskCanceledException)
			{
				Logger?.Invoke(_header + "data receiver task canceled [" + ipPort + "]");
				break;
			}
			catch (ObjectDisposedException)
			{
				Logger?.Invoke(_header + "data receiver canceled due to disposal [" + ipPort + "]");
				break;
			}
			catch (Exception ex5)
			{
				Exception e = ex5;
				Logger?.Invoke($"{_header}data receiver exception [{ipPort}]:{Environment.NewLine}{e}{Environment.NewLine}");
				break;
			}
		}
		Logger?.Invoke(_header + "data receiver terminated for client " + ipPort);
		if (_clientsKicked.ContainsKey(ipPort))
		{
			OnClientDisconnected(this, new ConnectionEventArgs(ipPort, DisconnectReason.Kicked));
		}
		else if (_clientsTimedout.ContainsKey(client.IpPort))
		{
			OnClientDisconnected(this, new ConnectionEventArgs(ipPort, DisconnectReason.Timeout));
		}
		else
		{
			OnClientDisconnected(this, new ConnectionEventArgs(ipPort, DisconnectReason.Normal));
		}
		_clients.TryRemove(ipPort, out ClientMetadata _);
		_clientsLastSeen.TryRemove(ipPort, out var value2);
		_clientsKicked.TryRemove(ipPort, out value2);
		_clientsTimedout.TryRemove(ipPort, out value2);
		client?.Dispose();
	}

	private async Task<ArraySegment<byte>> DataReadAsync(ClientMetadata client, CancellationToken token)
	{
		byte[] buffer = new byte[_settings.StreamBufferSize];
		using MemoryStream ms = new MemoryStream();
		int read = await client.NetworkStream.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(continueOnCapturedContext: false);
		if (read > 0)
		{
			await ms.WriteAsync(buffer, 0, read, token).ConfigureAwait(continueOnCapturedContext: false);
			return new ArraySegment<byte>(ms.GetBuffer(), 0, (int)ms.Length);
		}
		throw new SocketException();
	}

	private async Task IdleClientMonitor()
	{
		while (!_token.IsCancellationRequested)
		{
			await Task.Delay(_settings.IdleClientEvaluationIntervalMs, _token).ConfigureAwait(continueOnCapturedContext: false);
			if (_settings.IdleClientTimeoutMs == 0)
			{
				continue;
			}
			try
			{
				DateTime idleTimestamp = DateTime.Now.AddMilliseconds(-1 * _settings.IdleClientTimeoutMs);
				foreach (KeyValuePair<string, DateTime> curr in _clientsLastSeen)
				{
					if (curr.Value < idleTimestamp)
					{
						_clientsTimedout.TryAdd(curr.Key, DateTime.Now);
						Logger?.Invoke(_header + "disconnecting " + curr.Key + " due to timeout");
						DisconnectClient(curr.Key);
					}
				}
			}
			catch (Exception ex)
			{
				Exception e = ex;
				Logger?.Invoke($"{_header}monitor exception: {e}");
			}
		}
	}

	private void UpdateClientLastSeen(string ipPort)
	{
		if (_clientsLastSeen.ContainsKey(ipPort))
		{
			_clientsLastSeen.TryRemove(ipPort, out var _);
		}
		_clientsLastSeen.TryAdd(ipPort, DateTime.Now);
	}

	private void SendInternal(string ipPort, long contentLength, Stream stream)
	{
		if (!_clients.TryGetValue(ipPort, out ClientMetadata value) || value == null)
		{
			return;
		}
		long num = contentLength;
		int num2 = 0;
		byte[] array = new byte[_settings.StreamBufferSize];
		try
		{
			value.SendLock.Wait();
			while (num > 0)
			{
				num2 = stream.Read(array, 0, array.Length);
				if (num2 > 0)
				{
					value.NetworkStream.Write(array, 0, num2);
					num -= num2;
				}
			}
			value.NetworkStream.Flush();
			OnDataSent(this, new DataSentEventArgs(ipPort, contentLength));
		}
		finally
		{
			value?.SendLock.Release();
		}
	}

	private async Task SendInternalAsync(string ipPort, long contentLength, Stream stream, CancellationToken token)
	{
		ClientMetadata client = null;
		try
		{
			if (!_clients.TryGetValue(ipPort, out client) || client == null)
			{
				return;
			}
			long bytesRemaining = contentLength;
			byte[] buffer = new byte[_settings.StreamBufferSize];
			await client.SendLock.WaitAsync(token).ConfigureAwait(continueOnCapturedContext: false);
			while (bytesRemaining > 0)
			{
				int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(continueOnCapturedContext: false);
				if (bytesRead > 0)
				{
					await client.NetworkStream.WriteAsync(buffer, 0, bytesRead, token).ConfigureAwait(continueOnCapturedContext: false);
					bytesRemaining -= bytesRead;
				}
			}
			await client.NetworkStream.FlushAsync(token).ConfigureAwait(continueOnCapturedContext: false);
			OnDataSent(this, new DataSentEventArgs(ipPort, contentLength));
		}
		catch (TaskCanceledException)
		{
		}
		catch (OperationCanceledException)
		{
		}
		finally
		{
			client?.SendLock.Release();
		}
	}

	private void EnableKeepalives()
	{
		try
		{
			byte[] array = new byte[12];
			Buffer.BlockCopy(BitConverter.GetBytes(1u), 0, array, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes((uint)_keepalive.TcpKeepAliveTimeMilliseconds), 0, array, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes((uint)_keepalive.TcpKeepAliveIntervalMilliseconds), 0, array, 8, 4);
			_listener.Server.IOControl(IOControlCode.KeepAliveValues, array, null);
		}
		catch (Exception)
		{
			Logger?.Invoke(_header + "keepalives not supported on this platform, disabled");
		}
	}

	private void EnableKeepalives(TcpClient client)
	{
		try
		{
			byte[] array = new byte[12];
			Buffer.BlockCopy(BitConverter.GetBytes(1u), 0, array, 0, 4);
			Buffer.BlockCopy(BitConverter.GetBytes((uint)_keepalive.TcpKeepAliveTimeMilliseconds), 0, array, 4, 4);
			Buffer.BlockCopy(BitConverter.GetBytes((uint)_keepalive.TcpKeepAliveIntervalMilliseconds), 0, array, 8, 4);
			client.Client.IOControl(IOControlCode.KeepAliveValues, array, null);
		}
		catch (Exception)
		{
			Logger?.Invoke(_header + "keepalives not supported on this platform, disabled");
			_keepalive.EnableTcpKeepAlives = false;
		}
	}

	public string SendAndWait(string clientName, string data, int timeoutMs = 5000)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(data);
		byte[] bytes2 = SendAndWait(clientName, timeoutMs, bytes);
		return Encoding.UTF8.GetString(bytes2);
	}

	public byte[] SendAndWait(string clientName, int timeoutMs, byte[] data)
	{
		string clientName2 = clientName;
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
			if (_clients.Any<KeyValuePair<string, ClientMetadata>>((KeyValuePair<string, ClientMetadata> c) => c.Value.Name == clientName2))
			{
				ClientMetadata value = _clients.First<KeyValuePair<string, ClientMetadata>>((KeyValuePair<string, ClientMetadata> c) => c.Value.Name == clientName2).Value;
				return SendAndWaitInternal(value, timeoutMs, contentLength, stream);
			}
			Logger?.Invoke("没有找到client[" + clientName2 + "]");
			return new byte[0];
		}
		catch (Exception ex)
		{
			Logger?.Invoke("Failed to sendAndWait,Exception Message is [" + ex.Message + "]");
			return new byte[0];
		}
	}

	private byte[] SendAndWaitInternal(ClientMetadata client, int timeoutMs, long contentLength, Stream stream)
	{
		if (client == null)
		{
			throw new ArgumentNullException("client");
		}
		if (contentLength > 0 && (stream == null || !stream.CanRead))
		{
			throw new ArgumentException("Cannot read from supplied stream.");
		}
		client.SendLock.Wait();
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
			SendDataStream(client, contentLength, stream);
		}
		catch (Exception)
		{
			DataReceived -= value;
			throw;
		}
		finally
		{
			client?.SendLock.Release();
		}
		responded.WaitOne(new TimeSpan(0, 0, 0, 0, timeoutMs));
		DataReceived -= value;
		if (ret != null)
		{
			return ret;
		}
		throw new TimeoutException("A response to a synchronous request was not received within the timeout window.");
	}

	private void SendDataStream(ClientMetadata client, long contentLength, Stream stream)
	{
		if (contentLength <= 0)
		{
			return;
		}
		long num = contentLength;
		int num2 = 0;
		byte[] array = new byte[_settings.StreamBufferSize];
		try
		{
			while (num > 0)
			{
				num2 = stream.Read(array, 0, array.Length);
				if (num2 > 0)
				{
					client.NetworkStream.Write(array, 0, num2);
					num -= num2;
				}
			}
			client.NetworkStream.Flush();
			OnDataSent(this, new DataSentEventArgs(client.IpPort, contentLength));
		}
		catch (Exception)
		{
		}
		client.NetworkStream.Flush();
	}
}
