using System;
using System.IO;
using System.Linq;
using ISmart;

namespace SmartDevice.Steps;

public class TestCommandLimits : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0cd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cec: Unknown result type (might be due to invalid IL or missing references)
		string opCode = ((dynamic)base.Info.Args).OpCode;
		string data = ((dynamic)base.Info.Args).Data;
		int num = 1;
		if (((dynamic)base.Info.Args).Readings != null)
		{
			num = ((dynamic)base.Info.Args).Readings;
		}
		int ignoreBytes = 0;
		if (((dynamic)base.Info.Args).IgnoreBytes != null)
		{
			ignoreBytes = ((dynamic)base.Info.Args).IgnoreBytes;
		}
		bool twosComplement = false;
		if (((dynamic)base.Info.Args).TwosComplement != null)
		{
			twosComplement = ((dynamic)base.Info.Args).TwosComplement;
		}
		bool ascii = false;
		if (((dynamic)base.Info.Args).Ascii != null)
		{
			ascii = ((dynamic)base.Info.Args).Ascii;
		}
		long num2 = 0L;
		foreach (int item in Enumerable.Range(1, num))
		{
			_ = item;
			num2 += GetReading(opCode, data, twosComplement, ignoreBytes, ascii);
		}
		if (num > 1)
		{
			num2 /= num;
		}
		double num3 = ((dynamic)base.Info.Args).Max;
		double num4 = ((dynamic)base.Info.Args).Min;
		if ((double)num2 > num3 && ((dynamic)base.Info.Args).HighMax != null && ((dynamic)base.Info.Args).HighMin != null)
		{
			Smart.Log.Debug(TAG, "Using higher limits for lmiits check");
			num3 = ((dynamic)base.Info.Args).HighMax;
			num4 = ((dynamic)base.Info.Args).HighMin;
		}
		int num5;
		int num6;
		if (num4 <= (double)num2)
		{
			num5 = (((double)num2 <= num3) ? 1 : 0);
			if (num5 != 0)
			{
				num6 = 8;
				goto IL_0cd9;
			}
		}
		else
		{
			num5 = 0;
		}
		num6 = 1;
		goto IL_0cd9;
		IL_0cd9:
		Result result = (Result)num6;
		string description = ((num5 != 0) ? "Value within limits" : "Value outside of limits");
		LogResult(result, description, num3, num4, num2);
	}

	private long GetReading(string opCode, string data, bool twosComplement, int ignoreBytes, bool ascii)
	{
		ITestCommandResponse obj = base.tcmd.SendCommand(opCode, data);
		if (obj.Failed)
		{
			throw new IOException($"TCMD Failed: [{opCode}] {data}");
		}
		byte[] array = obj.Data;
		if (ignoreBytes > 0)
		{
			byte[] array2 = new byte[array.Length - ignoreBytes];
			Array.Copy(array, ignoreBytes, array2, 0, array.Length - ignoreBytes);
			array = array2;
		}
		long num = 0L;
		num = ((!ascii) ? Smart.Convert.BytesToLong(array) : long.Parse(Smart.Convert.BytesToAscii(array)));
		if (twosComplement)
		{
			num = Smart.Convert.TwosComplement((int)num);
		}
		return num;
	}
}
