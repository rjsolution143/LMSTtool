using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class SwitchESIMProfile : ESimStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Invalid comparison between Unknown and I4
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Invalid comparison between Unknown and I4
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Result val2 = (Result)8;
		Dictionary<string, ProfileData> dictionary = new Dictionary<string, ProfileData>();
		try
		{
			if (!base.Log.Info.ContainsKey("eSIMEid"))
			{
				val2 = ReadESimId();
				if ((int)val2 != 8)
				{
					LogResult((Result)4, "Failed to read ESIM EId");
					return;
				}
			}
			if (!base.Log.Info.ContainsKey("eSIMIccid"))
			{
				ReadActiveIccid();
			}
			Thread.Sleep(1000);
			SortedList<string, string> sortedList = Tell("REFRESH");
			Thread.Sleep(5000);
			sortedList = Tell("SET_LPA_TEST_MODE");
			Smart.Log.Debug(TAG, "Switched mode");
			Thread.Sleep(5000);
			sortedList = Tell("REFRESH");
			Thread.Sleep(5000);
			try
			{
				sortedList = Tell("GET_ESIM_PROFILE_LIST");
			}
			catch (Exception ex)
			{
				Smart.Log.Debug(TAG, "CQA replies 'impolitely' that there is no profile: ");
				Smart.Log.Debug(TAG, ex.Message);
				LogPass();
				return;
			}
			string text = sortedList["PROFILE_LIST_LEN"];
			int num = Convert.ToInt32(text);
			Smart.Log.Debug(TAG, $"eSIM profile list lenght is {text}");
			if (num == 0)
			{
				LogPass();
				return;
			}
			for (int i = 1; i <= num; i++)
			{
				ProfileData profileData = new ProfileData();
				string text2 = $"PROFILE{i}";
				profileData.ProfileId = sortedList[text2 + "_ID"];
				profileData.ProfileIccid = sortedList[text2 + "_ICCID"];
				profileData.ProfileName = sortedList[text2 + "_NAME"];
				profileData.ProfileStatus = sortedList[text2 + "_STATUS"];
				Smart.Log.Debug(TAG, $"eSIM Profile {i}: ID={profileData.ProfileId}, Status={profileData.ProfileStatus}, Name={profileData.ProfileName}, Iccid={profileData.ProfileIccid}");
				Add(dictionary, profileData);
			}
			List<string> list = new List<string>(dictionary.Keys);
			list.Sort();
			string text3 = string.Empty;
			Smart.Log.Debug(TAG, "Checking for default profile");
			bool flag = false;
			IThreadLocked val3 = Smart.Rsd.LocalOptions();
			try
			{
				dynamic data = val3.Data;
				if (data.DefaultESim != null)
				{
					flag = data.DefaultESim;
				}
				if (flag && data.ESimProfile != null)
				{
					text3 = data.ESimProfile;
				}
				if (!list.Contains(text3))
				{
					Smart.Log.Debug(TAG, "Could not find default profile: " + text3);
					text3 = string.Empty;
				}
				if (text3 == string.Empty)
				{
					string text4 = Smart.Locale.Xlate("eSIM Profiles");
					string text5 = Smart.Locale.Xlate("Please select eSIM profile name to activate");
					DialogResult val4 = val.Prompt.SearchSelect(text4, text5, list, ref text3);
					if ((int)val4 != 1)
					{
						Smart.Log.Error(TAG, $"Diaglog result: {((object)(DialogResult)(ref val4)).ToString()}");
						throw new Exception("Invalid user action");
					}
				}
				Smart.Log.Debug(TAG, "Selected Profile: " + text3);
				if (flag && text3 != data.ESimProfile)
				{
					Smart.Log.Debug(TAG, $"Setting selected profile {text3} as default profile");
					data.ESimProfile = text3;
					val3.Data = (object)data;
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			if (!dictionary[text3].ProfileStatus.Equals("Enabled", StringComparison.OrdinalIgnoreCase))
			{
				sortedList = Tell($"SWITCH_TO_PROFILE ESIM_ID={dictionary[text3].ProfileId}");
				Smart.Log.Debug(TAG, "Switched to profile " + text3);
				Thread.Sleep(1000);
			}
		}
		catch (Exception ex2)
		{
			Smart.Log.Error(TAG, $"Exception - errorMsg: {ex2.Message}");
			Smart.Log.Error(TAG, ex2.StackTrace);
			if (!ignoreErrors)
			{
				throw;
			}
			val2 = (Result)7;
		}
		LogResult(val2);
	}

	private void Add(Dictionary<string, ProfileData> profileLookup, ProfileData profile)
	{
		bool flag = false;
		string key = profile.ProfileName;
		int num = 1;
		while (!flag)
		{
			if (profileLookup.ContainsKey(key))
			{
				key = profile.ProfileName + "-" + num++;
				continue;
			}
			profileLookup.Add(key, profile);
			flag = true;
		}
	}
}
