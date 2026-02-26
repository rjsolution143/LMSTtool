using System;
using ISmart;

namespace SmartDevice.Steps;

public class ReadSerialNumberFtm : FtmStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		try
		{
			IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
			string text = ((dynamic)base.Info.Args).Data;
			string text2 = ((dynamic)base.Info.Args).SnType;
			char[] array = base.ftm.SendCommand(text).ToHex().Substring(0, 18)
				.ToCharArray();
			char[] array2 = new char[15];
			for (int i = 2; i <= array.Length - 2; i += 2)
			{
				if (i == 2)
				{
					array2[i - 2] = array[i];
					continue;
				}
				array2[i - 3] = array[i + 1];
				array2[i - 2] = array[i];
			}
			string text3 = new string(array2);
			string text4 = Smart.Convert.CalculateCheckDigit(text3);
			Smart.Log.Info(TAG, $"{text2}: {text4}");
			val.Communicating = true;
			val.LastConnected = DateTime.Now;
			if (((dynamic)base.Info.Args).Verify != null && (bool)((dynamic)base.Info.Args).Verify)
			{
				bool flag = false;
				if (!(text2.EndsWith("2") ? (val.SerialNumber2 == text4) : (val.SerialNumber == text4)))
				{
					Smart.Log.Error(TAG, $"Read serial number {text4} does not match original SN");
					LogResult((Result)1, "SN mismatch");
					return;
				}
			}
			else
			{
				base.Log.AddInfo(text2, text4);
				if (!text2.EndsWith("2"))
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
			Smart.Log.Error(TAG, $"Error during SN read FTM: {ex.Message}");
			Smart.Log.Verbose(TAG, ex.ToString());
			LogResult((Result)4, $"Error during SN read FTM: {ex.Message}");
			return;
		}
		LogPass();
	}
}
