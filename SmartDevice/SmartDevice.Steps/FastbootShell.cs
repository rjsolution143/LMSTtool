using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class FastbootShell : FastbootStep
{
	private List<List<string>> mLogMsgToPrompts;

	private List<string> mLogMsgToClosePrompts;

	private List<List<string>> mLogMsgToCommands;

	private List<List<string>> mLogMsgToExtractValues;

	private MessageBoxIcon mIconType = (MessageBoxIcon)64;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1831: Unknown result type (might be due to invalid IL or missing references)
		//IL_1834: Invalid comparison between Unknown and I4
		//IL_182f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b02: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b05: Invalid comparison between Unknown and I4
		//IL_1b07: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b0a: Invalid comparison between Unknown and I4
		//IL_1868: Unknown result type (might be due to invalid IL or missing references)
		//IL_186d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b55: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b37: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab9: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).Command;
		bool flag = false;
		if (text != null && text != string.Empty)
		{
			flag = text.ToLowerInvariant().Trim() == "reboot" || text.ToLowerInvariant().Trim() == "continue" || text.ToLowerInvariant().Trim() == "reboot bootloader";
		}
		string text2 = null;
		string text3 = null;
		string text4 = string.Empty;
		Result result = (Result)8;
		bool flag2 = false;
		if (((dynamic)base.Info.Args).IgnoreCmdFailed != null)
		{
			flag2 = (bool)((dynamic)base.Info.Args).IgnoreCmdFailed;
		}
		if (((dynamic)base.Info.Args).Expected != null)
		{
			text2 = ((dynamic)base.Info.Args).Expected;
			if (text2.StartsWith("$"))
			{
				string key = text2.Substring(1);
				text2 = base.Cache[key];
			}
		}
		if (((dynamic)base.Info.Args).NotExpected != null)
		{
			text3 = ((dynamic)base.Info.Args).NotExpected;
			if (text3.StartsWith("$"))
			{
				string key2 = text3.Substring(1);
				text3 = base.Cache[key2];
			}
		}
		if (((dynamic)base.Info.Args).Data != null)
		{
			text4 = ((dynamic)base.Info.Args).Data;
			string text5 = BaseStep.VariableSubstitution(text4);
			if (text4.Contains("<") && text4.Contains(">"))
			{
				base.Cache.Add(text4.Replace("<", "").Replace(">", ""), text5);
			}
			text4 = text5;
			if (text4.StartsWith("$"))
			{
				string key3 = text4.Substring(1);
				text4 = base.Cache[key3];
			}
		}
		if (((dynamic)base.Info.Args).Format != null)
		{
			List<string> list = new List<string>();
			foreach (object item6 in ((dynamic)base.Info.Args).Format)
			{
				string text6 = (string)(dynamic)item6;
				string item = text6;
				if (text6.StartsWith("$"))
				{
					string key4 = text6.Substring(1);
					item = base.Cache[key4];
				}
				list.Add(item);
			}
			text4 = string.Format(text4, list.ToArray());
			Smart.Log.Debug(TAG, text4);
		}
		if (((dynamic)base.Info.Args).LogMsgToPrompts != null)
		{
			mLogMsgToPrompts = new List<List<string>>();
			foreach (dynamic item7 in ((dynamic)base.Info.Args).LogMsgToPrompts)
			{
				List<string> list2 = new List<string>();
				foreach (object item8 in item7)
				{
					string item2 = (string)(dynamic)item8;
					list2.Add(item2);
				}
				mLogMsgToPrompts.Add(list2);
			}
		}
		if (((dynamic)base.Info.Args).LogMsgToClosePrompts != null)
		{
			mLogMsgToClosePrompts = new List<string>();
			foreach (object item9 in ((dynamic)base.Info.Args).LogMsgToClosePrompts)
			{
				string item3 = (string)(dynamic)item9;
				mLogMsgToClosePrompts.Add(item3);
			}
		}
		if (((dynamic)base.Info.Args).LogMsgToActions != null)
		{
			mLogMsgToCommands = new List<List<string>>();
			foreach (dynamic item10 in ((dynamic)base.Info.Args).LogMsgToActions)
			{
				List<string> list3 = new List<string>();
				foreach (object item11 in item10)
				{
					string item4 = (string)(dynamic)item11;
					list3.Add(item4);
				}
				mLogMsgToCommands.Add(list3);
			}
		}
		if (((dynamic)base.Info.Args).LogMsgToExtractValues != null)
		{
			mLogMsgToExtractValues = new List<List<string>>();
			foreach (dynamic item12 in ((dynamic)base.Info.Args).LogMsgToExtractValues)
			{
				List<string> list4 = new List<string>();
				foreach (object item13 in item12)
				{
					string item5 = (string)(dynamic)item13;
					list4.Add(item5);
				}
				mLogMsgToExtractValues.Add(list4);
			}
		}
		if (text4 != string.Empty)
		{
			text = text + " " + text4;
		}
		Smart.Log.Debug(TAG, "command: " + text);
		int num = 30000;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
			num *= 1000;
		}
		string filePathName = Smart.Rsd.GetFilePathName("fastbootExe", base.Recipe.Info.UseCase, val);
		int num2 = default(int);
		List<string> list5 = Smart.MotoAndroid.Shell(val.ID, text, num, filePathName, ref num2, 6000, false);
		string respString = string.Join("\r\n", list5.ToArray());
		Smart.Log.Verbose(TAG, $"Fastboot response (count: {list5.Count}): {respString}");
		if (!flag2 && (respString.Contains("error") || respString.Contains("fail") || num2 != 0))
		{
			result = (Result)1;
		}
		if ((int)result == 8)
		{
			if ((text2 != null && text2 != string.Empty) || (text3 != null && text3 != string.Empty))
			{
				result = VerifyContainedPropertyValue(text2, text3, respString);
			}
			else if (((dynamic)base.Info.Args).Max != null || ((dynamic)base.Info.Args).Min != null)
			{
				result = ValidateResponseWithinLimit(text, list5, out respString);
			}
		}
		if (!MessageToAction(respString, mLogMsgToClosePrompts, mLogMsgToPrompts, mLogMsgToCommands, mLogMsgToExtractValues))
		{
			Smart.Log.Error(TAG, "Action reports failure - ignoring it now");
		}
		SetPreconditionAndProperty(list5);
		VerifyOnly(ref result);
		if (((int)result == 1 || (int)result == 4) && respString != null && respString.Trim() != string.Empty)
		{
			string dynamicError = string.Join(" ", list5.ToArray());
			LogResult(result, "Fastboot command failed", dynamicError);
			return;
		}
		if (flag && val != null)
		{
			val.ReportMode((DeviceMode)16);
		}
		LogResult(result);
	}

	private Result ValidateResponseWithinLimit(string command, List<string> resp, out string respString)
	{
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Invalid comparison between Unknown and I4
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		respString = string.Empty;
		double num = double.MaxValue;
		double num2 = double.MinValue;
		if (((dynamic)base.Info.Args).Max != null)
		{
			num = ((dynamic)base.Info.Args).Max;
		}
		if (((dynamic)base.Info.Args).Min != null)
		{
			num2 = ((dynamic)base.Info.Args).Min;
		}
		int index = 0;
		if (((dynamic)base.Info.Args).ExpectedResponseIndex != null)
		{
			index = ((dynamic)base.Info.Args).ExpectedResponseIndex;
		}
		char c = ':';
		if (((dynamic)base.Info.Args).SeperatorInExpectedResponse != null)
		{
			c = ((dynamic)base.Info.Args).SeperatorInExpectedResponse;
		}
		double num3 = Convert.ToDouble(resp[index].Split(new char[1] { c })[1].Trim());
		Result val = (Result)((num2 > num3 || num3 > num) ? 1 : 8);
		Smart.Log.Verbose(TAG, $"LowerLimit = {num2},Value = {num3},UpperLimit = {num},Result = {((object)(Result)(ref val)).ToString()}");
		if ((int)val == 1 && command == "getvar battery-voltage")
		{
			if (num3 < num2)
			{
				respString = "Warning: battery's capacity is very low";
			}
			if (num3 > num)
			{
				respString = "Warning: battery's capacity is very high";
			}
		}
		return val;
	}

	private void SetPreconditionAndProperty(List<string> response)
	{
		List<string> list = FilterResponse(response);
		List<int> list2 = new List<int> { 0 };
		if (((dynamic)base.Info.Args).PreCondValueIndexes != null)
		{
			string text = (string)((dynamic)base.Info.Args).PreCondValueIndexes;
			List<string> list3 = new List<string>(text.Split(new char[1] { ',' }));
			list2[0] = int.Parse(list3[0].Trim());
			for (int i = 1; i < list3.Count; i++)
			{
				list2.Add(int.Parse(list3[i].Trim()));
			}
			Smart.Log.Debug(TAG, $"PreCondValueIndexes: {text} resp count: {list.Count}");
		}
		else if (((dynamic)base.Info.Args).PreCondValueIndex != null)
		{
			list2[0] = ((dynamic)base.Info.Args).PreCondValueIndex;
			Smart.Log.Debug(TAG, string.Format("PreCondValueIndex: {0} resp count: {1}", ((dynamic)base.Info.Args).PreCondValueIndex, list.Count));
		}
		List<string> list4 = new List<string>();
		if (((dynamic)base.Info.Args).PropertyValues != null)
		{
			list4 = new List<string>(((string)((dynamic)base.Info.Args).PropertyValues).Split(new char[1] { ',' }));
		}
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = ",";
		if (((dynamic)base.Info.Args).Separator != null)
		{
			text4 = ((dynamic)base.Info.Args).Separator;
		}
		foreach (int item in list2)
		{
			if (item >= list.Count)
			{
				continue;
			}
			string text5 = list[item].Replace("(bootloader)", string.Empty).Trim().Split(new char[1] { ':' })[^1].Trim();
			text2 += text5;
			if (((dynamic)base.Info.Args).Property != null)
			{
				string text6 = ((dynamic)base.Info.Args).Property;
				if (((dynamic)base.Info.Args).PropertyValues != null)
				{
					if (list4.Contains(text5))
					{
						text3 += text5;
						base.Cache[text6] = text3;
						Smart.Log.Verbose(TAG, $"{text6} = {text3}");
						text3 += text4;
					}
				}
				else
				{
					base.Cache[text6] = text2;
					Smart.Log.Verbose(TAG, $"{text6} = {text2}");
				}
			}
			text2 += text4;
		}
		if (text2.Length > 0)
		{
			text2 = text2.Substring(0, text2.Length - text4.Length);
		}
		Smart.Log.Debug(TAG, $"PreCondValue: {text2}");
		SetPreCondition(text2);
	}

	private List<string> FilterResponse(List<string> responses)
	{
		List<string> list = new List<string>();
		foreach (string response in responses)
		{
			string text = response.Trim().ToLower();
			if (text.Contains("waiting for") || text.Contains("okay [") || text.Contains("finished. total time"))
			{
				Smart.Log.Debug(TAG, $"Skip line \"{response}\"");
				continue;
			}
			if (((dynamic)base.Info.Args).SkipLines != null)
			{
				string skipLines = ((dynamic)base.Info.Args).SkipLines;
				if (IsMatched(text, skipLines))
				{
					Smart.Log.Debug(TAG, $"Skip line \"{response}\"");
					continue;
				}
			}
			list.Add(response);
		}
		return CombineMultilieResponse(list);
	}

	private List<string> CombineMultilieResponse(List<string> filteredResps)
	{
		if (((dynamic)base.Info.Args).CombineMultiLineResponse != null && bool.Parse(((dynamic)base.Info.Args).CombineMultiLineResponse.ToString()))
		{
			string value = "(bootloader)";
			string value2 = ":";
			string text = string.Empty;
			string text2 = string.Empty;
			foreach (string filteredResp in filteredResps)
			{
				if (!filteredResp.Contains(value))
				{
					continue;
				}
				if (filteredResp.Contains(value2))
				{
					if (string.IsNullOrEmpty(text))
					{
						int num = filteredResp.IndexOf(")") + 1;
						int num2 = filteredResp.IndexOf(":");
						if (filteredResp.Contains("["))
						{
							num2 = filteredResp.IndexOf("[");
						}
						text = filteredResp.Substring(num, num2 - num).Trim();
					}
					string text3 = filteredResp.Substring(filteredResp.IndexOf(":") + 1).Trim();
					text2 += text3;
				}
				else
				{
					string text4 = filteredResp.Substring(filteredResp.IndexOf(")") + 1).Trim();
					text2 += text4;
				}
			}
			Smart.Log.Verbose(TAG, $"Extracted fastboot property name \"{text}\" and value \"{text2}\"");
			filteredResps = new List<string> { $"(bootloader) {text}: {text2}" };
		}
		return filteredResps;
	}

	private new bool IsMatched(string value, string skipLines)
	{
		bool result = false;
		string[] array = skipLines.Split(new char[1] { ',' });
		foreach (string text in array)
		{
			if (!(text == string.Empty) && value.Contains(text.ToLower()))
			{
				Smart.Log.Verbose(TAG, $"line \"{value}\" contains \"{text}\"");
				result = true;
				break;
			}
		}
		return result;
	}
}
