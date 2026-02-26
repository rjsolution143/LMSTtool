using System;
using ISmart;
using SmartDevice.Cfc;

namespace SmartDevice.Steps;

public class FastbootAuthCID : FastbootCID
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (device = (IDevice)((dynamic)base.Recipe.Info.Args).Device);
		string rsdLogId = val.Log.RsdLogId;
		string text = ((dynamic)base.Info.Args).SNType;
		if (text.ToLowerInvariant() == "imei")
		{
			ProgramAuthCIDViaFastbootIMEI programAuthCIDViaFastbootIMEI = new ProgramAuthCIDViaFastbootIMEI();
			programAuthCIDViaFastbootIMEI.ProgramChannelIDInDataBlockRequest = "no";
			programAuthCIDViaFastbootIMEI.Execute(val.SerialNumber, val.SerialNumber, val.SerialNumber, string.Empty, base.Fastboot, val.SerialNumber, rsdLogId);
		}
		else
		{
			if (!(text.ToLowerInvariant() == "meid"))
			{
				throw new NotSupportedException("SN Type not supported: " + text);
			}
			ProgramAuthCIDViaFastbootMEID programAuthCIDViaFastbootMEID = new ProgramAuthCIDViaFastbootMEID();
			programAuthCIDViaFastbootMEID.ProgramChannelIDInDataBlockRequest = "no";
			programAuthCIDViaFastbootMEID.Execute(val.SerialNumber, val.SerialNumber, val.SerialNumber, string.Empty, base.Fastboot, val.SerialNumber, rsdLogId);
		}
		LogPass();
	}
}
