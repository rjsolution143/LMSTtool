using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SmartDevice.Steps;

public class FtmCfcStep : FtmStep
{
	protected enum SubsidyLockTypes
	{
		UnitIsNotLocked = 6,
		UnitIsNWSCPLocked,
		UnitIsServiceProviderLocked,
		UnitIsLockedForOtherLocks
	}

	protected enum STATUS_RESULTS
	{
		MMGSDI_SUCCESS,
		MMGSDI_INCORRECT_PARAMS,
		MMGSDI_CMD_QUEUE_FULL,
		MMGSDI_ERROR,
		MMGSDI_ACCESS_DENIED,
		MMGSDI_NOT_FOUND,
		MMGSDI_INCOMPAT_PIN_STATUS,
		MMGSDI_INCORRECT_CODE,
		MMGSDI_CODE_BLOCKED,
		MMGSDI_INCREASE_IMPOSSIBLE,
		MMGSDI_NOT_SUPPORTED,
		MMGSDI_NOT_INIT,
		MMGSDI_SUCCESS_BUT_ILLEGAL_SIM,
		MMGSDI_AUTH_ERROR_INCORRECT_MAC,
		MMGSDI_AUTH_ERROR_GSM_CNTXT_NOT_SUP,
		MMGSDI_SIM_TECHNICAL_PROBLEMS,
		MMGSDI_NO_EF_SELECTED,
		MMGSDI_EF_INCONSISTENT,
		MMGSDI_ERROR_NO_EVENT_NEEDED,
		MMGSDI_PIN_NOT_INITIALIZED,
		MMGSDI_UNKNOWN_INST_CLASS,
		MMGSDI_WARNING_NO_INFO_GIVEN,
		MMGSDI_WARNING_POSSIBLE_CORRUPTION,
		MMGSDI_INCORRECT_LENGTH,
		MMGSDI_UIM_CMD_TIMEOUT,
		MMGSDI_CODE_PERM_BLOCKED,
		MMGSDI_REFRESH_SUCCESS,
		MMGSDI_REFRESH_IN_PROGRESS
	}

	protected enum RESP_TYPE_OPTIONS
	{
		IMMEDIATE_RESPONSE,
		DELAYED_RESPONSE
	}

	private static Dictionary<string, string> computedMD5Hash = new Dictionary<string, string>();

	private static readonly object _genericLock = new object();

	protected string CMD_CODE = string.Empty;

	protected string SUBSYS_ID = string.Empty;

	protected string SUBSYS_CMD_CODE = string.Empty;

	protected string CMD_STATUS = string.Empty;

	protected string CMD_COUNTER = string.Empty;

	protected string RESP_TYPE = string.Empty;

	protected string STATUS = string.Empty;

	protected int ActivateSubsidyLockReceivedFailed = 1;

	protected int NoError;

	private string TAG => GetType().FullName;

	protected string ReverseString(string s)
	{
		char[] array = s.ToCharArray();
		Array.Reverse((Array)array);
		return new string(array);
	}

	protected string ByteSwapString(string s)
	{
		char[] array = s.ToCharArray();
		for (int i = 0; i < array.Length; i += 2)
		{
			char c = array[i];
			array[i] = array[i + 1];
			array[i + 1] = c;
		}
		return new string(array);
	}

	protected int EvaluateSolicitedResponse(string responseData)
	{
		CMD_STATUS = responseData.Substring(8, 8);
		CMD_COUNTER = responseData.Substring(16, 4);
		RESP_TYPE = responseData.Substring(20, 4);
		return ParseSolicitedResponse(responseData);
	}

