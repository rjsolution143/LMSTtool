using System;
using System.Collections.Generic;
using System.IO;
using ISmart;
using SmartDevice.Cfc;

namespace SmartDevice.Steps;

public class FastbootCID : FastbootStep
{
	protected IDevice device;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (device = (IDevice)((dynamic)base.Recipe.Info.Args).Device);
		string rsdLogId = val.Log.RsdLogId;
		string text = string.Empty;
		string text2 = "no";
		if (((dynamic)base.Info.Args).CidInDataBlock != null)
		{
			text2 = ((dynamic)base.Info.Args).CidInDataBlock;
			text2 = text2.ToLower();
			if (text2 != "yes" && text2 != "no")
			{
				Smart.Log.Debug(TAG, $"CidInDataBlock = {text2} is not supported");
				throw new NotSupportedException($"CidInDataBlock = {text2} is not supported");
			}
		}
		if (text2 == "yes")
		{
			text = ((dynamic)base.Info.Args).ChannelID;
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = base.Cache[key];
			}
		}
		string text3 = ((dynamic)base.Info.Args).SNType;
		if (text3.ToLowerInvariant() == "imei")
		{
			ProgramCIDViaFastbootIMEI programCIDViaFastbootIMEI = new ProgramCIDViaFastbootIMEI();
			programCIDViaFastbootIMEI.ProgramChannelIDInDataBlockRequest = text2;
			programCIDViaFastbootIMEI.Execute(val.SerialNumber, val.SerialNumber, val.SerialNumber, text, Fastboot, val.SerialNumber, rsdLogId);
		}
		else
		{
			if (!(text3.ToLowerInvariant() == "meid"))
			{
				throw new NotSupportedException("SN Type not supported: " + text3);
			}
			ProgramCIDViaFastbootMEID programCIDViaFastbootMEID = new ProgramCIDViaFastbootMEID();
			programCIDViaFastbootMEID.ProgramChannelIDInDataBlockRequest = text2;
			programCIDViaFastbootMEID.Execute(val.SerialNumber, val.SerialNumber, val.SerialNumber, text, Fastboot, val.SerialNumber, rsdLogId);
		}
		LogPass();
	}

	protected string Fastboot(string command)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		string filePathName = Smart.Rsd.GetFilePathName("fastbootExe", base.Recipe.Info.UseCase, device);
		int num = 60000;
		int num2 = default(int);
		List<string> values = Smart.MotoAndroid.Shell(device.ID, command, num, filePathName, ref num2, 6000, false);
		if (num2 != 0)
		{
			Smart.Log.Error(TAG, $"Fastboot EXE command failed with status {num2}: '{command}'");
			throw new IOException($"Fastboot EXE command failed with status {num2}: '{command}'");
		}
		return string.Join(Environment.NewLine, values);
	}
}
