using System;
using System.Collections.Generic;
using System.Net.Security;

namespace KillSwitchAPI.TinyTcp;

public class TinyTcpServerSettings
{
	public bool AcceptInvalidCertificates = true;

	public bool MutuallyAuthenticate = true;

	public bool UseAsyncDataReceivedEvents = true;

	public bool CheckCertificateRevocation = true;

	public RemoteCertificateValidationCallback CertificateValidationCallback = null;

	private bool _noDelay = false;

	private int _streamBufferSize = 65536;

	private int _maxConnections = 4096;

	private int _idleClientTimeoutMs = 0;

	private int _idleClientEvaluationIntervalMs = 5000;

	private List<string> _permittedIPs = new List<string>();

	private List<string> _blockedIPs = new List<string>();

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
				throw new ArgumentException("StreamBufferSize must be less than or equal to 65,536.");
			}
			_streamBufferSize = value;
		}
	}

	public int IdleClientTimeoutMs
	{
		get
		{
			return _idleClientTimeoutMs;
		}
		set
		{
			if (value < 0)
			{
				throw new ArgumentException("IdleClientTimeoutMs must be zero or greater.");
			}
			_idleClientTimeoutMs = value;
		}
	}

	public int MaxConnections
	{
		get
		{
			return _maxConnections;
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentException("Max connections must be greater than zero.");
			}
			_maxConnections = value;
		}
	}

	public int IdleClientEvaluationIntervalMs
	{
		get
		{
			return _idleClientEvaluationIntervalMs;
		}
		set
		{
			if (value < 1)
			{
				throw new ArgumentOutOfRangeException("IdleClientEvaluationIntervalMs must be one or greater.");
			}
			_idleClientEvaluationIntervalMs = value;
		}
	}

	public List<string> PermittedIPs
	{
		get
		{
			return _permittedIPs;
		}
		set
		{
			if (value == null)
			{
				_permittedIPs = new List<string>();
			}
			else
			{
				_permittedIPs = value;
			}
		}
	}

	public List<string> BlockedIPs
	{
		get
		{
			return _blockedIPs;
		}
		set
		{
			if (value == null)
			{
				_blockedIPs = new List<string>();
			}
			else
			{
				_blockedIPs = value;
			}
		}
	}
}
