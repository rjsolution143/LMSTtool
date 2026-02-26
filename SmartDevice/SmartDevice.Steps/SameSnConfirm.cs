using ISmart;

namespace SmartDevice.Steps;

public class SameSnConfirm : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Invalid comparison between Unknown and I4
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Invalid comparison between Unknown and I4
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).InputName.ToString();
		string text2 = base.Cache[text];
		string text3 = val.ID;
		string text4 = string.Empty;
		if (base.Cache.ContainsKey(text + "Dual"))
		{
			text4 = base.Cache[text + "Dual"];
		}
		bool flag = (int)base.Log.OverallResult == 8;
		if ((int)val.Type == 2)
		{
			if (val.Group == string.Empty)
			{
				text3 = ((val.PSN.Length > 10) ? val.PSN.Substring(val.PSN.Length - 10, 10) : val.PSN);
			}
			if (val.WiFiOnlyDevice)
			{
				string text5 = base.Cache["GSNIn"];
				Smart.Web.SameSnConfirm(text5, string.Empty, text3, flag);
				LogPass();
				return;
			}
		}
		Smart.Web.SameSnConfirm(text2, text4, text3, flag);
		LogPass();
	}
}
