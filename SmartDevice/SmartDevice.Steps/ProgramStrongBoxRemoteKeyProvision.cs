using System;
using System.Globalization;
using ISmart;

namespace SmartDevice.Steps;

public class ProgramStrongBoxRemoteKeyProvision : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			ITestCommandResponse val = base.tcmd.SendCommand("006A", "0D00");
			if (val.Failed)
			{
				string text = $"Get sb_csr failed with test command return code {val.ResponseCode}";
				Smart.Log.Error(TAG, text);
				LogResult((Result)1, text);
				return;
			}
			if (!val.DataHex.StartsWith("00"))
			{
				string text2 = $"Get sb_csr failed with response {val.DataHex.Substring(0, 2)}";
				Smart.Log.Error(TAG, text2);
				LogResult((Result)1, text2);
				return;
			}
			int num = int.Parse(val.DataHex.Substring(2, 4), NumberStyles.HexNumber);
			string text3 = Smart.Convert.BytesToAscii(val.Data).Substring(3);
			Smart.Log.Debug(TAG, $"sb_csr size {num} and sb_csr data length {text3.Length}");
			if (text3.Length != num || num == 0)
			{
				string text4 = $"SB public key size {text3.Length} does not match expected length {num} or size is 0";
				Smart.Log.Error(TAG, text4);
				LogResult((Result)1, text4);
				return;
			}
			base.Log.AddInfo("googleCsr2", text3);
			val = base.tcmd.SendCommand("006A", "0D02");
			if (val.Failed)
			{
				string text5 = $"SB RKP Complete status failed with test command return code {val.ResponseCode}";
				Smart.Log.Error(TAG, text5);
				LogResult((Result)1, text5);
				return;
			}
			if (!val.DataHex.StartsWith("00"))
			{
				string text6 = $"SB RKP Complete status failed with response {val.DataHex}";
				Smart.Log.Error(TAG, text6);
				LogResult((Result)1, text6);
				return;
			}
			val = base.tcmd.SendCommand("006A", "0D01");
			if (val.Failed)
			{
				string text7 = $"Get SB RKP status failed with test command return code {val.ResponseCode}";
				Smart.Log.Error(TAG, text7);
				LogResult((Result)1, text7);
			}
			else if (!val.DataHex.StartsWith("03"))
			{
				string text8 = $"Get SB RKP status failed with response {val.DataHex}";
				Smart.Log.Error(TAG, text8);
				LogResult((Result)1, text8);
			}
			else
			{
				LogPass();
			}
		}
		catch (Exception ex)
		{
			string text9 = $"Exception ErrMsg {ex.Message}";
			Smart.Log.Error(TAG, text9 + " stackTrace: " + ex.StackTrace);
			LogResult((Result)4, text9);
		}
	}
}
