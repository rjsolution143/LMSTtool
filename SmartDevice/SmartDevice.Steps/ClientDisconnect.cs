using System;
using ISmart;

namespace SmartDevice.Steps;

public abstract class ClientDisconnect : BaseStep
{
	private string TAG => GetType().FullName;

	protected abstract string ClientName { get; }

	public override void Run()
	{
		if (!base.Cache.ContainsKey(ClientName))
		{
			LogResult((Result)7);
			return;
		}
		((IDisposable)(INetworkClient)base.Cache[ClientName]).Dispose();
		base.Cache.Remove(ClientName);
		LogPass();
	}
}
