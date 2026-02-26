using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ISmart;

namespace SmartDevice.Steps;

public class VerifyAmpsConnectors : BaseStep
{
	private byte[] CWIRE_MSM_GPIO44_HIGH = new byte[1] { 1 };

	private int[] AMPS_AIN0_LOW_VOLTAGE = new int[2] { 0, 200000 };

	private byte[] CWIRE_MSM_GPIO44_LOW = new byte[1];

	private int[] AMPS_AIN0_HIGH_VOLTAGE = new int[2] { 500000, 5000000 };

	private int[] AMPS_AIN1_C1_VOLTAGE = new int[2] { 1700000, 1850000 };

	private byte[] AMPS_AIN1_C1_GPIO_STATES = new byte[3] { 1, 1, 1 };

	private int[] AMPS_AIN1_C2_VOLTAGE = new int[2] { 595000, 640000 };

	private byte[] AMPS_AIN1_C2_GPIO_STATES = new byte[3] { 0, 1, 1 };

	private int[] AMPS_AIN1_C3_VOLTAGE = new int[2] { 180000, 195000 };

	private byte[] AMPS_AIN1_C3_GPIO_STATES = new byte[3] { 0, 0, 1 };

	private int[] AMPS_AIN1_C4_VOLTAGE = new int[2] { 460000, 495000 };

	private byte[] AMPS_AIN1_C4_GPIO_STATES = new byte[3] { 0, 1, 1 };

	private int[] AMPS_AIN1_C5_VOLTAGE = new int[2] { 1700000, 1850000 };

	private byte[] AMPS_AIN1_C5_GPIO_STATES = new byte[3] { 1, 1, 1 };

	private int[] AMPS_AIN1_C6_VOLTAGE = new int[2] { 359000, 387000 };

	private byte[] AMPS_AIN1_C6_GPIO_STATES = new byte[3] { 0, 1, 1 };

	private int[] AMPS_AIN1_C7_VOLTAGE = new int[2] { 95000, 103500 };

	private byte[] AMPS_AIN1_C7_GPIO_STATES = new byte[3] { 0, 0, 1 };

	private int[] AMPS_AIN1_C8_VOLTAGE = new int[2] { 265000, 290000 };

	private byte[] AMPS_AIN1_C8_GPIO_STATES = new byte[3] { 0, 1, 0 };

	private byte[] AMPS_C9_MSM_GPIO_60_STATES = new byte[3];

	private int[] AMPS_AIN2_VOLTAGE = new int[2] { 2700000, 3400000 };

	private int[] AMPS_AIN3_VOLTAGE = new int[2] { 2900000, 3450000 };

	private byte[] AMPS_I2S_STATE = new byte[1];

	private int ADC_Step = 125;

	private Dictionary<string, byte[]> AMPS_GPIO_STATES_LOOKUP;

