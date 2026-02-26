using System;
using ISmart;

namespace SmartDevice.Steps;

public class UninstallAPK : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Invalid comparison between Unknown and I4
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		string empty = string.Empty;
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).APKName;
		try
		{
			Smart.ADB.Uninstall(val.ID, text);
		}
		catch (Exception ex)
		{
			empty = ex.Message;
			Smart.Log.Error(TAG, $"Uninstall errorMsg: {empty}");
			result = (Result)1;
		}
		VerifyOnly(ref result);
		if ((int)result == 1)
		{
			LogResult(result, "APK uninstallation failed", $"Failed to uninstall {text}");
		}
		else
		{
			LogResult(result);
		}
	}
}
