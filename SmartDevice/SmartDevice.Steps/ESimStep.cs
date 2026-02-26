using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public abstract class ESimStep : CommServerStep
{
	internal class ProfileData
	{
		public string ProfileId { get; set; }

		public string ProfileIccid { get; set; }

		public string ProfileName { get; set; }

		public string ProfileStatus { get; set; }

		public override string ToString()
		{
			return $"Profile: ID={ProfileId}, Name={ProfileName}, Iccid={ProfileIccid}, Status={ProfileStatus},";
		}
	}

	private string TAG => GetType().FullName;

	public Result ReadESimId()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		try
		{
			SortedList<string, string> sortedList = Tell("GET_ESIM_EID");
			if (!sortedList.ContainsKey("ESIM_EID"))
			{
				Smart.Log.Debug(TAG, "Could not read eSIM EID with key ESIM_EID");
				result = (Result)1;
			}
			else
			{
				Smart.Log.Debug(TAG, string.Format("{0}: {1}", "ESIM_EID", sortedList["ESIM_EID"]));
				string text = sortedList["ESIM_EID"];
				base.Log.AddInfo("eSIMEid", text);
				base.Cache["eSIMEid"] = text;
				if (((dynamic)base.Info.Args).ExpectedEIDLength != null)
				{
					int num = ((dynamic)base.Info.Args).ExpectedEIDLength;
					if (text.Length != num)
					{
						result = (Result)1;
						string text2 = $"eSIMEid '{text}' length {text.Length} does not match expected length {num}";
						Smart.Log.Error(TAG, text2);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Exception - Failed to read ESIM ID. ErrorMessage: " + ex.Message);
			result = (Result)1;
		}
		return result;
	}

	public Result ReadActiveIccid()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		try
		{
			SortedList<string, string> sortedList = Tell("GET_ENABLED_PROFILE_ICCID");
			if (!sortedList.ContainsKey("ENABLED_PROFILE_ICCID"))
			{
				Smart.Log.Debug(TAG, "Could not read active ICCID with key ENABLED_PROFILE_ICCID");
				result = (Result)1;
			}
			else
			{
				Smart.Log.Debug(TAG, string.Format("{0}: {1}", "ENABLED_PROFILE_ICCID", sortedList["ENABLED_PROFILE_ICCID"]));
				base.Log.AddInfo("eSIMIccid", sortedList["ENABLED_PROFILE_ICCID"]);
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Exception - Failed to read active ICCID. ErrorMessage: " + ex.Message);
			result = (Result)1;
		}
		return result;
	}
}
