using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class ValidateNetworkConfig : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		string text = Smart.File.RunCommand("ipconfig /all");
		List<SortedList<string, string>> list = new List<SortedList<string, string>>();
		SortedList<string, string> sortedList = null;
		string[] array = text.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string text2 in array)
		{
			if (!text2.StartsWith(" ") && text2.Trim() != string.Empty)
			{
				if (sortedList != null)
				{
					list.Add(sortedList);
				}
				sortedList = new SortedList<string, string>();
				sortedList["Name"] = text2;
			}
			else if (sortedList != null && text2.Contains(" . ") && text2.Contains(" : "))
			{
				string key = text2.Substring(0, text2.IndexOf(" . ")).Trim().ToLowerInvariant();
				string value = text2.Substring(text2.IndexOf(" : ") + 3).Trim().ToLowerInvariant();
				sortedList[key] = value;
			}
		}
		if (sortedList != null)
		{
			list.Add(sortedList);
		}
		foreach (SortedList<string, string> item in list)
		{
			if (!item.ContainsKey("description"))
			{
				continue;
			}
			string text3 = item["description"];
			if (!text3.Contains("motorola usb networking driver"))
			{
				continue;
			}
			string text4 = string.Empty;
			foreach (string key2 in item.Keys)
			{
				if (key2.Contains("ipv4 address"))
				{
					text4 = key2;
				}
			}
			if (!(text4 == string.Empty))
			{
				string text5 = item[text4];
				if (!text5.StartsWith("192.168.137."))
				{
					Smart.Log.Debug(TAG, text);
					string dynamicError = text3 + ": " + text5;
					LogResult((Result)1, "Bad Network Configuration", dynamicError);
					return;
				}
			}
		}
		LogPass();
	}
}
