using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ISmart;

namespace SmartDevice.Steps;

public class LogPowerTestResults : BaseStep
{
	private IDevice device;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		bool flag = false;
		string text = ((dynamic)base.Info.Args).LocalPath;
		if (string.IsNullOrEmpty(text))
		{
			text = base.Cache["TempFolder"];
			flag = true;
		}
		else if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		Smart.Log.Debug(TAG, "Local path is " + text);
		List<string> list = Smart.File.FindFiles("results_*.csv", text, true);
		if (list.Count == 0)
		{
			Smart.Log.Error(TAG, "No results files found");
			throw new FileNotFoundException("No results file found");
		}
		Smart.Log.Debug(TAG, $"{list.Count} results_*.csv file(s) found");
		List<string> list2 = Smart.File.FindFiles("log_*.csv", text, true);
		Smart.Log.Debug(TAG, $"{list2.Count} log_*.csv file(s) found");
		CSVPowerTestLog cSVPowerTestLog = new CSVPowerTestLog(list);
		Result val = (Result)8;
		Result result = (Result)8;
		foreach (string[] record in cSVPowerTestLog.Records)
		{
			try
			{
				double num = ((!(record[1] != string.Empty)) ? 0.0 : double.Parse(record[1]));
				double num2 = ((!(record[2] != string.Empty)) ? 0.0 : double.Parse(record[2]));
				double num3 = ((!(record[3] != string.Empty)) ? 0.0 : double.Parse(record[3]));
				string text2 = record[0];
				string text3 = record[4];
				string apkSerialNumber = record[6];
				GetSerialNumber(apkSerialNumber, device);
				if (num < num2 || num > num3)
				{
					val = (Result)1;
					result = (Result)1;
				}
				else
				{
					val = (Result)8;
				}
				base.Log.AddResult("PowerTest-" + text2, GetType().Name, val, text3, "", "", num3, num2, num, (SortedList<string, object>)null);
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, $"Exception: Could not parse record: {Smart.Convert.ToCommaSeparated((IEnumerable)record)} - error {ex.Message}");
			}
		}
		List<string> list3 = new List<string>();
		list3.AddRange(list);
		list3.AddRange(list2);
		string filePathName = Smart.Rsd.GetFilePathName("powerTestLogs", base.Recipe.Info.UseCase, device);
		foreach (string item in list3)
		{
			string path = string.Format(arg2: Path.GetFileName(item), arg1: DateTime.Now.ToString("MM-dd-yyyy.HH.mm.ss"), format: "{0}_{1}_{2}", arg0: device.SerialNumber);
			string destFileName = Path.Combine(filePathName, path);
			File.Copy(item, destFileName, overwrite: true);
		}
		if (flag)
		{
			Directory.Delete(text, recursive: true);
		}
		LogResult(result);
	}

	private void GetSerialNumber(string apkSerialNumber, IDevice device)
	{
		if (!string.IsNullOrEmpty(apkSerialNumber) && apkSerialNumber != "null")
		{
			if (string.IsNullOrEmpty(device.SerialNumber) || device.SerialNumber == "UNKNOWN")
			{
				device.SerialNumber = apkSerialNumber;
			}
		}
		else if (string.IsNullOrEmpty(device.SerialNumber) || device.SerialNumber == "UNKNOWN")
		{
			string title = Smart.Locale.Xlate(base.Info.Name);
			string promptText = Smart.Locale.Xlate("Please enter the phone IMEI number");
			string text = Smart.Thread.RunAndWait<string>((Func<string>)(() => device.Prompt.InputBox(title, promptText, (string)null)), true);
			if (text != null)
			{
				text = text.Trim();
			}
			Smart.Log.Debug(TAG, $"User inputs IMEI {text}");
			if (!string.IsNullOrEmpty(text) && Smart.Convert.IsSerialNumberValid(text, (SerialNumberType)0))
			{
				device.SerialNumber = text;
			}
		}
		if (string.IsNullOrEmpty(device.SerialNumber) || device.SerialNumber == "UNKNOWN")
		{
			Smart.Log.Debug(TAG, "No IMEI number");
			throw new Exception("No IMEI");
		}
	}
}
