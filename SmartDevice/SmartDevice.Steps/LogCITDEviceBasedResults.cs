using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ISmart;

namespace SmartDevice.Steps;

public class LogCITDEviceBasedResults : BaseStep
{
	private IDevice device;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
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
		List<string> list = Smart.File.FindFiles("hwdResult_*.xml", text, true);
		if (list.Count == 0)
		{
			Smart.Log.Error(TAG, "No results files found");
			throw new FileNotFoundException("No results file found");
		}
		Smart.Log.Debug(TAG, $"{list.Count} hwdResult_*.xml file(s) found");
		CitDeviceBasedTestLog citDeviceBasedTestLog = new CitDeviceBasedTestLog(list[0]);
		Result val = (Result)8;
		Result testResult = citDeviceBasedTestLog.TestResult;
		foreach (string[] record in citDeviceBasedTestLog.Records)
		{
			try
			{
				double num = ((!(record[1] != string.Empty)) ? 0.0 : double.Parse(record[1]));
				double num2 = ((!(record[2] != string.Empty)) ? 0.0 : double.Parse(record[2]));
				double num3 = ((!(record[3] != string.Empty)) ? 0.0 : double.Parse(record[3]));
				string text2 = record[0];
				string text3 = record[4];
				val = ((record[5] == "2") ? ((Result)8) : ((!(record[5] == "0")) ? ((Result)1) : ((Result)7)));
				string text4 = record[6];
				if (text4 != string.Empty)
				{
					string text5 = $"TestName: {text2} result: {((object)(Result)(ref val)).ToString()} ErrorMsg: {text4}";
					Smart.Log.Error(TAG, text5);
					text2 = text2 + "-" + text4;
				}
				base.Log.AddResult("CITDevBd-" + text2, GetType().Name, val, text3, "", "", num3, num2, num, (SortedList<string, object>)null);
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, $"Exception: Could not parse record: {Smart.Convert.ToCommaSeparated((IEnumerable)record)} - error {ex.Message}");
			}
		}
		if (flag)
		{
			Directory.Delete(text, recursive: true);
		}
		LogResult(testResult);
	}
}
