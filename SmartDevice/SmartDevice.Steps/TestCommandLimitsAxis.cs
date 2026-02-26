using System;
using System.Collections.Generic;
using System.IO;
using ISmart;

namespace SmartDevice.Steps;

public class TestCommandLimitsAxis : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_087f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0836: Unknown result type (might be due to invalid IL or missing references)
		string text = ((dynamic)base.Info.Args).OpCode;
		string text2 = ((dynamic)base.Info.Args).Data;
		int num = 0;
		if (((dynamic)base.Info.Args).IgnoreBytes != null)
		{
			num = ((dynamic)base.Info.Args).IgnoreBytes;
		}
		bool flag = false;
		if (((dynamic)base.Info.Args).TwosComplement != null)
		{
			flag = ((dynamic)base.Info.Args).TwosComplement;
		}
		bool flag2 = false;
		if (((dynamic)base.Info.Args).Ascii != null)
		{
			flag2 = ((dynamic)base.Info.Args).Ascii;
		}
		List<string> obj = new List<string> { "X", "Y", "Z" };
		ITestCommandResponse obj2 = base.tcmd.SendCommand(text, text2);
		if (obj2.Failed)
		{
			throw new IOException($"TCMD Failed: [{text}] {text2}");
		}
		byte[] array = obj2.Data;
		if (num > 0)
		{
			byte[] array2 = new byte[array.Length - num];
			Array.Copy(array, 0, array2, 0, array.Length - num);
			array = array2;
		}
		int num2 = array.Length / 3;
		int num3 = 0;
		Result result = (Result)8;
		string description = string.Empty;
		string dynamicError = string.Empty;
		foreach (string item in obj)
		{
			byte[] array3 = new byte[num2];
			Array.Copy(array, num3, array3, 0, num2);
			num3 += num2;
			long num4 = 0L;
			num4 = ((!flag2) ? Smart.Convert.BytesToLong(array3) : long.Parse(Smart.Convert.BytesToAscii(array3)));
			if (flag)
			{
				num4 = Smart.Convert.TwosComplement((int)num4);
			}
			double num5 = ((dynamic)base.Info.Args)["Max" + item];
			double num6 = ((dynamic)base.Info.Args)["Min" + item];
			if (!(num6 <= (double)num4) || !((double)num4 <= num5))
			{
				result = (Result)1;
				description = "Value outside of limits";
				dynamicError = $"Value {num4} outside of limits '{num6}'-'{num5}'";
				break;
			}
		}
		LogResult(result, description, dynamicError);
	}
}
