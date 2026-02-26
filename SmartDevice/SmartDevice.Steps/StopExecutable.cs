using System;
using System.Diagnostics;
using ISmart;

namespace SmartDevice.Steps;

public class StopExecutable : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		string key = "ExeProcess";
		if (((dynamic)base.Info.Args).Name != null)
		{
			key = ((dynamic)base.Info.Args).Name;
		}
		bool flag = false;
		if (((dynamic)base.Info.Args).WaitForExited != null)
		{
			flag = (bool)((dynamic)base.Info.Args).WaitForExited;
		}
		int milliseconds = -1;
		if (flag && ((dynamic)base.Info.Args).Timeout != null)
		{
			milliseconds = ((dynamic)base.Info.Args).Timeout * 1000;
		}
		if (base.Cache.ContainsKey(key))
		{
			try
			{
				Process process = base.Cache[key];
				process.Kill();
				if (flag)
				{
					process.WaitForExit(milliseconds);
				}
			}
			catch (Exception ex)
			{
				Smart.Log.Debug(TAG, "Exception - errMsg: " + ex.Message);
			}
		}
		LogResult((Result)8);
	}
}
