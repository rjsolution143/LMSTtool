using System;

namespace SmartDevice.Steps;

public class GpsRsuRequest : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		throw new NotSupportedException("Step not supported, please remove from recipe");
	}
}
