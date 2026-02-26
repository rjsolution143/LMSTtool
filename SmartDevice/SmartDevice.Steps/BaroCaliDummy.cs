using System;
using ISmart;

namespace SmartDevice.Steps;

public class BaroCaliDummy : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		//IL_0943: Unknown result type (might be due to invalid IL or missing references)
		double num = 0.0;
		string empty = string.Empty;
		if (((dynamic)base.Info.Args).Max != null && ((dynamic)base.Info.Args).Min != null)
		{
			double num2 = ((dynamic)base.Info.Args).Max;
			double num3 = ((dynamic)base.Info.Args).Min;
			try
			{
				string opCode = "0098";
				string data = "01";
				empty = TestCommand(opCode, data).Trim();
				Smart.Log.Debug(TAG, "Baro reading response from phone:" + empty);
				string text = "";
				int num5;
				int num6;
				if (empty.Substring(0, 2) == "00")
				{
					Smart.Log.Info(TAG, "Response indicate tcmd pass.");
					text = empty.Substring(2);
					Smart.Log.Debug(TAG, "Pressure read from phone in Hex:" + text);
					int num4 = Convert.ToInt32(text, 16);
					Smart.Log.Debug(TAG, $"Pressure read from phone is:{num4.ToString()}");
					if (num4.ToString().Length > 6)
					{
						num4 = Convert.ToInt32(text, 16) / 100;
					}
					Smart.Log.Debug(TAG, $"Formatted Pressure from phone is:{num4.ToString()}Pa = {num4 / 100}hPa");
					opCode = "0098";
					data = "02" + Convert.ToString(num4 * 100, 16).ToUpper().PadLeft(8, '0');
					empty = TestCommand(opCode, data);
					Smart.Log.Debug(TAG, "Baro writing response from phone:" + empty);
					num = (double)Convert.ToInt32(empty, 16) / 10000.0;
					Smart.Log.Debug(TAG, $"Barometer dummy calibrated offset is {num}(hPa).");
					if (num > num2 && ((dynamic)base.Info.Args).HighMax != null && ((dynamic)base.Info.Args).HighMin != null)
					{
						Smart.Log.Debug(TAG, "Using higher limits for lmiits check");
						num2 = ((dynamic)base.Info.Args).HighMax;
						num3 = ((dynamic)base.Info.Args).HighMin;
					}
					if (num3 <= num)
					{
						num5 = ((num <= num2) ? 1 : 0);
						if (num5 != 0)
						{
							num6 = 8;
							goto IL_0930;
						}
					}
					else
					{
						num5 = 0;
					}
					num6 = 1;
					goto IL_0930;
				}
				string text2 = "Response indicate tcmd fail.";
				Smart.Log.Info(TAG, text2);
				throw new Exception(text2);
				IL_0930:
				Result result = (Result)num6;
				string description = ((num5 != 0) ? "Value within limits" : "Value outside of limits");
				LogResult(result, description, num2, num3, num);
				return;
			}
			catch (Exception ex)
			{
				Smart.Log.Debug(TAG, ex.Message + ex.StackTrace);
				LogResult((Result)1, "Error occurs while reading/writing baro value");
				return;
			}
		}
		throw new NotSupportedException("Max/Min limits are required");
	}

	protected string TestCommand(string opCode, string data)
	{
		return base.tcmd.SendCommand(opCode, data).DataHex;
	}
}
