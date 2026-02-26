using System;
using ISmart;

namespace SmartDevice.Steps;

internal class ReadSerialNumberSamsung : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		try
		{
			IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
			string text = "0F01";
			if (((dynamic)base.Info.Args).OpCode != null)
			{
				text = ((dynamic)base.Info.Args).OpCode;
			}
			string text2 = ((dynamic)base.Info.Args).SnType;
			string text3 = ((text2.ToLowerInvariant() == "imei") ? "0A5341542B5244494D45493D3100" : "0A5341542B5244494D45493D3200");
			if (((dynamic)base.Info.Args).Data != null)
			{
				text3 = ((dynamic)base.Info.Args).Data;
			}
			Smart.Log.Debug(TAG, $"AT cmd - opCode: {text}, data: {text3}");
			ITestCommandResponse val2 = base.tcmd.SendCommand(text, text3);
			string text4 = Smart.Convert.BytesToAscii(val2.Data);
			Smart.Log.Debug(TAG, $"AT cmd response: {text4}");
			string text5 = "UNKNOWN";
			string text6 = text2.ToLowerInvariant();
			if (!(text6 == "imei"))
			{
				if (!(text6 == "imei2"))
				{
					throw new NotSupportedException(string.Format("SN Type not supported: ", text2));
				}
				if (text4.StartsWith("SAT+RDIMEI=2"))
				{
					int num = text4.IndexOf(':');
					if (num > 0)
					{
						text5 = text4.Substring(num + 1, 15);
					}
				}
			}
			else if (text4.StartsWith("SAT+RDIMEI=1"))
			{
				int num2 = text4.IndexOf(':');
				if (num2 > 0)
				{
					text5 = text4.Substring(num2 + 1, 15);
				}
			}
			Smart.Log.Info(TAG, $"{text2}: {text5}");
			val.Communicating = true;
			val.LastConnected = DateTime.Now;
			if (((dynamic)base.Info.Args).Verify != null && (bool)((dynamic)base.Info.Args).Verify)
			{
				bool flag = false;
				if (!(text2.EndsWith("2") ? (val.SerialNumber2 == text5) : (val.SerialNumber == text5)))
				{
					Smart.Log.Error(TAG, $"Read serial number {text5} does not match original SN");
					LogResult((Result)1, $"Read serial number {text5} does not match original SN");
					return;
				}
			}
			else
			{
				base.Log.AddInfo(text2, text5);
				if (!text2.EndsWith("2"))
				{
					val.SerialNumber = text5;
					if ((bool?)((dynamic)base.Recipe.Info.Args).Options.CopySN == true)
					{
						Smart.File.ClipboardWrite(text5);
					}
				}
				else
				{
					val.SerialNumber2 = text5;
				}
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, $"Error during SN read TCMD: {ex.Message}");
			Smart.Log.Verbose(TAG, ex.ToString());
			LogResult((Result)4, $"Error during SN read TCMD: {ex.Message}");
			return;
		}
		LogPass();
	}
}
