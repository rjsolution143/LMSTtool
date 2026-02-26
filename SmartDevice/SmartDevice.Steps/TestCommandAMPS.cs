using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ISmart;

namespace SmartDevice.Steps;

public class TestCommandAMPS : TestCommandStep
{
	private enum Parameter
	{
		EnableAPBAfactorytestmode = 0,
		SetStationGPIO = 1,
		ReadStationGPIO = 2,
		ReadStationADC = 3,
		VerifySPIloopback = 4,
		SetMSMGPIOCCcontrol = 5,
		ReadMSMGPIOControl = 6,
		VerifyANX7816_7805Init = 7,
		DisableAPBA = 16,
		HSICLoopback = 17
	}

	private int GPIO_PIN_NUMBER;

	private int GPIO_VALUE;

	private int ADC_CH_NUMBER;

	private int MSM_GPIO_PIN_NUMBER;

	private int MSM_GPIO_VALUE;

	private int NUMBER_OF_LOOPS_TO_RUN;

	private byte[] AMPS_SPI_STATE = new byte[1];

	private byte[] GPIO_VALUE_STATE = new byte[1];

	private int[] STATION_ADC_VOLTAGE = new int[2] { 0, 5000000 };

	private byte[] MSM_GPIO_VALUE_STATE = new byte[1];

	private byte[] ANX7816_7805INIT_STATE = new byte[1];

	private Dictionary<string, byte[]> AMPS_GPIO_STATES_LOOKUP;

	private Dictionary<string, int[]> AMPS_AIN_VOLTAGES_LOOKUP;

