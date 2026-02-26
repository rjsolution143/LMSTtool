using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;
using ISmart;

namespace SmartDevice.Steps;

public class ProgramSubsidyNvmFileTcmdMtk : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_090c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0992: Unknown result type (might be due to invalid IL or missing references)
		//IL_0994: Invalid comparison between Unknown and I4
		//IL_0973: Unknown result type (might be due to invalid IL or missing references)
		Result val = (Result)8;
		string text = ((dynamic)base.Info.Args).XML;
		string preCondition = "unlocked";
		IDevice val2 = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		bool flag = true;
		if (((dynamic)base.Info.Args).IgnoreResponse != null)
		{
			flag = ((dynamic)base.Info.Args).IgnoreResponse;
		}
		bool flag2 = false;
		if (((dynamic)base.Info.Args).GPSGenLockcode != null)
		{
			flag2 = ((dynamic)base.Info.Args).GPSGenLockcode;
		}
		XElement obj = ((XContainer)XElement.Parse(Smart.File.ReadText(text))).Descendants(XName.op_Implicit("subsidy_lock_config")).First();
		string value = obj.Attribute(XName.op_Implicit("name")).Value;
		string value2 = obj.Attribute(XName.op_Implicit("MD5")).Value;
		string text2 = Smart.File.PathJoin(Path.GetDirectoryName(text), value);
		if (!Smart.File.Exists(text2))
		{
			throw new FileNotFoundException($"Could not find config file {text2}");
		}
		using (MD5 mD = MD5.Create())
		{
			using Stream inputStream = Smart.File.ReadStream(text2);
			string text3 = Smart.Convert.BytesToHex(mD.ComputeHash(inputStream));
			if (text3.ToLowerInvariant() != value2.Trim().ToLowerInvariant())
			{
				string text4 = $"File MD5 {text3} does not match expected value {value2}";
				Smart.Log.Error(TAG, text4);
				throw new FileLoadException(text4);
			}
		}
		string[] array = File.ReadAllLines(text2);
		if (array != null && array.Count() > 0)
		{
			List<string> list = new List<string>();
			byte[] array2 = new byte[16];
			RandomNumberGenerator.Create().GetBytes(array2);
			string newValue = byteToHexStr(array2).ToLower();
			SubsidyLockRandomGenerator subsidyLockRandomGenerator = SubsidyLockRandomGenerator.CreateSubsidyLockRandomGenerator();
			string error = string.Empty;
			string empty = string.Empty;
			string text5 = string.Empty;
			string text6 = string.Empty;
			int keyLength = 8;
			string text7 = string.Empty;
			long num = 0L;
			Pbkdf2 pbkdf = new Pbkdf2();
			if (flag2)
			{
				text5 = Smart.Web.GpsRsu(val2.SerialNumber, val2.ID);
				Smart.Log.Debug(TAG, $"Received KD lock code with length {text5.Length / 2} from GpsRsu web service");
			}
			string[] array3 = array;
			foreach (string text8 in array3)
			{
				string empty2 = string.Empty;
				if (text8.IndexOf("<SALT>") >= 0)
				{
					empty2 = text8.Replace("<SALT>", newValue);
					num = Convert.ToInt64(text8.Split(new char[1] { ',' })[^1].Trim(new char[1] { '"' }), 16);
					Smart.Log.Debug(TAG, $"HCK iter is {num}");
				}
				else if (text8.IndexOf("<HCK>") >= 0)
				{
					if (string.IsNullOrEmpty(text7) && num != 0L)
					{
						empty = subsidyLockRandomGenerator.NextKey(keyLength, out error);
						if (!string.IsNullOrEmpty(error))
						{
							Smart.Log.Error(TAG, $"Error - SubsidyLockRandomGenerator return {error}");
							throw new InvalidDataException($"SubsidyLockRandomGenerator returned error {error}");
						}
						Smart.Log.Debug(TAG, $"Generated random lock code with length {empty.Length}");
						text7 = pbkdf.Pbkdf2HMAC("SHA256", empty, array2, num).ToLower();
						Smart.Log.Debug(TAG, $"HCK: {text7}");
						text6 = empty;
					}
					empty2 = text8.Replace("<HCK>", text7);
				}
				else if (text8.IndexOf("<KD-32>") >= 0)
				{
					if (text5.Length / 2 != 32)
					{
						Smart.Log.Error(TAG, $"VZW_SUBSIDY_KD key length {text5.Length / 2} mis-match NVM command requested 32");
						throw new InvalidDataException($"Invalid VZW_SUBSIDY_KD key length {text5.Length / 2}");
					}
					empty2 = text8.Replace("<KD-32>", text5);
					text6 = text5;
				}
				else
				{
					empty2 = text8;
				}
				list.Add(StrToHex(empty2));
			}
			string text9 = "0FF0";
			foreach (string item in list)
			{
				string text10 = "07" + item;
				Smart.Log.Debug(TAG, $"AT cmd - opCode: {text9}, data: {text10}");
				ITestCommandResponse val3 = base.tcmd.SendCommand(text9, text10);
				if (val3.Failed)
				{
					Smart.Log.Error(TAG, $"Test command, opCode: {text9} data: {text10} failed");
					val = (Result)1;
					break;
				}
				string text11 = Smart.Convert.BytesToAscii(val3.Data);
				Smart.Log.Debug(TAG, $"AT cmd response: {text11}");
				if (!flag && text11.IndexOf("OK") < 0)
				{
					Smart.Log.Error(TAG, $"AT test command: {item} failed with response: {text11}");
					val = (Result)1;
					break;
				}
			}
			if ((int)val == 8)
			{
				if (!string.IsNullOrEmpty(text6))
				{
					Smart.Log.Verbose(TAG, string.Format("Set lock1 to", text6));
					base.Cache["lock1"] = text6;
				}
				preCondition = "locked";
			}
		}
		else
		{
			Smart.Log.Info(TAG, $"SubsidyNvm file {text2} is empty, no subsidy lock required");
		}
		SetPreCondition(preCondition);
		LogResult(val);
	}
}
