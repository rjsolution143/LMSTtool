using System;
using ISmart;
using SmartDevice.Cfc;

namespace SmartDevice.Steps;

public class FastbootFactoryCID : FastbootCID
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (device = (IDevice)((dynamic)base.Recipe.Info.Args).Device);
		string rsdLogId = val.Log.RsdLogId;
		string originalImei = val.SerialNumber;
		if (val.Log.Info.ContainsKey("OriginalImei"))
		{
			originalImei = val.Log.Info["OriginalImei"];
		}
		string text = ((dynamic)base.Info.Args).SNType;
		if (text.ToLowerInvariant() == "imei")
		{
			ProgramFactoryCIDViaFastbootIMEI programFactoryCIDViaFastbootIMEI = new ProgramFactoryCIDViaFastbootIMEI();
			programFactoryCIDViaFastbootIMEI.ProgramChannelIDInDataBlockRequest = "no";
			programFactoryCIDViaFastbootIMEI.Execute(val.SerialNumber, val.SerialNumber, val.SerialNumber, string.Empty, base.Fastboot, originalImei, rsdLogId);
		}
		else
		{
			if (!(text.ToLowerInvariant() == "meid"))
			{
				throw new NotSupportedException("SN Type not supported: " + text);
			}
			ProgramFactoryCIDViaFastbootMEID programFactoryCIDViaFastbootMEID = new ProgramFactoryCIDViaFastbootMEID();
			programFactoryCIDViaFastbootMEID.ProgramChannelIDInDataBlockRequest = "no";
			programFactoryCIDViaFastbootMEID.Execute(val.SerialNumber, val.SerialNumber, val.SerialNumber, string.Empty, base.Fastboot, originalImei, rsdLogId);
		}
		LogPass();
	}
}
