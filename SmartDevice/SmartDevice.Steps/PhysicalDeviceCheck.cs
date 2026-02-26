using System;
using ISmart;

namespace SmartDevice.Steps;

public class PhysicalDeviceCheck : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		if ((((IDevice)((dynamic)base.Recipe.Info.Args).Device) ?? throw new NotSupportedException("No device to check")).ManualDevice)
		{
			LogResult((Result)1, "Physically connected device is required");
		}
		else
		{
			LogPass();
		}
	}
}
