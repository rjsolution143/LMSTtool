using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using ISmart;

namespace SmartDevice.Steps;

public class ProgramSubsidyNvmFileGenLockcodeFtm : FtmCfcStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_096c: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		string text = "unlocked";
		string error = string.Empty;
		string empty = string.Empty;
		string empty2 = string.Empty;
		string text2 = string.Empty;
		int num = -1;
		bool flag = true;
		if (((dynamic)base.Info.Args).IsSubsdiyNVMFileMandatory != null)
		{
			flag = ((dynamic)base.Info.Args).IsSubsdiyNVMFileMandatory;
		}
		string text3 = ((dynamic)base.Info.Args).XML;
		if (text3.StartsWith("$"))
		{
			string key = text3.Substring(1);
			text3 = base.Cache[key];
		}
		SubsidyLockRandomGenerator subsidyLockRandomGenerator = SubsidyLockRandomGenerator.CreateSubsidyLockRandomGenerator();
		string directoryName = Path.GetDirectoryName(text3);
		XDocument obj = XDocument.Load(text3);
		IEnumerable source = (IEnumerable)Extensions.XPathEvaluate((XNode)(object)obj, "/flashing/header/subsidy_lock_config/@name");
		IEnumerable source2 = (IEnumerable)Extensions.XPathEvaluate((XNode)(object)obj, "/flashing/header/subsidy_lock_config/@MD5");
		XAttribute val = source.Cast<XAttribute>().FirstOrDefault();
		if ((val == null || string.IsNullOrEmpty(val.Value)) && flag)
		{
			Smart.Log.Error(TAG, $"Fastboot file {text3} is missing attribute version at path /flashing/header/subsidylocknvmfile/@name");
			LogResult((Result)1, "Missing attribute version in @name");
			return;
		}
		XAttribute val2 = source2.Cast<XAttribute>().FirstOrDefault();
		if ((val2 == null || string.IsNullOrEmpty(val2.Value)) && flag)
		{
			Smart.Log.Error(TAG, $"Fastboot file {text3} is missing attribute version at path /flashing/header/subsidylocknvmfile/@MD5");
			LogResult((Result)1, "Missing attribute version in @MD5");
			return;
		}
		if (val2 != null && !string.IsNullOrEmpty(val2.Value))
		{
			string text4 = FtmCfcStep.ComputeMd5Hash(File.ReadAllText(directoryName + "\\" + val.Value));
			if (!text4.ToLower().Equals(val2.Value.ToLower()))
			{
				Smart.Log.Error(TAG, $"Fastboot MD5 {val2.Value.ToLower()} does not match calculated MD5 {text4.ToLower()}");
				LogResult((Result)1, "MD5 mismatch");
				return;
			}
		}
		if (val != null && !string.IsNullOrEmpty(val.Value))
		{
			string text5 = Path.Combine(directoryName, val.Value);
			string[] array = File.ReadAllLines(text5);
			if (array == null || array.Count() == 0)
			{
				Smart.Log.Debug(TAG, $"SubsidyNvm file {text5} is empty, no subsidy lock required");
				SetPreCondition("unlocked");
				LogPass();
				return;
			}
			string[] array2 = new string[array.Length];
			int num2 = 0;
			List<string> list = new List<string>();
			string empty3 = string.Empty;
			string[] array3 = array;
			foreach (string text6 in array3)
			{
				empty3 = text6;
				if (string.IsNullOrEmpty(text6))
				{
					array2[num2++] = text6;
					continue;
				}
				int num3 = text6.ToUpper().IndexOf("<HCK-");
				if (num3 != -1)
				{
					Smart.Log.Error(TAG, string.Format("SubsidyNvm file {0} contains <HCK-, not supported by this program subsidy NVM with generated code", directoryName + "\\" + val.Value));
					LogResult((Result)1, "HCK not supported");
					return;
				}
				num3 = text6.ToUpper().IndexOf("<CK-");
				int num4 = -1;
				if (num3 != -1)
				{
					num4 = text6.ToUpper().IndexOf(">", num3);
					if (num4 == -1)
					{
						Smart.Log.Error(TAG, string.Format("SubsidyNvm file {0} is missing > for <CK-", directoryName + "\\" + val.Value));
						LogResult((Result)1, "Malformed CK block");
						return;
					}
					text2 = text6.Substring(10, 2);
					empty2 = text6.Substring(num3 + 4, num4 - (num3 + 4));
					try
					{
						num = int.Parse(empty2);
					}
					catch (Exception)
					{
						Smart.Log.Error(TAG, string.Format("SubsidyNvm file {0} has invalid length for CK", directoryName + "\\" + val.Value));
						LogResult((Result)1, "Invalid CK length");
						return;
					}
					empty = subsidyLockRandomGenerator.NextKey(num, out error);
					if (!string.IsNullOrEmpty(error))
					{
						Smart.Log.Error(TAG, $"SubsidyLockRandomGenerator returned error {error}");
						LogResult((Result)1, "Random generator returned an error");
						return;
					}
					list.Add(empty);
					empty3 = text6.ToUpper().Replace("<CK-" + empty2 + ">", Smart.Convert.BytesToHex(Smart.Convert.AsciiToBytes(empty)));
				}
				array2[num2++] = empty3;
			}
			if (list.Count > 0)
			{
				base.Cache["lock1"] = list[0];
				Smart.Log.Debug(TAG, "Save generatedlockcodes[0] to Cache[\"lock1\"]");
				for (int j = 1; j < list.Count; j++)
				{
					string text7 = $"lock{j + 1}";
					base.Cache[text7] = list[j];
					Smart.Log.Verbose(TAG, $"Save generatedlockcodes[{j}]: {list[j]} to Cache[\"{text7}\"]");
				}
				text = "locked";
			}
			Smart.Log.Debug(TAG, "Sending CK Subsidy Lock commands...");
			empty3 = string.Empty;
			array3 = array2;
			foreach (string text8 in array3)
			{
				empty3 = text8;
				if (!string.IsNullOrEmpty(text8))
				{
					Smart.Log.Verbose(TAG, $"Sending line to phone: {text8}");
					CMD_CODE = empty3.Substring(0, 2);
					SUBSYS_ID = empty3.Substring(2, 2);
					SUBSYS_CMD_CODE = empty3.Substring(4, 4);
					IFtmResponse val3 = base.ftm.SendCommand(text8, true, true);
					string responseData = Smart.Convert.BytesToHex(val3.Raw);
					if (EvaluateSolicitedResponse(responseData) != NoError)
					{
						Smart.Log.Debug(TAG, "CK Subsidy lock command failed - bad solicited response");
						LogResult((Result)1, "CK Subsidy lock command failed - bad solicited response");
						return;
					}
					if (val3.UnSolicitedResponse == null)
					{
						Smart.Log.Error(TAG, "Unsolicited response is missing");
						LogResult((Result)1, "Unsolicited response is missing");
						return;
					}
					string responseData2 = Smart.Convert.BytesToHex(val3.UnSolicitedResponse.Raw);
					if (EvaluateUnSolicitedResponse(responseData2) != NoError)
					{
						Smart.Log.Debug(TAG, "CK Subsidy lock command failed - bad unsolicited response");
						LogResult((Result)1, "CK Subsidy lock command failed - - bad unsolicited response");
						return;
					}
				}
			}
			SubsidyLockTypes subsidyLockTypes = SubsidyLockTypes.UnitIsNotLocked;
			if (text2 != string.Empty)
			{
				subsidyLockTypes = ((text2 == "00") ? SubsidyLockTypes.UnitIsNWSCPLocked : ((!(text2 == "02")) ? SubsidyLockTypes.UnitIsLockedForOtherLocks : SubsidyLockTypes.UnitIsServiceProviderLocked));
			}
			Smart.Log.Debug(TAG, "Lock Status: " + text);
			Smart.Log.Debug(TAG, "subsidyLock type: " + subsidyLockTypes);
			base.Log.AddInfo("SubsidyLockStatus", subsidyLockTypes.ToString());
			SetPreCondition(text);
			LogResult(result);
		}
		else
		{
			Smart.Log.Error(TAG, $"SUBSIDY_NVM_FROM_XML is not defined");
			LogResult((Result)1, "SUBSIDY_NVM_FROM_XML is not defined");
		}
	}
}
