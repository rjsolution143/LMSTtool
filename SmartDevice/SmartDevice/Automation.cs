using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using ISmart;
using KillSwitchAPI.KillSwitchStation;

namespace SmartDevice;

public class Automation : IAutomation, IDisposable
{
	private DataEraseProcessClient client;

	private Thread refreshThread;

	private string ipPort = "127.0.0.1:9999";

	private int fixtureCount = 12;

	public static object ClientLock = new object();

	private SortedList<int, Thread> fixtureThreads = new SortedList<int, Thread>();

	private bool disposedValue;

	private string TAG => GetType().FullName;

	public bool Running { get; private set; }

	private List<Fixture> Fixtures { get; } = new List<Fixture>();


	public void Start()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		Smart.Log.Debug(TAG, "Create client...");
		client = new DataEraseProcessClient(ipPort);
		for (int i = 0; i < fixtureCount; i++)
		{
			Fixture item = new Fixture(i + 1, client);
			Fixtures.Add(item);
			fixtureThreads[i + 1] = null;
		}
		Smart.Log.Debug(TAG, "Staring Automation client");
		string empty = string.Empty;
		bool flag = false;
		lock (ClientLock)
		{
			Smart.Log.Debug(TAG, "Connect to server");
			flag = client.Init(ipPort, ref empty);
		}
		if (!flag)
		{
			Smart.Log.Error(TAG, "Could not connect to Automation server");
			throw new WebException("Could not connect to Automation server");
		}
		Running = true;
		refreshThread = Smart.Thread.RunThread((ThreadStart)RefreshLoop);
	}

	public void Stop()
	{
		Smart.Log.Debug(TAG, "Stopping Automation client");
		Running = false;
	}

	private void Launch()
	{
		List<int> list = new List<int>();
		foreach (int key in fixtureThreads.Keys)
		{
			if (fixtureThreads.ContainsKey(key) && fixtureThreads[key] != null && !fixtureThreads[key].IsAlive)
			{
				Smart.Log.Debug(TAG, $"Fixture {key} thread is finished");
				list.Add(key);
			}
		}
		foreach (int item in list)
		{
			fixtureThreads[item] = null;
		}
		foreach (Fixture fixture in Fixtures)
		{
			if (!fixture.Processing && fixtureThreads[fixture.Index] != null)
			{
				Smart.Log.Error(TAG, $"Fixture {fixture.Index} in hanging state");
			}
			else if (!fixture.Processing && fixture.Status == FixtureStatus.Ready)
			{
				Smart.Log.Debug(TAG, $"Fixture {fixture.Index} starting new process thread");
				Thread value = Smart.Thread.RunThread((ThreadStart)fixture.ProcessThread);
				fixtureThreads[fixture.Index] = value;
			}
		}
	}

	private void RefreshLoop()
	{
		while (true)
		{
			try
			{
				Smart.UsbPorts.PortRefresh();
				Refresh();
				Launch();
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, "Error duing Automation Refresh Loop: " + ex.Message);
				Smart.Log.Debug(TAG, ex.ToString());
			}
			Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
		}
	}

	private void Refresh()
	{
		if (!Running)
		{
			Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
			return;
		}
		if (!client.IsConnected)
		{
			_ = string.Empty;
			if (0 == 0)
			{
				Smart.Log.Error(TAG, "Could not connect to Automation server");
				throw new WebException("Could not connect to Automation server");
			}
		}
		Smart.Log.Verbose(TAG, "Refreshing fixture status");
		foreach (Fixture fixture in Fixtures)
		{
			try
			{
				fixture.Refresh();
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, $"Error updating Fixture {fixture.Index}: {ex.Message}");
				Smart.Log.Verbose(TAG, ex.ToString());
			}
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposedValue)
		{
			return;
		}
		if (disposing)
		{
			Running = false;
			if (client != null)
			{
				client = null;
			}
			foreach (Fixture fixture in Fixtures)
			{
				fixture.Dispose();
			}
			Fixtures.Clear();
		}
		disposedValue = true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
