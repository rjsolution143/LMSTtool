using ISmart;

namespace SmartDevice.Steps;

public class LockRelease : LockSet
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		string text = ((dynamic)base.Info.Args).LockName;
		string key = "Lock-" + text;
		Smart.Log.Debug(TAG, $"Ready to release lock '{text}'");
		bool flag = !base.Cache.ContainsKey(key);
		if (flag || ((flag | (base.Cache[key] != true)) ? true : false))
		{
			Smart.Log.Warning(TAG, $"Lock not set for '{text}', skipping lock release");
			LogResult((Result)7, "Lock not set", text);
			return;
		}
		base.Cache.Remove(key);
		lock (LockSet.locker)
		{
			if (!LockSet.locks.ContainsKey(text))
			{
				Smart.Log.Error(TAG, $"No lock found for '{text}'");
				LogResult((Result)4, "No lock found", text);
				return;
			}
			LockSet.locks[text]++;
			Smart.Log.Debug(TAG, $"Released lock '{text}', {LockSet.locks[text]} left");
		}
		LogPass();
	}
}
