using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class VerifyDeviceType : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Invalid comparison between Unknown and I4
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Invalid comparison between Unknown and I4
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		DeviceType type = val.Type;
		Result val2 = VerifyPropertyValue(((object)(DeviceType)(ref type)).ToString(), logOnFailed: true, "device type");
		if ((int)val2 == 1 && ((((dynamic)base.Info.Args).PromptText != null) ? true : false))
		{
			string text = ((dynamic)base.Info.Args).PromptText.ToString();
			text = Smart.Locale.Xlate(text);
			string text2 = Smart.Locale.Xlate(base.Info.Name);
			val.Prompt.MessageBox(text2, text, (MessageBoxButtons)0, (MessageBoxIcon)64);
		}
		if ((int)val2 == 8)
		{
			LogPass();
		}
	}
}
