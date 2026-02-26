using System;
using ISmart;

namespace SmartDevice.Steps;

public class PhysicalDeviceMerge : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0d62: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6c: Invalid comparison between Unknown and I4
		//IL_0cfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d05: Invalid comparison between Unknown and I4
		//IL_0d0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d14: Invalid comparison between Unknown and I4
		//IL_0d1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d23: Invalid comparison between Unknown and I4
		//IL_0d2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d32: Invalid comparison between Unknown and I4
		//IL_0d3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d41: Invalid comparison between Unknown and I4
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (val == null)
		{
			throw new NotSupportedException("No device associated with currently running use case");
		}
		if (!val.ManualDevice)
		{
			Smart.Log.Warning(TAG, "No Manual Device for PhysicalDeviceMerge step");
			LogResult((Result)7, "Device merge requires a manually registered device");
			return;
		}
		bool flag = true;
		if (((dynamic)base.Info.Args).SinglePhysical != null)
		{
			flag = ((dynamic)base.Info.Args).SinglePhysical;
		}
		bool flag2 = false;
		if (((dynamic)base.Info.Args).MatchSN != null)
		{
			flag2 = ((dynamic)base.Info.Args).MatchSN;
		}
		bool flag3 = false;
		if (((dynamic)base.Info.Args).MatchTrackID != null)
		{
			flag3 = ((dynamic)base.Info.Args).MatchTrackID;
		}
		bool flag4 = false;
		if (((dynamic)base.Info.Args).MatchModel != null)
		{
			flag4 = ((dynamic)base.Info.Args).MatchModel;
		}
		bool flag5 = true;
		if (((dynamic)base.Info.Args).WaitForDevice != null)
		{
			flag5 = ((dynamic)base.Info.Args).WaitForDevice;
		}
		bool flag6 = true;
		if (((dynamic)base.Info.Args).WaitForReady != null)
		{
			flag6 = ((dynamic)base.Info.Args).WaitForReady;
		}
		int num = 30;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
		}
		IDevice val2 = null;
		DateTime now = DateTime.Now;
		while (val2 == null)
		{
			if (flag5)
			{
				Smart.Thread.Wait(TimeSpan.FromSeconds(1.0));
				if (DateTime.Now.Subtract(now).TotalSeconds > (double)num)
				{
					throw new TimeoutException("Timed out waiting for physical device");
				}
			}
			if (flag && Smart.DeviceManager.Devices.Count > 2)
			{
				Smart.Log.Debug(TAG, "Too many devices connected...");
				if (!flag5)
				{
					break;
				}
				continue;
			}
			foreach (IDevice value in Smart.DeviceManager.Devices.Values)
			{
				if (!value.ManualDevice && (!flag2 || !(val.SerialNumber != value.SerialNumber)) && (!flag3 || !(val.ID != value.ID)) && (!flag4 || !(val.ModelId != value.ModelId)))
				{
					val2 = value;
					break;
				}
			}
		}
		if (flag6)
		{
			while ((int)val2.Log.UseCase != 134)
			{
				if (DateTime.Now.Subtract(now).TotalSeconds > (double)num)
				{
					throw new TimeoutException("Timed out waiting for LMST_Read");
				}
				if ((int)val2.Log.UseCase == 141 && ((int)val2.Log.OverallResult == 1 || (int)val2.Log.OverallResult == 4 || (int)val2.Log.OverallResult == 2 || (int)val2.Log.OverallResult == 5))
				{
					break;
				}
				Smart.Thread.Wait(TimeSpan.FromSeconds(1.0));
			}
			Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
		}
		if (val2.Locked)
		{
			if (!flag5)
			{
				throw new NotSupportedException("Cannot merge locked physical device");
			}
			while (val2.Locked)
			{
				Smart.Thread.Wait(TimeSpan.FromSeconds(1.0));
				if (DateTime.Now.Subtract(now).TotalSeconds > (double)num)
				{
					throw new TimeoutException("Timed out waiting for locked device");
				}
			}
		}
		Smart.DeviceManager.MergeManualDevice(val, val2);
		LogPass();
	}
}
