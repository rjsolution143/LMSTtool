using ISmart;

namespace SmartDevice.Steps;

public class Reboot : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).Mode;
		Smart.ADB.Reboot(val.ID, text);
		LogPass();
	}
}