	private int ParseSolicitedResponse(string responseData)
	{
		int noError = NoError;
		if (!CMD_CODE.ToUpper().Equals(responseData.ToUpper().Substring(0, 2)))
		{
			Smart.Log.Error(TAG, $"EXPECTED CMD_CODE= {CMD_CODE} EXPECTED SUBSYS_ID = {SUBSYS_ID} EXPECTED SUBSYS_CMD_CODE {SUBSYS_CMD_CODE} ACTUAL  CMD_CODE {responseData.Substring(0, 2)} ACTUAL SUBSYS_ID {responseData.Substring(2, 2)} ACTUAL SUBSYS_CMD_CODE {responseData.Substring(4, 4)}");
			return ActivateSubsidyLockReceivedFailed;
		}
		if (!SUBSYS_ID.ToUpper().Equals(responseData.ToUpper().Substring(2, 2)))
		{
			Smart.Log.Error(TAG, $"EXPECTED CMD_CODE= {CMD_CODE} EXPECTED SUBSYS_ID = {SUBSYS_ID} EXPECTED SUBSYS_CMD_CODE {SUBSYS_CMD_CODE} ACTUAL  CMD_CODE {responseData.Substring(0, 2)} ACTUAL SUBSYS_ID {responseData.Substring(2, 2)} ACTUAL SUBSYS_CMD_CODE {responseData.Substring(4, 4)}");
			return ActivateSubsidyLockReceivedFailed;
		}
		if (!SUBSYS_CMD_CODE.ToUpper().Equals(responseData.ToUpper().Substring(4, 4)))
		{
			Smart.Log.Error(TAG, $"EXPECTED CMD_CODE= {CMD_CODE} EXPECTED SUBSYS_ID = {SUBSYS_ID} EXPECTED SUBSYS_CMD_CODE {SUBSYS_CMD_CODE} ACTUAL  CMD_CODE {responseData.Substring(0, 2)} ACTUAL SUBSYS_ID {responseData.Substring(2, 2)} ACTUAL SUBSYS_CMD_CODE {responseData.Substring(4, 4)}");
			return ActivateSubsidyLockReceivedFailed;
		}
		if (Convert.ToInt64(ByteSwapString(ReverseString(responseData.Substring(20, 4)))) != Convert.ToInt64(RESP_TYPE_OPTIONS.IMMEDIATE_RESPONSE))
		{
			Smart.Log.Error(TAG, $"CMD_CODE= {CMD_CODE} SUBSYS_ID = {SUBSYS_ID} SUBSYS_CMD_CODE {SUBSYS_CMD_CODE} the RESP_TYPE=  {ByteSwapString(ReverseString(responseData.Substring(20, 4)))} Expected RESP_TYPE= {RESP_TYPE_OPTIONS.DELAYED_RESPONSE}");
			return ActivateSubsidyLockReceivedFailed;
		}
		if (Convert.ToInt64(responseData.Substring(8, 8)) != NoError)
		{
			Smart.Log.Error(TAG, $"CMD_CODE= {CMD_CODE} SUBSYS_ID = {SUBSYS_ID} SUBSYS_CMD_CODE {SUBSYS_CMD_CODE} with CMD_STATUS = {responseData.Substring(8, 8)} ");
			return ActivateSubsidyLockReceivedFailed;
		}
		return noError;
	}

