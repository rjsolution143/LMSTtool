using System;

namespace KillSwitchAPI.TinyTcp;

public class DataSentEventArgs : EventArgs
{
	public string IpPort { get; }

	public long BytesSent { get; }

	internal DataSentEventArgs(string ipPort, long bytesSent)
	{
		IpPort = ipPort;
		BytesSent = bytesSent;
	}
}
