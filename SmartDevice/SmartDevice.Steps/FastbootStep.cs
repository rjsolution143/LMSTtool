using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public abstract class FastbootStep : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Setup()
	{
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0807: Unknown result type (might be due to invalid IL or missing references)
		base.Setup();
		if (!((((dynamic)base.Info.Args).ConnectionCheck != null && ((dynamic)base.Info.Args).ConnectionCheck == true) ? true : false))
		{
			return;
		}
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		DateTime now = DateTime.Now;
		int num = 90;
		if (((dynamic)base.Info.Args).ConnectionTimeout != null)
		{
			num = ((dynamic)base.Info.Args).ConnectionTimeout;
		}
		string text = "getvar version";
		if (((dynamic)base.Info.Args).ConnectionCommand != null && ((dynamic)base.Info.Args).ConnectionCommand != string.Empty)
		{
			text = ((dynamic)base.Info.Args).ConnectionCommand;
		}
		while (true)
		{
			if (!((Enum)val.Mode).HasFlag((Enum)(object)(DeviceMode)4))
			{
				Smart.Log.Verbose(TAG, "Waiting for Fastboot mode...");
				Smart.Thread.Wait(TimeSpan.FromSeconds(5.0));
				if (DateTime.Now.Subtract(now).TotalSeconds > (double)num)
				{
					throw new TimeoutException("Timed out waiting for Fastboot mode");
				}
				continue;
			}
			if (DateTime.Now.Subtract(now).TotalSeconds > (double)num)
			{
				throw new TimeoutException("Failed to send test Fastboot command");
			}
			Smart.Log.Verbose(TAG, "Sending test Fastboot command");
			string filePathName = Smart.Rsd.GetFilePathName("fastbootExe", base.Recipe.Info.UseCase, val);
			int num2 = -1;
			List<string> list = Smart.MotoAndroid.Shell(val.ID, text, 10, filePathName, ref num2, 6000, false);
			string text2 = string.Empty;
			if (list.Count > 0)
			{
				text2 = string.Join(Environment.NewLine, list.ToArray());
			}
			if (num2 == 0)
			{
				break;
			}
			Smart.Log.Error(TAG, $"Test Fastboot command failed with exit status {num2}");
			Smart.Log.Verbose(TAG, text2);
			Smart.Thread.Wait(TimeSpan.FromSeconds(5.0));
		}
		Smart.Log.Verbose(TAG, "Test Fastboot command success");
	}
}
