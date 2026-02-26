using System;
using ISmart;

namespace SmartDevice.Steps;

public class ReadDeviceMode : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Invalid comparison between Unknown and I4
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = string.Empty;
		DeviceMode val2 = val.Mode;
		if (((dynamic)base.Info.Args).LastMode != null)
		{
			val2 = val.LastMode;
		}
		foreach (DeviceMode value in Enum.GetValues(typeof(DeviceMode)))
		{
			DeviceMode val3 = value;
			if (((Enum)val2).HasFlag((Enum)(object)val3))
			{
				text = text + ((object)(DeviceMode)(ref val3)).ToString() + "+";
			}
		}
		text = text.Substring(0, text.Length - 1);
		SetPreCondition(text);
		Smart.Log.Info(TAG, $"Device mode: {text}");
		if ((int)VerifyPropertyValue(text, logOnFailed: true, "mode") == 8)
		{
			LogPass();
		}
	}
}
