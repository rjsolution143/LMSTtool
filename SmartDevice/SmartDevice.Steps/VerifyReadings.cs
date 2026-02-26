using System;
using System.Collections.Generic;
using System.Threading;
using ISmart;

namespace SmartDevice.Steps;

public class VerifyReadings : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0792: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a20: Invalid comparison between Unknown and I4
		//IL_0a29: Unknown result type (might be due to invalid IL or missing references)
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
		if (((dynamic)base.Info.Args).DelaySecAfterTcmd != null)
		{
			Thread.Sleep((int)((dynamic)base.Info.Args).DelaySecAfterTcmd * 1000);
		}
		string text4 = ((dynamic)base.Info.Args).ClearCommand;
		if (text4 != null)
		{
			Tell(text4);
		}
		double value = ((dynamic)base.Info.Args).Timeout;
		Result result = Smart.Thread.Wait<Result>(TimeSpan.FromSeconds(value), (Checker<Result>)VerifyReading, (Result)8);
		if (text4 != null)
		{
			Tell(text4);
		}
		if (((dynamic)base.Info.Args).TcmdEndOpCode != null)
		{
			string text5 = ((dynamic)base.Info.Args).TcmdEndOpCode;
			string text6 = ((dynamic)base.Info.Args).TcmdEndData;
			((ITestCommandClient)base.Cache["tcmd"]).SendCommand(text5, text6);
		}
		VerifyOnly(ref result);
		if ((int)result != 8)
		{
			CommServerStep.CitDelay = true;
		}
		LogResult(result);
	}

	private Result VerifyReading()
	{
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		string command = ((dynamic)base.Info.Args).ReadingCommand;
		SortedList<string, string> sortedList = Tell(command);
		return (Result)LimitCheck(sortedList, ((dynamic)base.Info.Args).Limits);
	}
}
