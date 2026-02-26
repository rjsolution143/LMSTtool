using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using ISmart;

namespace SmartDevice.Steps;

public class ProgramSubsidyNvmFileTcmdSamSung : TestCommandStep
{
	private static Dictionary<string, string> computedmd5hash = new Dictionary<string, string>();

	private static readonly object _genericlock = new object();

	private static int pseudoPortNumber = 1;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		string preCondition = "unlocked";
		bool flag = true;
		if (((dynamic)base.Info.Args).IsSubsdiyNVMFileMandatory != null)
		{
			flag = ((dynamic)base.Info.Args).IsSubsdiyNVMFileMandatory;
		}
		string text = ((dynamic)base.Info.Args).XML;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		SamsungIO samsungIO = new SamsungIO();
		SubsidyLockRandomGenerator subsidyLockRandomGenerator = SubsidyLockRandomGenerator.CreateSubsidyLockRandomGenerator();
		string directoryName = Path.GetDirectoryName(text);
		XDocument obj = XDocument.Load(text);
		IEnumerable source = (IEnumerable)Extensions.XPathEvaluate((XNode)(object)obj, "/flashing/header/subsidy_lock_config/@name");
		IEnumerable source2 = (IEnumerable)Extensions.XPathEvaluate((XNode)(object)obj, "/flashing/header/subsidy_lock_config/@MD5");
		XAttribute val = source.Cast<XAttribute>().FirstOrDefault();
		if ((val == null || string.IsNullOrEmpty(val.Value)) && flag)
		{
			Smart.Log.Error(TAG, $"Fastboot file {text} is missing attribute version at path /flashing/header/subsidylocknvmfile/@name");
			LogResult((Result)1, "Missing attribute version name");
			return;
		}
		XAttribute val2 = source2.Cast<XAttribute>().FirstOrDefault();
		if ((val2 == null || string.IsNullOrEmpty(val2.Value)) && flag)
		{
			Smart.Log.Error(TAG, $"Fastboot file {text} is missing attribute version at path /flashing/header/subsidylocknvmfile/@MD5");
			LogResult((Result)1, "Missing attribute version MD5");
			return;
		}
		if (val2 != null && !string.IsNullOrEmpty(val2.Value))
		{
			string contents = File.ReadAllText(directoryName + "\\" + val.Value);
			string text2 = ComputeMd5Hash(contents);
			if (!text2.ToLower().Equals(val2.Value.ToLower()))
			{
				Smart.Log.Error(TAG, $"Fastboot MD5 {val2.Value.ToLower()} does not match calculated MD5 {text2.ToLower()}");
				LogResult((Result)1, "MD5 mismatch");
				return;
			}
		}
		if (val != null && !string.IsNullOrEmpty(val.Value))
		{
			string text3 = Path.Combine(directoryName, val.Value);
			string[] array = File.ReadAllLines(text3);
			if (array == null || array.Count() == 0)
			{
				Smart.Log.Debug(TAG, $"SubsidyNvm file {text3} is empty, no subsidy lock required");
				SetPreCondition("unlocked");
				LogPass();
				return;
			}
			_ = string.Empty;
			int num = -1;
			string[] array2 = array;
			foreach (string text4 in array2)
			{
				if (text4.IndexOf("LC_LENGTH") >= 0)
				{
					num = int.Parse(text4.Substring(10).Trim());
					break;
				}
			}
			Smart.Log.Debug(TAG, $"Lock code length from {text3} is {num}");
			string error;
			string text5 = subsidyLockRandomGenerator.NextKey(num, out error);
			if (!string.IsNullOrEmpty(error) || num == -1)
			{
				Smart.Log.Error(TAG, $"SubsidyLockRandomGenerator returned error {error} or lock code length from {text3} is empty");
				LogResult((Result)1, "Random generation failed");
				return;
			}
			byte[] array3 = new byte[512];
			int outCmdLen = 512;
			int index = GetPseudoPortNumber();
			samsungIO.createSamsungIO(index, null);
			int simLockInfo = samsungIO.getSimLockInfo(index, text3, text5, num, ref array3[0], ref outCmdLen);
			string text6 = convertToASCIIString(array3, outCmdLen);
			Smart.Log.Debug(TAG, $"function getSimLockInfo from Samsung tool returns {simLockInfo}, sim lock info:{text6}");
			string text7 = $"SAT+LOCKWRITE={text6}";
			string text8 = Smart.Convert.AtToTCMD(text7);
			string text9 = "0F01";
			Smart.Log.Debug(TAG, $"AT TCMD - opCode: {text9}, data: {text8}");
			ITestCommandResponse val3 = base.tcmd.SendCommand(text9, text8);
			string text10 = Smart.Convert.BytesToAscii(val3.Data);
			Smart.Log.Debug(TAG, $"AT command returned response {text10}");
			if (text10.IndexOf("OK") < 0)
			{
				result = (Result)1;
			}
			else
			{
				preCondition = "locked";
				base.Cache["lock1"] = text5;
			}
			samsungIO.destroySamsungIO(index);
		}
		else
		{
			Smart.Log.Debug(TAG, "subsidy_lock_config.name is not defined");
		}
		SetPreCondition(preCondition);
		LogResult(result);
	}

	private string ComputeMd5Hash(string contents)
	{
		if (computedmd5hash.ContainsKey(contents))
		{
			return computedmd5hash[contents];
		}
		MD5 mD = MD5.Create();
		byte[] bytes = Encoding.Default.GetBytes(contents);
		string text = BitConverter.ToString(mD.ComputeHash(bytes)).Replace("-", "");
		try
		{
			lock (_genericlock)
			{
				if (!computedmd5hash.ContainsKey(contents))
				{
					computedmd5hash.Add(contents, text);
				}
			}
		}
		catch (Exception)
		{
		}
		return text;
	}

	private string convertToASCIIString(byte[] array, int numofbyte)
	{
		string text = string.Empty;
		for (int i = 0; i < numofbyte; i++)
		{
			byte utf = array[i];
			text += char.ConvertFromUtf32(utf);
		}
		return text;
	}

	private int GetPseudoPortNumber()
	{
		int result = pseudoPortNumber++;
		if (pseudoPortNumber > 16)
		{
			pseudoPortNumber = 1;
		}
		return result;
	}
}
