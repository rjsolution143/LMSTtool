using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ISmart;

namespace SmartDevice.Steps;

public class ProcessCitDeviceBasedResult : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		string key = ((dynamic)base.Info.Args).Data;
		Result result = (Result)7;
		if (base.Cache.ContainsKey(key))
		{
			string text = base.Cache[key];
			Smart.Log.Debug(TAG, "data: " + text);
			string[] array = text.Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			Regex regex = new Regex("[^\\d]");
			SortedList<string, string> sortedList = new SortedList<string, string>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = array2[i].Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
				if (array3.Length == 2)
				{
					string key2 = array3[0].Trim();
					Result val = ((!(regex.Replace(array3[1].Trim(), "") == "2")) ? ((Result)1) : ((Result)8));
					sortedList.Add(key2, ((object)(Result)(ref val)).ToString());
				}
			}
			if (sortedList.Count > 0)
			{
				if (base.Cache.ContainsKey("SubResults"))
				{
					base.Cache["SubResults"] = sortedList;
				}
				else
				{
					base.Cache.Add("SubResults", sortedList);
				}
				result = (Result)8;
			}
			else
			{
				Smart.Log.Error(TAG, "No sub-results found");
				result = (Result)1;
			}
		}
		LogResult(result);
	}
}
