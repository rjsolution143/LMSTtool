using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SmartDevice.Steps;

public class ProgramFtmFileNvm : FtmStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		string text = ((dynamic)base.Info.Args).XML;
		string preCondition = "unlocked";
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		XElement obj = ((XContainer)XElement.Parse(Smart.File.ReadText(text))).Descendants(XName.op_Implicit("subsidy_lock_config")).First();
		string value = obj.Attribute(XName.op_Implicit("name")).Value;
		string value2 = obj.Attribute(XName.op_Implicit("MD5")).Value;
		string text2 = Smart.File.PathJoin(Path.GetDirectoryName(text), value);
		if (!Smart.File.Exists(text2))
		{
			Smart.Log.Error(TAG, $"Could not find config file {text2}");
			throw new FileNotFoundException("Could not find config file");
		}
		using (MD5 mD = MD5.Create())
		{
			using Stream inputStream = Smart.File.ReadStream(text2);
			string text3 = Smart.Convert.BytesToHex(mD.ComputeHash(inputStream));
			if (text3.ToLowerInvariant() != value2.Trim().ToLowerInvariant())
			{
				Smart.Log.Error(TAG, $"File MD5 {text3} does not match expected value {value2}");
				throw new FileLoadException("File MD5 does not match expected value");
			}
		}
		string[] array = Smart.File.ReadText(text2).Split(new char[1] { '\n' });
		Random random = new Random();
		List<string> list = new List<string>();
		string text4 = string.Empty;
		string[] array2 = array;
		foreach (string text5 in array2)
		{
			if (text5.Trim() == string.Empty)
			{
				continue;
			}
			string text6 = text5.ToUpperInvariant().Trim();
			Match match = Regex.Match(text6, "<CK-(?<length>\\d+)>");
			if (match.Success)
			{
				if (text5.Substring(10, 2) != "00")
				{
					throw new NotSupportedException("Code types other than NWSCP are not supported");
				}
				int count = int.Parse(match.Groups["length"].Value);
				string value3 = match.Value;
				text4 = "";
				foreach (int item in Enumerable.Range(0, count))
				{
					_ = item;
					string text7 = random.Next(10).ToString();
					text4 += text7;
				}
				string newValue = Smart.Convert.BytesToHex(Smart.Convert.AsciiToBytes(text4));
				text6 = text6.Replace(value3, newValue);
			}
			list.Add(text6);
		}
		if (text4 != string.Empty)
		{
			Smart.Log.Info(TAG, "Programmed lock code");
			Smart.Log.Verbose(TAG, $"Programmed lock code {text4}");
			base.Cache["lock1"] = text4;
			preCondition = "locked";
		}
		else
		{
			Smart.Log.Info(TAG, "No lock code programmed");
		}
		foreach (string item2 in list)
		{
			base.ftm.SendCommand(item2, true, true);
		}
		SetPreCondition(preCondition);
		LogPass();
	}
}
