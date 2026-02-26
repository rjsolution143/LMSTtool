using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using ISmart;

namespace SmartDevice.Steps;

public class ProgramSubsidyNvmFileGPSFtm : FtmCfcStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a77: Unknown result type (might be due to invalid IL or missing references)
		string text = string.Empty;
		string text2 = string.Empty;
		int num = -1;
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Result result = (Result)8;
		string text3 = "unlocked";
		bool flag = true;
		if (((dynamic)base.Info.Args).IsSubsdiyNVMFileMandatory != null)
		{
			flag = ((dynamic)base.Info.Args).IsSubsdiyNVMFileMandatory;
		}
		string text4 = ((dynamic)base.Info.Args).XML;
		if (text4.StartsWith("$"))
		{
			string key = text4.Substring(1);
			text4 = base.Cache[key];
		}
		string text5 = Smart.Web.GpsRsu(val.SerialNumber, val.ID);
		Smart.Log.Verbose(TAG, $"Received KD lock code with length {text5.Length} from GpsRsu web service");
		string directoryName = Path.GetDirectoryName(text4);
		XDocument obj = XDocument.Load(text4);
		IEnumerable source = (IEnumerable)Extensions.XPathEvaluate((XNode)(object)obj, "/flashing/header/subsidy_lock_config/@name");
		IEnumerable source2 = (IEnumerable)Extensions.XPathEvaluate((XNode)(object)obj, "/flashing/header/subsidy_lock_config/@MD5");
		XAttribute val2 = source.Cast<XAttribute>().FirstOrDefault();
		if ((val2 == null || string.IsNullOrEmpty(val2.Value)) && flag)
		{
			Smart.Log.Error(TAG, $"Fastboot file {text4} is missing attribute version at path /flashing/header/subsidylocknvmfile/@name");
			LogResult((Result)1, "Missing attribute version in @name path");
			return;
		}
		XAttribute val3 = source2.Cast<XAttribute>().FirstOrDefault();
		if ((val3 == null || string.IsNullOrEmpty(val3.Value)) && flag)
		{
			Smart.Log.Error(TAG, $"Fastboot file {text4} is missing attribute version at path /flashing/header/subsidylocknvmfile/@MD5");
			LogResult((Result)1, "Missing attribute version in @MD5 path");
			return;
		}
		if (val3 != null && !string.IsNullOrEmpty(val3.Value))
		{
			string text6 = FtmCfcStep.ComputeMd5Hash(File.ReadAllText(directoryName + "\\" + val2.Value));
			if (!text6.ToLower().Equals(val3.Value.ToLower()))
			{
				Smart.Log.Error(TAG, $"Fastboot MD5 {val3.Value.ToLower()} does not match calculated MD5 {text6.ToLower()}");
				LogResult((Result)1, "MD5 mismatch");
				return;
			}
		}
		if (val2 != null && !string.IsNullOrEmpty(val2.Value))
		{
			string text7 = Path.Combine(directoryName, val2.Value);
			string[] array = File.ReadAllLines(text7);
			if (array == null || array.Count() == 0)
			{
				Smart.Log.Debug(TAG, $"SubsidyNvm file {text7} is empty, no subsidy lock required");
				SetPreCondition("unlocked");
				LogPass();
				return;
			}
			string[] array2 = new string[array.Length];
			int num2 = 0;
			string empty = string.Empty;
			string[] array3 = array;
			foreach (string text8 in array3)
			{
				empty = text8;
				if (string.IsNullOrEmpty(text8))
				{
					array2[num2++] = text8;
					continue;
				}
				int num3 = text8.ToUpper().IndexOf("<CK-");
				if (num3 != -1)
				{
					Smart.Log.Error(TAG, string.Format("SubsidyNvm file {0} contains <CK-, not supported by this program subsidy NVM with GPS code", directoryName + "\\" + val2.Value));
					LogResult((Result)1, "CK not supported");
					return;
				}
				num3 = text8.ToUpper().IndexOf("<HCK-");
				if (num3 != -1)
				{
					Smart.Log.Error(TAG, string.Format("SubsidyNvm file {0} contains <HCK-, not supported by this program subsidy NVM with GPS code", directoryName + "\\" + val2.Value));
					LogResult((Result)1, "HCK not supported");
					return;
				}
				num3 = text8.ToUpper().IndexOf("<KD-");
				int num4 = -1;
				if (num3 != -1)
				{
					num4 = text8.ToUpper().IndexOf(">", num3);
					if (num4 == -1)
					{
						Smart.Log.Error(TAG, string.Format("SubsidyNvm file {0} is missing > for <KD-", directoryName + "\\" + val2.Value));
						LogResult((Result)1, "Malformed KD block");
						return;
					}
					text = text8.Substring(num3 + 1, 2);
					string text9 = text8.Substring(num3 + 4, num4 - (num3 + 4));
					try
					{
						num = int.Parse(text9);
					}
					catch (Exception ex)
					{
						Smart.Log.Error(TAG, $"parsing error {ex.Message} on lock code length {text9}");
						LogResult((Result)1, "Generic parsing error");
						return;
					}
					if (text5.Length / 2 != num)
					{
						Smart.Log.Error(TAG, $"VZW_SUBSIDY_KD key length {text5.Length / 2} mis-match NVM command requested {num}");
						LogResult((Result)1, "Key length mismatch");
						return;
					}
					empty = text8.ToUpper().Replace("<KD-" + text9 + ">", text5);
				}
				else if (!string.IsNullOrEmpty(text) && text.ToUpper().Equals("KD") && text8.ToUpper().StartsWith("802162EA") && text8.ToUpper().Contains("802162EA"))
				{
					text2 = text8.Substring(10, 2);
					Smart.Log.Debug(TAG, "Lock code type: " + text2);
				}
				array2[num2++] = empty;
			}
			if (string.IsNullOrEmpty(text))
			{
				Smart.Log.Error(TAG, string.Format("SubsidyNvm file {0} does not contains VZW_SUBSIDY_KD type", directoryName + "\\" + val2.Value));
				LogResult((Result)1, "KD type not found in file");
				return;
			}
			Smart.Log.Debug(TAG, "Sending VZW KD Subsidy Lock commands...");
			empty = string.Empty;
			array3 = array2;
			foreach (string text10 in array3)
			{
				empty = text10;
				if (!string.IsNullOrEmpty(text10))
				{
					Smart.Log.Verbose(TAG, $"Sending line to phone: {text10}");
					CMD_CODE = empty.Substring(0, 2);
					SUBSYS_ID = empty.Substring(2, 2);
					SUBSYS_CMD_CODE = empty.Substring(4, 4);
					IFtmResponse val4 = base.ftm.SendCommand(text10, true, true);
					string responseData = Smart.Convert.BytesToHex(val4.Raw);
					if (EvaluateSolicitedResponse(responseData) != NoError)
					{
						Smart.Log.Debug(TAG, "KD Subsidy lock command failed - bad solicited response");
						LogResult((Result)1, "KD Subsidy lock command failed - bad solicited response");
						return;
					}
					if (val4.UnSolicitedResponse == null)
					{
						Smart.Log.Error(TAG, "Unsolicited response is missing");
						LogResult((Result)1, "Unsolicited response is missing");
						return;
					}
					string responseData2 = Smart.Convert.BytesToHex(val4.UnSolicitedResponse.Raw);
					if (EvaluateUnSolicitedResponse(responseData2) != NoError)
					{
						Smart.Log.Debug(TAG, "KD Subsidy lock command failed - bad unsolicited response");
						LogResult((Result)1, "KD Subsidy lock command failed - - bad unsolicited response");
						return;
					}
				}
			}
			SubsidyLockTypes subsidyLockTypes = SubsidyLockTypes.UnitIsNotLocked;
			if (text2 != string.Empty)
			{
				base.Cache["lock1"] = text5;
				text3 = "locked";
				subsidyLockTypes = ((text2 == "00") ? SubsidyLockTypes.UnitIsNWSCPLocked : ((!(text2 == "02")) ? SubsidyLockTypes.UnitIsLockedForOtherLocks : SubsidyLockTypes.UnitIsServiceProviderLocked));
			}
			Smart.Log.Debug(TAG, "Lock Status: " + text3);
			Smart.Log.Debug(TAG, "subsidyLock type: " + subsidyLockTypes);
			base.Log.AddInfo("SubsidyLockStatus", subsidyLockTypes.ToString());
			SetPreCondition(text3);
			LogResult(result);
		}
		else
		{
			Smart.Log.Error(TAG, $"SUBSIDY_NVM_FROM_XML is not defined");
			LogResult((Result)1, "SUBSIDY_NVM_FROM_XML is not defined");
		}
	}
}