	private int ADC_Step = 125;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0d2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d33: Invalid comparison between Unknown and I4
		//IL_0d48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d45: Unknown result type (might be due to invalid IL or missing references)
		AMPS_GPIO_STATES_LOOKUP = new Dictionary<string, byte[]>
		{
			{ "AMPS_SPI_STATE", AMPS_SPI_STATE },
			{ "GPIO_VALUE_STATE", GPIO_VALUE_STATE },
			{ "MSM_GPIO_VALUE_STATE", MSM_GPIO_VALUE_STATE },
			{ "ANX7816_7805INIT_STATE", ANX7816_7805INIT_STATE }
		};
		AMPS_AIN_VOLTAGES_LOOKUP = new Dictionary<string, int[]> { { "STATION_ADC_VOLTAGE", STATION_ADC_VOLTAGE } };
		UpdateStatesAndLimitsFromRecipeArgs();
		string text = "0112";
		if (((dynamic)base.Info.Args).OpCode != null)
		{
			text = ((dynamic)base.Info.Args).OpCode;
		}
		Parameter parameter = ((((dynamic)base.Info.Args).Action != null) ? ((Parameter)Enum.Parse(typeof(Parameter), (string)((dynamic)base.Info.Args).Action, ignoreCase: true)) : Parameter.VerifySPIloopback);
		GPIO_PIN_NUMBER = ((((dynamic)base.Info.Args).GpioPinNumber != null) ? ((dynamic)base.Info.Args).GpioPinNumber : ((object)0));
		GPIO_VALUE = ((((dynamic)base.Info.Args).GpioValue != null) ? ((dynamic)base.Info.Args).GpioValue : ((object)0));
		ADC_CH_NUMBER = ((((dynamic)base.Info.Args).AdcChannelNumber != null) ? ((dynamic)base.Info.Args).AdcChannelNumber : ((object)0));
		MSM_GPIO_PIN_NUMBER = ((((dynamic)base.Info.Args).MsnGpioPinNumber != null) ? ((dynamic)base.Info.Args).MsnGpioPinNumber : ((object)0));
		MSM_GPIO_VALUE = ((((dynamic)base.Info.Args).MsnGpioValue != null) ? ((dynamic)base.Info.Args).MsnGpioValue : ((object)0));
		NUMBER_OF_LOOPS_TO_RUN = ((((dynamic)base.Info.Args).NumberOfLoopsToRun != null) ? ((dynamic)base.Info.Args).NumberOfLoopsToRun : ((object)1));
		string text2 = BuildData(parameter);
		ITestCommandResponse val = base.tcmd.SendCommand(text, text2);
		Result val2 = (Result)(val.Failed ? 1 : 8);
		if ((int)val2 == 8 && !ValidateTestResults(val.DataHex, parameter))
		{
			val2 = (Result)1;
		}
		LogResult(val2);
	}

	private string BuildData(Parameter parameter)
	{
		string result = string.Empty;
		switch (parameter)
		{
		case Parameter.EnableAPBAfactorytestmode:
		case Parameter.VerifySPIloopback:
		case Parameter.VerifyANX7816_7805Init:
		case Parameter.DisableAPBA:
		{
			int num = (int)parameter;
			result = num.ToString("X2");
			break;
		}
		case Parameter.SetStationGPIO:
		{
			int num = (int)parameter;
			result = num.ToString("X2") + GPIO_PIN_NUMBER.ToString("X2") + GPIO_VALUE.ToString("X2");
			break;
		}
		case Parameter.ReadStationGPIO:
		{
			int num = (int)parameter;
			result = num.ToString("X2") + GPIO_PIN_NUMBER.ToString("X2");
			break;
		}
		case Parameter.ReadStationADC:
		{
			int num = (int)parameter;
			result = num.ToString("X2") + ADC_CH_NUMBER.ToString("X2");
			break;
		}
		case Parameter.SetMSMGPIOCCcontrol:
		{
			int num = (int)parameter;
			result = num.ToString("X2") + MSM_GPIO_PIN_NUMBER.ToString("X2") + MSM_GPIO_VALUE.ToString("X2");
			break;
		}
		case Parameter.ReadMSMGPIOControl:
		{
			int num = (int)parameter;
			result = num.ToString("X2") + MSM_GPIO_PIN_NUMBER.ToString("X2");
			break;
		}
		case Parameter.HSICLoopback:
		{
			int num = (int)parameter;
			result = num.ToString("X2") + NUMBER_OF_LOOPS_TO_RUN.ToString("X4");
			break;
		}
		default:
			Smart.Log.Error(TAG, "Unsupported Parameter " + parameter);
			break;
		}
		return result;
	}

	private bool ValidateTestResults(string resultHexString, Parameter parameter)
	{
		bool result = false;
		switch (parameter)
		{
		case Parameter.EnableAPBAfactorytestmode:
		case Parameter.SetStationGPIO:
		case Parameter.SetMSMGPIOCCcontrol:
		case Parameter.DisableAPBA:
		case Parameter.HSICLoopback:
			result = true;
			break;
		case Parameter.VerifySPIloopback:
			result = ValidateStates(resultHexString, AMPS_GPIO_STATES_LOOKUP["AMPS_SPI_STATE"], "AMPS_SPI_STATE");
			break;
		case Parameter.ReadStationGPIO:
			result = ValidateStates(resultHexString, AMPS_GPIO_STATES_LOOKUP["GPIO_VALUE_STATE"], "GPIO_VALUE_STATE");
			break;
		case Parameter.ReadStationADC:
			result = ValidateVoltageToRange(resultHexString, AMPS_AIN_VOLTAGES_LOOKUP["STATION_ADC_VOLTAGE"], "STATION_ADC_VOLTAGE");
			break;
		case Parameter.ReadMSMGPIOControl:
			result = ValidateStates(resultHexString, AMPS_GPIO_STATES_LOOKUP["MSM_GPIO_VALUE_STATE"], "MSM_GPIO_VALUE_STATE");
			break;
		case Parameter.VerifyANX7816_7805Init:
			result = ValidateStates(resultHexString, AMPS_GPIO_STATES_LOOKUP["ANX7816_7805INIT_STATE"], "ANX7816_7805INIT_STATE");
			break;
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
