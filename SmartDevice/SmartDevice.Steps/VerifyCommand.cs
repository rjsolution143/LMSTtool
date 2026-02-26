using System;
using ISmart;

namespace SmartDevice.Steps;

public class VerifyCommand : TestCommandStep
{
	private string dynamicError = string.Empty;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Invalid comparison between Unknown and I4
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Invalid comparison between Unknown and I4
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		bool flag = true;
		if (((dynamic)base.Info.Args).CheckFirst != null)
		{
			flag = ((dynamic)base.Info.Args).CheckFirst;
		}
		Result result = (Result)0;
		if (flag)
		{
			result = Verify();
		}
		if ((int)result != 8)
		{
			if (((dynamic)base.Info.Args).PromptType != null && ((dynamic)base.Info.Args).PromptText != null)
			{
				string type = ((dynamic)base.Info.Args).PromptType;
				string text = ((dynamic)base.Info.Args).PromptText;
				text = Smart.Locale.Xlate(text);
				Prompt(type, text);
			}
			double value = ((((dynamic)base.Info.Args).Timeout != null) ? ((dynamic)base.Info.Args).Timeout : ((object)60));
			result = Smart.Thread.Wait<Result>(TimeSpan.FromSeconds(value), (Checker<Result>)Verify, (Result)8);
		}
		VerifyOnly(ref result);
		if ((int)result != 8)
		{
			LogResult(result, "", dynamicError);
		}
		else
		{
			LogResult(result);
		}
	}

	private Result Verify()
	{
		//IL_0986: Unknown result type (might be due to invalid IL or missing references)
		//IL_098a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0924: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			string text = ((dynamic)base.Info.Args).OpCode;
			string text2 = ((dynamic)base.Info.Args).Data;
			string text3 = (dynamicError = base.tcmd.SendCommand(text, text2).DataHex);
			if (((dynamic)base.Info.Args).TrimLeft != null)
			{
				int num = ((dynamic)base.Info.Args).TrimLeft;
				text3 = ((text3.Length > num) ? text3.Substring(num) : string.Empty);
			}
			if (((dynamic)base.Info.Args).Expected != null)
			{
				string[] array = ((string)((dynamic)base.Info.Args).Expected).Split(new char[1] { ',' });
				Result result = (Result)1;
				string[] array2 = array;
				foreach (string text4 in array2)
				{
					Smart.Log.Info(TAG, $"Expected return value {text4}, found value {text3}");
					if ((!(text4 == string.Empty) || !(text3 != string.Empty)) && text3.StartsWith(text4))
					{
						result = (Result)8;
						break;
					}
				}
				return result;
			}
			if (((dynamic)base.Info.Args).NotExpected != null)
			{
				string[] array3 = ((string)((dynamic)base.Info.Args).NotExpected).Split(new char[1] { ',' });
				Result result2 = (Result)8;
				string[] array2 = array3;
				foreach (string text5 in array2)
				{
					Smart.Log.Info(TAG, $"NotExpected return value {text5}, found value {text3}");
					if ((!(text5 == string.Empty) || !(text3 != string.Empty)) && text3.StartsWith(text5))
					{
						result2 = (Result)1;
						break;
					}
				}
				return result2;
			}
			if (((dynamic)base.Info.Args).SetPreCond != null && (bool)((dynamic)base.Info.Args).SetPreCond)
			{
				SetPreCondition(text3);
				return (Result)8;
			}
			throw new Exception("Either \"Expected\" or \"NotExpected\" must be specified");
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, $"Error during Verify: {ex.Message}");
			Smart.Log.Verbose(TAG, ex.ToString());
			Smart.Thread.Wait(TimeSpan.FromSeconds(1.0));
			return (Result)4;
		}
	}
}
