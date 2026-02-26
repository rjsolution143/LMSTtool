using System;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class InfoPrompt : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Invalid comparison between Unknown and I4
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0981: Unknown result type (might be due to invalid IL or missing references)
		//IL_0982: Unknown result type (might be due to invalid IL or missing references)
		//IL_0983: Unknown result type (might be due to invalid IL or missing references)
		//IL_0988: Unknown result type (might be due to invalid IL or missing references)
		//IL_098a: Unknown result type (might be due to invalid IL or missing references)
		//IL_098f: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (((dynamic)base.Info.Args).PromptText != null)
		{
			string text = ((dynamic)base.Info.Args).PromptText.ToString();
			text = Smart.Locale.Xlate(text);
			if ((int)val.Type == 2 && val.WiFiOnlyDevice && ((dynamic)base.Info.Args).PromptText1 != null)
			{
				text = ((dynamic)base.Info.Args).PromptText1;
				text = Smart.Locale.Xlate(text);
			}
			MessageBoxButtons val2 = (MessageBoxButtons)0;
			if (((dynamic)base.Info.Args).PromptType != null)
			{
				string value = ((dynamic)base.Info.Args).PromptType;
				val2 = (MessageBoxButtons)Enum.Parse(typeof(MessageBoxButtons), value, ignoreCase: true);
			}
			MessageBoxIcon val3 = (MessageBoxIcon)64;
			if (((dynamic)base.Info.Args).IconType != null)
			{
				string value2 = ((dynamic)base.Info.Args).IconType;
				val3 = (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), value2, ignoreCase: true);
			}
			string text2 = Smart.Locale.Xlate(base.Info.Name);
			if (((dynamic)base.Info.Args).Title != null)
			{
				text2 = ((dynamic)base.Info.Args).Title;
				text2 = Smart.Locale.Xlate(text2);
			}
			DialogResult val4 = val.Prompt.MessageBox(text2, text, val2, val3);
			if ((int)val4 == 0)
			{
				val4 = (DialogResult)2;
			}
			string text3 = ((object)(DialogResult)(ref val4)).ToString();
			string text4 = string.Empty;
			if (((dynamic)base.Info.Args).InputName != null)
			{
				text4 = ((dynamic)base.Info.Args).InputName.ToString();
			}
			if (!string.IsNullOrEmpty(text4))
			{
				base.Cache[text4] = text3;
				base.Log.AddInfo(text4, text3);
				base.Log.AddInfo("InputName", text4);
			}
			SetPreCondition(((object)(DialogResult)(ref val4)).ToString());
		}
		LogPass();
	}
}
