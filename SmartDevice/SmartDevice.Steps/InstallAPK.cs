using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class InstallAPK : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Invalid comparison between Unknown and I4
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Invalid comparison between Unknown and I4
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).APKPath;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		string text2 = string.Empty;
		if (((dynamic)base.Info.Args).Option != null)
		{
			text2 = ((dynamic)base.Info.Args).Option;
		}
		int timeoutMs = 10000;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			timeoutMs = ((dynamic)base.Info.Args).Timeout;
			timeoutMs *= 1000;
		}
		string filePathName = Smart.Rsd.GetFilePathName("adbExe", base.Recipe.Info.UseCase, val);
		string command = "install -r " + text2 + " \"" + text + "\"";
		Result result = InstallApkWithRetries(val.ID, command, timeoutMs, filePathName, text);
		VerifyOnly(ref result);
		if ((int)result == 1 || (int)result == 4)
		{
			LogResult(result, "APK installation failed", $"Failed to install {text}");
		}
		else
		{
			LogResult(result);
		}
	}

	private Result InstallApkWithRetries(string deviceID, string command, int timeoutMs, string exe, string apkPath)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		int i;
		int num = default(int);
		for (i = 1; i < 4; i++)
		{
			try
			{
				List<string> list = Smart.MotoAndroid.Shell(deviceID, command, timeoutMs, exe, ref num, 6000, false);
				Smart.Log.Debug(TAG, string.Format("Installation {0} has response {1}", apkPath, string.Join("\r\n", list.ToArray())));
				if (num == 0)
				{
					Smart.Log.Debug(TAG, $"Installed {apkPath} count: {i}");
					return (Result)8;
				}
			}
			catch (Exception ex)
			{
				Smart.Log.Debug(TAG, $"Failed to install {apkPath} count: {i}, errorMsg: {ex.Message}");
			}
			Smart.Thread.Wait(TimeSpan.FromSeconds(1.0));
		}
		Smart.Log.Error(TAG, $"Failed to install {apkPath} after {i} tries");
		return (Result)1;
	}
}
