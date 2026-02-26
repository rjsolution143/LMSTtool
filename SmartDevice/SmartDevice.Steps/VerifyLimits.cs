using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class VerifyLimits : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		if (((dynamic)base.Info.Args).TcmdStartOpCode != null)
		{
			string text = ((dynamic)base.Info.Args).TcmdStartOpCode;
			string text2 = ((dynamic)base.Info.Args).TcmdStartData;
			((ITestCommandClient)base.Cache["tcmd"]).SendCommand(text, text2);
		}
		double value = ((dynamic)base.Info.Args).Timeout;
		Result result = Smart.Thread.Wait<Result>(TimeSpan.FromSeconds(value), (Checker<Result>)VerifyReading, (Result)8);
		if (((dynamic)base.Info.Args).TcmdEndOpCode != null)
		{
			string text3 = ((dynamic)base.Info.Args).TcmdEndOpCode;
			string text4 = ((dynamic)base.Info.Args).TcmdEndData;
			((ITestCommandClient)base.Cache["tcmd"]).SendCommand(text3, text4);
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
