using System;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class VerifyClientVersion : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Version version = new Version(Smart.App.Version);
		Result result = (Result)8;
		if (((dynamic)base.Info.Args).Expected != null)
		{
			Version version2 = new Version(((dynamic)base.Info.Args).Expected);
			if (version < version2)
			{
				result = (Result)1;
				if (((dynamic)base.Info.Args).PromptText != null)
				{
					string text = ((dynamic)base.Info.Args).PromptText.ToString();
					text = Smart.Locale.Xlate(text);
					string text2 = Smart.Locale.Xlate(base.Info.Name);
					val.Prompt.MessageBox(text2, text, (MessageBoxButtons)0, (MessageBoxIcon)64);
				}
			}
		}
		VerifyOnly(ref result);
		LogResult(result);
	}
}
