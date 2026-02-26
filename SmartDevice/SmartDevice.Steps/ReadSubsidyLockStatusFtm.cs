using System;
using ISmart;

namespace SmartDevice.Steps;

public class ReadSubsidyLockStatusFtm : FtmCfcStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		string empty = string.Empty;
		string empty2 = string.Empty;
		Result val = (Result)1;
		string text = ((object)(Result)(ref val)).ToString();
		try
		{
			CMD_CODE = "80";
			SUBSYS_ID = "21";
			SUBSYS_CMD_CODE = "64EA";
			IFtmResponse val2 = base.ftm.SendCommand(CMD_CODE + SUBSYS_ID + SUBSYS_CMD_CODE, true, true);
			string text2 = Smart.Convert.BytesToHex(val2.Raw);
			Smart.Log.Debug(TAG, "solicited resp: " + text2);
			if (EvaluateSolicitedResponse(text2) != NoError)
			{
				if (Smart.Convert.BytesToAscii(val2.Data).Contains("Item never programmed"))
				{
					text = SubsidyLockTypes.UnitIsNotLocked.ToString();
				}
				return;
			}
			if (val2.UnSolicitedResponse == null)
			{
				Smart.Log.Error(TAG, "Unsolicited response is missing");
				return;
			}
			string text3 = Smart.Convert.BytesToHex(val2.UnSolicitedResponse.Raw);
			Smart.Log.Debug(TAG, "Unsolicited resp: " + text3);
			if (EvaluateUnSolicitedResponse(text3) != NoError)
			{
				return;
			}
			if (text3.Length < 1004)
			{
				Smart.Log.Error(TAG, $"response data {text3} is of unsufficient length {text3.Length} < 1004");
				return;
			}
			string text4 = text3.Substring(36, 968);
			SubsidyLockTypes subsidyLockTypes = SubsidyLockTypes.UnitIsNotLocked;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 11; j++)
				{
					empty2 = text4.Substring(i * 242 + j * 22, 2);
					empty = text4.Substring(i * 242 + j * 22 + 2, 2);
					Smart.Log.Debug(TAG, $"slot {i + 1}, lock type {empty2}, lock value found is {empty}");
					if (!empty.Equals("00"))
					{
						if (empty.Equals("01"))
						{
							subsidyLockTypes = ((empty2 == "00") ? SubsidyLockTypes.UnitIsNWSCPLocked : ((!(empty2 == "02")) ? SubsidyLockTypes.UnitIsLockedForOtherLocks : SubsidyLockTypes.UnitIsServiceProviderLocked));
						}
						else
						{
							Smart.Log.Error(TAG, $"lock value {empty} is neither 00 nor 01");
						}
					}
				}
			}
			text = subsidyLockTypes.ToString();
			Smart.Log.Debug(TAG, "Lock Status: " + text);
			base.Log.AddInfo("SubsidyLockStatus", text);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, $"Exception ErrorMsg: {ex.Message}");
			Smart.Log.Error(TAG, ex.StackTrace);
		}
		finally
		{
			Result result = VerifyPropertyValue(text);
			SetPreCondition(text);
			LogResult(result);
		}
	}
}
