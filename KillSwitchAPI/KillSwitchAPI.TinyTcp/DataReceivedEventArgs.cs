using System;

namespace KillSwitchAPI.TinyTcp;

public class DataReceivedEventArgs : EventArgs
{
	public string IpPort { get; }

	public ArraySegment<byte> Data { get; }

	internal DataReceivedEventArgs(string ipPort, ArraySegment<byte> data)
	{
		IpPort = ipPort;
		Data = data;
	}
}
