using System;
using ISmart;
using SmartDevice.Cfc;

namespace SmartDevice.Steps;

public class TestCommandCID : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).ChannelID;
		string rsdLogId = val.Log.RsdLogId;
		string originalImei = val.SerialNumber;
		if (val.Log.Info.ContainsKey("OriginalImei"))
		{
			originalImei = val.Log.Info["OriginalImei"];
		}
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
		{
			text = text.Substring(2);
		}
		string text2 = ((dynamic)base.Info.Args).SNType;
		if (text2.ToLowerInvariant() == "imei")
		{
			ProgramCidDataBlockIMEI programCidDataBlockIMEI = new ProgramCidDataBlockIMEI();
			programCidDataBlockIMEI.ProgramChannelIDInDataBlockRequest = "yes";
			programCidDataBlockIMEI.Execute(text, val.SerialNumber, val.SerialNumber, TestCommand, originalImei, rsdLogId);
		}
		else
		{
			if (!(text2.ToLowerInvariant() == "meid"))
			{
				throw new NotSupportedException("SN Type not supported: " + text2);
			}
			ProgramCidDataBlockMEID programCidDataBlockMEID = new ProgramCidDataBlockMEID();
			programCidDataBlockMEID.ProgramChannelIDInDataBlockRequest = "yes";
			programCidDataBlockMEID.Execute(text, val.SerialNumber, val.SerialNumber, TestCommand, originalImei, rsdLogId);
		}
		LogPass();
	}

	protected string TestCommand(string opCode, string data)
	{
		return base.tcmd.SendCommand(opCode, data).DataHex;
	}
}
