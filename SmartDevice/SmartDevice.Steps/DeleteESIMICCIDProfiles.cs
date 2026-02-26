using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class DeleteESIMICCIDProfiles : ESimStep
{
	private bool mESimLogEnabled;

	private IDevice mDevice;

	private string mESimLogDir;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Invalid comparison between Unknown and I4
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Invalid comparison between Unknown and I4
		//IL_0755: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Unknown result type (might be due to invalid IL or missing references)
		mDevice = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		GetUserSettings();
		Result val = (Result)1;
		string text = Smart.Locale.Xlate(base.Name);
		if (!base.Log.Info.ContainsKey("eSIMEid"))
		{
			val = ReadESimId();
			if ((int)val != 8)
			{
				LogResult((Result)4, "Failed to read ESIM EId");
				return;
			}
		}
		if (!base.Log.Info.ContainsKey("eSIMIccid"))
		{
			ReadActiveIccid();
		}
		Thread.Sleep(5000);
		SortedList<string, string> sortedList = Tell("GET_LPA_TEST_MODE");
		string text2 = sortedList["TEST_MODE_STATUS"];
		Smart.Log.Debug(TAG, "TEST_MODE_STATUS=" + text2);
		if (text2 == "1")
		{
			string text3 = Smart.Locale.Xlate("eSIM LPA test mode is enabled now. Please run eSIM Switch usecase to disable the test mode");
			Smart.User.MessageBox(text, text3, (MessageBoxButtons)0, (MessageBoxIcon)64);
			LogResult((Result)4, "eSIM LPA Test Mode is enabled");
			return;
		}
		if (text2 == "FF")
		{
			string text3 = Smart.Locale.Xlate("eSIM LPA mode is UNKNOWN. Please run eSIM Switch usecase to clear UNKNOWN state");
			Smart.User.MessageBox(text, text3, (MessageBoxButtons)0, (MessageBoxIcon)64);
			LogResult((Result)4, "eSIM LPA mode unknown");
			return;
		}
		List<ProfileData> list = ReadProfileList();
		FilterProfiles(list);
		if (list.Count == 0)
		{
			Smart.Log.Debug(TAG, "There is no ESIM ICCID Profile to delete");
			CreateESimLog(base.Log.Info["eSIMEid"], (Result)8);
			LogResult((Result)8, "No ICCID profile to delete");
			return;
		}
		if (((dynamic)base.Info.Args).PromptText != null)
		{
			mDevice.Prompt.CloseMessageBox();
			string text4 = ((dynamic)base.Info.Args).PromptText.ToString();
			string text3 = Smart.Locale.Xlate(text4) + "\r\n" + GetProfileNames(list);
			if ((int)Smart.User.MessageBox(text, text3, (MessageBoxButtons)4, (MessageBoxIcon)32) != 6)
			{
				LogResult((Result)4, "User not confirm deletion of eSIM profiles");
				Smart.Log.Debug(TAG, "User not confirm deletion of eSIM profiles");
				return;
			}
		}
		try
		{
			string text5 = "DELETE_RESULT=0";
			if (((dynamic)base.Info.Args).ExpectedDeleteResult != null)
			{
				text5 = ((dynamic)base.Info.Args).ExpectedDeleteResult;
			}
			string text6 = text5.Split(new char[1] { '=' })[0];
			string text7 = text5.Split(new char[1] { '=' })[1];
			foreach (ProfileData item in list)
			{
				Smart.Log.Debug(TAG, "Try to Delete " + item.ToString());
				for (int i = 0; i < 3; i++)
				{
					sortedList = Tell("DELETE_PROFILE ESIM_ID=" + item.ProfileId);
					if (sortedList.Keys.Contains(text6) && sortedList[text6] == text7)
					{
						Smart.Log.Info(TAG, "Delete " + item.ToString() + " success");
						break;
					}
					Smart.Log.Error(TAG, "Delete " + item.ToString() + " error, retry");
				}
				Thread.Sleep(10000);
			}
			list = ReadProfileList();
			FilterProfiles(list);
			if (list.Count > 0)
			{
				LogResult((Result)1, "Failed to delete some profiles");
				val = (Result)1;
				Smart.Log.Error(TAG, $"Fail to delete {list.Count} profiles");
			}
			else
			{
				LogResult((Result)8);
				val = (Result)8;
			}
		}
		catch (Exception ex)
		{
			LogResult((Result)1, "Exception when deleting profile");
			Smart.Log.Error(TAG, "Exception - Error message:" + ex.Message);
			val = (Result)1;
		}
		finally
		{
			CreateESimLog(base.Log.Info["eSIMEid"], val);
		}
	}

	private void FilterProfiles(List<ProfileData> profiles)
	{
		string text = "CRTC,ALW0007a,LTE-Advanced,Agilent,R&S,Schwarz,Anritsu,Thales,GSMA_TEST";
		if (((dynamic)base.Info.Args).ProfileNameToBeReserved != null)
		{
			text = (string)((dynamic)base.Info.Args).ProfileNameToBeReserved;
		}
		for (int num = profiles.Count - 1; num >= 0; num--)
		{
			ProfileData profileData = profiles[num];
			string profileName = profileData.ProfileName.Trim();
			if (text.Split(new char[1] { ',' }).ToList().Any((string word) => profileName.Contains(word)))
			{
				Smart.Log.Debug(TAG, $"Profile {profileName} need to be reserved, remove from current list");
				profiles.RemoveAt(num);
			}
		}
	}

	private string CreateXmlContent(string eSimId, Result result)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		string serialNumber = mDevice.SerialNumber;
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		string text2 = ((StationDescriptor)(ref stationDescriptor)).ToId();
		string text3 = (((int)result == 8) ? "pass" : "fail");
		return "<?xml version=\"1.0\" encoding=\"UTF-8\"?><ProfileResetStatus><TimeStamp>\"" + text + "\"</TimeStamp><IMEI>\"" + serialNumber + "\"</IMEI><EID>\"" + eSimId + "\"</EID><StationID>\"" + text2 + "\"</StationID><ProfileDeletionOutcome>\"" + text3 + "\"</ProfileDeletionOutcome></ProfileResetStatus>";
	}

	private string GetXmlFilePathName()
	{
		string path = mDevice.SerialNumber + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".XML";
		return Path.Combine(mESimLogDir, path);
	}

	private void CreateESimLog(string eSimId, Result result)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Smart.Log.Debug(TAG, $"CreateSimLog eSImId: {eSimId}, result: {((object)(Result)(ref result)).ToString()} mESimLogEnabled: {mESimLogEnabled}");
		if (mESimLogEnabled)
		{
			string text = CreateXmlContent(eSimId, result);
			string xmlFilePathName = GetXmlFilePathName();
			File.WriteAllText(xmlFilePathName, text, Encoding.UTF8);
			Smart.Log.Debug(TAG, $"Successfully saved xml file to {xmlFilePathName}, content:\r\n{text}");
		}
	}

	private List<ProfileData> ReadProfileList()
	{
		List<ProfileData> list = new List<ProfileData>();
		SortedList<string, string> sortedList = Tell("REFRESH");
		Thread.Sleep(5000);
		try
		{
			sortedList = Tell("GET_ESIM_PROFILE_LIST");
			string text = sortedList["PROFILE_LIST_LEN"];
			int num = Convert.ToInt32(text);
			Smart.Log.Debug(TAG, $"eSIM profile list lenght is {text}");
			for (int i = 1; i <= num; i++)
			{
				ProfileData profileData = new ProfileData();
				string text2 = "PROFILE" + i;
				profileData.ProfileId = sortedList[text2 + "_ID"];
				profileData.ProfileIccid = sortedList[text2 + "_ICCID"];
				profileData.ProfileName = sortedList[text2 + "_NAME"];
				profileData.ProfileStatus = sortedList[text2 + "_STATUS"];
				Smart.Log.Debug(TAG, "Found " + profileData.ToString());
				list.Add(profileData);
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Debug(TAG, "CQA replies 'impolitely' that there is no profile: ");
			Smart.Log.Debug(TAG, ex.Message);
		}
		return list;
	}

	private void GetUserSettings()
	{
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		bool flag = true;
		IThreadLocked val = Smart.Rsd.LocalOptions();
		string text;
		try
		{
			dynamic data = val.Data;
			text = data.ESimLogPath;
			if (data.ESimLogging != null)
			{
				flag = data.ESimLogging;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		mESimLogEnabled = flag;
		if (flag)
		{
			string filePathName = Smart.Rsd.GetFilePathName("eSimLogs", base.Recipe.Info.UseCase, mDevice);
			if (string.IsNullOrEmpty(text))
			{
				text = filePathName;
			}
			else if (!Directory.Exists(text))
			{
				try
				{
					Directory.CreateDirectory(text);
				}
				catch (Exception ex)
				{
					Smart.Log.Error(TAG, $"Failed to create directory {text} - Error: {ex.Message}");
					text = filePathName;
				}
			}
		}
		mESimLogDir = text;
	}

	private string GetProfileNames(List<ProfileData> profiles)
	{
		string text = string.Empty;
		foreach (ProfileData profile in profiles)
		{
			text = text + profile.ProfileName + ", ";
		}
		if (text.Length > 0)
		{
			text = text.Substring(0, text.Length - 2);
		}
		return text;
	}
}
