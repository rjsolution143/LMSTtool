using System;
using System.Net;
using System.Net.Sockets;
using ISmart;

namespace SmartDevice.Steps;

public class ADBPortForward : BaseStep
{
	private static object locker = new object();

	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		int num = ((dynamic)base.Info.Args).Port;
		lock (locker)
		{
			TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 0);
			tcpListener.Start();
			int port = ((IPEndPoint)tcpListener.LocalEndpoint).Port;
			tcpListener.Stop();
			try
			{
				Smart.ADB.ForwardPort(val.ID, num, port);
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, "Error setting up port forwarding: " + ex.Message);
				Smart.Log.Error(TAG, ex.ToString());
				LogResult((Result)4, "Error setting up port forwarding", ex.Message);
				return;
			}
			string key = $"Port{num}";
			base.Cache[key] = port;
		}
		LogPass();
	}
}
