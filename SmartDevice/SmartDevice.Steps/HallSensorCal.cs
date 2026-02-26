using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class HallSensorCal : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c01: Unknown result type (might be due to invalid IL or missing references)
		string text = ((dynamic)base.Info.Args).TcmdReadOpCode;
		string text2 = ((dynamic)base.Info.Args).TcmdReadData;
		string text3 = ((dynamic)base.Info.Args).TcmdWriteOpCode;
		string text4 = ((dynamic)base.Info.Args).TcmdWriteData;
		ITestCommandClient val = (ITestCommandClient)base.Cache["tcmd"];
		string text5 = ((dynamic)base.Info.Args).ReadingCommand;
		double num = 20.0;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
		}
		int num2 = 100;
		if (((dynamic)base.Info.Args).WaitMilliseconds != null)
		{
			num2 = ((dynamic)base.Info.Args).WaitMilliseconds;
		}
		if (((dynamic)base.Info.Args).PromptOpenText != null)
		{
			string type = ((dynamic)base.Info.Args).PromptOpenType;
			string text6 = ((dynamic)base.Info.Args).PromptOpenText;
			text6 = Smart.Locale.Xlate(text6);
			Prompt(type, text6);
		}
		if (!(bool)VerifyFlip(text5, ((dynamic)base.Info.Args).LimitsOpen, num, num2))
		{
			LogResult((Result)1, "Flip not detected as Open");
			return;
		}
		byte[] data = val.SendCommand(text, text2).Data;
		int num3 = Smart.Convert.BytesToInt(data);
		if (((dynamic)base.Info.Args).PromptCloseText != null)
		{
			string type2 = ((dynamic)base.Info.Args).PromptCloseType;
			string text7 = ((dynamic)base.Info.Args).PromptCloseText;
			text7 = Smart.Locale.Xlate(text7);
			Prompt(type2, text7);
		}
		if (!(bool)VerifyFlip(text5, ((dynamic)base.Info.Args).LimitsClose, num, num2))
		{
			LogResult((Result)1, "Flip not detected as Closed");
			return;
		}
		Smart.Thread.Wait(TimeSpan.FromSeconds(1.0));
		byte[] data2 = val.SendCommand(text, text2).Data;
		int num4 = Smart.Convert.BytesToInt(data2);
		Smart.Log.Debug(TAG, $"Hall open - '{num3}' and close - '{num4}'");
		double num5 = (double)num3 + (double)(num4 - num3) * 0.25;
		double num6 = (double)num3 + (double)(num4 - num3) * 0.75;
		string text8 = Smart.Convert.BytesToHex(Smart.Convert.IntToBytes((int)num5));
		text8 = text8.PadLeft(8, '0');
		string text9 = Smart.Convert.BytesToHex(Smart.Convert.IntToBytes((int)num6));
		text9 = text9.PadLeft(8, '0');
		Smart.Log.Debug(TAG, $"Hall open threshold - '{text8}' and close threshold - '{text9}'");
		val.SendCommand(text3, text4 + text8 + text9);
		if (((dynamic)base.Info.Args).TcmdVerifyOpCode != null)
		{
			string text10 = ((dynamic)base.Info.Args).TcmdVerifyOpCode;
			string text11 = ((dynamic)base.Info.Args).TcmdVerifyData;
			ITestCommandResponse obj = val.SendCommand(text10, text11);
			string text12 = obj.DataHex.Substring(0, 2);
			string text13 = obj.DataHex.Substring(2, 8);
			string text14 = obj.DataHex.Substring(10, 8);
			Smart.Log.Debug(TAG, $"Verify check byte '{text12}' (should be 02), hall open threshold read from device - '{text13}' and close threshold read from device - '{text14}'");
			if (text12 != "02" || text13.ToLowerInvariant() != text8.ToLowerInvariant() || text14.ToLowerInvariant() != text9.ToLowerInvariant())
			{
				LogResult((Result)1, "Could not verify Hall Sensor data was programmed");
				return;
			}
		}
		LogPass();
	}

	private bool VerifyFlip(string readingCommand, dynamic limits, double timeout, int waitMilliseconds)
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Invalid comparison between Unknown and I4
		DateTime now = DateTime.Now;
		while (DateTime.Now.Subtract(now).TotalSeconds < timeout)
		{
			SortedList<string, string> sortedList = Tell(readingCommand);
			if ((int)(Result)LimitCheck(sortedList, limits) == 8)
			{
				return true;
			}
			Smart.Thread.Wait(TimeSpan.FromMilliseconds(waitMilliseconds));
		}
		return false;
	}
}
