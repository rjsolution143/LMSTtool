using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class DownloadLogsADB : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0919: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (!((Enum)val.LastMode).HasFlag((Enum)(object)(DeviceMode)2))
		{
			Smart.Log.Error(TAG, $"Unsupported mode for ADB: {val.LastMode}");
			DeviceMode lastMode = val.LastMode;
			LogResult((Result)4, "Device not in ADB mode", "Device mode is " + ((object)(DeviceMode)(ref lastMode)).ToString());
			return;
		}
		int num = 200000;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
			num *= 1000;
		}
		string arg = "log";
		if (((dynamic)base.Info.Args).LogLabel != null && ((dynamic)base.Info.Args).LogLabel != string.Empty)
		{
			arg = ((dynamic)base.Info.Args).LogLabel.ToString();
		}
		string text = "UNKNOWN";
		if (((dynamic)base.Info.Args).LogCommand != null && ((dynamic)base.Info.Args).LogCommand != string.Empty)
		{
			string text2 = ((dynamic)base.Info.Args).LogCommand.ToString();
			text = Smart.ADB.Shell(val.ID, text2, num);
		}
		else
		{
			string filePathName = Smart.Rsd.GetFilePathName("adbExe", base.Recipe.Info.UseCase, val);
			string text3 = "logcat";
			int num2 = -1;
			List<string> list = Smart.MotoAndroid.Shell(val.ID, text3, num, filePathName, ref num2, 6000, false);
			text = string.Join("\r\n", list.ToArray());
		}
		string commonStorageDir = Smart.File.CommonStorageDir;
		string text4 = "DeviceLogs";
		commonStorageDir = Smart.File.PathJoin(commonStorageDir, text4);
		string serialNumber = val.SerialNumber;
		string arg2 = DateTime.Now.ToString("MM-dd-yyyy-HH-mm-ss");
		string text5 = $"{serialNumber}_{arg}_{arg2}.log";
		commonStorageDir = Smart.File.PathJoin(commonStorageDir, text5);
		Smart.File.WriteText(commonStorageDir, text);
		Smart.Log.Debug(TAG, text);
		LogPass();
	}
}
