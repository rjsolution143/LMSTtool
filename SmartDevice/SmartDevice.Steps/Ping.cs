using System;
using ISmart;

namespace SmartDevice.Steps;

public class Ping : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		ICommServerClient val = (ICommServerClient)base.Cache["commServer"];
		int num = 30;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
		}
		if ((double)num < val.Timeout.TotalSeconds)
		{
			val.Timeout = TimeSpan.FromSeconds(num);
		}
		DateTime now = DateTime.Now;
		bool flag = false;
		while (!flag)
		{
			try
			{
				val.SendCommand("PING");
				flag = true;
			}
			catch (Exception)
			{
				if (DateTime.Now.Subtract(now).TotalSeconds < (double)num)
				{
					Smart.Thread.Wait(TimeSpan.FromSeconds(5.0));
					continue;
				}
				base.Cache.Remove("commServer");
				throw;
			}
		}
		LogPass();
	}
}
