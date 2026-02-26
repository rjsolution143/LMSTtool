using System;
using ISmart;

namespace SmartDevice.Steps;

public class VerifyValueCheck : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_0859: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0863: Unknown result type (might be due to invalid IL or missing references)
		string text = ((dynamic)base.Info.Args).Value;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		string text2 = string.Empty;
		if (((dynamic)base.Info.Args).ColorCheck != null && bool.Parse(((dynamic)base.Info.Args).ColorCheck.ToString()) == true)
		{
			IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
			text2 = Smart.Rsd.GetFilePathName(text, base.Recipe.Info.UseCase, val);
			Smart.Log.Debug(TAG, $"Loaded imagePath {text2}");
		}
		string text3 = ((dynamic)base.Info.Args).PromptText.ToString();
		text3 = Smart.Locale.Xlate(text3);
		text3 = text3 + Environment.NewLine + text;
		Result val2 = (Result)0;
		val2 = ((!(text2 != string.Empty)) ? ((Result)Prompt(((dynamic)base.Info.Args).PromptType.ToString(), text3)) : ((Result)ColorPrompt(((dynamic)base.Info.Args).PromptType.ToString(), text3, text2)));
		VerifyOnly(ref val2);
		LogResult(val2);
	}
}
