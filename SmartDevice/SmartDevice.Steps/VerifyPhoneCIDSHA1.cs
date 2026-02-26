using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using ISmart;

namespace SmartDevice.Steps;

public class VerifyPhoneCIDSHA1 : TestCommandStep
{
	private string TAG => GetType().FullName;

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
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Expected O, but got Unknown
		string text = ((dynamic)base.Info.Args).FileName;
		string text2 = ((dynamic)base.Info.Args).XML;
		if (text2.StartsWith("$"))
		{
			string key = text2.Substring(1);
			text2 = base.Cache[key];
		}
		Path.GetDirectoryName(text2);
		IEnumerable<XElement> enumerable = ((XContainer)XDocument.Load(text2)).Descendants(XName.op_Implicit("cid_template_config"));
		string text3 = string.Empty;
		string text4 = string.Empty;
		foreach (XElement item in (IEnumerable)enumerable)
		{
			XElement val = item;
			if (((object)val.Name).ToString().Equals("cid_template_config", StringComparison.InvariantCultureIgnoreCase))
			{
				text4 = val.Attribute(XName.op_Implicit("name")).Value;
				text3 = val.Attribute(XName.op_Implicit("SHA1")).Value;
				break;
			}
		}
		if (string.IsNullOrEmpty(text4) || string.IsNullOrEmpty(text3))
		{
			Smart.Log.Error(TAG, $"Cannot find \"cid_template_config\" xml item in flashfile XML {text2}");
			LogResult((Result)1);
			return;
		}
		Smart.Log.Debug(TAG, $"CID template name: {text4} SHA1: {text3}");
		if (!RetrieveFileHashValue(text, out var hashValue))
		{
			Smart.Log.Error(TAG, $"Failed to read SHA1 if phone file {text}");
			LogResult((Result)1);
			return;
		}
		Smart.Log.Debug(TAG, $"SHA1 hash of file {text} is {hashValue}");
		if (!hashValue.Equals(text3, StringComparison.InvariantCultureIgnoreCase))
		{
			Smart.Log.Error(TAG, $"Read phone file SHA1 {hashValue.ToLower()} does not match expected SHA1 {text3.ToLower()}");
			LogResult((Result)1);
		}
		else
		{
			Smart.Log.Debug(TAG, $"Read phone file SHA1 {hashValue.ToLower()} matches expected SHA1 {text3.ToLower()}");
			LogResult((Result)8);
		}
	}
}
