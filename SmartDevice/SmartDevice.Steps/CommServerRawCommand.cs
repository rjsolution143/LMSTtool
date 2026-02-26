using ISmart;

namespace SmartDevice.Steps;

public class CommServerRawCommand : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		string command = ((dynamic)base.Info.Args).Command;
		string text = ((dynamic)base.Info.Args).Data;
		if (text == null)
		{
			text = string.Empty;
		}
		SendCommand(command, text);
		Result result = (Result)8;
		if (((dynamic)base.Info.Args).PromptText != null)
		{
			string text2 = ((dynamic)base.Info.Args).PromptText.ToString();
			text2 = Smart.Locale.Xlate(text2);
			result = (Result)Prompt(((dynamic)base.Info.Args).PromptType.ToString(), text2);
		}
		LogResult(result);
	}
}
