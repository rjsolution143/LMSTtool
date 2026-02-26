using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using ISmart;

namespace SmartUtil;

public class PortChecker : IPortChecker, IDisposable
{
	private static int[] commonOffsets = new int[5] { 2, 10, 18, 5, 6 };

	private const string subnet = "192.168.137";

	private const int port = 11000;

	private object portLock = new object();

	private SortedList<int, PortStatus> ports = new SortedList<int, PortStatus>();

	private bool running;

	private bool disposedValue;

	private string TAG => GetType().FullName;

	public bool Running
	{
		get
		{
			return running;
		}
		set
		{
			running = value;
			if (value)
			{
				Smart.Thread.Run((ThreadStart)PingLoop);
			}
		}
	}

	public PortChecker()
	{
		int[] array = commonOffsets;
		foreach (int key in array)
		{
			ports[key] = PortStatus.Unused;
		}
	}

	private void PingLoop()
	{
		while (Running)
		{
			int num = 0;
			int num2 = 0;
			int[] array = commonOffsets;
			foreach (int num3 in array)
			{
				if (!Running)
				{
					return;
				}
				PortStatus portStatus = PortStatus.Unused;
				lock (portLock)
				{
					portStatus = ports[num3];
				}
				if (portStatus == PortStatus.Unrecognized)
				{
					continue;
				}
				bool flag = PingPort(num3);
				PortStatus portStatus2 = portStatus;
				if (portStatus == PortStatus.Unused && flag)
				{
					portStatus2 = PortStatus.Standby;
				}
				else if (portStatus == PortStatus.Offline && flag)
				{
					portStatus2 = PortStatus.Standby;
				}
				else if (portStatus == PortStatus.Online && !flag)
				{
					portStatus2 = PortStatus.Offline;
				}
				else if (portStatus == PortStatus.Standby && !flag)
				{
					portStatus2 = PortStatus.Offline;
				}
				if (portStatus2 == PortStatus.Online || portStatus2 == PortStatus.Standby)
				{
					num++;
				}
				else
				{
					num2++;
				}
				if (portStatus2 != portStatus)
				{
					lock (portLock)
					{
						ports[num3] = portStatus2;
					}
					Smart.Log.Debug(TAG, $"Host {num3} port {portStatus2}");
				}
			}
			Smart.Log.Debug(TAG, $"{num} online devices ({num2} offline)");
		}
	}

	private bool PingPort(int offset)
	{
		string text = "192.168.137." + offset;
		Smart.Log.Debug(TAG, $"Checking IP {text}...");
		using TcpClient tcpClient = new TcpClient();
		tcpClient.SendTimeout = 1000;
		tcpClient.ReceiveTimeout = 1000;
		try
		{
			return tcpClient.ConnectAsync(text, 11000).Wait(1000);
		}
		catch (Exception ex)
		{
			_ = ex.Message;
			return false;
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
