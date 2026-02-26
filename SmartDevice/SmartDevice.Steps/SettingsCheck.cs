using ISmart;

namespace SmartDevice.Steps;

public class SettingsCheck : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		Set(((dynamic)base.Info.Args).SettingsType.ToString(), ((dynamic)base.Info.Args).Settings);
		string text = ((dynamic)base.Info.Args).PromptText.ToString();
		text = Smart.Locale.Xlate(text);
		Result result = (Result)Prompt(((dynamic)base.Info.Args).PromptType.ToString(), text);
		LogResult(result);
	}
}
