using System;
using System.IO;

namespace KillSwitchAPI.TinyTcp;

internal static class TinyTcpCommon
{
	internal static void ParseIpPort(string ipPort, out string ip, out int port)
	{
		if (string.IsNullOrEmpty(ipPort))
		{
			throw new ArgumentNullException("ipPort");
		}
		ip = null;
		port = -1;
		int num = ipPort.LastIndexOf(':');
		if (num != -1)
		{
			ip = ipPort.Substring(0, num);
			port = Convert.ToInt32(ipPort.Substring(num + 1));
		}
	}

	internal static void BytesToStream(byte[] data, int start, out int contentLength, out Stream stream)
	{
		contentLength = 0;
		stream = new MemoryStream(new byte[0]);
		if (data != null && data.Length != 0)
		{
			contentLength = data.Length - start;
			stream = new MemoryStream();
			stream.Write(data, start, contentLength);
			stream.Seek(0L, SeekOrigin.Begin);
		}
	}
}
