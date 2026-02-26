using ISmart;

namespace SmartDevice.Steps;

public class SetDeviceID : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).Value;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		text = text.Trim();
		if (Smart.DeviceManager.Devices.ContainsKey(text))
		{
			Smart.Log.Error(TAG, "DeviceManager already has an entry for new ID, this step needs to be moved earlier in the recipe");
			LogResult((Result)4, "New device ID is already in use");
		}
		else
		{
			Smart.Log.Debug(TAG, $"Changing {val.ID} device ID to {text}");
			val.ID = text;
			LogPass();
		}
	}
}
