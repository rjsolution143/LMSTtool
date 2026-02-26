using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class RecordRepairHistory : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string serialNumber = val.SerialNumber;
		string repairHistory = Smart.Rsd.GetRepairHistory(serialNumber);
		dynamic val2 = Smart.Json.Load(repairHistory);
		string text = val2["status"];
		string arg = val2["message"];
		if (text.Trim().ToLowerInvariant() != "success")
		{
			string text2 = $"{text} - {arg}";
			Smart.Log.Error(TAG, "Error during Record Repair History: " + text2);
			LogResult((Result)1, text2);
			return;
		}
		string text3 = val2["serialno"];
		string text4 = val2["modelnumber"];
		if (text3.Trim() != serialNumber)
		{
			Smart.Log.Warning(TAG, $"Response SN {text3} does not match input SN {serialNumber}");
		}
		val.Log.AddInfo("HistorySN", text3);
		val.Log.AddInfo("HistoryModel", text4);
		List<string> list = new List<string>();
		int num = 0;
		foreach (dynamic item in val2["repairlist"])
		{
			num++;
			string text5 = item["result"];
			string value = item["result_description"];
			string text6 = item["usecase"];
			string value2 = item["repairtime"];
			SortedList<string, object> sortedList = new SortedList<string, object>();
			string text8 = (string)(sortedList["name"] = $"RepairHistory{num}");
			sortedList["step"] = text6;
			if (text5.Trim().ToLowerInvariant() == "pass")
			{
				sortedList["result"] = (object)(Result)8;
				if (!list.Contains(text6))
				{
					list.Add(text6);
				}
			}
			else
			{
				sortedList["result"] = (object)(Result)7;
			}
			sortedList["description"] = value;
			sortedList["dynamic"] = value2;
			val.Log.AddResult(text8, sortedList);
		}
		foreach (string item2 in list)
		{
			val.Log.AddInfo("UseCase_" + item2, "PASS");
		}
		LogPass();
	}
}
