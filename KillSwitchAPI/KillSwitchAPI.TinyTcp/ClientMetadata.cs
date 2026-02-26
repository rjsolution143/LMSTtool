using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;

namespace KillSwitchAPI.TinyTcp;

internal class ClientMetadata : IDisposable
{
	internal SemaphoreSlim SendLock = new SemaphoreSlim(1, 1);

	internal SemaphoreSlim ReceiveLock = new SemaphoreSlim(1, 1);

	private TcpClient _tcpClient = null;

	private NetworkStream _networkStream = null;

	private SslStream _sslStream = null;

	private string _ipPort = null;

	internal TcpClient Client => _tcpClient;

	internal NetworkStream NetworkStream => _networkStream;

	internal SslStream SslStream
	{
		get
		{
			return _sslStream;
		}
		set
		{
			_sslStream = value;
		}
	}

	internal string IpPort => _ipPort;

	public string Name { get; set; } = null;


	internal CancellationTokenSource TokenSource { get; set; }

	internal CancellationToken Token { get; set; }

	internal ClientMetadata(TcpClient tcp)
	{
		if (tcp == null)
		{
			throw new ArgumentNullException("tcp");
		}
		_tcpClient = tcp;
		_networkStream = tcp.GetStream();
		_ipPort = tcp.Client.RemoteEndPoint.ToString();
		TokenSource = new CancellationTokenSource();
		Token = TokenSource.Token;
	}

	public void Dispose()
	{
		if (TokenSource != null && !TokenSource.IsCancellationRequested)
		{
			TokenSource.Cancel();
			TokenSource.Dispose();
		}
		_sslStream?.Close();
		_networkStream?.Close();
		if (_tcpClient != null)
		{
			_tcpClient.Close();
		}
		SendLock.Dispose();
		ReceiveLock.Dispose();
	}
}
