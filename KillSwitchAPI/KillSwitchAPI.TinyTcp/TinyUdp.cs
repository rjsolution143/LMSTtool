using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KillSwitchAPI.TinyTcp;

public class TinyUdp
{
	private Socket udpSocket;

	private Thread threadReceive;

	private bool IsStarted;

	public Action<string> Logger = null;

	private EndPoint udpEndPoint;

	private readonly SemaphoreSlim _sendLock = new SemaphoreSlim(1, 1);

	public event EventHandler<DataReceivedEventArgs> DataReceived;

	internal void OnDataReceived(object sender, DataReceivedEventArgs args)
	{
		this.DataReceived?.Invoke(sender, args);
	}

	public TinyUdp(string ipPort)
	{
		if (string.IsNullOrEmpty(ipPort))
		{
			throw new ArgumentNullException("ipPort");
		}
		TinyTcpCommon.ParseIpPort(ipPort, out string ip, out int port);
		if (port < 0)
		{
			throw new ArgumentException("Port must be zero or greater.");
		}
		if (string.IsNullOrEmpty(ip))
		{
			throw new ArgumentNullException("Server IP or hostname must not be null.");
		}
		if (!IPAddress.TryParse(ip, out IPAddress address))
		{
			address = Dns.GetHostEntry(ip).AddressList[0];
			ip = address.ToString();
		}
		udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		udpSocket.Bind(new IPEndPoint(address, port));
	}

	public void Start()
	{
		IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
		EndPoint endPoint = iPEndPoint;
		threadReceive = new Thread(ThreadReceiveUdpCycle)
		{
			IsBackground = true
		};
		threadReceive.Start();
		IsStarted = true;
	}

	private void ThreadReceiveUdpCycle()
	{
		byte[] array = new byte[2048];
		while (IsStarted)
		{
			int num = 0;
			try
			{
				EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
				num = udpSocket.ReceiveFrom(array, 0, array.Length, SocketFlags.None, ref remoteEP);
				udpEndPoint = remoteEP;
				if (num > 0)
				{
				}
				if (num > 0)
				{
					ArraySegment<byte> data = default(ArraySegment<byte>);
					using (MemoryStream memoryStream = new MemoryStream())
					{
						memoryStream.Write(array, 0, num);
						data = new ArraySegment<byte>(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
					}
					OnDataReceived(this, new DataReceivedEventArgs(remoteEP.ToString(), data));
				}
			}
			catch (Exception)
			{
			}
		}
	}

	public void Send(string remoteIpPort, byte[] data)
	{
		if (string.IsNullOrEmpty(remoteIpPort))
		{
			throw new ArgumentNullException("remoteIpPort");
		}
		if (data == null || data.Length == 0)
		{
			throw new ArgumentNullException("data");
		}
		TinyTcpCommon.ParseIpPort(remoteIpPort, out string ip, out int port);
		if (port < 0)
		{
			throw new ArgumentException("Port must be zero or greater.");
		}
		if (string.IsNullOrEmpty(ip))
		{
			throw new ArgumentNullException("Remote ip must not be null.");
		}
		if (!IPAddress.TryParse(ip, out IPAddress address))
		{
			throw new ArgumentException("_remoteIp [" + ip + "] can't parse to ipAddress.");
		}
		udpSocket.SendTo(data, new IPEndPoint(address, port));
	}

	private byte[] SendAndWait(string remoteIpPort, byte[] data, int timeoutMs)
	{
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
			Send(remoteIpPort, data);
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
			Logger?.Invoke("failed to get message to " + remoteIpPort + ex3.Message);
			DataReceived -= value;
			throw;
		}
		finally
		{
			_sendLock.Release();
		}
		responded.WaitOne(new TimeSpan(0, 0, 0, 0, timeoutMs));
		DataReceived -= value;
		if (ret != null)
		{
			return ret;
		}
		Logger?.Invoke("synchronous response not received within the timeout window");
		ret = new byte[0];
		return ret;
	}

	public async Task<string> SendAndWaitAsync(string remoteIpPort, string data, int timeoutMs = 5000)
	{
		string data2 = data;
		string remoteIpPort2 = remoteIpPort;
		return await Task.Run(delegate
		{
			byte[] bytes = Encoding.UTF8.GetBytes(data2);
			byte[] bytes2 = SendAndWait(remoteIpPort2, bytes, timeoutMs);
			return Encoding.UTF8.GetString(bytes2);
		}).ConfigureAwait(continueOnCapturedContext: true);
	}
}
