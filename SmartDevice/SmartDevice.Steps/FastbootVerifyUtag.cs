using System.Collections.Generic;
using System.Xml;
using ISmart;

namespace SmartDevice.Steps;

public class FastbootVerifyUtag : FastbootStep
{
	private const string BOOTLOADER = "(bootloader) ";

	private const string UTAG_START = "<UTAG";

	private const string UTAG_END = "</UTAG>";

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).Utag;
		Result val2 = (Result)8;
		string text2 = "oem config " + text;
		Smart.Log.Debug(TAG, "command: " + text2);
		int num = 3000;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
			num *= 1000;
		}
		string filePathName = Smart.Rsd.GetFilePathName("fastbootExe", base.Recipe.Info.UseCase, val);
		int num2 = default(int);
		List<string> lines = Smart.MotoAndroid.Shell(val.ID, text2, num, filePathName, ref num2, 6000, false);
		if (num2 == 0)
		{
			string valueFromOemConfigResp = GetValueFromOemConfigResp(lines);
			Smart.Log.Debug(TAG, $"UTAG: {text} has value {valueFromOemConfigResp}");
			val2 = VerifyPropertyValue(valueFromOemConfigResp);
			SetPreCondition(valueFromOemConfigResp);
		}
		else
		{
			val2 = (Result)1;
		}
		LogResult(val2);
	}

	private string GetValueFromOemConfigResp(List<string> lines)
	{
		string result = string.Empty;
		string text = string.Empty;
		bool flag = false;
		foreach (string line in lines)
		{
			if (line.Contains("<UTAG"))
			{
				flag = true;
				text = line.Replace("(bootloader) ", string.Empty);
			}
			else if (flag)
			{
				text += line.Replace("(bootloader) ", string.Empty);
			}
			if (line.Contains("</UTAG>"))
			{
				flag = false;
				text = text.Trim();
			}
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(text);
		XmlNode xmlNode = xmlDocument.SelectSingleNode("UTAG/value");
		if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.InnerText))
		{
			result = xmlNode.InnerText.Trim();
		}
		return result;
	}
}
