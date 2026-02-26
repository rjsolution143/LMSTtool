using System.Collections.Generic;
using System.Globalization;
using ISmart;

namespace SmartDevice.Steps;

public class VerifyAmpsUSB : BaseStep
{
	private byte[] AMPS_USB_SS_STATUS = new byte[2];

	private byte[] AMPS_OFF_SS_STATUS = new byte[2];

	private byte[] AMPS_USB_HS_STATUS = new byte[2];

	private byte[] AMPS_OFF_HS_STATUS = new byte[2];

	private Dictionary<string, byte[]> AMPS_USB_STATUS_LOOKUP;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		string resultHexString = base.Cache["FSACReadHexResponse"];
		AMPS_USB_STATUS_LOOKUP = new Dictionary<string, byte[]>
		{
			{ "AMPS_USB_SS_STATUS", AMPS_USB_SS_STATUS },
			{ "AMPS_OFF_SS_STATUS", AMPS_OFF_SS_STATUS },
			{ "AMPS_USB_HS_STATUS", AMPS_USB_HS_STATUS },
			{ "AMPS_OFF_HS_STATUS", AMPS_OFF_HS_STATUS }
		};
		UpdateLimitsFromRecipeArgs();
		if (!ValidateTestResults(resultHexString))
		{
			result = (Result)1;
		}
		LogResult(result);
	}

	private void UpdateLimitsFromRecipeArgs()
	{
		if (!((((dynamic)base.Info.Args).Limits != null) ? true : false))
		{
			return;
		}
		foreach (dynamic item in ((dynamic)base.Info.Args).Limits)
		{
			string key = DynamicKey(item);
			if (AMPS_USB_STATUS_LOOKUP.ContainsKey(key))
			{
				AMPS_USB_STATUS_LOOKUP[key][0] = item.Value.Min;
				AMPS_USB_STATUS_LOOKUP[key][1] = item.Value.Max;
			}
		}
	}

	private bool ValidateTestResults(string resultHexString)
	{
		bool result = true;
		int num = 0;
		foreach (string key in AMPS_USB_STATUS_LOOKUP.Keys)
		{
			if (!ValidateStateToRange(resultHexString.Substring(num, 2), AMPS_USB_STATUS_LOOKUP[key], key))
			{
				result = false;
			}
			num += 2;
		}
		return result;
	}

	private bool ValidateStateToRange(string hexStringState, byte[] range, string name)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		bool result = true;
		byte b = byte.Parse(hexStringState, NumberStyles.HexNumber);
		if ((int)CheckLimits(name, b, range[0], range[1], "Limits") == 1)
		{
			Smart.Log.Error(TAG, $"{name} state {b} off limits [{range[0]},{range[1]}]");
			result = false;
		}
		return result;
	}

	private Result CheckLimits(string name, byte value, byte lowLimit, byte highLimit, string unit)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Result val = ((!(value < lowLimit || value > highLimit)) ? ((Result)8) : ((Result)1));
		base.Log.AddResult("AMPUsb-" + base.Info.Name, GetType().Name, val, unit, "", "", (double)(int)highLimit, (double)(int)lowLimit, (double)(int)value, (SortedList<string, object>)null);
		return val;
	}
}
