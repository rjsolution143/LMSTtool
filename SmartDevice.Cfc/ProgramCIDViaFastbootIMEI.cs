using System;
using ISmart;

namespace SmartDevice.Cfc;

public class ProgramCIDViaFastbootIMEI : ProgramCIDViaFastboot
{
	private string TAG => GetType().FullName;

	public virtual void Execute(string imei, string scannedImei, string deviceImei, string channelId, Func<string, string> fastboot, string originalImei, string logId)
	{
		base.CurrentDataBlockType = DataBlockTypes.IMEI;
		base.SerialNumberType = "00";
		base.UpdatePSNInDataBlockRequest = true;
		base.DataBlockType = "00F0";
		base.WebServiceDataBlockRequestType = "0x02";
		string text = imei;
		if (string.IsNullOrEmpty(text))
		{
			text = ((!string.IsNullOrEmpty(scannedImei)) ? scannedImei : "000000000000000");
		}
		if (!Smart.Convert.IsSerialNumberValid(text, (SerialNumberType)1))
		{
			Smart.Log.Debug(TAG, $"Scanned serial number {text} is a not a legal IMEI ,using default IMEI 000000000000000");
			text = "000000000000000";
		}
		Smart.Log.Info(TAG, $"program CID datablock using IMEI value {text}");
		if (!string.IsNullOrEmpty(deviceImei))
		{
			base.OldSerialNumber = deviceImei;
		}
		else
		{
			Smart.Log.Debug(TAG, $"no phone imei read, using unit data set value {text} as old serial number");
			base.OldSerialNumber = text;
		}
		if (!Smart.Convert.IsSerialNumberValid(base.OldSerialNumber, (SerialNumberType)1))
		{
			Smart.Log.Debug(TAG, "using default IMEI 000000000000000 as OldSerialNumber");
			base.OldSerialNumber = "000000000000000";
		}
		base.NewSerialNumber = text;
		if (base.ProgramChannelIDInDataBlockRequest.ToLower().Equals("yes"))
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
		base.Execute(fastboot, originalImei, logId);
	}
}
