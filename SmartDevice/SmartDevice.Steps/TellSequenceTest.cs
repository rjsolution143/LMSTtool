using System.Collections.Generic;
using System.Threading;
using ISmart;

namespace SmartDevice.Steps;

public class TellSequenceTest : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Invalid comparison between Unknown and I4
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Invalid comparison between Unknown and I4
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		Result val = (Result)8;
		SortedList<string, string> sortedList = new SortedList<string, string>();
		foreach (object item in ((dynamic)base.Info.Args).Commands)
		{
			string text = (string)(dynamic)item;
			if (text.ToLowerInvariant().StartsWith("#test"))
			{
				val = (Result)ResultCheck(sortedList, ((dynamic)base.Info.Args).Tests[text], false, false);
				if ((int)val != 8)
				{
					break;
				}
			}
			else if (text.ToLowerInvariant().StartsWith("#prompt"))
			{
				string text2 = ((dynamic)base.Info.Args).Prompts[text].PromptText;
				string type = ((dynamic)base.Info.Args).Prompts[text].PromptType;
				text2 = Smart.Locale.Xlate(text2);
				Result val2 = Prompt(type, text2);
				if ((int)val2 != 8)
				{
					val = val2;
				}
			}
			else
			{
				sortedList = Tell(text);
				if (((dynamic)base.Info.Args).DelaySecAfterTellCmd != null)
				{
					Thread.Sleep((int)((dynamic)base.Info.Args).DelaySecAfterTellCmd * 1000);
				}
			}
		}
		SetPreCondition(((object)(Result)(ref val)).ToString());
		LogResult(val);
	}
}
