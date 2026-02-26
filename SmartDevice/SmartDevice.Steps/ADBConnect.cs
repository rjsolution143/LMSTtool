using System;
using ISmart;

namespace SmartDevice.Steps;

public class ADBConnect : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (!((Enum)val.LastMode).HasFlag((Enum)(object)(DeviceMode)2))
		{
			Smart.Log.Error(TAG, $"Unsupported mode for ADB: {val.LastMode}");
			DeviceMode lastMode = val.LastMode;
			LogResult((Result)4, "Device not in ADB mode", "Device mode is " + ((object)(DeviceMode)(ref lastMode)).ToString());
			return;
		}
		int num = 30;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
		}
		DateTime now = DateTime.Now;
		bool flag = false;
		while (!flag)
		{
			try
			{
				Smart.ADB.FindDevices();
				Smart.ADB.Shell(val.ID, "pwd", 10000);
			}
			catch (Exception ex)
			{
				if (DateTime.Now.Subtract(now).TotalSeconds < (double)num)
				{
					Smart.Thread.Wait(TimeSpan.FromSeconds(5.0));
					continue;
				}
				Smart.Log.Error(TAG, "ADB connection timed out");
				Smart.Log.Error(TAG, ex.ToString());
				LogResult((Result)4, "ADB connection timed out");
				return;
			}
			flag = true;
		}
		LogPass();
	}
}
