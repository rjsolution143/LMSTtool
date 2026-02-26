using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class ValidateCacheValue : BaseStep
{
	protected enum CheckType
	{
		Equal,
		NotEqual,
		Greater,
		Less,
		Contains,
		StartWith
	}

	private string TAG => GetType().FullName;

	public override void Run()
	{
		SortedList<CheckType, string> sortedList = new SortedList<CheckType, string>();
		sortedList[CheckType.Equal] = " = ";
		sortedList[CheckType.NotEqual] = " != ";
		sortedList[CheckType.Greater] = " > ";
		sortedList[CheckType.Less] = " < ";
		sortedList[CheckType.Contains] = " Contains ";
		sortedList[CheckType.StartWith] = " StartWith ";
		bool flag = false;
		if (((dynamic)base.Info.Args).CheckAll != null)
		{
			flag = ((dynamic)base.Info.Args).CheckAll;
		}
		List<string> list = new List<string>();
		foreach (object item2 in ((dynamic)base.Info.Args).ValueToCheck)
		{
			string item = (string)(dynamic)item2;
			list.Add(item);
		}
		foreach (string item3 in list)
		{
			Smart.Log.Debug(TAG, "Validating: " + item3);
			string[] separator = new string[5] { " AND ", " And ", " and ", " & ", " && " };
			List<string> list2 = new List<string>(item3.Split(separator, StringSplitOptions.RemoveEmptyEntries));
			bool flag2 = true;
			foreach (string item4 in list2)
			{
				Smart.Log.Debug(TAG, "Checking: " + item4);
				string text = "";
				string text2 = "";
				CheckType checkType = CheckType.Equal;
				foreach (CheckType key in sortedList.Keys)
				{
					string text3 = sortedList[key];
					if (item4.Contains(text3))
					{
						string[] separator2 = new string[1] { text3 };
						string[] array = item4.Split(separator2, StringSplitOptions.None);
						if (array.Length != 2)
						{
							LogResult((Result)1, "Could not parse Validation check", "Bad check: " + item4);
							return;
						}
						text = array[0];
						text2 = array[1];
						checkType = key;
						break;
					}
				}
				if (text == string.Empty && text2 == string.Empty)
				{
					LogResult((Result)1, "Could not understand Validation check", "Unrecognized check: " + item4);
					return;
				}
				if (text.StartsWith("$"))
				{
					text = text.Substring(1);
					Smart.Log.Debug(TAG, "Checking left value in Cache: " + text);
					text = base.Cache[text];
					Smart.Log.Debug(TAG, "left value: " + text);
				}
				if (text2.StartsWith("$"))
				{
					text2 = text2.Substring(1);
					Smart.Log.Debug(TAG, "Checking right value in Cache: " + text2);
					text2 = base.Cache[text2];
					Smart.Log.Debug(TAG, "right value: " + text2);
				}
				bool flag3 = false;
				if (checkType switch
				{
					CheckType.Equal => (text.ToLowerInvariant().Trim() == text2.ToLowerInvariant().Trim()) ? 1 : 0, 
					CheckType.NotEqual => (text.ToLowerInvariant().Trim() != text2.ToLowerInvariant().Trim()) ? 1 : 0, 
					CheckType.Greater => (float.Parse(text.ToLowerInvariant().Trim()) > float.Parse(text2.ToLowerInvariant().Trim())) ? 1 : 0, 
					CheckType.Less => (float.Parse(text.ToLowerInvariant().Trim()) < float.Parse(text2.ToLowerInvariant().Trim())) ? 1 : 0, 
					CheckType.Contains => text.ToLowerInvariant().Trim().Contains(text2.ToLowerInvariant().Trim()) ? 1 : 0, 
					CheckType.StartWith => text.ToLowerInvariant().Trim().StartsWith(text2.ToLowerInvariant().Trim()) ? 1 : 0, 
					_ => throw new ArgumentOutOfRangeException("Bad CheckType found"), 
				} == 0)
				{
					Smart.Log.Debug(TAG, "Check Failed: " + item4);
					flag2 = false;
					break;
				}
			}
			if (flag)
			{
				if (!flag2)
				{
					Smart.Log.Debug(TAG, "Validation Failed: " + item3);
					LogResult((Result)1, "Validation Failed", "Failed Check: " + item3);
					return;
				}
				Smart.Log.Debug(TAG, "Validated: " + item3);
			}
			else
			{
				if (flag2)
				{
					Smart.Log.Debug(TAG, "Validated: " + item3);
					LogResult((Result)8, "Validation Passed", "Passed Check: " + item3);
					return;
				}
				Smart.Log.Debug(TAG, "Validation Failed: " + item3);
			}
		}
		if (flag)
		{
			LogPass();
		}
		else
		{
			LogResult((Result)1, "Validation Failed");
		}
	}
}
