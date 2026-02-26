using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class WaitForDevice : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Invalid comparison between Unknown and I4
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string iD = device.ID;
		int num = 30;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
		}
		DeviceMode val = (DeviceMode)1;
		if (((dynamic)base.Info.Args).Mode != null)
		{
			string text = ((dynamic)base.Info.Args).Mode;
			if (text.Trim() != string.Empty)
			{
				val = (DeviceMode)Enum.Parse(typeof(DeviceMode), text);
			}
		}
		int num2 = num / 2;
		if (((dynamic)base.Info.Args).PromptDelay != null)
		{
			num2 = ((dynamic)base.Info.Args).PromptDelay;
		}
		DateTime now = DateTime.Now;
		bool flag = false;
		bool flag2 = true;
		while (!flag)
		{
			try
			{
				if (!Smart.DeviceManager.Devices.ContainsKey(iD))
				{
					continue;
				}
				IDevice val2 = Smart.DeviceManager.Devices[iD];
				if ((int)val == 1 || ((Enum)val2.Mode).HasFlag((Enum)(object)val))
				{
					flag = true;
					break;
				}
				double totalSeconds = DateTime.Now.Subtract(now).TotalSeconds;
				if (totalSeconds > (double)num && !flag)
				{
					Smart.Log.Error(TAG, $"Timed out waiting for device {iD} in mode {val}");
					throw new TimeoutException("Timed out waiting for device");
				}
				if (((dynamic)base.Info.Args).PromptText != null && flag2 && totalSeconds > (double)num2)
				{
					flag2 = false;
					Task.Run(delegate
					{
						//IL_011c: Unknown result type (might be due to invalid IL or missing references)
						string text2 = ((dynamic)base.Info.Args).PromptText.ToString();
						text2 = Smart.Locale.Xlate(text2);
						string text3 = Smart.Locale.Xlate(base.Info.Name);
						device.Prompt.MessageBox(text3, text2, (MessageBoxButtons)0, (MessageBoxIcon)64);
					});
				}
			}
			catch (Exception)
			{
				Smart.Thread.Wait(TimeSpan.FromSeconds(5.0));
				if (DateTime.Now.Subtract(now).TotalSeconds < (double)num)
				{
					continue;
				}
				throw;
			}
		}
		SetPreCondition(((object)(DeviceMode)(ref val)).ToString());
		LogPass();
	}
}
