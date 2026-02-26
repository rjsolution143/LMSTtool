using System;
using ISmart;

namespace SmartDevice.Steps;

public class ReadSerialNumberMtk : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		string text = "UNKNOWN";
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		try
		{
			string text2 = "0FF0";
			if (((dynamic)base.Info.Args).OpCode != null)
			{
				text2 = ((dynamic)base.Info.Args).OpCode;
			}
			text = ((dynamic)base.Info.Args).SnType;
			string text3 = ((text.ToLowerInvariant() == "imei") ? "075443415453454E442C302C41542B4347534E" : "075443415453454E442C312C2041542B4347534E");
			if (((dynamic)base.Info.Args).Data != null)
			{
				text3 = ((dynamic)base.Info.Args).Data;
			}
			Smart.Log.Debug(TAG, $"AT cmd - opCode: {text2}, data: {text3}");
			ITestCommandResponse val2 = base.tcmd.SendCommand(text2, text3);
			string text4 = Smart.Convert.BytesToAscii(val2.Data);
			if (string.Compare(text4, "ERROR", ignoreCase: true) == 0)
			{
				text4 = "000000000000000";
			}
			Smart.Log.Info(TAG, $"{text}: {text4}");
			val.Communicating = true;
			val.LastConnected = DateTime.Now;
			if (((dynamic)base.Info.Args).Verify != null && (bool)((dynamic)base.Info.Args).Verify)
			{
				bool flag = false;
				if (!(text.EndsWith("2") ? (val.SerialNumber2 == text4) : (val.SerialNumber == text4)))
				{
					Smart.Log.Error(TAG, $"Read serial number {text4} does not match original SN");
					LogResult((Result)1, "SN mismatch");
					return;
				}
			}
			else
			{
				base.Log.AddInfo(text, text4);
				if (!text.EndsWith("2"))
				{
					val.SerialNumber = text4;
					if ((bool?)((dynamic)base.Recipe.Info.Args).Options.CopySN == true)
					{
						Smart.File.ClipboardWrite(text4);
					}
				}
				else
				{
					val.SerialNumber2 = text4;
				}
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, $"Error during SN read TCMD: {ex.Message}");
			Smart.Log.Verbose(TAG, ex.ToString());
			LogResult((Result)4, $"Error during SN read TCMD: {ex.Message}");
			return;
		}
		LogPass();
	}
}