	private int ParseUnSolicitedResponse(string responseData)
	{
		int noError = NoError;
		if (!CMD_CODE.Equals(responseData.Substring(0, 2)))
		{
			Smart.Log.Error(TAG, $"EXPECTED CMD_CODE= {CMD_CODE} EXPECTED SUBSYS_ID = {SUBSYS_ID} EXPECTED SUBSYS_CMD_CODE {SUBSYS_CMD_CODE} ACTUAL  CMD_CODE {responseData.Substring(0, 2)} ACTUAL SUBSYS_ID {responseData.Substring(2, 2)} ACTUAL SUBSYS_CMD_CODE {responseData.Substring(4, 4)}");
			return ActivateSubsidyLockReceivedFailed;
		}
		if (!SUBSYS_ID.ToUpper().Equals(responseData.ToUpper().Substring(2, 2)))
		{
			Smart.Log.Error(TAG, $"EXPECTED CMD_CODE= {CMD_CODE} EXPECTED SUBSYS_ID = {SUBSYS_ID} EXPECTED SUBSYS_CMD_CODE {SUBSYS_CMD_CODE} ACTUAL  CMD_CODE {responseData.Substring(0, 2)} ACTUAL SUBSYS_ID {responseData.Substring(2, 2)} ACTUAL SUBSYS_CMD_CODE {responseData.Substring(4, 4)}");
			return ActivateSubsidyLockReceivedFailed;
		}
		if (!SUBSYS_CMD_CODE.ToUpper().Equals(responseData.ToUpper().Substring(4, 4)))
		{
			Smart.Log.Error(TAG, $"EXPECTED CMD_CODE= {CMD_CODE} EXPECTED SUBSYS_ID = {SUBSYS_ID} EXPECTED SUBSYS_CMD_CODE {SUBSYS_CMD_CODE} ACTUAL  CMD_CODE {responseData.Substring(0, 2)} ACTUAL SUBSYS_ID {responseData.Substring(2, 2)} ACTUAL SUBSYS_CMD_CODE {responseData.Substring(4, 4)}");
			return ActivateSubsidyLockReceivedFailed;
		}
		if (Convert.ToInt64(ByteSwapString(ReverseString(responseData.Substring(20, 4)))) != Convert.ToInt64(RESP_TYPE_OPTIONS.DELAYED_RESPONSE))
		{
			Smart.Log.Error(TAG, $"CMD_CODE= {CMD_CODE} SUBSYS_ID = {SUBSYS_ID} SUBSYS_CMD_CODE {SUBSYS_CMD_CODE} the RESP_TYPE=  {ByteSwapString(ReverseString(responseData.Substring(20, 4)))} Expected RESP_TYPE= {RESP_TYPE_OPTIONS.DELAYED_RESPONSE}");
			return ActivateSubsidyLockReceivedFailed;
		}
		if (Convert.ToInt64(responseData.Substring(8, 8)) != 0L)
		{
			Smart.Log.Error(TAG, $"CMD_CODE= {CMD_CODE} SUBSYS_ID = {SUBSYS_ID} SUBSYS_CMD_CODE {SUBSYS_CMD_CODE} with CMD_STATUS = {responseData.Substring(8, 8)} ");
			return ActivateSubsidyLockReceivedFailed;
		}
		if (Convert.ToInt64(responseData.Substring(24, 8)) != 0L)
		{
			Smart.Log.Error(TAG, $"CMD_CODE= {CMD_CODE} SUBSYS_ID = {SUBSYS_ID} SUBSYS_CMD_CODE {SUBSYS_CMD_CODE} with value of {responseData.Substring(24, 8)} ");
			if (responseData.Substring(24, 8).Equals("06000000"))
			{
				Smart.Log.Error(TAG, "Device is already subsidy locked, run datablock test to unlock");
			}
			return ActivateSubsidyLockReceivedFailed;
		}
		return noError;
	}

	protected int EvaluateUnSolicitedResponse(string responseData)
	{
		CMD_STATUS = responseData.Substring(8, 8);
		CMD_COUNTER = responseData.Substring(16, 4);
		RESP_TYPE = responseData.Substring(20, 4);
		STATUS = responseData.Substring(24, 8);
		return ParseUnSolicitedResponse(responseData);
	}

	protected static string ComputeMd5Hash(string contents)
	{
		if (computedMD5Hash.ContainsKey(contents))
		{
			return computedMD5Hash[contents];
		}
		MD5 mD = MD5.Create();
		byte[] bytes = Encoding.Default.GetBytes(contents);
		string text = BitConverter.ToString(mD.ComputeHash(bytes)).Replace("-", "");
		try
		{
			lock (_genericLock)
			{
				if (!computedMD5Hash.ContainsKey(contents))
				{
					computedMD5Hash.Add(contents, text);
				}
			}
		}
		catch (Exception)
		{
		}
		return text;
	}

	public override void Run()
	{
		throw new NotImplementedException("FtmCfcStep is a base class");
	}
}
