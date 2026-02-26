using System;
using ISmart;

namespace SmartDevice.Steps;

public class FindReadings : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		if (((dynamic)base.Info.Args).PromptText != null)
		{
			string type = ((dynamic)base.Info.Args).PromptType;
			string text = ((dynamic)base.Info.Args).PromptText;
			text = Smart.Locale.Xlate(text);
			Prompt(type, text);
		}
		string text2 = ((dynamic)base.Info.Args).ClearCommand;
		if (text2 != null)
		{
			Tell(text2);
		}
		double value = ((dynamic)base.Info.Args).Timeout;
		bool num = Smart.Thread.Wait(TimeSpan.FromSeconds(value), (Checker<bool>)FindReading);
		if (text2 != null)
		{
			Tell(text2);
		}
		Result result = (Result)1;
		if (num)
		{
			result = (Result)8;
		}
		LogResult(result);
	}

	private bool FindReading()
	{
		string command = ((dynamic)base.Info.Args).ReadingCommand;
		return Tell(command).Count > 0;
	}
}
