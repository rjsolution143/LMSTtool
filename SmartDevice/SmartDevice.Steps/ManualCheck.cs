using ISmart;

namespace SmartDevice.Steps;

public class ManualCheck : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		string text = ((dynamic)base.Info.Args).PromptText.ToString();
		text = Smart.Locale.Xlate(text);
		Result result = (Result)Prompt(((dynamic)base.Info.Args).PromptType.ToString(), text);
		LogResult(result);
	}
}
