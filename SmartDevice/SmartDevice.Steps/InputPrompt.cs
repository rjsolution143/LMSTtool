using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using ISmart;

namespace SmartDevice.Steps;

public class InputPrompt : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0e26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2c: Invalid comparison between Unknown and I4
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = string.Empty;
		if (((dynamic)base.Info.Args).InputName != null)
		{
			text = ((dynamic)base.Info.Args).InputName.ToString();
		}
		string text2 = ((dynamic)base.Info.Args).PromptText.ToString();
		string tPromptText = Smart.Locale.Xlate(text2);
		string title = Smart.Locale.Xlate(base.Info.Name);
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
		string text3 = string.Empty;
		if (!flag && base.Cache.ContainsKey(text))
		{
			text3 = base.Cache[text];
		}
		if (text3 == string.Empty)
		{
			text3 = Smart.Thread.RunAndWait<string>((Func<string>)(() => device.Prompt.InputBox(title, tPromptText, watermark)), true);
			text3 = text3.Trim();
			Smart.Log.Info(TAG, $"User entered '{text3}' for input value {text}");
		}
		else
		{
			Smart.Log.Info(TAG, $"Using existing value '{text3}' for input value {text}");
		}
		if (text3 == string.Empty)
		{
			throw new InvalidDataException("User did not enter a response");
		}
		if (((dynamic)base.Info.Args).Pattern != null)
		{
			string pattern = ((dynamic)base.Info.Args).Pattern;
			if (!Regex.Match(text3, pattern).Success)
			{
				throw new InvalidDataException("User input format is invalid");
			}
		}
		if (((dynamic)base.Info.Args).InputRange != null)
		{
			string inputRangeString = ((dynamic)base.Info.Args).InputRange.ToString();
			if (!InputInRange(text3, inputRangeString))
			{
				throw new InvalidDataException("User input is out of range");
			}
		}
		if (((dynamic)base.Info.Args).ExpectedInputLength != null)
		{
			int num = ((dynamic)base.Info.Args).ExpectedInputLength;
			if (text3.Trim().Length != num)
			{
				throw new InvalidDataException($"User input length doesn't meet expected length {num}");
			}
		}
		if (text.ToLowerInvariant().Contains("serialnumber"))
		{
			text3 = Smart.Convert.CalculateCheckDigit(text3.Trim());
		}
		if (text.ToLowerInvariant().Contains("gsnin") && (int)device.Type == 2 && device.WiFiOnlyDevice)
		{
			base.Cache["SerialNumberIn"] = text3;
		}
		if (text.ToLowerInvariant().Contains("value"))
		{
			string[] array = text2.Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string text4 = array[i].Trim().Replace("|", string.Empty);
				if (!(text4 == string.Empty))
				{
					string[] array2 = text4.Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);
					if (array2[0] == text3)
					{
						text3 = array2[1];
						break;
					}
				}
			}
		}
		if (text.ToLowerInvariant().Contains("scannedgsn"))
		{
			device.ScannedGsn = text3;
			base.Log.AddInfo(text, text3);
		}
		if (text.ToLowerInvariant().Contains("scannedtrackid"))
		{
			device.ScannedTrackId = text3;
			base.Log.AddInfo(text, text3);
		}
		Smart.Log.Info(TAG, $"Using '{text3}' for input value {text}");
		if (!string.IsNullOrEmpty(text))
		{
			base.Cache[text] = text3;
			base.Log.AddInfo(text, text3);
			base.Log.AddInfo("InputName", text);
		}
		SetPreCondition(text3);
		LogPass();
	}

	private bool InputInRange(string inputString, string inputRangeString)
	{
		if (string.IsNullOrEmpty(inputRangeString))
		{
			return true;
		}
		bool result = true;
		string[] array = inputRangeString.Split(new char[1] { '-' });
		int num = int.Parse(inputString);
		List<int> list = new List<int>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text != string.Empty)
			{
				list.Add(int.Parse(text));
			}
			else
			{
				list.Add(-1);
			}
		}
		if (array.Length == 1)
		{
			if (num != list[0])
			{
				result = false;
			}
		}
		else if (array[1] == string.Empty)
		{
			if (num < list[0])
			{
				result = false;
			}
		}
		else if (array[0] == string.Empty)
		{
			if (num > list[1])
			{
				result = false;
			}
		}
		else if ((num < list[0]) | (num > list[1]))
		{
			result = false;
		}
		return result;
	}
}
