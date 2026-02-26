using System;
using ISmart;

namespace SmartDevice.Steps;

public class ADBRemoveForward : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		int num = ((dynamic)base.Info.Args).Port;
		string key = $"Port{num}";
		int num2 = base.Cache[key];
		try
		{
			Smart.ADB.RemoveForward(val.ID, num2);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error removing port forwarding: " + ex.Message);
			Smart.Log.Error(TAG, ex.ToString());
			LogResult((Result)4, "Error removing up port forwarding", ex.Message);
			return;
		}
		LogPass();
	}
}
