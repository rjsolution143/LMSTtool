using System.Threading;
using ISmart;

namespace SmartDevice.Steps;

public class TellSequenceCheck : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Invalid comparison between Unknown and I4
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		foreach (object item in ((dynamic)base.Info.Args).Commands)
		{
			string text = (string)(dynamic)item;
			if (text.ToLowerInvariant().StartsWith("#prompt"))
			{
				string text2 = ((dynamic)base.Info.Args).Prompts[text].PromptText;
				string type = ((dynamic)base.Info.Args).Prompts[text].PromptType;
				text2 = Smart.Locale.Xlate(text2);
				Result val = Prompt(type, text2);
				if ((int)val != 8)
				{
					result = val;
				}
			}
			else
			{
				Tell(text);
				if (((dynamic)base.Info.Args).DelaySecAfterTellCmd != null)
				{
					Thread.Sleep((int)((dynamic)base.Info.Args).DelaySecAfterTellCmd * 1000);
				}
			}
		}
		LogResult(result);
	}
}
