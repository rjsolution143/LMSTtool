using System;
using System.Text.RegularExpressions;
using ISmart;

namespace SmartUtil;

public class RefurbInfo : IRefurbInfo
{
	public bool Enabled
	{
		get
		{
			string text = Smart.Rsd.GeneratePdf();
			if (text != null)
			{
				return text.ToLowerInvariant() == "yes";
			}
			return false;
		}
	}

	private string TAG => GetType().FullName;

	public RefurbInfo CollectInfo(IDevice device)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Invalid comparison between Unknown and I4
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		string text = Smart.App.Name;
		if (text.ToLowerInvariant() == "lm smart tool")
		{
			text = "LMST";
		}
		string text2 = $"{text} {Smart.App.Version}";
		IResultLogger log = device.Log;
		DateTime startTime = log.StartTime;
		TimeSpan timeSpan = log.EndTime.Subtract(log.StartTime);
		bool flag = (int)log.OverallResult == 8;
		UseCase useCase = log.UseCase;
		string text3 = "UNKNOWN";
		if (device.ModelId != null && device.ModelId != string.Empty)
		{
			text3 = device.ModelId.Split(new char[1] { '|' })[0];
		}
		bool flag2 = default(bool);
		string value = Smart.Rsd.GetValue("sku", useCase, device, ref flag2, false);
		if (flag2)
		{
			text3 = value;
		}
		string text4 = "Default";
		string logInfoValue = device.GetLogInfoValue("Flash Size");
		if (logInfoValue != null && logInfoValue != string.Empty)
		{
			text4 = logInfoValue;
		}
		string logInfoValue2 = device.GetLogInfoValue("Fingerprint");
		string logInfoValue3 = device.GetLogInfoValue("FlashId");
		string text5 = string.Empty;
		if (logInfoValue3 != null && logInfoValue3 != string.Empty)
		{
			text5 = ParseVersion(logInfoValue3);
		}
		string iD = device.ID;
		string serialNumber = device.SerialNumber;
		string text6 = "Unknown";
		string roCarrier = device.RoCarrier;
		if (roCarrier != null && roCarrier != string.Empty)
		{
			text6 = roCarrier;
		}
		return new RefurbInfo(text2, startTime, timeSpan, flag, useCase, text3, text4, logInfoValue2, text5, iD, serialNumber, text6);
	}

	private string ParseVersion(string input)
	{
		Regex regex = new Regex("^(?<androidVersion>([a-z]|[r-z][0-9]))[a-z]{2,4}[0-9]{2,3}\\.[a-z]?[0-9]+[a-z]?([-.][a-z]?[0-9]+)*", RegexOptions.IgnoreCase);
		string[] array = input.Split(new char[1] { ' ' });
		foreach (string input2 in array)
		{
			if (regex.IsMatch(input2))
			{
				return regex.Match(input2).Groups["androidVersion"].Value;
			}
		}
		return string.Empty;
	}
}
