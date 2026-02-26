using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class VerifyReadingsDiff : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a70: Unknown result type (might be due to invalid IL or missing references)
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
		string command = ((dynamic)base.Info.Args).ReadingCommandHigh;
		string command2 = ((dynamic)base.Info.Args).ReadingCommandLow;
		string key = ((dynamic)base.Info.Args).ReadingNameHigh;
		string key2 = ((dynamic)base.Info.Args).ReadingNameLow;
		double num = ((dynamic)base.Info.Args).MaxDiff;
		SortedList<string, string> sortedList = Tell(command);
		SortedList<string, string> sortedList2 = Tell(command2);
		string text4 = sortedList[key];
		string text5 = sortedList2[key2];
		double num2 = double.Parse(text4);
		double num3 = double.Parse(text5);
		bool flag = num2 - num3 < num;
		if (!flag && ((dynamic)base.Info.Args).FailPromptText != null)
		{
			string type2 = ((dynamic)base.Info.Args).FailPromptType;
			string text6 = ((dynamic)base.Info.Args).FailPromptText;
			text6 = Smart.Locale.Xlate(text6);
			Prompt(type2, text6);
		}
		if (((dynamic)base.Info.Args).TcmdEndOpCode != null)
		{
			string text7 = ((dynamic)base.Info.Args).TcmdEndOpCode;
			string text8 = ((dynamic)base.Info.Args).TcmdEndData;
			((ITestCommandClient)base.Cache["tcmd"]).SendCommand(text7, text8);
		}
		if (flag)
		{
			LogPass();
			return;
		}
		LogResult((Result)1, "Readings difference is too high", $"{text4} - {text5} > {num}");
		CommServerStep.CitDelay = true;
	}
}
