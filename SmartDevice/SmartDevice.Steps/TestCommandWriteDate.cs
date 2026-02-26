using System;
using ISmart;

namespace SmartDevice.Steps;

public class TestCommandWriteDate : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Invalid comparison between Unknown and I4
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
		string text = ((dynamic)base.Info.Args).OpCode;
		string text2 = ((dynamic)base.Info.Args).Data;
		DateTime dateTime = DateTime.UtcNow;
		if (((dynamic)base.Info.Args).Date != null)
		{
			string text3 = ((dynamic)base.Info.Args).Date;
			if (text3.StartsWith("$"))
			{
				string key = text3.Substring(1);
				text3 = base.Cache[key];
			}
			dateTime = DateTime.Parse(text3);
		}
		bool flag = false;
		if (((dynamic)base.Info.Args).UseSeconds != null)
		{
			flag = ((dynamic)base.Info.Args).UseSeconds;
		}
		DateTime value = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		TimeSpan timeSpan = dateTime.Subtract(value);
		string empty = string.Empty;
		if (flag)
		{
			int num = (int)timeSpan.TotalSeconds;
			byte[] array = Smart.Convert.IntToBytes(num);
			empty = Smart.Convert.BytesToHex(array);
			empty = empty.PadLeft(8, '0');
		}
		else
		{
			long num2 = (long)timeSpan.TotalMilliseconds;
			byte[] array2 = Smart.Convert.LongToBytes(num2);
			empty = Smart.Convert.BytesToHex(array2);
			empty = empty.PadLeft(16, '0');
		}
		text2 += empty;
		ITestCommandResponse val = base.tcmd.SendCommand(text, text2);
		Result val2 = (Result)(val.Failed ? 1 : 8);
		if ((int)val2 == 8)
		{
			val2 = VerifyPropertyValue(val.DataHex);
		}
		if (((dynamic)base.Info.Args).VerifyOpCode != null)
		{
			string text4 = ((dynamic)base.Info.Args).VerifyOpCode;
			string text5 = ((dynamic)base.Info.Args).VerifyData;
			string dataHex = base.tcmd.SendCommand(text4, text5).DataHex;
			if (empty.Trim().ToLowerInvariant() != dataHex.Trim().ToLowerInvariant())
			{
				LogResult((Result)1, "Could not verify programmed Date value", $"Programmed value '{dataHex}' does not match expected value '{empty}'");
				return;
			}
		}
		LogResult(val2);
	}
}
