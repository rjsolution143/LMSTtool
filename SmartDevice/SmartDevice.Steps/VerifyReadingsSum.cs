using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class VerifyReadingsSum : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_09be: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c70: Invalid comparison between Unknown and I4
		//IL_0c79: Unknown result type (might be due to invalid IL or missing references)
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
		bool flag = false;
		if (((dynamic)base.Info.Args).AllowAllZeros != null && ((dynamic)base.Info.Args).AllowAllZeros == true)
		{
			flag = true;
		}
		string command = ((dynamic)base.Info.Args).ReadingCommand;
		dynamic val = ((dynamic)base.Info.Args).SumReadings;
		SortedList<string, string> sortedList = Tell(command);
		double num = 0.0;
		foreach (object item in val)
		{
			string key = (string)(dynamic)item;
			double value = double.Parse(sortedList[key]);
			num += Math.Abs(value);
		}
		sortedList["SUM"] = num.ToString();
		Result result = (Result)LimitCheck(sortedList, ((dynamic)base.Info.Args).Limits);
		if (!flag && num == 0.0)
		{
			Smart.Log.Error(TAG, "Reading was all zeros");
			result = (Result)1;
		}
		if (((dynamic)base.Info.Args).TcmdEndOpCode != null)
		{
			string text4 = ((dynamic)base.Info.Args).TcmdEndOpCode;
			string text5 = ((dynamic)base.Info.Args).TcmdEndData;
			((ITestCommandClient)base.Cache["tcmd"]).SendCommand(text4, text5);
		}
		VerifyOnly(ref result);
		if ((int)result != 8)
		{
			CommServerStep.CitDelay = true;
		}
		LogResult(result);
	}
}
