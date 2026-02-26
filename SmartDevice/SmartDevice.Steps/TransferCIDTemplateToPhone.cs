using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Linq;
using ISmart;

namespace SmartDevice.Steps;

public class TransferCIDTemplateToPhone : FSACWriteFile
{
	private static Dictionary<string, string> sComputedSha1Hash = new Dictionary<string, string>();

	private static readonly object sGenericLock = new object();

	private string TAG => GetType().FullName;

	public static string ComputeSha1Hash(string filePath)
	{
		if (sComputedSha1Hash.ContainsKey(filePath))
		{
			return sComputedSha1Hash[filePath];
		}
		SHA1 sHA = SHA1.Create();
		byte[] buffer = File.ReadAllBytes(filePath);
		string text = BitConverter.ToString(sHA.ComputeHash(buffer)).Replace("-", "");
		try
		{
			lock (sGenericLock)
			{
				if (!sComputedSha1Hash.ContainsKey(filePath))
				{
					sComputedSha1Hash.Add(filePath, text);
				}
			}
		}
		catch (Exception)
		{
		}
		return text;
	}

	private bool RetrieveFileHashValue(string fullPhoneFile, out string hashValue)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		hashValue = string.Empty;
		try
		{
			string text = "00" + Smart.Convert.AsciiStringToHexString(fullPhoneFile) + "00";
			ITestCommandResponse val = base.tcmd.SendCommand("008A", text);
			ResponseCode responseCode;
			if (val.Failed)
			{
				ILog log = Smart.Log;
				string tAG = TAG;
				responseCode = val.ResponseCode;
				log.Error(tAG, $"Search request for file, {fullPhoneFile}, failed with return code {((object)(ResponseCode)(ref responseCode)).ToString()}");
				return false;
			}
			Smart.Log.Debug(TAG, "Search Request response.DataHex: " + val.DataHex);
			int num = int.Parse(val.DataHex.Substring(0, 8), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
			if (num != 1)
			{
				Smart.Log.Error(TAG, $"Did not find the correct number of files: {num}");
				return false;
			}
			val = base.tcmd.SendCommand("008A", "0100000001");
			if (val.Failed)
			{
				ILog log2 = Smart.Log;
				string tAG2 = TAG;
				responseCode = val.ResponseCode;
				log2.Error(tAG2, $"Request of hash values failed with return code {((object)(ResponseCode)(ref responseCode)).ToString()}");
				return false;
			}
			Smart.Log.Debug(TAG, "Hash Request response.DataHex: " + val.DataHex);
			int num2 = int.Parse(val.DataHex.Substring(0, 8), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
			hashValue = val.DataHex.Substring(8, 40);
			string text2 = Smart.Convert.HexStringToAsciiString(val.DataHex.Substring(48));
			if (num2 != 1 || !text2.Trim(new char[1]).Equals(fullPhoneFile, StringComparison.OrdinalIgnoreCase))
			{
				Smart.Log.Error(TAG, $"Request contained incorrect count {num2} or file: {num2}, {text2}");
				return false;
			}
			Smart.Log.Debug(TAG, $"For filename: {text2.Trim(new char[1])}, hash value received is: {hashValue}");
			return true;
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, string.Format("Exception - ErrorMsg:", ex.Message));
			Smart.Log.Verbose(TAG, ex.StackTrace);
			return false;
		}
	}

	public override void Run()
	{
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Invalid comparison between Unknown and I4
		string text = ((dynamic)base.Info.Args).XML;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		string directoryName = Path.GetDirectoryName(text);
		IEnumerable<XElement> enumerable = ((XContainer)XDocument.Load(text)).Descendants(XName.op_Implicit("cid_template_config"));
		string text2 = string.Empty;
		string text3 = string.Empty;
		foreach (XElement item in (IEnumerable)enumerable)
		{
			XElement val = item;
			if (((object)val.Name).ToString().Equals("cid_template_config", StringComparison.InvariantCultureIgnoreCase))
			{
				text3 = val.Attribute(XName.op_Implicit("name")).Value;
				text2 = val.Attribute(XName.op_Implicit("SHA1")).Value;
				break;
			}
		}
		if (string.IsNullOrEmpty(text3) || string.IsNullOrEmpty(text2))
		{
			Smart.Log.Error(TAG, $"Cannot find \"cid_template_config\" xml item in flashfile XML {text}");
			LogResult((Result)1);
			return;
		}
		Smart.Log.Debug(TAG, $"CID template name: {text3} SHA1: {text2}");
		string text4 = Path.Combine(directoryName, text3);
		if (File.Exists(text4))
		{
			if (new FileInfo(text4).Length == 0L)
			{
				Smart.Log.Debug(TAG, $"CID template file {text3} is empty, no CID template override needed");
				SetPreCondition("CID_skipped");
				LogResult((Result)8);
				return;
			}
			string text5 = ComputeSha1Hash(text4);
			Smart.Log.Debug(TAG, $"File {text4} has caculated SHA1 hash: {text5}");
			if (!text5.ToLower().Equals(text2.ToLower()))
			{
				Smart.Log.Error(TAG, $"File SHA1 {text2.ToLower()} does not match calculated SHA1 {text5.ToLower()}");
				LogResult((Result)1);
				return;
			}
			string text6 = ((dynamic)base.Info.Args).FileName;
			base.SourceFileName = text4;
			base.ResultLogged = false;
			Smart.Log.Debug(TAG, $"FSAG writing file {text4} to {text6}...");
			base.Run();
			if ((int)base.TestResult != 8)
			{
				Smart.Log.Error(TAG, $"Failed to FSAC write file {text4} to {text6}");
				LogResult((Result)1);
				return;
			}
			Smart.Log.Debug(TAG, $"Succeed to FSAC write file {text4} to {text6}");
			if (!RetrieveFileHashValue(text6, out var hashValue))
			{
				Smart.Log.Error(TAG, $"Failed to read SHA1 if phone file {text6}");
				LogResult((Result)1);
				return;
			}
			Smart.Log.Debug(TAG, $"SHA1 hash of file {text6} is {hashValue}");
			if (!hashValue.Equals(text2, StringComparison.InvariantCultureIgnoreCase))
			{
				Smart.Log.Error(TAG, $"Read phone file SHA1 {hashValue.ToLower()} does not match expected SHA1 {text2.ToLower()}");
				LogResult((Result)1);
			}
			else
			{
				Smart.Log.Debug(TAG, $"SHA1 hashes matched. File {text4} is FSAC written successfully to {text6}");
				SetPreCondition("CID_written");
				LogResult((Result)8);
			}
		}
		else
		{
			Smart.Log.Error(TAG, $"File {text4} does not exist in flashfile package");
			LogResult((Result)1);
		}
	}
}
