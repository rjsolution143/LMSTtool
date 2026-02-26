namespace SmartDevice.Steps;

public class VerifyValueRequired : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		string text = ((dynamic)base.Info.Args).Input;
		string text2 = text;
		if (text.StartsWith("$"))
		{
			text2 = text.Substring(1);
			text = base.Cache[text2];
		}
		string preCondition = "Required";
		if (string.Compare(text2, "simlock", ignoreCase: true) == 0)
		{
			if (text == "0")
			{
				preCondition = "NotRequired";
			}
		}
		else if (text == string.Empty)
		{
			preCondition = "NotRequired";
		}
		SetPreCondition(preCondition);
		LogPass();
	}
}
