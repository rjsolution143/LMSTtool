using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class LockSet : BaseStep
{
	protected static SortedList<string, int> locks = new SortedList<string, int>();

	protected static object locker = new object();

	private string TAG => GetType().FullName;

	public override void Run()
	{
		string text = ((dynamic)base.Info.Args).LockName;
		int num = 1;
		if (((dynamic)base.Info.Args).LockSize != null)
		{
			num = ((dynamic)base.Info.Args).LockSize;
		}
		TimeSpan timeSpan = TimeSpan.FromHours(1.0);
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			timeSpan = TimeSpan.FromSeconds((int)((dynamic)base.Info.Args).Timeout);
		}
		lock (locker)
		{
			if (!locks.ContainsKey(text))
			{
				locks[text] = num;
				Smart.Log.Debug(TAG, $"Created lock for '{text}' of size {num}");
			}
		}
		Smart.Log.Debug(TAG, $"Checking status for lock '{text}'");
		DateTime now = DateTime.Now;
		while (true)
		{
			lock (locker)
			{
				if (locks[text] > 0)
				{
					locks[text]--;
					Smart.Log.Debug(TAG, $"Set lock '{text}', {locks[text]} left");
					string key = "Lock-" + text;
					base.Cache[key] = true;
					break;
				}
			}
			if (DateTime.Now.Subtract(now).TotalSeconds > timeSpan.TotalSeconds)
			{
				Smart.Log.Error(TAG, $"Timed out waiting for lock '{text}'");
				LogResult((Result)4, "Timed out waiting for lock", text);
				return;
			}
			Smart.Thread.Wait(TimeSpan.FromMilliseconds(100.0));
		}
		LogPass();
	}
}
