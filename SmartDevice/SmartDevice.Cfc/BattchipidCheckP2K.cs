using System;
using System.Diagnostics;
using System.Threading;

namespace SmartDevice.Cfc;

public class BattchipidCheckP2K : BaseTest
{
	private string TAG => GetType().FullName;

	public long TimeoutInMilliSeconds { get; set; }

	public bool Execute(Func<string, string, string> testcommand)
	{
		bool flag = false;
		try
		{
			string empty = string.Empty;
			Stopwatch stopwatch = Stopwatch.StartNew();
			do
			{
				empty = testcommand("0085", "040011");
				if (empty.Equals("00004041"))
				{
					flag = true;
				}
				Thread.Sleep(7500);
			}
			while (stopwatch.ElapsedMilliseconds < TimeoutInMilliSeconds && !flag);
			stopwatch.Stop();
			Smart.Log.Debug(TAG, string.Format("BattchipidCheckP2K response = {0} ,excepted respone = '00004041' , result = {1} ", empty, flag ? "PASS" : "FALL"));
		}
		catch (Exception ex)
		{
			if (ex.Message != string.Empty)
			{
				flag = false;
				Smart.Log.Debug(TAG, ex.Message + "," + ex.StackTrace);
			}
		}
		return flag;
	}
}