	private Dictionary<string, int[]> AMPS_AIN_VOLTAGES_LOOKUP;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		string resultHexString = base.Cache["FSACReadHexResponse"];
		AMPS_GPIO_STATES_LOOKUP = new Dictionary<string, byte[]>
		{
			{ "CWIRE_MSM_GPIO44_HIGH", CWIRE_MSM_GPIO44_HIGH },
			{ "CWIRE_MSM_GPIO44_LOW", CWIRE_MSM_GPIO44_LOW },
			{ "AMPS_AIN1_C1_GPIO_STATES", AMPS_AIN1_C1_GPIO_STATES },
			{ "AMPS_AIN1_C2_GPIO_STATES", AMPS_AIN1_C2_GPIO_STATES },
			{ "AMPS_AIN1_C3_GPIO_STATES", AMPS_AIN1_C3_GPIO_STATES },
			{ "AMPS_AIN1_C4_GPIO_STATES", AMPS_AIN1_C4_GPIO_STATES },
			{ "AMPS_AIN1_C5_GPIO_STATES", AMPS_AIN1_C5_GPIO_STATES },
			{ "AMPS_AIN1_C6_GPIO_STATES", AMPS_AIN1_C6_GPIO_STATES },
			{ "AMPS_AIN1_C7_GPIO_STATES", AMPS_AIN1_C7_GPIO_STATES },
			{ "AMPS_AIN1_C8_GPIO_STATES", AMPS_AIN1_C8_GPIO_STATES },
			{ "AMPS_C9_MSM_GPIO_60_STATES", AMPS_C9_MSM_GPIO_60_STATES },
			{ "AMPS_I2S_STATE", AMPS_I2S_STATE }
		};
		AMPS_AIN_VOLTAGES_LOOKUP = new Dictionary<string, int[]>
		{
			{ "AMPS_AIN0_LOW_VOLTAGE", AMPS_AIN0_LOW_VOLTAGE },
			{ "AMPS_AIN0_HIGH_VOLTAGE", AMPS_AIN0_HIGH_VOLTAGE },
			{ "AMPS_AIN1_C1_VOLTAGE", AMPS_AIN1_C1_VOLTAGE },
			{ "AMPS_AIN1_C2_VOLTAGE", AMPS_AIN1_C2_VOLTAGE },
			{ "AMPS_AIN1_C3_VOLTAGE", AMPS_AIN1_C3_VOLTAGE },
			{ "AMPS_AIN1_C4_VOLTAGE", AMPS_AIN1_C4_VOLTAGE },
			{ "AMPS_AIN1_C5_VOLTAGE", AMPS_AIN1_C5_VOLTAGE },
			{ "AMPS_AIN1_C6_VOLTAGE", AMPS_AIN1_C6_VOLTAGE },
			{ "AMPS_AIN1_C7_VOLTAGE", AMPS_AIN1_C7_VOLTAGE },
			{ "AMPS_AIN1_C8_VOLTAGE", AMPS_AIN1_C8_VOLTAGE },
			{ "AMPS_AIN2_VOLTAGE", AMPS_AIN2_VOLTAGE },
			{ "AMPS_AIN3_VOLTAGE", AMPS_AIN3_VOLTAGE }
		};
		UpdateStatesAndLimitsFromRecipeArgs();
		if (!ValidateTestResults(resultHexString))
		{
			result = (Result)1;
		}
		LogResult(result);
	}

	private bool ValidateTestResults(string resultHexString)
	{
		bool result = true;
		if (!ValidateStates(resultHexString.Substring(2, 2), AMPS_GPIO_STATES_LOOKUP["CWIRE_MSM_GPIO44_HIGH"], "CWIRE_MSM_GPIO44_HIGH") && !ValidateVoltageToRange(resultHexString.Substring(4, 4), AMPS_AIN_VOLTAGES_LOOKUP["AMPS_AIN0_LOW_VOLTAGE"], "AMPS_AIN0_LOW_VOLTAGE"))
		{
			result = false;
		}
		if (!ValidateStates(resultHexString.Substring(8, 2), AMPS_GPIO_STATES_LOOKUP["CWIRE_MSM_GPIO44_LOW"], "CWIRE_MSM_GPIO44_LOW"))
		{
			result = false;
		}
		if (!ValidateVoltageToRange(resultHexString.Substring(10, 4), AMPS_AIN_VOLTAGES_LOOKUP["AMPS_AIN0_HIGH_VOLTAGE"], "AMPS_AIN0_HIGH_VOLTAGE"))
		{
			result = false;
		}
		int num = 7;
		for (int i = 2; i < AMPS_GPIO_STATES_LOOKUP.Count - 2; i++)
		{
			string text = AMPS_AIN_VOLTAGES_LOOKUP.Keys.ElementAt(i);
			if (!ValidateVoltageToRange(resultHexString.Substring(num * 2, 4), AMPS_AIN_VOLTAGES_LOOKUP[text], text))
			{
				result = false;
			}
			num += 2;
			text = AMPS_GPIO_STATES_LOOKUP.Keys.ElementAt(i);
			if (!ValidateStates(resultHexString.Substring(num * 2, 6), AMPS_GPIO_STATES_LOOKUP[text], text))
			{
				result = false;
			}
			num += 3;
		}
		if (!ValidateStates(resultHexString.Substring(94, 6), AMPS_GPIO_STATES_LOOKUP["AMPS_C9_MSM_GPIO_60_STATES"], "AMPS_C9_MSM_GPIO_60_STATES"))
		{
			result = false;
		}
		if (!ValidateVoltageToRange(resultHexString.Substring(100, 4), AMPS_AIN_VOLTAGES_LOOKUP["AMPS_AIN2_VOLTAGE"], "AMPS_AIN2_VOLTAGE"))
		{
			result = false;
		}
		if (!ValidateVoltageToRange(resultHexString.Substring(104, 4), AMPS_AIN_VOLTAGES_LOOKUP["AMPS_AIN3_VOLTAGE"], "AMPS_AIN3_VOLTAGE"))
		{
			result = false;
		}
		if (!ValidateStates(resultHexString.Substring(108, 2), AMPS_GPIO_STATES_LOOKUP["AMPS_I2S_STATE"], "AMPS_I2S_STATE"))
		{
			result = false;
		}
		return result;
	}

	private bool ValidateVoltageToRange(string hexStringVoltage, int[] range, string name)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		bool result = true;
		int num = int.Parse(hexStringVoltage, NumberStyles.HexNumber) * ADC_Step;
		if ((int)CheckLimits(name, num, range[0], range[1], "Volt") == 1)
		{
			string text = $"{name} voltage {num} off limits [{range[0]},{range[1]}]";
			Smart.Log.Error(TAG, text);
			result = false;
		}
		return result;
	}

	private bool ValidateStates(string hexStringStates, byte[] refStates, string name)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		bool result = true;
		byte[] array = Smart.Convert.HexToBytes(hexStringStates);
		int num = Smart.Convert.BytesToInt(array);
		int num2 = Smart.Convert.BytesToInt(refStates);
		if ((int)CheckLimits(name, num, num2, num2, "Binary") == 1)
		{
			string text = $"{name} states {num} off limits [{num2},{num2}]";
			Smart.Log.Error(TAG, text);
			result = false;
		}
		return result;
	}

	private Result CheckLimits(string name, int value, int lowLimit, int highLimit, string unit)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Result val = ((!(value < lowLimit || value > highLimit)) ? ((Result)8) : ((Result)1));
		base.Log.AddResult("AMPConTest-" + base.Info.Name, GetType().Name, val, unit, "", "", (double)highLimit, (double)lowLimit, (double)value, (SortedList<string, object>)null);
		return val;
	}

	private void UpdateStatesAndLimitsFromRecipeArgs()
	{
		if (((dynamic)base.Info.Args).States != null)
		{
			foreach (dynamic item in ((dynamic)base.Info.Args).States)
			{
				string text = DynamicKey(item);
				if (AMPS_GPIO_STATES_LOOKUP.Keys.Contains(text))
				{
					string[] array = ((string)item.Value).Split(new char[1] { ',' });
					for (int i = 0; i < Math.Min(array.Length, AMPS_GPIO_STATES_LOOKUP[text].Length); i++)
					{
						AMPS_GPIO_STATES_LOOKUP[text][i] = byte.Parse(array[i]);
					}
				}
			}
		}
		if (!((((dynamic)base.Info.Args).Limits != null) ? true : false))
		{
			return;
		}
		foreach (dynamic item2 in ((dynamic)base.Info.Args).Limits)
		{
			string text = DynamicKey(item2);
			if (AMPS_AIN_VOLTAGES_LOOKUP.Keys.Contains(text))
			{
				AMPS_AIN_VOLTAGES_LOOKUP[text][0] = item2.Value.Min;
				AMPS_AIN_VOLTAGES_LOOKUP[text][1] = item2.Value.Max;
			}
		}
	}
}
