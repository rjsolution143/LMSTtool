using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class CheckCrashStats : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		SortedList<string, SortedList<string, object>> eventData = Smart.Maintenance.EventData;
		if (!eventData.ContainsKey("CrashRecovery"))
		{
			LogPass();
			return;
		}
		SortedList<string, object> sortedList = eventData["CrashRecovery"];
		int num = int.Parse(sortedList["Count"].ToString());
		base.Log.AddResult("LMST Crash Count", "CheckCrashStats", (Result)8, "LMST crash count", "", "", 10000.0, -1.0, (double)num, (SortedList<string, object>)null);
		DateTime value = DateTime.Parse(sortedList["Last"].ToString());
		int num2 = (int)DateTime.Now.Subtract(value).TotalDays;
		base.Log.AddResult("Last LMST Crash", "CheckCrashStats", (Result)8, "Days since last crash", "", "", 10000.0, -1.0, (double)num2, (SortedList<string, object>)null);
		LogPass();
	}
}
