using System;
using System.Collections.Generic;
using System.Threading;
using ISmart;

namespace SmartDevice.Steps;

public class VerifyRepeatedReadings : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f17: Unknown result type (might be due to invalid IL or missing references)
		//IL_119c: Unknown result type (might be due to invalid IL or missing references)
		//IL_119f: Invalid comparison between Unknown and I4
		//IL_11a8: Unknown result type (might be due to invalid IL or missing references)
		if (((dynamic)base.Info.Args).PromptText != null)
		{
			string type = ((dynamic)base.Info.Args).PromptType;
			string text = ((dynamic)base.Info.Args).PromptText;
			text = Smart.Locale.Xlate(text);
			Prompt(type, text);
		}
		if (((dynamic)base.Info.Args).TcmdStartOpCode != null)
		{
			string text2 = ((dynamic)base.Info.Args).TcmdStartOpCode;
			string text3 = ((dynamic)base.Info.Args).TcmdStartData;
			((ITestCommandClient)base.Cache["tcmd"]).SendCommand(text2, text3);
		}
		if (((dynamic)base.Info.Args).DelayAfterSendTcmdStart != null)
		{
			Thread.Sleep((int)((dynamic)base.Info.Args).DelayAfterSendTcmdStart * 1000);
		}
		string text4 = ((dynamic)base.Info.Args).ClearCommand;
		if (text4 != null)
		{
			Tell(text4);
		}
		double num = ((dynamic)base.Info.Args).Timeout;
		int num2 = ((dynamic)base.Info.Args).Readings;
		string command = ((dynamic)base.Info.Args).ReadingCommand;
		int num3 = 100;
		if (((dynamic)base.Info.Args).WaitMilliseconds != null)
		{
			num3 = ((dynamic)base.Info.Args).WaitMilliseconds;
		}
		TimeSpan.FromMilliseconds(num3);
		SortedList<string, int> sortedList = new SortedList<string, int>();
		SortedList<string, double> sortedList2 = new SortedList<string, double>();
		SortedList<string, double> sortedList3 = new SortedList<string, double>();
		SortedList<string, double> sortedList4 = new SortedList<string, double>();
		DateTime now = DateTime.Now;
		for (int i = 0; i < num2; i++)
		{
			SortedList<string, string> sortedList5 = Tell(command);
			foreach (string key in sortedList5.Keys)
			{
				TimeSpan timeSpan = DateTime.Now.Subtract(now);
				if (timeSpan.TotalSeconds > num)
				{
					throw new TimeoutException($"Timed out after {i} readings in {timeSpan.TotalSeconds} seconds");
				}
				string text5 = sortedList5[key];
				if (text5 == null || text5.Trim() == string.Empty)
				{
					continue;
				}
				double result = 0.0;
				if (double.TryParse(text5, out result))
				{
					if (!sortedList.ContainsKey(key))
					{
						sortedList[key] = 0;
						sortedList2[key] = 0.0;
						sortedList3[key] = result;
						sortedList4[key] = result;
					}
					sortedList[key]++;
					sortedList2[key] += result;
					if (sortedList3[key] > result)
					{
						sortedList3[key] = result;
					}
					if (sortedList4[key] < result)
					{
						sortedList4[key] = result;
					}
				}
			}
			Smart.Thread.Wait(TimeSpan.FromMilliseconds(num3));
		}
		SortedList<string, string> sortedList6 = new SortedList<string, string>();
		SortedList<string, string> sortedList7 = new SortedList<string, string>();
		foreach (string key2 in sortedList.Keys)
		{
			int num4 = sortedList[key2];
			double num5 = sortedList2[key2];
			double num6 = sortedList3[key2];
			double num7 = sortedList4[key2];
			double num8 = num5 / (double)num4;
			double num9 = num7 - num6;
			sortedList6[key2] = num8.ToString();
			sortedList7[key2] = num9.ToString();
		}
		SortedList<string, string> sortedList8 = sortedList6;
		if ((((dynamic)base.Info.Args).Calculation != null) && ((string)((dynamic)base.Info.Args).Calculation).Trim().ToLowerInvariant() == "delta")
		{
			sortedList8 = sortedList7;
		}
		Result val = (Result)LimitCheck(sortedList8, ((dynamic)base.Info.Args).Limits);
		if (text4 != null)
		{
			Tell(text4);
		}
		if (((dynamic)base.Info.Args).TcmdEndOpCode != null)
		{
			string text6 = ((dynamic)base.Info.Args).TcmdEndOpCode;
			string text7 = ((dynamic)base.Info.Args).TcmdEndData;
			((ITestCommandClient)base.Cache["tcmd"]).SendCommand(text6, text7);
		}
		if ((int)val != 8)
		{
			CommServerStep.CitDelay = true;
		}
		LogResult(val);
	}
}
