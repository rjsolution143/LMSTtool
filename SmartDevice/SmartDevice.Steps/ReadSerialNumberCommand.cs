using System;
using ISmart;

namespace SmartDevice.Steps;

public class ReadSerialNumberCommand : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		string text = "UNKNOWN";
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		try
		{
			string text2 = ((dynamic)base.Info.Args).OpCode;
			string text3 = ((dynamic)base.Info.Args).Data;
			text = ((dynamic)base.Info.Args).SnType;
			string dataHex = base.tcmd.SendCommand(text2, text3).DataHex;
			string text4 = "UNKNOWN";
			switch (text.ToLowerInvariant())
			{
			case "imei":
				text4 = dataHex.Substring(6, 18);
				text4 = new string(new char[15]
				{
					text4[2],
					text4[5],
					text4[4],
					text4[7],
					text4[6],
					text4[9],
					text4[8],
					text4[11],
					text4[10],
					text4[13],
					text4[12],
					text4[15],
					text4[14],
					text4[17],
					text4[16]
				});
				break;
			case "imei2":
				text4 = dataHex.Substring(16, 28);
				text4 = new string(new char[15]
				{
					text4[2],
					text4[5],
					text4[4],
					text4[7],
					text4[6],
					text4[9],
					text4[8],
					text4[11],
					text4[10],
					text4[13],
					text4[12],
					text4[15],
					text4[14],
					text4[17],
					text4[16]
				});
				break;
			case "meid":
			case "meid2":
				text4 = dataHex.Substring(6, 14);
				text4 = new string(new char[14]
				{
					text4[12],
					text4[13],
					text4[10],
					text4[11],
					text4[8],
					text4[9],
					text4[6],
					text4[7],
					text4[4],
					text4[5],
					text4[2],
					text4[3],
					text4[0],
					text4[1]
				});
				break;
			default:
				throw new NotSupportedException(string.Format("SN Type not supported: ", text));
			}
			text4 = Smart.Convert.CalculateCheckDigit(text4);
			Smart.Log.Info(TAG, $"{text}: {text4}");
			val.Communicating = true;
			val.LastConnected = DateTime.Now;
			if (((dynamic)base.Info.Args).Verify != null && (bool)((dynamic)base.Info.Args).Verify)
			{
				bool flag = false;
				if (!(text.EndsWith("2") ? (val.SerialNumber2 == text4) : (val.SerialNumber == text4)))
				{
					Smart.Log.Error(TAG, $"Read serial number {text4} does not match original SN");
					LogResult((Result)1, "SN mismatch");
					return;
				}
			}
			else
			{
				base.Log.AddInfo(text, text4);
				if (!text.EndsWith("2"))
				{
					val.SerialNumber = text4;
					if ((bool?)((dynamic)base.Recipe.Info.Args).Options.CopySN == true)
					{
						Smart.File.ClipboardWrite(text4);
					}
				}
				else
				{
					val.SerialNumber2 = text4;
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
