using System;
using System.IO;
using System.Text.RegularExpressions;
using ISmart;

namespace SmartDevice.Steps;

public class InputBatteryCode : BaseStep
{
	private enum Date31
	{
		Code1 = 1,
		Code2,
		Code3,
		Code4,
		Code5,
		Code6,
		Code7,
		Code8,
		Code9,
		CodeA,
		CodeB,
		CodeC,
		CodeD,
		CodeE,
		CodeF,
		CodeG,
		CodeH,
		CodeJ,
		CodeK,
		CodeL,
		CodeM,
		CodeN,
		CodeP,
		CodeR,
		CodeS,
		CodeT,
		CodeV,
		CodeW,
		CodeX,
		CodeY,
		CodeZ
	}

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd1: Unknown result type (might be due to invalid IL or missing references)
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = "BatteryCode";
		if (((dynamic)base.Info.Args).InputName != null)
		{
			text = ((dynamic)base.Info.Args).InputName.ToString();
		}
		string text2 = "MM-dd-yyyy";
		string text3 = ((dynamic)base.Info.Args).PromptText.ToString();
		string tPromptText = Smart.Locale.Xlate(text3);
		string title = Smart.Locale.Xlate(base.Info.Name);
		if (((dynamic)base.Info.Args).PromptText != null)
		{
			string type = ((dynamic)base.Info.Args).PromptType;
			Prompt(type, text3);
		}
		DateTime dateTime = new DateTime(2020, 1, 1);
		if (((dynamic)base.Info.Args).DateMin != null)
		{
			string text4 = ((dynamic)base.Info.Args).DateMin;
			if (text4.StartsWith("$"))
			{
				string key = text4.Substring(1);
				text4 = base.Cache[key];
			}
			else if (text4.Trim().ToLowerInvariant() == "now")
			{
				text4 = DateTime.UtcNow.ToShortDateString();
			}
			dateTime = DateTime.Parse(text4);
		}
		if (((dynamic)base.Info.Args).DateFormat != null)
		{
			text2 = ((dynamic)base.Info.Args).DateFormat;
		}
		DateTime utcNow = DateTime.UtcNow;
		if (((dynamic)base.Info.Args).DateMax != null)
		{
			string text5 = ((dynamic)base.Info.Args).DateMax;
			if (text5.StartsWith("$"))
			{
				string key2 = text5.Substring(1);
				text5 = base.Cache[key2];
			}
			else if (text5.Trim().ToLowerInvariant() == "now")
			{
				text5 = DateTime.UtcNow.ToShortDateString();
			}
			dateTime = DateTime.Parse(text5);
		}
		string watermark = null;
		if (((dynamic)base.Info.Args).Watermark != null)
		{
			watermark = ((dynamic)base.Info.Args).Watermark.ToString();
			watermark = Smart.Locale.Xlate(watermark);
		}
		bool flag = true;
		if (((dynamic)base.Info.Args).Overwrite != null && ((dynamic)base.Info.Args).Overwrite != true)
		{
			flag = false;
		}
		string text6 = string.Empty;
		if (!flag && base.Cache.ContainsKey(text))
		{
			text6 = base.Cache[text];
		}
		if (text6 == string.Empty)
		{
			device.Prompt.CitPrompt(title, "Text", "", base.ImageFile);
			text6 = Smart.Thread.RunAndWait<string>((Func<string>)(() => device.Prompt.InputBox(title, tPromptText, watermark)), true);
			text6 = text6.Trim();
			Smart.Log.Info(TAG, $"User entered '{text6}' for input value {text}");
		}
		else
		{
			Smart.Log.Info(TAG, $"Using existing value '{text6}' for input value {text}");
		}
		if (text6 == string.Empty)
		{
			throw new InvalidDataException("User did not enter a response");
		}
		if (((dynamic)base.Info.Args).Pattern != null)
		{
			string pattern = ((dynamic)base.Info.Args).Pattern;
			if (!Regex.Match(text6, pattern).Success)
			{
				throw new InvalidDataException("User input format is invalid");
			}
		}
		Smart.Log.Info(TAG, $"Using '{text6}' for input value {text}");
		text6 = text6.Trim();
		string text7 = text + "SN";
		string text8 = text + "Date";
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (text6.Length == 10)
		{
			empty = text6;
		}
		else if (((dynamic)base.Info.Args).StartCharToTrimValue != null || ((dynamic)base.Info.Args).EndCharToTrimValue != null)
		{
			char? c = null;
			char? c2 = null;
			if (((dynamic)base.Info.Args).StartCharToTrimValue != null)
			{
				c = ((dynamic)base.Info.Args).StartCharToTrimValue;
			}
			if (((dynamic)base.Info.Args).EndCharToTrimValue != null)
			{
				c2 = ((dynamic)base.Info.Args).EndCharToTrimValue;
			}
			empty = text6;
			if (c.HasValue)
			{
				int num = empty.IndexOf(c.Value);
				empty = empty.Substring(num + 1);
			}
			if (c2.HasValue)
			{
				empty = empty[..empty.LastIndexOf(c2.Value)];
			}
			Smart.Log.Debug(TAG, $"Trimmed value {empty}");
		}
		else if (((dynamic)base.Info.Args).BaseZeroStartIndex != null && ((dynamic)base.Info.Args).Length != null)
		{
			int startIndex = ((dynamic)base.Info.Args).BaseZeroStartIndex;
			int length = ((dynamic)base.Info.Args).Length;
			empty = text6.Substring(startIndex, length);
			Smart.Log.Debug(TAG, $"Final value {empty}");
		}
		else
		{
			if (text6.Length != 53 || !text6.Contains("--") || !text6.Contains("%"))
			{
				throw new FormatException("Incorrect scanning, please scan the correct QR code or input the correct Batt SN");
			}
			bool flag2 = true;
			if (text6.IndexOf('-') == 31)
			{
				flag2 = false;
			}
			else if (text6.IndexOf('-') == 36)
			{
				flag2 = true;
			}
			if (((dynamic)base.Info.Args).UseBattIdInsteadOfBattPn != null)
			{
				flag2 = ((dynamic)base.Info.Args).UseBattIdInsteadOfBattPn;
			}
			if (flag2)
			{
				empty = text6.Substring(24, 3) + text6.Substring(31, 4) + text6.Substring(43, 3);
				empty2 = text6.Substring(27, 4);
			}
			else
			{
				int num2 = text6.IndexOf("%") + 1;
				int num3 = text6.IndexOf("%", num2);
				empty = text6.Substring(num2, num3 - num2);
				empty2 = text6.Substring(27, 4);
			}
			string arg = "20" + empty2.Substring(0, 2);
			string arg2 = ((int)Enum.Parse(typeof(Date31), "Code" + empty2.Substring(2, 1))).ToString();
			string arg3 = ((int)Enum.Parse(typeof(Date31), "Code" + empty2.Substring(3, 1))).ToString();
			string text9 = $"{arg2}-{arg3}-{arg}";
			DateTime dateTime2 = DateTime.Parse(text9);
			if (text2 != "MM-dd-yyyy")
			{
				text9 = dateTime2.ToString(text2);
			}
			Smart.Log.Debug(TAG, $"Parsed battery date code {text9}");
			if (dateTime2 > utcNow)
			{
				string text10 = $"Scanned date {text9} is ahead of max {utcNow.ToShortDateString()}";
				Smart.Log.Verbose(TAG, text10);
				LogResult((Result)1, "Scanned Battery Date is invalid (too far ahead)", text10);
				return;
			}
			if (dateTime2 < dateTime)
			{
				string text11 = $"Scanned date {text9} is earlier than min {dateTime.ToShortDateString()}";
				Smart.Log.Verbose(TAG, text11);
				LogResult((Result)1, "Scanned Battery Date is invalid (too old)", text11);
				return;
			}
			base.Cache[text8] = text9;
			base.Log.AddInfo(text8, text9);
		}
		Smart.Log.Debug(TAG, $"Parsed SN {empty}");
		base.Cache[text7] = empty;
		base.Log.AddInfo(text7, empty);
		SetPreCondition(text6);
		LogPass();
	}
}
