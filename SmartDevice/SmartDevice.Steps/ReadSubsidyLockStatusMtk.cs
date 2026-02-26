using System;
using ISmart;

namespace SmartDevice.Steps;

public class ReadSubsidyLockStatusMtk : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Invalid comparison between Unknown and I4
		Result val = (Result)8;
		try
		{
			string text = "0FF0";
			if (((dynamic)base.Info.Args).OpCode != null)
			{
				text = ((dynamic)base.Info.Args).OpCode;
			}
			string text2 = "075443415453454e442c302c41542b4d4f54534d4c44423d34232b4d4f54534d4c44423a";
			if (((dynamic)base.Info.Args).Data != null)
			{
				text2 = ((dynamic)base.Info.Args).Data;
			}
			Smart.Log.Debug(TAG, $"AT cmd - opcode: {text}, data: {text2}");
			ITestCommandResponse val2 = base.tcmd.SendCommand(text, text2);
			string text3 = Smart.Convert.BytesToAscii(val2.Data);
			Smart.Log.Debug(TAG, $"AT cmd response: {text3}");
			string text4 = "UNKNOWN";
			if (text3.Equals("+MOTSMLDB:1", StringComparison.OrdinalIgnoreCase))
			{
				text4 = "locked";
			}
			else if (text3.Equals("+MOTSMLDB:0", StringComparison.OrdinalIgnoreCase))
			{
				text4 = "unlocked";
			}
			Smart.Log.Debug(TAG, $"Lock status: {text4}");
			SetPreCondition(text4);
			val = VerifyPropertyValue(text4, logOnFailed: true, "lock status");
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, $"Error during reading lock status: {ex.Message}");
			Smart.Log.Verbose(TAG, ex.ToString());
			LogResult((Result)4, $"Error during reading lock status: {ex.Message}");
			return;
		}
		if ((int)val == 8)
		{
			LogPass();
		}
	}
}
