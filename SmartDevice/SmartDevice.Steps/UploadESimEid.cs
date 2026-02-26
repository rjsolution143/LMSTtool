using ISmart;

namespace SmartDevice.Steps;

public class UploadESimEid : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string empty = string.Empty;
		string text = "UNKNOWN";
		if (((dynamic)base.Info.Args).InputName != null && ((dynamic)base.Info.Args).InputName != string.Empty)
		{
			empty = ((dynamic)base.Info.Args).InputName.ToString();
			text = base.Cache[empty];
		}
		else
		{
			empty = "DeviceSerialNumber";
			text = val.SerialNumber;
			if (text == string.Empty || text == "UNKNOWN")
			{
				text = val.ID;
			}
		}
		string text2 = "UNKNOWN";
		string text3 = "$eSIMEid";
		if (((dynamic)base.Info.Args).Value != null)
		{
			text3 = ((dynamic)base.Info.Args).Value;
		}
		if (text3.StartsWith("$"))
		{
			string key = text3.Substring(1);
			text2 = base.Cache[key];
		}
		else
		{
			text2 = text3;
		}
		Smart.Web.GpsLockCode(text, val.ID, (string)null, (string)null, (string)null, (string)null, (string)null, text2);
		LogPass();
	}
}
