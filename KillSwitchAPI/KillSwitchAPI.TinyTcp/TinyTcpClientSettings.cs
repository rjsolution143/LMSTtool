using System;
using System.Net.Security;

namespace KillSwitchAPI.TinyTcp;

public class TinyTcpClientSettings
{
	public bool AcceptInvalidCertificates = true;

	public bool MutuallyAuthenticate = true;

	public bool UseAsyncDataReceivedEvents = true;

	public bool CheckCertificateRevocation = true;

	public RemoteCertificateValidationCallback CertificateValidationCallback = null;

	private bool _noDelay = false;

	private int _streamBufferSize = 65536;

	private int _connectTimeoutMs = 5000;

	private int _readTimeoutMs = 1000;

	private int _idleServerTimeoutMs = 0;

	private int _idleServerEvaluationIntervalMs = 1000;

	private int _connectionLostEvaluationIntervalMs = 200;

	public bool NoDelay
	{
		get
		{
			return _noDelay;
		}
		set
		{
			_noDelay = value;
		}
	}

	public int StreamBufferSize
	{
		get
		{
			return _streamBufferSize;
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentException("StreamBufferSize must be one or greater.");
			}
			if (value > 65536)
			{
				throw new ArgumentException("StreamBufferSize must be less than 65,536.");
			}
			_streamBufferSize = value;
		}
	}

	public int ConnectTimeoutMs
	{
		get
		{
			return _connectTimeoutMs;
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentException("ConnectTimeoutMs must be greater than zero.");
			}
			_connectTimeoutMs = value;
		}
	}

	public int ReadTimeoutMs
	{
		get
		{
			return _readTimeoutMs;
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentException("ReadTimeoutMs must be greater than zero.");
			}
			_readTimeoutMs = value;
		}
	}

	public int IdleServerTimeoutMs
	{
		get
		{
			return _idleServerTimeoutMs;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentException("IdleClientTimeoutMs must be zero or greater.");
			}
			_idleServerTimeoutMs = value;
		}
	}

	public int IdleServerEvaluationIntervalMs
	{
		get
		{
			return _idleServerEvaluationIntervalMs;
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentOutOfRangeException("IdleServerEvaluationIntervalMs must be one or greater.");
			}
			_idleServerEvaluationIntervalMs = value;
		}
	}

	public int ConnectionLostEvaluationIntervalMs
	{
		get
		{
			return _connectionLostEvaluationIntervalMs;
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentOutOfRangeException("ConnectionLostEvaluationIntervalMs must be one or greater.");
			}
			_connectionLostEvaluationIntervalMs = value;
		}
	}
}
