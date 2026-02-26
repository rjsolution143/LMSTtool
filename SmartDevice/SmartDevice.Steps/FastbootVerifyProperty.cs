using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class FastbootVerifyProperty : FastbootStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0923: Unknown result type (might be due to invalid IL or missing references)
		//IL_0928: Unknown result type (might be due to invalid IL or missing references)
		//IL_092a: Unknown result type (might be due to invalid IL or missing references)
		//IL_092d: Invalid comparison between Unknown and I4
		//IL_0b5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b38: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fc: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).Property;
		string text2 = text;
		if (text.ToLower() == "androidversion")
		{
			text2 = "ro.build.fingerprint";
		}
		string text3 = "getvar " + text2.ToLower();
		Smart.Log.Debug(TAG, "command: " + text3);
		int num = 10000;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
			num *= 1000;
		}
		string filePathName = Smart.Rsd.GetFilePathName("fastbootExe", base.Recipe.Info.UseCase, val);
		int num2 = default(int);
		List<string> resps = Smart.MotoAndroid.Shell(val.ID, text3, num, filePathName, ref num2, 6000, false);
		string text4 = ExtractPropertyValue(resps, text2);
		if (text.ToLower() == "androidversion")
		{
			int num3 = 1;
			if (((dynamic)base.Info.Args).VersionSize != null)
			{
				num3 = ((dynamic)base.Info.Args).VersionSize;
			}
			string[] array = text4.Split(new char[1] { '/' });
			if (array.Length > 3)
			{
				string text5 = array[3].Trim();
				text4 = ((text5.Length >= num3) ? text5.Substring(0, num3) : text5);
			}
			else
			{
				text4 = string.Empty;
			}
		}
		Smart.Log.Debug(TAG, $"Property: {text} has value {text4}");
		Result result = (Result)1;
		string[] array2 = ParseArgument(((dynamic)base.Info.Args).Expected);
		string toCheck = "latestFingerPrint";
		if (Array.Exists(array2, (string element) => element.Contains(toCheck)))
		{
			int num4 = Array.FindIndex(array2, (string element) => element.Contains(toCheck));
			string text6 = array2[num4];
			if (text6.StartsWith("$"))
			{
				text6 = base.Cache[text6.Substring(1)];
			}
			Smart.Log.Debug(TAG, $"Expected value: {text6}");
			char c = ',';
			if (((dynamic)base.Info.Args).Seperator != null)
			{
				c = (char)((dynamic)base.Info.Args).Seperator;
			}
			foreach (string item in text6.Split(new char[1] { c }).ToList())
			{
				if (string.Compare(item, text4, ignoreCase: true) == 0)
				{
					Smart.Log.Debug(TAG, $"Passed: Phone value {text4} match Expected value {item}");
					result = (Result)8;
					break;
				}
			}
		}
		else
		{
			result = VerifyPropertyValue(text4);
		}
		if ((int)result == 1 && ((((dynamic)base.Info.Args).PromptText != null) ? true : false))
		{
			string text7 = ((dynamic)base.Info.Args).PromptText.ToString();
			text7 = Smart.Locale.Xlate(text7);
			string text8 = Smart.Locale.Xlate(base.Info.Name);
			val.Prompt.MessageBox(text8, text7, (MessageBoxButtons)0, (MessageBoxIcon)64);
		}
		base.Cache[text] = text4;
		SetPreCondition(text4);
		VerifyOnly(ref result);
		LogResult(result);
	}

	private string ExtractPropertyValue(List<string> resps, string property)
	{
		string text = string.Empty;
		List<string> list = new List<string>();
		foreach (string resp in resps)
		{
			if (resp.Contains(property))
			{
				list.Add(resp);
			}
		}
		if (list.Count > 0)
		{
			if (list[0].StartsWith("(bootloader)"))
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].StartsWith("(bootloader)"))
					{
						string text2 = (list[i].Contains("[") ? $"(bootloader) {property}[{i}]: " : $"(bootloader) {property}: ");
						text += list[i].Substring(text2.Length);
					}
				}
				text = text.Trim();
			}
			else
			{
				text = list[0].Replace(property + ":", string.Empty).Trim();
			}
		}
		Smart.Log.Debug(TAG, "propValue = " + text);
		return text;
	}
}
