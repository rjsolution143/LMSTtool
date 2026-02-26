using System;

namespace SmartDevice.Cfc;

public class ProgramAuthCIDViaFastbootIMEI : ProgramCIDViaFastbootIMEI
{
	public override void Execute(string imei, string scannedImei, string deviceImei, string channelId, Func<string, string> fastboot, string originalImei, string logId)
	{
		prov_req_command_string = "cid_prov_req factory_auth";
		base.Execute(imei, scannedImei, deviceImei, channelId, fastboot, originalImei, logId);
	}
}
