using ISmart;

namespace SmartDevice.Steps;

public class CheckRsdLogs : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		int upgradeLogCount = Smart.Rsd.GetUpgradeLogCount();
		LogResult((Result)8, "RSD Log Count", 10000.0, -1.0, upgradeLogCount);
	}
}
