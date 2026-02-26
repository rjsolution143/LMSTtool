using System;

namespace SmartDevice.Cfc;

public class ProgramCIDViaFastbootMEID : ProgramCIDViaFastboot
{
	private string TAG => GetType().FullName;

	public virtual void Execute(string meid, string scannedMeid, string deviceMeid, string channelId, Func<string, string> fastboot, string originalImei, string logId)
	{
		base.CurrentDataBlockType = DataBlockTypes.MEID;
		base.SerialNumberType = "05";
		base.UpdatePSNInDataBlockRequest = true;
		base.DataBlockType = "00F0";
		base.WebServiceDataBlockRequestType = "0x02";
		string text = meid;
		if (string.IsNullOrEmpty(text))
		{
			text = ((!string.IsNullOrEmpty(scannedMeid)) ? scannedMeid : "000000000000000");
		}
		Smart.Log.Info(TAG, $"program CID datablock using MEID value {text}");
		if (!string.IsNullOrEmpty(deviceMeid))
		{
			base.OldSerialNumber = deviceMeid;
		}
		else
		{
			Smart.Log.Debug(TAG, $"no phone MEID read, using unit data set value {text} as old serial number");
			base.OldSerialNumber = text;
		}
		base.NewSerialNumber = text;
		if (base.ProgramChannelIDInDataBlockRequest.ToLower().Equals("yes"))
		{
			Smart.Log.Debug(TAG, $"Channel ID from business system is {channelId}");
			if (string.IsNullOrEmpty(channelId))
			{
				Smart.Log.Error(TAG, $"ProgramChannelIDInDataBlockRequest is set to yes, a CHANNEL_ID is required in the unit data set");
				throw new NotSupportedException("Channel ID is required");
			}
			base.Channel_ID = channelId;
			if (base.Channel_ID.ToLower().StartsWith("0x"))
			{
				base.Channel_ID = base.Channel_ID.Substring(2);
			}
			Smart.Log.Debug(TAG, $"ProgramChannelIDInDataBlockRequest is set to yes, a CHANNEL_ID is {base.Channel_ID}");
		}
		base.Execute(fastboot, originalImei, logId);
	}
}
