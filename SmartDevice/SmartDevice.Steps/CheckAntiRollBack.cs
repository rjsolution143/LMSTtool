using System;
using System.Collections.Generic;
using System.IO;
using ISmart;

namespace SmartDevice.Steps;

public class CheckAntiRollBack : BaseStep
{
	private static Dictionary<string, string> mDeviceToSigningInfo = new Dictionary<string, string>
	{
		{ "vbmeta", "HAB_SECURITY_VERSION" },
		{ "RIL #0", "HAB_SECURITY_VERSION" }
	};

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_074c: Unknown result type (might be due to invalid IL or missing references)
		//IL_071a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0798: Unknown result type (might be due to invalid IL or missing references)
		//IL_0811: Unknown result type (might be due to invalid IL or missing references)
		//IL_0838: Unknown result type (might be due to invalid IL or missing references)
		//IL_0856: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = "signing-info.txt";
		if (((dynamic)base.Info.Args).SigningInfoFile != null)
		{
			text = ((dynamic)base.Info.Args).SigningInfoFile;
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = base.Cache[key];
			}
		}
		string text2 = "oem read_sv";
		if (((dynamic)base.Info.Args).Command != null)
		{
			text2 = ((dynamic)base.Info.Args).Command;
			if (text2.StartsWith("$"))
			{
				string key2 = text2.Substring(1);
				text2 = base.Cache[key2];
			}
		}
		int num = 10000;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
			num *= 1000;
		}
		string filePathName = Smart.Rsd.GetFilePathName(text, base.Recipe.Info.UseCase, val);
		Result val2;
		if (!File.Exists(filePathName))
		{
			Smart.Log.Debug(TAG, $"File {filePathName} not found");
			val2 = (Result)7;
			SetPreCondition(((object)(Result)(ref val2)).ToString());
			LogResult((Result)7);
			return;
		}
		string[] array = File.ReadAllLines(filePathName);
		if (array.Length == 0)
		{
			Smart.Log.Debug(TAG, $"file {filePathName} is empty");
			val2 = (Result)7;
			SetPreCondition(((object)(Result)(ref val2)).ToString());
			LogResult((Result)7);
			return;
		}
		int signingInfoSecVersion = GetSigningInfoSecVersion(array, new List<string>(mDeviceToSigningInfo.Values));
		Smart.Log.Debug(TAG, $"signingInfoSecVersion: {signingInfoSecVersion}");
		if (signingInfoSecVersion == -1)
		{
			Smart.Log.Debug(TAG, $"Cannot get signingInfoSecVersion in file {filePathName}");
			val2 = (Result)7;
			SetPreCondition(((object)(Result)(ref val2)).ToString());
			LogResult((Result)7);
			return;
		}
		string filePathName2 = Smart.Rsd.GetFilePathName("fastbootExe", base.Recipe.Info.UseCase, val);
		int num2 = default(int);
		List<string> resps = Smart.MotoAndroid.Shell(val.ID, text2, num, filePathName2, ref num2, 6000, false);
		if (num2 != 0)
		{
			Smart.Log.Debug(TAG, $"Command {text2} failed");
			val2 = (Result)7;
			SetPreCondition(((object)(Result)(ref val2)).ToString());
			LogResult((Result)7);
			return;
		}
		int deviceSecVersion = GetDeviceSecVersion(resps, new List<string>(mDeviceToSigningInfo.Keys));
		Smart.Log.Debug(TAG, $"deviceSecVersion: {deviceSecVersion}");
		if (deviceSecVersion == -1)
		{
			Smart.Log.Debug(TAG, $"Cannot get deviceSecVersion in 'fastboot {text2}' response");
			val2 = (Result)7;
			SetPreCondition(((object)(Result)(ref val2)).ToString());
			LogResult((Result)7);
		}
		else
		{
			Result result = (Result)((deviceSecVersion > signingInfoSecVersion) ? 1 : 8);
			SetPreCondition(((object)(Result)(ref result)).ToString());
			VerifyOnly(ref result);
			LogResult(result);
		}
	}

	private int GetSigningInfoSecVersion(string[] signingInfoLines, List<string> keys)
	{
		int result = -1;
		string[] separator = new string[1] { " " };
		foreach (string text in signingInfoLines)
		{
			foreach (string key in keys)
			{
				if (text.Contains(key))
				{
					string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
					if (array.Length > 1)
					{
						result = int.Parse(array[1]);
						return result;
					}
				}
			}
		}
		return result;
	}

	private int GetDeviceSecVersion(List<string> resps, List<string> keys)
	{
		int result = -1;
		foreach (string resp in resps)
		{
			foreach (string key in keys)
			{
				if (!resp.Contains(key))
				{
					continue;
				}
				string[] array = resp.Split(new char[1] { '=' });
				if (array.Length > 1)
				{
					if (array[1].ToLower().Contains("0x"))
					{
						result = Convert.ToInt32(array[1].Trim(), 16);
					}
					else if (!int.TryParse(array[1].Trim(), out result))
					{
						result = -1;
					}
					return result;
				}
			}
		}
		return result;
	}
}
