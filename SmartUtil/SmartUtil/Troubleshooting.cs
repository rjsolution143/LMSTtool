using System;
using System.Collections.Generic;
using System.Linq;
using FuzzySharp;
using ISmart;

namespace SmartUtil;

public class Troubleshooting : ITroubleshooting
{
	private ICsvFile csv = Smart.NewCsvFile();

	private dynamic language = new List<object>();

	private string TAG => GetType().FullName;

	public void Load()
	{
		string filePathName = Smart.Rsd.GetFilePathName("troubleShoot", (UseCase)0, (IDevice)null);
		string text = Smart.Rsd.ReadTroubleShootFileContent(filePathName);
		try
		{
			string filePathName2 = Smart.Rsd.GetFilePathName("troubleShootTrans", (UseCase)0, (IDevice)null);
			string text2 = Smart.File.ReadText(filePathName2);
			language = Smart.Json.Load(text2);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Could not load Troubleshooting translations");
			Smart.Log.Error(TAG, ex.ToString());
		}
		csv.Load(text, ',');
	}

	public IResultSubLogger NewTroubleshootingLogger(IDevice device)
	{
		return (IResultSubLogger)(object)new TroubleshootingLogger(device);
	}

	public List<TroubleshootingInfo> FindInfo(IDevice device)
	{
		List<TroubleshootingInfo> result = new List<TroubleshootingInfo>();
		foreach (IResultSubLogger subLog in device.Log.SubLogs)
		{
			if (typeof(TroubleshootingLogger).IsAssignableFrom(((object)subLog).GetType()))
			{
				result = ((TroubleshootingLogger)(object)subLog).TopInfo;
			}
		}
		return result;
	}

	public List<TroubleshootingInfo> CalculateTop(string useCase, string model, string stepName, string description, string failureCode, string dynamicData)
	{
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		SortedList<int, int> sortedList = new SortedList<int, int>();
		foreach (SortedList<string, string> item2 in (IEnumerable<SortedList<string, string>>)csv)
		{
			if (num < 1)
			{
				num++;
				continue;
			}
			int num2 = 0;
			string[] array = new string[6] { "USE_CASE", "PHONE_MODEL", "STEP_NAME", "RESULT_DESCRIPTION", "FAILURE_CODE", "DYNAMIC_DATA" };
			foreach (string key in array)
			{
				if (item2[key].Trim() != string.Empty)
				{
					num2++;
				}
			}
			if (num2 >= 2)
			{
				int num3 = ScoreMatch(useCase, item2["USE_CASE"], (CompareType)0);
				int num4 = ScoreMatch(model, item2["PHONE_MODEL"], (CompareType)0);
				int num5 = ScoreMatch(stepName, item2["STEP_NAME"], (CompareType)0);
				int num6 = ScoreMatch(description, item2["RESULT_DESCRIPTION"], (CompareType)1);
				int num7 = ScoreMatch(failureCode, item2["FAILURE_CODE"], (CompareType)1, 60);
				int num8 = ScoreMatch(dynamicData, item2["DYNAMIC_DATA"], (CompareType)2, 40);
				int num9 = num3 * 2 + num4 + num5 * 4 + num6 * 2 + num7 + num8 * 3;
				int num10 = 5;
				string text = item2["PRIORITY"];
				if (text.Trim() != string.Empty)
				{
					num10 = int.Parse(text);
					Smart.Log.Assert(TAG, num10 >= 0 && num10 <= 10, "Priority value should be between 0 and 10");
				}
				double num11 = (double)num10 / 5.0;
				num9 = (int)((double)num9 * num11);
				sortedList[num] = num9;
				num++;
			}
		}
		List<KeyValuePair<int, int>> list = sortedList.ToList();
		list.Sort((KeyValuePair<int, int> x, KeyValuePair<int, int> y) => y.Value.CompareTo(x.Value));
		List<TroubleshootingInfo> list2 = new List<TroubleshootingInfo>();
		int num12 = 0;
		int num13 = 5;
		TroubleshootingInfo item = default(TroubleshootingInfo);
		foreach (KeyValuePair<int, int> item3 in list)
		{
			if (num12 >= num13)
			{
				break;
			}
			SortedList<string, string> sortedList2 = csv[item3.Key];
			string arg = sortedList2["URL"];
			arg = $"{arg}&lang={Smart.Locale.LanguageCode.ToLowerInvariant()}";
			bool flag = false;
			foreach (TroubleshootingInfo item4 in list2)
			{
				TroubleshootingInfo current3 = item4;
				if (((TroubleshootingInfo)(ref current3)).URL.Trim().ToLowerInvariant() == arg.Trim().ToLowerInvariant())
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				continue;
			}
			string text2 = $"Top Result {num12 + 1}";
			if (sortedList2.ContainsKey("TITLE"))
			{
				text2 = sortedList2["TITLE"];
				foreach (dynamic item5 in language)
				{
					if (((string)item5["title"]).Trim().ToLowerInvariant() == text2.Trim().ToLowerInvariant())
					{
						dynamic val = item5["translation"];
						string text3 = Smart.Locale.LanguageCode.ToLowerInvariant();
						if (val[text3] != null)
						{
							text2 = val[text3];
						}
					}
				}
			}
			num12++;
			string empty = string.Empty;
			string empty2 = string.Empty;
			int value = item3.Value;
			((TroubleshootingInfo)(ref item))._002Ector(text2, arg, value, empty, empty2);
			list2.Add(item);
		}
		foreach (TroubleshootingInfo item6 in list2)
		{
			TroubleshootingInfo current5 = item6;
			Smart.Log.Info(TAG, "Found Troubleshooting info");
			Smart.Log.Info(TAG, ((object)(TroubleshootingInfo)(ref current5)).ToString());
		}
		return list2;
	}

	private int ScoreMatch(string targetValue, string rowValue, CompareType compare)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return ScoreMatch(targetValue, rowValue, compare, 80);
	}

	private int ScoreMatch(string targetValue, string rowValue, CompareType compare, int emptyScore)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Invalid comparison between Unknown and I4
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Invalid comparison between Unknown and I4
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		targetValue = targetValue.Trim().ToLowerInvariant();
		rowValue = rowValue.Trim().ToLowerInvariant();
		if (rowValue == string.Empty)
		{
			return emptyScore;
		}
		if ((int)compare == 0)
		{
			if (rowValue == targetValue)
			{
				return 100;
			}
			return 25;
		}
		if ((int)compare == 1)
		{
			return Fuzz.Ratio(targetValue, rowValue);
		}
		if ((int)compare == 2)
		{
			return Fuzz.WeightedRatio(targetValue, rowValue);
		}
		throw new NotSupportedException("Score type not supported: " + compare);
	}
}
