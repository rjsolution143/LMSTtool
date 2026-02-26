using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using ISmart;

namespace SmartDevice.Steps;

public class UnisocEnterClibrationModeFromAdb : UnisocEnterClibrationMode
{
	private string TAG => GetType().FullName;

	public override void Redirected(List<string> dataList, object sender, DataReceivedEventArgs e)
	{
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (e.Data == null)
		{
			return;
		}
		Smart.Log.Debug(TAG, e.Data);
		string text = e.Data.Trim();
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		dataList.Add(text);
		try
		{
			if (text.ToLower().Contains("Start to <EnterMode".ToLower()))
			{
				if (DeviceInAdbMode(device))
				{
					Smart.Thread.RunAndWait<bool>((Func<bool>)(() => AdbPowerOff(device)), true);
					Smart.Thread.Run((ThreadStart)delegate
					{
						GetAndSaveComportToLocalFileByPidVid(base.Cache["pidvid"]);
					}, true);
				}
				else if (DeviceInCalMode(base.Cache["pidvid"]))
				{
					Smart.Log.Error(TAG, "Device already in cal mode,and com port number is already updated to correct number last time");
				}
				else
				{
					string text2 = "Device not in adb mode";
					Smart.Log.Error(TAG, text2);
					dynamic_data = dynamic_data + "_" + text2;
					prompt_for_retry = "Connect phone in FASTBOOT mode -> Click OK to continue";
					KillExistingExe("FrameworkDemo.exe");
				}
			}
			else if (text.Contains("<EnterMode> pass") || text.Contains("All Finished, pass") || text.Contains("<EnterMode1> pass") || text.Contains(strToIndicateTestPass))
			{
				dataList.Add(strToIndicateTestPass);
				Smart.Thread.RunAndWait<int>((Func<int>)(() => ExtractComPortFromShellResponse(dataList)), true);
				SavePortToLocalFile(base.Cache["pidvid"], base.Cache["kid"], ComportCfgFile);
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Info(TAG, ex.Message + Environment.NewLine + ex.StackTrace);
			throw ex;
		}
	}

	private bool AdbPowerOff(IDevice device)
	{
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		Smart.Log.Info(MethodBase.GetCurrentMethod().Name, "enter...");
		bool flag = true;
		string text = "reboot -p";
		if (((dynamic)base.Info.Args).PowerOffCommand != null && ((dynamic)base.Info.Args).PowerOffCommand != string.Empty)
		{
			text = ((dynamic)base.Info.Args).PowerOffCommand;
		}
		int num = 10;
		if (((dynamic)base.Info.Args).PowerOffTimeoutSec != null && ((dynamic)base.Info.Args).PowerOffTimeoutSec != string.Empty)
		{
			num = ((dynamic)base.Info.Args).PowerOffTimeoutSec;
		}
		num *= 1000;
		string text2 = "Done";
		string empty = string.Empty;
		try
		{
			string filePathName = Smart.Rsd.GetFilePathName("adbExe", base.Recipe.Info.UseCase, device);
			int num2 = -1;
			List<string> list = Smart.MotoAndroid.Shell(device.ID, text, num, filePathName, ref num2, 6000, false);
			empty = string.Join("\r\n", list.ToArray());
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error sending ADB Command: " + ex.Message);
			Smart.Log.Error(TAG, ex.ToString());
			LogResult((Result)4, "Could not send ADB Command", ex.Message);
			return false;
		}
		if (empty.ToUpper().Contains(text2.ToUpper()))
		{
			Smart.Log.Info(TAG, "adb power off pass");
			return true;
		}
		Smart.Log.Info(TAG, "adb power off fail");
		return false;
	}
}
