using ISmart;

namespace SmartDevice.Steps;

public class UploadLockCodes : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
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
		SerialNumberType val2 = Smart.Convert.ToSerialNumberType(text);
		string text2 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
		bool flag = false;
		if (((dynamic)base.Info.Args).Pcb != null)
		{
			flag = ((dynamic)base.Info.Args).Pcb;
		}
		string text3 = "00000000";
		if (base.Cache.ContainsKey("lock1"))
		{
			text3 = base.Cache["lock1"];
		}
		string text4 = (base.Cache.ContainsKey("lock2") ? base.Cache["lock2"] : string.Empty);
		string text5 = (base.Cache.ContainsKey("lock3") ? base.Cache["lock3"] : string.Empty);
		string text6 = (base.Cache.ContainsKey("rsuSecretKey") ? base.Cache["rsuSecretKey"] : string.Empty);
		if (text6 == string.Empty)
		{
			if (!flag)
			{
				Smart.Web.UpdUpdate(text, (string)null, text2, (string)null, text3, text4, text5, (string)null, (string)null, (string)null, (string)null, (string)null, (string)null, (string)null, (string)null, (string)null, (string)null, (string)null, (string)null, (string)null, (string)null, (string)null, (string)null, text6);
			}
			else
			{
				string text7 = "I";
				Smart.Web.PcbaSuccessUpdate(text, text2, text7, text3, text4, text5);
			}
		}
		else
		{
			Smart.Web.GpsLockCode(text, val.ID, val.SerialNumber2, text3, text4, text5, text6, (string)null);
		}
		LogPass();
	}
}
