using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class CheckZeroTouch : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0a42: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a25: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f5: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (((dynamic)base.Info.Args).InputName != null && ((dynamic)base.Info.Args).InputName != string.Empty)
		{
			empty = ((dynamic)base.Info.Args).InputName.ToString();
			empty2 = base.Cache[empty];
		}
		else
		{
			empty = "DeviceSerialNumber";
			empty2 = val.SerialNumber;
			if (empty2 == string.Empty || empty2 == "UNKNOWN")
			{
				empty2 = val.ID;
			}
		}
		bool flag = false;
		if (((dynamic)base.Info.Args).Expected != null)
		{
			flag = ((dynamic)base.Info.Args).Expected;
		}
		bool flag2 = false;
		if (((dynamic)base.Info.Args).BlockUseCases != null)
		{
			flag2 = ((dynamic)base.Info.Args).BlockUseCases;
		}
		Smart.Log.Debug(TAG, $"Checking {empty} value '{empty2}' for Zero Touch status");
		bool flag3 = Smart.Web.CheckZeroTouch(empty2);
		string text = "ZeroTouchStatus";
		if (empty != "DeviceSerialNumber")
		{
			text = empty + "-" + text;
		}
		val.Log.AddInfo(text, flag3.ToString());
		if (flag != flag3)
		{
			Result result = (Result)1;
			Smart.Log.Debug(TAG, $"Zero Touch status for '{empty2}' is {flag3}, expected {flag}");
			if (((dynamic)base.Info.Args).PromptText != null)
			{
				string text2 = ((dynamic)base.Info.Args).PromptText.ToString();
				text2 = Smart.Locale.Xlate(text2);
				string text3 = Smart.Locale.Xlate(base.Info.Name);
				val.Prompt.MessageBox(text3, text2, (MessageBoxButtons)0, (MessageBoxIcon)64);
			}
			if (flag2)
			{
				val.ZeroTouchDevice = true;
			}
			SetPreCondition(((object)(Result)(ref result)).ToString());
			VerifyOnly(ref result);
			if (!flag)
			{
				LogResult(result, "Connected device is Zero Touch device");
			}
			else
			{
				LogResult(result, "Only Zero Touch device is supported");
			}
		}
		else
		{
			Result val2 = (Result)8;
			SetPreCondition(((object)(Result)(ref val2)).ToString());
			LogPass();
		}
	}
}
