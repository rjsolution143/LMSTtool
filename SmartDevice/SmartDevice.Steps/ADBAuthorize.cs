using System;
using System.Windows.Forms;
using AdvancedSharpAdbClient.Exceptions;
using ISmart;

namespace SmartDevice.Steps;

public class ADBAuthorize : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Invalid comparison between Unknown and I4
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (!((Enum)val.LastMode).HasFlag((Enum)(object)(DeviceMode)2))
		{
			Smart.Log.Error(TAG, $"Unsupported mode for ADB: {val.LastMode}");
			DeviceMode lastMode = val.LastMode;
			LogResult((Result)4, "Device not in ADB mode", "Device mode is " + ((object)(DeviceMode)(ref lastMode)).ToString());
			return;
		}
		int num = 20;
		if (((dynamic)base.Info.Args).DialogTimeout != null)
		{
			num = ((dynamic)base.Info.Args).DialogTimeout;
		}
		DateTime now = DateTime.Now;
		bool flag = false;
		bool flag2 = false;
		while (!flag)
		{
			try
			{
				Smart.ADB.FindDevices();
				Smart.ADB.Shell(val.ID, "pwd", 10000);
				flag = true;
			}
			catch (Exception innerException)
			{
				Smart.Log.Error(TAG, innerException.Message + Environment.NewLine + innerException.StackTrace);
				if (innerException.InnerException != null)
				{
					innerException = innerException.InnerException;
				}
				if (innerException.GetType() == typeof(AdbException) && innerException.Message.ToLowerInvariant().Contains("unauthorized"))
				{
					flag2 = true;
				}
				if (DateTime.Now.Subtract(now).TotalSeconds < (double)num)
				{
					Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
					continue;
				}
				if (flag2)
				{
					string text = Smart.Locale.Xlate("ADB Unauthorized");
					string text2 = "Please click \"Allow\" or \"Confirm\" on device to authorize Adb connection";
					if (((dynamic)base.Info.Args).PromptText != null)
					{
						text2 = ((dynamic)base.Info.Args).PromptText.ToString();
					}
					string text3 = Smart.Locale.Xlate(text2);
					if ((int)val.Prompt.MessageBox(text, text3, (MessageBoxButtons)1, (MessageBoxIcon)48) != 2)
					{
						break;
					}
					Smart.Log.Error(TAG, "User cancelled ADB connection dialog");
					Smart.Log.Error(TAG, innerException.ToString());
					LogResult((Result)4, "User cancelled ADB connection dialog");
				}
				else
				{
					LogResult((Result)4, "Adb Shell pwd cmd fail");
				}
				return;
			}
		}
		int num2 = 10;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num2 = ((dynamic)base.Info.Args).Timeout;
		}
		now = DateTime.Now;
		while (!flag)
		{
			try
			{
				Smart.ADB.FindDevices();
				Smart.ADB.Shell(val.ID, "pwd", 10000);
			}
			catch (Exception ex)
			{
				if (DateTime.Now.Subtract(now).TotalSeconds < (double)num2)
				{
					Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
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
