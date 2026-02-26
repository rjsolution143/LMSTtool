using System;
using ISmart;
using SmartDevice.Cfc;

namespace SmartDevice.Steps;

public class CheckBattChipID : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		long timeoutInMilliSeconds = Convert.ToInt64(((dynamic)base.Info.Args).Timeout);
		if (new BattchipidCheckP2K
		{
			TimeoutInMilliSeconds = timeoutInMilliSeconds
		}.Execute(TestCommand))
		{
			LogPass();
		}
		else
		{
			LogResult((Result)1, "CheckBattChipID failed");
		}
	}

	protected string TestCommand(string opCode, string data)
	{
		return base.tcmd.SendCommand(opCode, data).DataHex;
	}
}
