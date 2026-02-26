using System;
using System.Collections.Generic;
using System.Threading;

namespace SmartDevice.Steps;

public class BackgroundScanStop : BaseStep
{
	protected static SortedList<string, int> locks = new SortedList<string, int>();

	protected static object locker = new object();

	private string TAG => GetType().FullName;

	public override void Run()
	{
		int num = 0;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
		}
		Smart.DeviceManager.BackgroundScan = false;
		if (num < 1)
		{
			Smart.Log.Debug(TAG, "Turning off background device scan");
		}
		else
		{
			Smart.Log.Debug(TAG, $"Turning off background device scan for {num} seconds");
			TimeSpan timeSpan = TimeSpan.FromSeconds(num);
			Smart.Thread.DelayedCallback((ThreadStart)StartCallback, timeSpan);
		}
		LogPass();
	}

	private void StartCallback()
	{
		Smart.Log.Debug(TAG, "Turning on background device scan (after delay)");
		Smart.DeviceManager.BackgroundScan = true;
	}
}
