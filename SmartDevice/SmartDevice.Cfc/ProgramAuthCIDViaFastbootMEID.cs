using System;

namespace SmartDevice.Cfc;

public class ProgramAuthCIDViaFastbootMEID : ProgramCIDViaFastbootMEID
{
	public override void Execute(string meid, string scannedMeid, string deviceMeid, string channelId, Func<string, string> fastboot, string originalImei, string logId)
	{
		prov_req_command_string = "cid_prov_req factory_auth";
		base.Execute(meid, scannedMeid, deviceMeid, channelId, fastboot, originalImei, logId);
	}
}
