using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;
using ISmart;

namespace SmartDevice.Steps;

public class ProgramSubsidyNvmFileTcmdMtkTrackFone : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b68: Invalid comparison between Unknown and I4
		//IL_0b47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5b: Invalid comparison between Unknown and I4
		//IL_0bc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c58: Unknown result type (might be due to invalid IL or missing references)
		Result val = (Result)8;
		string text = ((dynamic)base.Info.Args).XML;
		string preCondition = "unlocked";
		_ = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
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
		if (((dynamic)base.Info.Args).RSUSecret != null)
		{
			flag2 = (bool)((dynamic)base.Info.Args).RSUSecret;
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
			byte[] array3 = new byte[16];
			byte[] array4 = new byte[16];
			RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
			randomNumberGenerator.GetBytes(array2);
			randomNumberGenerator.GetBytes(array3);
			randomNumberGenerator.GetBytes(array4);
			string newValue = byteToHexStr(array2).ToLower();
			string newValue2 = byteToHexStr(array3).ToLower();
			string newValue3 = byteToHexStr(array4).ToLower();
			SubsidyLockRandomGenerator subsidyLockRandomGenerator = SubsidyLockRandomGenerator.CreateSubsidyLockRandomGenerator();
			string error = string.Empty;
			string text5 = string.Empty;
			string text6 = string.Empty;
			string text7 = string.Empty;
			int keyLength = 16;
			string text8 = string.Empty;
			string text9 = string.Empty;
			string text10 = string.Empty;
			long num = 0L;
			long num2 = 0L;
			long num3 = 0L;
			Pbkdf2 pbkdf = new Pbkdf2();
			string[] array5 = array;
			foreach (string text11 in array5)
			{
				string empty = string.Empty;
				if (text11.IndexOf("<SALT>") >= 0)
				{
					empty = text11.Replace("<SALT>", newValue);
					num = Convert.ToInt64(text11.Split(new char[1] { ',' })[^1].Trim(new char[1] { '"' }), 16);
					Smart.Log.Debug(TAG, $"HCK iter is {num}");
				}
				else if (text11.IndexOf("<HCK>") >= 0)
				{
					if (string.IsNullOrEmpty(text8) && num != 0L)
					{
						text5 = subsidyLockRandomGenerator.NextKey(keyLength, out error);
						if (!string.IsNullOrEmpty(error))
						{
							string text12 = $"SubsidyLockRandomGenerator returned error {error}";
							Smart.Log.Error(TAG, text12);
							throw new InvalidDataException(text12);
						}
						Smart.Log.Debug(TAG, $"Generated random lockCode with length {text5.Length}");
						text8 = pbkdf.Pbkdf2HMAC("SHA256", text5, array2, num).ToLower();
						Smart.Log.Debug(TAG, $"HCK: {text8}");
					}
					empty = text11.Replace("<HCK>", text8);
				}
				else if (text11.IndexOf("<SALT2>") >= 0)
				{
					empty = text11.Replace("<SALT2>", newValue2);
					num2 = Convert.ToInt64(text11.Split(new char[1] { ',' })[^1].Trim(new char[1] { '"' }), 16);
					Smart.Log.Debug(TAG, $"HCK iter2 is {num2}");
				}
				else if (text11.IndexOf("<HCK2>") >= 0)
				{
					if (string.IsNullOrEmpty(text9) && num2 != 0L)
					{
						text6 = subsidyLockRandomGenerator.NextKey(keyLength, out error);
						if (!string.IsNullOrEmpty(error))
						{
							string text13 = $"SubsidyLockRandomGenerator returned error2 {error}";
							Smart.Log.Error(TAG, text13);
							throw new InvalidDataException(text13);
						}
						Smart.Log.Debug(TAG, $"Generated random lockCode2 with length {text6.Length}");
						text9 = pbkdf.Pbkdf2HMAC("SHA256", text6, array3, num2).ToLower();
						Smart.Log.Debug(TAG, $"HCK2: {text9}");
					}
					empty = text11.Replace("<HCK2>", text9);
				}
				else if (text11.IndexOf("<SALT3>") >= 0 && text11.IndexOf("<HCK3>") >= 0)
				{
					empty = text11.Replace("<SALT3>", newValue3);
					num3 = Convert.ToInt64(text11.Split(new char[1] { ',' })[^2].Trim(new char[1] { '"' }), 16);
					Smart.Log.Debug(TAG, $"HCK iter3 is {num3}");
					if (string.IsNullOrEmpty(text10) && num3 != 0L)
					{
						text7 = subsidyLockRandomGenerator.NextKey(keyLength, out error);
						if (!string.IsNullOrEmpty(error))
						{
							string text14 = $"SubsidyLockRandomGenerator returned error3 {error}";
							Smart.Log.Error(TAG, text14);
							throw new InvalidDataException(text14);
						}
						text10 = pbkdf.Pbkdf2HMAC("SHA256", text7, array4, num3).ToLower();
					}
					empty = empty.Replace("<HCK3>", text10);
				}
				else
				{
					empty = text11;
				}
				list.Add(StrToHex(empty));
			}
			string text15 = "0FF0";
			foreach (string item in list)
			{
				string text16 = "07" + item;
				Smart.Log.Debug(TAG, $"AT cmd - opCode: {text15}, data: {text16}");
				ITestCommandResponse val2 = base.tcmd.SendCommand(text15, text16);
				if (val2.Failed)
				{
					Smart.Log.Error(TAG, $"Test command, opCode: {text15} data: {text16} failed");
					val = (Result)1;
					break;
				}
				string text17 = Smart.Convert.BytesToAscii(val2.Data);
				Smart.Log.Debug(TAG, $"AT cmd response: {text17}");
				if (!flag && text17.IndexOf("OK") < 0)
				{
					Smart.Log.Error(TAG, $"AT test command: {item} failed with response: {text17}");
					val = (Result)1;
					break;
				}
			}
			if ((int)val == 8)
			{
				string text18 = string.Empty;
				if (flag2)
				{
					text15 = "0FF0";
					string text16 = "075443415453454E442C302C41542B45534D4C5253553D362C36232B45534D4C5253553A";
					ITestCommandResponse val2 = base.tcmd.SendCommand(text15, text16);
					if (val2.Failed)
					{
						Smart.Log.Error(TAG, $"Test command, opCode: {text15} data: {text16} failed");
						val = (Result)1;
					}
					else
					{
						string text17 = Smart.Convert.BytesToAscii(val2.Data);
						Smart.Log.Debug(TAG, $"atCmdResponse: {text17}");
						int startIndex = text17.IndexOf("\"") + 1;
						text18 = text17.Substring(startIndex, 64);
						Smart.Log.Debug(TAG, $"rsuSecretKey: {text18}");
						if (text18.Length != 64)
						{
							Smart.Log.Error(TAG, "Malformed RSU key: " + text18);
							val = (Result)1;
						}
					}
				}
				if ((int)val == 8)
				{
					Smart.Log.Verbose(TAG, $"Set lock1 to {text5}");
					base.Cache["lock1"] = text5;
					Smart.Log.Verbose(TAG, $"Set lock2 to {text6}");
					base.Cache["lock2"] = text6;
					Smart.Log.Verbose(TAG, $"Set lock3 to {text7}");
					base.Cache["lock3"] = text7;
					Smart.Log.Verbose(TAG, $"Set rsuKey to {text18}");
					base.Cache["rsuSecretKey"] = text18;
					preCondition = "locked";
				}
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
