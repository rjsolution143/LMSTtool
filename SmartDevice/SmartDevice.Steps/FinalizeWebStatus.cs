using ISmart;

namespace SmartDevice.Steps;

public class FinalizeWebStatus : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Invalid comparison between Unknown and I4
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Invalid comparison between Unknown and I4
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		bool flag = false;
		if (base.Cache.ContainsKey("updatedStatus"))
		{
			flag = base.Cache["updatedStatus"];
		}
		if (flag)
		{
			Smart.Log.Debug(TAG, "Web service status is already updated");
			LogPass();
			return;
		}
		bool flag2 = false;
		if (base.Cache.ContainsKey("dualSim"))
		{
			flag2 = base.Cache["dualSim"];
		}
		string text = "F";
		if ((int)val.Log.OverallResult == 8)
		{
			text = "S";
		}
		SerialNumberType val2;
		string text3;
		if ((int)val.Type == 2)
		{
			string text2 = base.Cache["GSNOut"];
			val2 = Smart.Convert.ToSerialNumberType(text2);
			text3 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
			Smart.Web.PcbaSuccessUpdate(text2, text3, text, string.Empty, string.Empty, string.Empty);
			if (val.WiFiOnlyDevice)
			{
				LogPass();
				return;
			}
		}
		string text4 = base.Cache["SerialNumberOut"];
		base.Log.AddInfo("SerialNumberOut", text4);
		val2 = Smart.Convert.ToSerialNumberType(text4);
		text3 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
		Smart.Web.PcbaSuccessUpdate(text4, text3, text, string.Empty, string.Empty, string.Empty);
		if (flag2)
		{
			text4 = base.Cache["SerialNumberOutDual"];
			val2 = Smart.Convert.ToSerialNumberType(text4);
			text3 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
			Smart.Web.PcbaSuccessUpdate(text4, text3, text, string.Empty, string.Empty, string.Empty);
		}
		LogPass();
	}
}
