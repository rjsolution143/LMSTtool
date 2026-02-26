using System;

namespace SmartDevice.Steps;

public class Sleep : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		int num = ((dynamic)base.Info.Args).Milliseconds;
		Smart.Thread.Wait(TimeSpan.FromMilliseconds(num));
		LogPass();
	}
}
