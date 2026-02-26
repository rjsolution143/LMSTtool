using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class VerifyResult : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_095a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1289: Unknown result type (might be due to invalid IL or missing references)
		//IL_128c: Invalid comparison between Unknown and I4
		//IL_0c05: Unknown result type (might be due to invalid IL or missing references)
		//IL_1295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffa: Unknown result type (might be due to invalid IL or missing references)
		string text = ((dynamic)base.Info.Args).Field;
		if (((dynamic)base.Info.Args).PromptText != null)
		{
			string type = ((dynamic)base.Info.Args).PromptType;
			string text2 = ((dynamic)base.Info.Args).PromptText;
			text2 = Smart.Locale.Xlate(text2);
			Prompt(type, text2);
		}
		if (((dynamic)base.Info.Args).TcmdStartOpCode != null)
		{
			string text3 = ((dynamic)base.Info.Args).TcmdStartOpCode;
			string text4 = ((dynamic)base.Info.Args).TcmdStartData;
			((ITestCommandClient)base.Cache["tcmd"]).SendCommand(text3, text4);
		}
		string command = ((dynamic)base.Info.Args).ResultCommand;
		bool flag = false;
		if (((dynamic)base.Info.Args).PartialMatch != null)
		{
			flag = ((dynamic)base.Info.Args).PartialMatch;
		}
		SortedList<string, string> sortedList = Tell(command);
		string text5 = "UNKNOWN";
		if (!sortedList.ContainsKey(text))
		{
			Smart.Log.Error(TAG, $"Could not find value for {text}");
		}
		else
		{
			text5 = sortedList[text];
			Smart.Log.Debug(TAG, $"Found value {text5} for {text}");
		}
		Result val = (Result)0;
		if (((dynamic)base.Info.Args).Expected != null)
		{
			string[] array = ((string)((dynamic)base.Info.Args).Expected).Split(new char[1] { ',' });
			val = (Result)1;
			string[] array2 = array;
			foreach (string text6 in array2)
			{
				Smart.Log.Error(TAG, $"Expected return value '{text6}', found value '{text5}'");
				if (!(text6 == string.Empty) || !(text5 != string.Empty))
				{
					bool flag2 = text5.ToLowerInvariant().Contains(text6.ToLowerInvariant());
					if (text5.ToLowerInvariant() == text6.ToLowerInvariant() || (flag && flag2))
					{
						val = (Result)8;
						break;
					}
				}
			}
		}
		else if (((dynamic)base.Info.Args).NotExpected != null)
		{
			string[] array3 = ((string)((dynamic)base.Info.Args).NotExpected).Split(new char[1] { ',' });
			val = (Result)8;
			string[] array2 = array3;
			foreach (string text7 in array2)
			{
				Smart.Log.Error(TAG, $"NotExpected return value '{text7}', found value '{text5}'");
				if (!(text7 == string.Empty) || !(text5 != string.Empty))
				{
					bool flag3 = text5.ToLowerInvariant().Contains(text7.ToLowerInvariant());
					if (text5.ToLowerInvariant() == text7.ToLowerInvariant() || (flag && flag3))
					{
						val = (Result)1;
						break;
					}
				}
			}
		}
		else if (((dynamic)base.Info.Args).ExpectedLength != null)
		{
			int num = ((dynamic)base.Info.Args).ExpectedLength;
			val = ((text5.Length != num) ? ((Result)1) : ((Result)8));
		}
		else
		{
			if (!((((dynamic)base.Info.Args).SetPreCond != null && (bool)((dynamic)base.Info.Args).SetPreCond) ? true : false))
			{
				throw new Exception("Either \"Expected\" or \"NotExpected\" must be specified");
			}
			SetPreCondition(text5);
			val = (Result)8;
		}
		if (((dynamic)base.Info.Args).TcmdEndOpCode != null)
		{
			string text8 = ((dynamic)base.Info.Args).TcmdEndOpCode;
			string text9 = ((dynamic)base.Info.Args).TcmdEndData;
			((ITestCommandClient)base.Cache["tcmd"]).SendCommand(text8, text9);
		}
		VerifyOnly(ref val);
		if ((int)val != 8)
		{
			CommServerStep.CitDelay = true;
		}
		LogResult(val);
	}
}
