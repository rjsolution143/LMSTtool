using ISmart;

namespace SmartDevice.Steps;

public class UpdateDevicePropertties : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (((dynamic)base.Info.Args).Imei != null)
		{
			string text = ((dynamic)base.Info.Args).Imei;
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = base.Cache[key];
			}
			base.Log.AddInfo("IMEI", text);
			val.SerialNumber = text;
			Smart.Log.Debug(TAG, "Set IMEI to " + text);
		}
		if (((dynamic)base.Info.Args).Imei2 != null)
		{
			string text2 = ((dynamic)base.Info.Args).Imei2;
			if (text2.StartsWith("$"))
			{
				string key2 = text2.Substring(1);
				text2 = base.Cache[key2];
			}
			base.Log.AddInfo("IMEI2", text2);
			val.SerialNumber2 = text2;
			Smart.Log.Debug(TAG, "Set IMEI2 to " + text2);
		}
		LogPass();
	}
}
