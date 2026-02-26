using ISmart;

namespace SmartDevice.Steps;

public class TellCheck : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		string command = ((dynamic)base.Info.Args).Command;
		Tell(command);
		Result result = (Result)8;
		if (((dynamic)base.Info.Args).PromptText != null)
		{
			string text = ((dynamic)base.Info.Args).PromptText.ToString();
			text = Smart.Locale.Xlate(text);
			result = (Result)Prompt(((dynamic)base.Info.Args).PromptType.ToString(), text);
		}
		LogResult(result);
	}
}
