using System;
using ISmart;
using SmartDevice.Cfc;

namespace SmartDevice.Steps;

public class TestCommandIMEI : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string rsdLogId = val.Log.RsdLogId;
		string originalImei = val.SerialNumber;
		if (val.Log.Info.ContainsKey("OriginalImei"))
		{
			originalImei = val.Log.Info["OriginalImei"];
		}
		bool flag = false;
		if (((dynamic)base.Info.Args).Dual != null)
		{
			flag = ((dynamic)base.Info.Args).Dual;
		}
		string empty = string.Empty;
		string text = string.Empty;
		if (((dynamic)base.Info.Args).InputName != null && ((dynamic)base.Info.Args).InputName != string.Empty)
		{
			string text2 = ((dynamic)base.Info.Args).InputName;
			if (text2.ToLowerInvariant() == "default".ToLowerInvariant())
			{
				empty = "000000000000000";
				text = "000000000000000";
			}
			else
			{
				empty = base.Cache[text2];
				if (base.Cache.ContainsKey(text2 + "Dual"))
				{
					text = base.Cache[text2 + "Dual"];
				}
			}
		}
		else
		{
			empty = val.SerialNumber;
			text = val.SerialNumber2;
		}
		if (string.IsNullOrEmpty(empty) || empty == "UNKNOWN" || (flag && (string.IsNullOrEmpty(text) || text == "UNKNOWN")))
		{
			throw new NotSupportedException("Device must have a valid serial number to program IMEI");
		}
		Smart.Log.Info(TAG, $"Serial number to be programmed {empty} {text}");
		string ulma = string.Empty;
		if (((dynamic)base.Info.Args).ULMA != null)
		{
			ulma = ((dynamic)base.Info.Args).ULMA;
		}
		string text3 = string.Empty;
		if (((dynamic)base.Info.Args).FSG != null)
		{
			text3 = ((dynamic)base.Info.Args).FSG;
		}
		if (text3.StartsWith("$"))
		{
			string key = text3.Substring(1);
			text3 = base.Cache[key];
		}
		_ = (string)((dynamic)base.Info.Args).SNType;
		if ((((dynamic)base.Info.Args).Dual == null) && text != null && text.Trim() != string.Empty && empty != text)
		{
			flag = true;
		}
		if (!flag)
		{
			new ProgramIMEIDataBlock().Execute(empty, ulma, text3, TestCommand, originalImei, rsdLogId);
		}
		else
		{
			new ProgramDualIMEIDataBlock().Execute(empty, text, ulma, text3, TestCommand, originalImei, rsdLogId);
		}
		LogPass();
	}

	protected string TestCommand(string opCode, string data)
	{
		return base.tcmd.SendCommand(opCode, data).DataHex;
	}
}
