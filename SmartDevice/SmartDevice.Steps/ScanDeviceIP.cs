using System;
using ISmart;

namespace SmartDevice.Steps;

public class ScanDeviceIP : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = val.IP;
		if (val.IP == null || val.IP == string.Empty)
		{
			TCMD tCMD = new TCMD();
			int num = 30;
			if (((dynamic)base.Info.Args).Timeout != null)
			{
				num = ((dynamic)base.Info.Args).Timeout;
			}
			DateTime now = DateTime.Now;
			while (DateTime.Now.Subtract(now).TotalSeconds < (double)num)
			{
				if (tCMD.FindDevices().Contains(val.ID))
				{
					text = tCMD.FindDevice(val.ID);
					break;
				}
			}
			if (text == null || text == string.Empty)
			{
				Smart.Log.Error(TAG, $"Timed out scanning for device {val.ID}");
				try
				{
					Smart.Log.Debug(TAG, "Capturing local network details...");
					string text2 = Smart.File.RunCommand("ipconfig /all");
					Smart.Log.Debug(TAG, text2);
				}
				catch (Exception)
				{
				}
				throw new TimeoutException("Timed out scanning for device");
			}
		}
		else
		{
			Smart.Log.Debug(TAG, $"Device IP already detected as {text}");
		}
		base.Cache["deviceIP"] = text;
		val.IP = text;
		LogPass();
	}
}
