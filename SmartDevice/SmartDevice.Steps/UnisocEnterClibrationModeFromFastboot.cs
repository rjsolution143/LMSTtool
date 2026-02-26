using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ISmart;

namespace SmartDevice.Steps;

public class UnisocEnterClibrationModeFromFastboot : UnisocEnterClibrationMode
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
				if (DeviceInFastbootMode(device))
				{
					Smart.Thread.RunAndWait<bool>((Func<bool>)(() => FastbootPowerOff(device)), true);
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
					string text2 = "Device not in fastboot mode";
					Smart.Log.Error(TAG, text2);
					dynamic_data = dynamic_data + "_" + text2;
					prompt_for_retry = "Connect phone in Fastboot mode -> Click OK to continue";
					KillExistingExe("FrameworkDemo.exe");
				}
			}
			else if (text.Contains("<EnterMode> pass") || text.Contains("All Finished, pass") || text.Contains("<EnterMode1> pass") || text.Contains(strToIndicateTestPass))
			{
				dataList.Add(strToIndicateTestPass);
				Smart.Thread.RunAndWait<int>((Func<int>)(() => ExtractComPortFromShellResponse(dataList)), true);
				SavePortToLocalFile(base.Cache["pidvid"], base.Cache["kId"], ComportCfgFile);
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Info(TAG, ex.Message + Environment.NewLine + ex.StackTrace);
		}
	}
}
