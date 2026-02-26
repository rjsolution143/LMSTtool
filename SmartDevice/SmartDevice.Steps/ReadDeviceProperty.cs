using System;
using ISmart;

namespace SmartDevice.Steps;

public class ReadDeviceProperty : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).Property;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		string text2 = string.Empty;
		switch (text)
		{
		case "RoCarrier":
			text2 = val.RoCarrier;
			break;
		case "ManufacturingDate":
			text2 = val.ManufacturingDate;
			break;
		case "IP":
			text2 = val.IP;
			break;
		case "ManualDevice":
			text2 = val.ManualDevice.ToString();
			break;
		case "Type":
		{
			DeviceType type = val.Type;
			text2 = ((object)(DeviceType)(ref type)).ToString();
			break;
		}
		case "WiFiOnlyDevice":
			text2 = val.WiFiOnlyDevice.ToString();
			break;
		case "Mode":
			foreach (DeviceMode value in Enum.GetValues(typeof(DeviceMode)))
			{
				DeviceMode val3 = value;
				if (((Enum)val.Mode).HasFlag((Enum)(object)val3))
				{
					text2 = text2 + ((object)(DeviceMode)(ref val3)).ToString() + "+";
				}
			}
			text2 = text2.Substring(0, text2.Length - 1);
			break;
		case "LastMode":
			foreach (DeviceMode value2 in Enum.GetValues(typeof(DeviceMode)))
			{
				DeviceMode val2 = value2;
				if (((Enum)val.LastMode).HasFlag((Enum)(object)val2))
				{
					text2 = text2 + ((object)(DeviceMode)(ref val2)).ToString() + "+";
				}
			}
			text2 = text2.Substring(0, text2.Length - 1);
			break;
		case "TrackId":
			text2 = val.ID;
			break;
		case "Group":
			text2 = val.Group;
			break;
		default:
			text2 = val.GetLogInfoValue(text);
			break;
		}
		base.Cache[text] = text2;
		SetPreCondition(text2);
		Result result = VerifyPropertyValue(text2, logOnFailed: false, text);
		VerifyOnly(ref result);
		LogResult(result);
	}
}
