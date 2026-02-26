using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class CheckDownloadStats : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		SortedList<string, SortedList<string, object>> eventData = Smart.Maintenance.EventData;
		if (!eventData.ContainsKey("SmartDownloader"))
		{
			LogPass();
			return;
		}
		SortedList<string, object> sortedList = eventData["SmartDownloader"];
		int num = int.Parse(sortedList["Count"].ToString());
		base.Log.AddResult("SmartDownloader Count", "CheckDownloadStats", (Result)8, "SmartDownloader use count", "", "", 10000.0, -1.0, (double)num, (SortedList<string, object>)null);
		DateTime value = DateTime.Parse(sortedList["Last"].ToString());
		int num2 = (int)DateTime.Now.Subtract(value).TotalDays;
		base.Log.AddResult("SmartDownloader Last Use", "CheckDownloadStats", (Result)8, "Days since last SmartDownloader use", "", "", 10000.0, -1.0, (double)num2, (SortedList<string, object>)null);
		LogPass();
	}
}
