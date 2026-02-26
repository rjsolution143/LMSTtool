using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using ISmart;

namespace SmartDevice.Steps;

public class ReadDeviceInfo : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		int num = ((dynamic)base.Info.Args).Port;
		string key = $"Port{num}";
		int port = base.Cache[key];
		string text = string.Empty;
		using (TcpClient tcpClient = new TcpClient("127.0.0.1", port))
		{
			NetworkStream stream = tcpClient.GetStream();
			Smart.Thread.Wait(TimeSpan.FromSeconds(10.0), (Checker<bool>)(() => stream.CanRead));
			Smart.Thread.Wait(TimeSpan.FromSeconds(10.0), (Checker<bool>)(() => stream.DataAvailable));
			byte[] array = new byte[tcpClient.ReceiveBufferSize];
			using MemoryStream memoryStream = new MemoryStream();
			while (stream.DataAvailable)
			{
				int num2 = stream.Read(array, 0, array.Length);
				if (num2 <= 0)
				{
					break;
				}
				memoryStream.Write(array, 0, num2);
			}
			text = Encoding.UTF8.GetString(memoryStream.ToArray());
		}
		dynamic val2 = Smart.Json.Load(text);
		string text2 = val2.IMEI;
		Smart.Log.Debug(TAG, $"Detected IMEI: {text2}");
		base.Log.AddInfo("IMEI", text2);
		val.SerialNumber = text2;
		LogPass();
	}
}
