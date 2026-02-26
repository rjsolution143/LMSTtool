using ISmart;

namespace SmartDevice.Steps;

public class ReadDetectionMode : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		string text = ((IDevice)((dynamic)base.Recipe.Info.Args).Device).GetLogInfoValue("Detection");
		Smart.Log.Debug(TAG, "Detection mode: " + text);
		if (text == string.Empty)
		{
			text = "ADB";
		}
		SetPreCondition(text);
		LogPass();
	}
}
