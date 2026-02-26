using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class FastbootVerifyOemHw : FastbootStep
{
	private const string BOOTLOADER = "(bootloader) ";

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).Utag;
		Result val2 = (Result)8;
		string text2 = "oem hw " + text;
		Smart.Log.Debug(TAG, "command: " + text2);
		string property = text;
		if (((dynamic)base.Info.Args).RespPattern != null)
		{
			property = ((dynamic)base.Info.Args).RespPattern;
		}
		int num = 3000;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
			num *= 1000;
		}
		string filePathName = Smart.Rsd.GetFilePathName("fastbootExe", base.Recipe.Info.UseCase, val);
		int num2 = default(int);
		List<string> resps = Smart.MotoAndroid.Shell(val.ID, text2, num, filePathName, ref num2, 6000, false);
		if (num2 == 0)
		{
			string text3 = ExtractPropertyValue(resps, property);
			Smart.Log.Debug(TAG, $"UTAG: {text} has value {text3}");
			val2 = VerifyPropertyValue(text3);
			string key = "oem_hw_" + text.Trim().ToLowerInvariant();
			base.Cache[key] = text3;
			SetPreCondition(text3);
		}
		else
		{
			val2 = (Result)1;
		}
		LogResult(val2);
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
