using System;
using ISmart;

namespace SmartDevice.Steps;

public abstract class ClientConnect : BaseStep
{
	private string TAG => GetType().FullName;

	protected abstract string ClientName { get; }

	protected abstract INetworkClient Client();

	public override void Run()
	{
		string text = "127.0.0.1";
		if (base.Cache.ContainsKey("deviceIP"))
		{
			text = base.Cache["deviceIP"];
		}
		int num = 30;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
		}
		int num2 = ((dynamic)base.Info.Args).Port;
		string key = $"Port{num2}";
		if (base.Cache.ContainsKey(key))
		{
			num2 = base.Cache[key];
		}
		DateTime now = DateTime.Now;
		INetworkClient val = Client();
		while (DateTime.Now.Subtract(now).TotalSeconds < (double)num && !val.IsConnected())
		{
			try
			{
				val.SetEndPoint(text, num2);
				val.Connect();
				IThread thread = Smart.Thread;
				TimeSpan timeSpan = TimeSpan.FromSeconds(20.0);
				INetworkClient obj = val;
				thread.Wait(timeSpan, (Checker<bool>)obj.IsConnected);
			}
			finally
			{
				if (!val.IsConnected())
				{
					((IDisposable)val).Dispose();
					val = Client();
				}
			}
		}
		if (!val.IsConnected())
		{
			throw new TimeoutException($"Connection to {ClientName} timed out");
		}
		base.Cache[ClientName] = val;
		LogPass();
	}
}
