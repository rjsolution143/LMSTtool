using System;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class VerifySDCardPresent : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).DevicePath;
		string text2 = "ls " + text;
		string empty = string.Empty;
		try
		{
			empty = Smart.ADB.Shell(val.ID, text2, 10000);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error sending ADB shell command: " + ex.Message);
			Smart.Log.Error(TAG, ex.ToString());
			LogResult((Result)4, "Could not send ADB shell command", ex.Message);
			return;
		}
		bool flag = true;
		if (empty.Contains("failed") || empty.Contains("denied"))
		{
			if (((dynamic)base.Info.Args).PromptText != null)
			{
				string text3 = ((dynamic)base.Info.Args).PromptText.ToString();
				text3 = Smart.Locale.Xlate(text3);
				string text4 = Smart.Locale.Xlate(base.Info.Name);
				val.Prompt.MessageBox(text4, text3, (MessageBoxButtons)0, (MessageBoxIcon)16);
			}
			flag = false;
		}
		bool flag2 = true;
		if (((dynamic)base.Info.Args).Present != null)
		{
			flag2 = ((dynamic)base.Info.Args).Present;
		}
		if (flag2 == flag)
		{
			LogPass();
		}
		else
		{
			LogResult((Result)1, "SD card not in correct state");
		}
	}
}
