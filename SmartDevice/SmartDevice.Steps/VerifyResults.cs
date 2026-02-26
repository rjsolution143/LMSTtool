using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class VerifyResults : CommServerStep
{
	public string errmessage = string.Empty;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Invalid comparison between Unknown and I4
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_097e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0980: Invalid comparison between Unknown and I4
		//IL_098a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		bool flag = true;
		if (((dynamic)base.Info.Args).CheckFirst != null)
		{
			flag = ((dynamic)base.Info.Args).CheckFirst;
		}
		if (((dynamic)base.Info.Args).TcmdStartOpCode != null)
		{
			string text = ((dynamic)base.Info.Args).TcmdStartOpCode;
			string text2 = ((dynamic)base.Info.Args).TcmdStartData;
			((ITestCommandClient)base.Cache["tcmd"]).SendCommand(text, text2);
		}
		Result result = (Result)0;
		if (flag)
		{
			result = VerifyResult();
		}
		if ((int)result != 8)
		{
			if (((dynamic)base.Info.Args).PromptText != null)
			{
				string type = ((dynamic)base.Info.Args).PromptType;
				string text3 = ((dynamic)base.Info.Args).PromptText;
				text3 = Smart.Locale.Xlate(text3);
				Prompt(type, text3);
			}
			double value = ((dynamic)base.Info.Args).Timeout;
			result = Smart.Thread.Wait<Result>(TimeSpan.FromSeconds(value), (Checker<Result>)VerifyResult, (Result)8);
		}
		if (((dynamic)base.Info.Args).TcmdEndOpCode != null)
		{
			string text4 = ((dynamic)base.Info.Args).TcmdEndOpCode;
			string text5 = ((dynamic)base.Info.Args).TcmdEndData;
			((ITestCommandClient)base.Cache["tcmd"]).SendCommand(text4, text5);
		}
		VerifyOnly(ref result);
		if ((int)result == 8)
		{
			LogPass();
			return;
		}
		LogResult(result, "Result Check failed", errmessage);
		CommServerStep.CitDelay = true;
	}

	private Result VerifyResult()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0859: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Invalid comparison between Unknown and I4
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0852: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0844: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_0847: Invalid comparison between Unknown and I4
		//IL_084d: Unknown result type (might be due to invalid IL or missing references)
		Result val = (Result)1;
		string command = ((dynamic)base.Info.Args).ResultCommand;
		bool flag = false;
		if (((dynamic)base.Info.Args).PartialMatch != null)
		{
			flag = ((dynamic)base.Info.Args).PartialMatch;
		}
		try
		{
			SortedList<string, string> sortedList = Tell(command);
			if (((dynamic)base.Info.Args).ExpectedLength != null)
			{
				val = (Result)LengthCheck(sortedList, ((dynamic)base.Info.Args).ExpectedLength);
				if ((int)val != 8)
				{
					return val;
				}
			}
			if (((dynamic)base.Info.Args).Expected != null)
			{
				return (Result)ResultCheck(sortedList, ((dynamic)base.Info.Args).Expected, false, flag, out errmessage);
			}
			if (((dynamic)base.Info.Args).NotExpected != null)
			{
				val = (Result)ResultCheck(sortedList, ((dynamic)base.Info.Args).NotExpected, false, flag, out errmessage);
				return (Result)(((int)val == 8) ? 1 : 8);
			}
			return (Result)8;
		}
		catch (Exception ex)
		{
			val = (Result)1;
			Smart.Log.Error(TAG, string.Format(ex.Message + Environment.NewLine + ex.StackTrace));
			return val;
		}
	}
}
