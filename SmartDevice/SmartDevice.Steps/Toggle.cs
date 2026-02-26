using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class Toggle : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		string type = ((dynamic)base.Info.Args).PromptOnType;
		string text = ((dynamic)base.Info.Args).PromptOnText;
		Checker<bool> val = () => !CheckOn();
		if (!CheckOn())
		{
			type = ((dynamic)base.Info.Args).PromptOffType;
			text = ((dynamic)base.Info.Args).PromptOffText;
			val = CheckOn;
		}
		text = Smart.Locale.Xlate(text);
		Prompt(type, text);
		double value = ((dynamic)base.Info.Args).Timeout;
		bool num = Smart.Thread.Wait(TimeSpan.FromSeconds(value), val);
		Result result = (Result)1;
		if (num)
		{
			result = (Result)8;
		}
		LogResult(result);
	}

	private bool CheckOn()
	{
		string command = ((dynamic)base.Info.Args).OnOffCheck;
		SortedList<string, string> sortedList = Tell(command);
		string key = ((dynamic)base.Info.Args).State;
		string text = ((dynamic)base.Info.Args).OnValue;
		if (sortedList.ContainsKey(key))
		{
			return sortedList[key].ToLowerInvariant() == text.ToLowerInvariant();
		}
		return false;
	}
}
