using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class AndroidCommand : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e22: Invalid comparison between Unknown and I4
		//IL_0e24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e27: Invalid comparison between Unknown and I4
		//IL_0e3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2d: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).Command;
		int num = 200000;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
			num *= 1000;
		}
		string empty = string.Empty;
		try
		{
			string filePathName = Smart.Rsd.GetFilePathName("adbExe", base.Recipe.Info.UseCase, val);
			int num2 = -1;
			List<string> list = Smart.MotoAndroid.Shell(val.ID, text, num, filePathName, ref num2, 6000, false);
			empty = string.Join("\r\n", list.ToArray());
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error sending ADB Command: " + ex.Message);
			Smart.Log.Error(TAG, ex.ToString());
			LogResult((Result)4, "Could not send ADB Command", ex.Message);
			return;
		}
		if (((dynamic)base.Info.Args).PromptText != null)
		{
			string text2 = ((dynamic)base.Info.Args).PromptText.ToString();
			text2 = Smart.Locale.Xlate(text2);
			string text3 = Smart.Locale.Xlate(base.Info.Name);
			val.Prompt.MessageBox(text3, text2, (MessageBoxButtons)0, (MessageBoxIcon)64);
		}
		if (((dynamic)base.Info.Args).RemovedText != null)
		{
			string oldValue = ((dynamic)base.Info.Args).RemovedText.ToString();
			empty = empty.Replace(oldValue, string.Empty);
		}
		string contained = ((dynamic)base.Info.Args).Expected;
		string notContained = ((dynamic)base.Info.Args).NotExpected;
		bool containedAll = false;
		if (((dynamic)base.Info.Args).ExpectAll != null)
		{
			containedAll = Convert.ToBoolean(((dynamic)base.Info.Args).ExpectAll.ToString());
		}
		Result result = VerifyContainedPropertyValue(contained, notContained, empty, logOnFailed: false, "value", containedAll);
		string key = text;
		if (((dynamic)base.Info.Args).InputName != null && ((dynamic)base.Info.Args).InputName != string.Empty)
		{
			key = ((dynamic)base.Info.Args).InputName.ToString();
		}
		base.Cache[key] = empty;
		SetPreCondition(empty);
		VerifyOnly(ref result);
		if (((int)result == 1 || (int)result == 4) && empty != null)
		{
			LogResult(result, "Android Command failed", empty);
		}
		else
		{
			LogResult(result);
		}
	}
}
