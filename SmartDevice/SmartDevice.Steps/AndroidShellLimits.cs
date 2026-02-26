using System;
using ISmart;

namespace SmartDevice.Steps;

public class AndroidShellLimits : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).Command;
		double num = ((dynamic)base.Info.Args).Max;
		double num2 = ((dynamic)base.Info.Args).Min;
		string empty = string.Empty;
		try
		{
			empty = Smart.ADB.Shell(val.ID, text, 10000);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error sending ADB shell command: " + ex.Message);
			Smart.Log.Error(TAG, ex.ToString());
			LogResult((Result)4, "Could not send ADB shell command", ex.Message);
			return;
		}
		Smart.Log.Info(TAG, "shell response:" + empty);
		double num3 = double.Parse(empty);
		int num4;
		int num5;
		if (num2 <= num3)
		{
			num4 = ((num3 <= num) ? 1 : 0);
			if (num4 != 0)
			{
				num5 = 8;
				goto IL_0305;
			}
		}
		else
		{
			num4 = 0;
		}
		num5 = 1;
		goto IL_0305;
		IL_0305:
		Result result = (Result)num5;
		string description = ((num4 != 0) ? "Value within limits" : "Value outside of limits");
		LogResult(result, description, num, num2, num3);
	}
}
