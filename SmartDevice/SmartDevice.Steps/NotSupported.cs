using System;
using ISmart;

namespace SmartDevice.Steps;

public class NotSupported : BaseStep
{
	public override void Run()
	{
		bool flag = false;
		if (((dynamic)base.Info.Args).SkipNotSupported != null)
		{
			flag = ((dynamic)base.Info.Args).SkipNotSupported;
		}
		if (flag)
		{
			LogResult((Result)7);
			return;
		}
		throw new Exception("Not supported step");
	}
}
