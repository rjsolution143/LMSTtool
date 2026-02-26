using System;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class AndroidShellSelect : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Invalid comparison between Unknown and I4
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Invalid comparison between Unknown and I4
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).PromptText.ToString();
		text = Smart.Locale.Xlate(text);
		string text2 = Smart.Locale.Xlate(base.Info.Name);
		DialogResult val2 = val.Prompt.MessageBox(text2, text, (MessageBoxButtons)4, (MessageBoxIcon)32);
		string key;
		string text3;
		if ((int)val2 == 6)
		{
			key = "AndroidShellSelectResp1";
			text3 = ((dynamic)base.Info.Args).Command1;
		}
		else
		{
			if ((int)val2 != 7)
			{
				throw new Exception("User cancelled");
			}
			key = "AndroidShellSelectResp2";
			text3 = ((dynamic)base.Info.Args).Command2;
		}
		string empty = string.Empty;
		try
		{
			empty = Smart.ADB.Shell(val.ID, text3, 10000);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error sending ADB shell command: " + ex.Message);
			Smart.Log.Error(TAG, ex.ToString());
			LogResult((Result)4, "Could not send ADB shell command", ex.Message);
			return;
		}
		Smart.Log.Verbose(TAG, "ADB Shell resp: " + empty);
		if (base.Cache.ContainsKey(key))
		{
			base.Cache[key] = empty;
		}
		else
		{
			base.Cache.Add(key, empty);
		}
		SetPreCondition(empty);
		LogPass();
	}
}
