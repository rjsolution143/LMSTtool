using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class VerifyModemVersion : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Invalid comparison between Unknown and I4
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_085a: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Invalid comparison between Unknown and I4
		//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		bool flag = true;
		if (((dynamic)base.Info.Args).CheckFirst != null)
		{
			flag = ((dynamic)base.Info.Args).CheckFirst;
		}
		Result val = (Result)0;
		if (flag)
		{
			val = Verify();
		}
		if ((int)val != 8)
		{
			if (((dynamic)base.Info.Args).PromptType != null && ((dynamic)base.Info.Args).PromptText != null)
			{
				string type = ((dynamic)base.Info.Args).PromptType;
				string text = ((dynamic)base.Info.Args).PromptText;
				text = Smart.Locale.Xlate(text);
				Prompt(type, text);
			}
			double value = ((((dynamic)base.Info.Args).Timeout != null) ? ((dynamic)base.Info.Args).Timeout : ((object)6));
			val = Smart.Thread.Wait<Result>(TimeSpan.FromSeconds(value), (Checker<Result>)Verify, (Result)8);
		}
		bool flag2 = false;
		if (((dynamic)base.Info.Args).ValidateOnly != null)
		{
			flag2 = ((dynamic)base.Info.Args).ValidateOnly;
		}
		if (flag2)
		{
			if ((int)val == 8)
			{
				SetPreCondition("Matched");
				base.Cache["ErasingModems"] = "yes";
			}
			else
			{
				SetPreCondition("Unmatched");
				base.Cache["ErasingModems"] = "no";
			}
			val = (Result)8;
		}
		LogResult(val);
	}

	private Result Verify()
	{
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Invalid comparison between Unknown and I4
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
			string empty = string.Empty;
			DeviceMode result = (DeviceMode)8;
			if (((dynamic)base.Info.Args).DeviceMode != null)
			{
				string text = ((dynamic)base.Info.Args).DeviceMode;
				if (!Enum.TryParse<DeviceMode>(text, ignoreCase: true, out result))
				{
					string text2 = "Recipe specifies an invalid device mode, " + text;
					Smart.Log.Error(TAG, text2);
					throw new ArgumentException(text2);
				}
				Smart.Log.Debug(TAG, "Use Info.Args.DeviceMode: " + ((object)(DeviceMode)(ref result)).ToString());
			}
			else if (!val.UnknownMode)
			{
				result = val.LastMode;
				Smart.Log.Debug(TAG, "Use device.Mode: " + ((object)(DeviceMode)(ref result)).ToString());
			}
			Smart.Log.Debug(TAG, "Device Mode is DeviceMode." + ((object)(DeviceMode)(ref result)).ToString());
			empty = (((int)result == 4) ? GetFlexIdInFastboot(val) : ((!((Enum)result).HasFlag((Enum)(object)(DeviceMode)2)) ? GetFlexIdInTcmd(val) : GetFlexIdInAdb(val)));
			Smart.Log.Debug(TAG, "Read flexId is " + empty);
			string[] array = ((string)((dynamic)base.Info.Args).Expected).Split(new char[1] { ',' });
			foreach (string text3 in array)
			{
				if (empty.EndsWith(text3.Trim(), StringComparison.CurrentCultureIgnoreCase))
				{
					Smart.Log.Debug(TAG, "Match found with " + text3);
					return (Result)8;
				}
			}
			return (Result)1;
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, $"Error during Verify: {ex.Message}");
			Smart.Log.Verbose(TAG, ex.ToString());
			Smart.Thread.Wait(TimeSpan.FromSeconds(1.0));
			return (Result)4;
		}
	}

	private string GetStringFromSingleLineResp(List<string> lines, string element)
	{
		string result = string.Empty;
		foreach (string line in lines)
		{
			if (line.Contains(element))
			{
				result = line.Split(new char[1] { ':' })[1].Trim();
				break;
			}
		}
		return result;
	}

	private string GetFlexIdInFastboot(IDevice device)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		string result = string.Empty;
		string filePathName = Smart.Rsd.GetFilePathName("fastbootExe", base.Recipe.Info.UseCase, device);
		string text = "version-baseband";
		string text2 = "getvar " + text;
		int num = default(int);
		List<string> resps = Smart.MotoAndroid.Shell(device.ID, text2, 3000, filePathName, ref num, 6000, false);
		string[] array = GetStringFromResp(resps, text).Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length != 0)
		{
			result = array[0].Replace(",", string.Empty);
		}
		return result;
	}

	private string GetStringFromResp(List<string> resps, string property)
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

	private string GetFlexIdInAdb(IDevice device)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		string result = string.Empty;
		string filePathName = Smart.Rsd.GetFilePathName("adbExe", base.Recipe.Info.UseCase, device);
		string text = "shell getprop gsm.version.baseband";
		int num = default(int);
		List<string> list = Smart.MotoAndroid.Shell(device.ID, text, 3000, filePathName, ref num, 6000, false);
		string text2 = string.Empty;
		if (list.Count > 0)
		{
			text2 = list[0];
		}
		string[] array = text2.Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length != 0)
		{
			result = array[0].Replace(",", string.Empty);
		}
		return result;
	}

	private string GetFlexIdInTcmd(IDevice device)
	{
		string text = "0020";
		string text2 = "1F4A000100000050";
		byte[] data = ((ITestCommandClient)base.Cache["tcmd"]).SendCommand(text, text2).Data;
		string text3 = Smart.Convert.BytesToAscii(data).Trim(new char[1]);
		int num = text3.IndexOf('\0');
		if (num > 0)
		{
			text3 = text3.Substring(0, num);
		}
		return text3;
	}
}
