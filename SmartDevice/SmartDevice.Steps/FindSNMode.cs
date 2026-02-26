using System;
using ISmart;

namespace SmartDevice.Steps;

public class FindSNMode : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		DeviceMode lastMode = ((IDevice)((dynamic)base.Recipe.Info.Args).Device).LastMode;
		DeviceMode val = (DeviceMode)1;
		if (((Enum)lastMode).HasFlag((Enum)(object)(DeviceMode)4))
		{
			val = (DeviceMode)4;
		}
		else if (((Enum)lastMode).HasFlag((Enum)(object)(DeviceMode)2))
		{
			val = (DeviceMode)2;
		}
		else
		{
			Smart.Log.Warning(TAG, "Unrecognized mode " + ((object)(DeviceMode)(ref lastMode)).ToString());
		}
		SetPreCondition(((object)(DeviceMode)(ref val)).ToString());
		base.Cache["SNMode"] = val;
		LogPass();
	}
}
