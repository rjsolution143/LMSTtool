using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class CqaVerifyIP68Spec : CommServerStep
{
	private string readingCommand = string.Empty;

	private int waitMiliSecBtwReadings = 500;

	private Dictionary<DateTime, double> timeVsReadings = new Dictionary<DateTime, double>();

	private bool StopReadThread;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0d6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1308: Unknown result type (might be due to invalid IL or missing references)
		//IL_130e: Invalid comparison between Unknown and I4
		//IL_16a9: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (((dynamic)base.Info.Args).Delta_MaxPressure_To_NormalAirPressure != null)
		{
			_ = (double)((dynamic)base.Info.Args).Delta_MaxPressure_To_NormalAirPressure;
		}
		double dropLimit = 0.5;
		if (((dynamic)base.Info.Args).AirPressureDropLimit != null)
		{
			dropLimit = ((dynamic)base.Info.Args).AirPressureDropLimit;
		}
		double jumpLimit = 2.0;
		if (((dynamic)base.Info.Args).AirPressureJumpLimit != null)
		{
			jumpLimit = ((dynamic)base.Info.Args).AirPressureJumpLimit;
		}
		double num = 2.0;
		if (((dynamic)base.Info.Args).WaitSecForStable != null)
		{
			num = ((dynamic)base.Info.Args).WaitSecForStable;
		}
		double timeout = ((dynamic)base.Info.Args).CqaCmdTimeoutSec;
		readingCommand = ((dynamic)base.Info.Args).ReadingCommand;
		if (((dynamic)base.Info.Args).WaitMiliSecBtwReadings != null)
		{
			waitMiliSecBtwReadings = ((dynamic)base.Info.Args).WaitMiliSecBtwReadings;
		}
		double readSecsForNormalPressure = 5.0;
		if (((dynamic)base.Info.Args).ReadSecForNormalPressure != null)
		{
			readSecsForNormalPressure = (double)((dynamic)base.Info.Args).ReadSecForNormalPressure;
		}
		double num2 = ReadNormalAirPressure(readingCommand, timeout, waitMiliSecBtwReadings, readSecsForNormalPressure);
		Smart.Log.Info(TAG, "normalAirPressure is " + num2);
		Thread thread = new Thread(ReadPressureThread);
		thread.IsBackground = true;
		thread.Start();
		string promptText = "Please put 1kg object on phone back cover M logo";
		if (((dynamic)base.Info.Args).PromptText != null)
		{
			promptText = ((dynamic)base.Info.Args).PromptText;
		}
		Smart.Log.Info(TAG, promptText);
		promptText = Smart.Locale.Xlate(promptText);
		Smart.Thread.RunAndWait<Result>((Func<Result>)(() => Prompt("OK", promptText)));
		int num3 = ((dynamic)base.Info.Args).MeasurementSecsWithObject;
		DateTime now = DateTime.Now;
		Smart.Log.Info(TAG, "Start to measure pressure with object on phone...");
		while (thread.IsAlive)
		{
			if (DateTime.Now.Subtract(now).TotalSeconds < (double)num3 + num + 3.0)
			{
				Smart.Log.Info(TAG, "measure...");
				Smart.Thread.Wait(TimeSpan.FromMilliseconds(1000.0));
				continue;
			}
			Smart.Log.Info(TAG, $"Has measure up to {num3}s, abort test...");
			StopReadThread = true;
			break;
		}
		while (thread.IsAlive)
		{
			Smart.Thread.Wait(TimeSpan.FromMilliseconds(100.0));
		}
		Smart.Log.Info(TAG, "measure done. save raw data to text...");
		string text = Path.Combine(Smart.File.CommonStorageDir, "IP68TestRawData");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		File.WriteAllLines(Path.Combine(text, val.ID + "_AirPressureValue_" + DateTime.Now.ToString("yyyy-MMdd-HHmmss") + ".txt"), timeVsReadings.Select((KeyValuePair<DateTime, double> x) => x.Key.ToString("yyyy-MM-dd-HH:mm:ss:ffff") + "," + x.Value).ToArray());
		Smart.Log.Info(TAG, "Save file done. check test value meet limit...");
		string dynamicMsg = string.Empty;
		Smart.Log.Info(TAG, "Test done. show data in chart.");
		Chart chart = new Chart();
		bool flag = chart.DrawChart(val.ID, timeVsReadings, num2, num3, waitMiliSecBtwReadings, dropLimit, jumpLimit, num, out dynamicMsg);
		((Form)chart).ShowDialog();
		string text2 = ((dynamic)base.Info.Args).ClearCommand;
		if (text2 != null)
		{
			Tell(text2);
		}
		if (text2 != null)
		{
			Tell(text2);
		}
		if (flag)
		{
			LogResult((Result)8, "Water Resistance test Pass", dynamicMsg);
		}
		else
		{
			LogResult((Result)1, "Water Resistance test Fail", dynamicMsg);
		}
		if (!((((dynamic)base.Info.Args).LoopTest != null && (bool)((dynamic)base.Info.Args).LoopTest) ? true : false))
		{
			return;
		}
		string text3 = Smart.Locale.Xlate("Loop Test");
		string text4 = Smart.Locale.Xlate("Do you want to test again?");
		if ((int)val.Prompt.MessageBox(text3, text4, (MessageBoxButtons)4, (MessageBoxIcon)48) != 6)
		{
			((dynamic)base.Info.Args).Retesting = false;
		}
		else
		{
			((dynamic)base.Info.Args).Retesting = true;
		}
		if ((bool)((dynamic)base.Info.Args).Retesting)
		{
			if (((dynamic)base.Info.Args).PromptTextForLoopTest != null)
			{
				text4 = Smart.Locale.Xlate(((dynamic)base.Info.Args).PromptTextForLoopTest.ToString());
				val.Prompt.MessageBox(text3, text4, (MessageBoxButtons)0, (MessageBoxIcon)48);
			}
			if (((dynamic)base.Info.Args).WaitSecBetweenLoop != null)
			{
				Thread.Sleep((int)(double)((dynamic)base.Info.Args).WaitSecBetweenLoop * 1000);
			}
			timeVsReadings = new Dictionary<DateTime, double>();
			StopReadThread = false;
		}
	}

	private void ReadPressureThread()
	{
		while (!StopReadThread)
		{
			SortedList<string, string> sortedList = Tell(readingCommand);
			foreach (string key in sortedList.Keys)
			{
				string text = sortedList[key];
				if (text != null && !(text.Trim() == string.Empty))
				{
					double result = 0.0;
					if (double.TryParse(text, out result))
					{
						DateTime now = DateTime.Now;
						timeVsReadings.Add(now, result);
						Smart.Log.Info(TAG, $"{now.ToString()}:{result.ToString()} ");
					}
				}
			}
			Smart.Thread.Wait(TimeSpan.FromMilliseconds(waitMiliSecBtwReadings));
		}
	}

	private double ReadNormalAirPressure(string readingCommand, double timeout, int waitMiliSecBtwReadings, double readSecsForNormalPressure)
	{
		int num = 0;
		double num2 = 0.0;
		DateTime now = DateTime.Now;
		do
		{
			SortedList<string, string> sortedList = Tell(readingCommand);
			foreach (string key in sortedList.Keys)
			{
				TimeSpan timeSpan = DateTime.Now.Subtract(now);
				if (timeSpan.TotalSeconds > timeout)
				{
					throw new TimeoutException($"Timed out after {timeSpan.TotalSeconds} seconds");
				}
				string text = sortedList[key];
				if (text != null && !(text.Trim() == string.Empty))
				{
					double result = 0.0;
					if (double.TryParse(text, out result))
					{
						num++;
						num2 += result;
					}
				}
			}
			Smart.Thread.Wait(TimeSpan.FromMilliseconds(waitMiliSecBtwReadings));
		}
		while (DateTime.Now.Subtract(now).TotalSeconds < readSecsForNormalPressure);
		num2 /= (double)num;
		Smart.Log.Info(TAG, "Average air pressure before put object " + num2);
		return num2;
	}

	private bool CheckDataMeetLimit(Dictionary<DateTime, double> timeVsReadings, int measureDuration, double normalAirPressure, double delta_AirPressure_DropFrom_MaxAirPressure, double delta_AirPressureFinal_To_NormalAirPressure, out string errorMsg)
	{
		errorMsg = string.Empty;
		double num = timeVsReadings.Values.ToList().Max();
		int num2 = timeVsReadings.Values.ToList().IndexOf(num);
		DateTime key = timeVsReadings.ElementAt(num2).Key;
		Smart.Log.Info(TAG, string.Format("Max pressure {0}, measurement time {1}, index {2}.", num, key.ToString("yyyy-MM-dd-HH:mm:ss:ffff"), num2));
		DateTime measureTimeToCheck = key.AddSeconds(measureDuration);
		Smart.Log.Info(TAG, string.Format("To check the air pressure value at measurement time {0}.", measureTimeToCheck.ToString("yyyy-MM-dd-HH:mm:ss:ffff")));
		KeyValuePair<DateTime, double> keyValuePair = default(KeyValuePair<DateTime, double>);
		try
		{
			keyValuePair = timeVsReadings.First((KeyValuePair<DateTime, double> q) => q.Key >= measureTimeToCheck);
		}
		catch (Exception ex)
		{
			Smart.Log.Info(TAG, ex.Message + Environment.NewLine + ex.StackTrace);
		}
		Smart.Log.Info(TAG, string.Format("Final air pressure measured at {0} is {1}.", keyValuePair.Key.ToString("yyyy-MM-dd-HH:mm:ss:ffff"), keyValuePair.Value));
		double num3 = num - keyValuePair.Value;
		Smart.Log.Info(TAG, $"Within test time, max pressure is {num}, final air pressure compared to normal air pressure is {keyValuePair.Value}. Dropped pressure is {num3}.");
		double num4 = keyValuePair.Value - normalAirPressure;
		Smart.Log.Info(TAG, $"Within test time, final air pressure is {keyValuePair.Value}, normal air pressure is {normalAirPressure}, delta is {num4}.");
		if (num3 <= delta_AirPressure_DropFrom_MaxAirPressure && num4 >= delta_AirPressureFinal_To_NormalAirPressure)
		{
			Smart.Log.Info(TAG, $"IP68 measured Pass");
			return true;
		}
		errorMsg = $"Test fail, Within test time, max pressure is {num}hPa, final is {keyValuePair.Value}, expected dropped is {delta_AirPressure_DropFrom_MaxAirPressure}hPa, expected final is {normalAirPressure + delta_AirPressureFinal_To_NormalAirPressure}hPa.";
		Smart.Log.Info(TAG, $"IP68 measured Fail.");
		return false;
	}
}
