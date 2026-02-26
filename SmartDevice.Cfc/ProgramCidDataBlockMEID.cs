using System;

namespace SmartDevice.Cfc;

public class ProgramCidDataBlockMEID : ProgramCidLockDataBlock
{
	private string TAG => GetType().FullName;

	public string ProgramChannelIDInDataBlockRequest { get; set; }

	public void Execute(string channelId, string imei, string deviceImei, Func<string, string, string> testcommand, string originalImei, string logId)
	{
		base.CurrentDataBlockType = DataBlockTypes.MEID;
		_ = string.Empty;
		_ = string.Empty;
		if (ProgramChannelIDInDataBlockRequest.ToLower().Equals("yes"))
		{
			Smart.Log.Debug(TAG, $"Channel ID from business system is {channelId}");
			if (string.IsNullOrEmpty(channelId))
			{
				Smart.Log.Error(TAG, "ProgramChannelIDInDataBlockRequest is set to yes, a CHANNEL_ID is required in the unit data set");
				throw new NotSupportedException("Channel ID is required");
			}
			base.Channel_ID = channelId;
			if (base.Channel_ID.ToLower().StartsWith("0x"))
			{
				base.Channel_ID = base.Channel_ID.Substring(2);
			}
			Smart.Log.Debug(TAG, $"ProgramChannelIDInDataBlockRequest is set to yes, a CHANNEL_ID is {channelId}");
		}
		Execute(imei, deviceImei, testcommand, originalImei, logId);
	}
}
