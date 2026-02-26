using System;

namespace KillSwitchAPI.TinyTcp;

public class ConnectionEventArgs : EventArgs
{
	public string IpPort { get; }

	public DisconnectReason Reason { get; } = DisconnectReason.None;


	internal ConnectionEventArgs(string ipPort, DisconnectReason reason = DisconnectReason.None)
	{
		IpPort = ipPort;
		Reason = reason;
	}
}
