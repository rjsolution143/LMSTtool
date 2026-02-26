using System.Collections.Generic;

namespace SmartDevice.Steps;

public class BackgroundScanStart : BaseStep
{
	protected static SortedList<string, int> locks = new SortedList<string, int>();

	protected static object locker = new object();

	private string TAG => GetType().FullName;

	public override void Run()
	{
		Smart.DeviceManager.BackgroundScan = true;
		Smart.Log.Debug(TAG, "Turning on background device scan");
		LogPass();
	}
}
