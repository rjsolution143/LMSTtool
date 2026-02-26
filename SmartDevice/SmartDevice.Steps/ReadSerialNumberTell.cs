using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class ReadSerialNumberTell : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string command = ((dynamic)base.Info.Args).TellCommand;
		string key = ((dynamic)base.Info.Args).Field;
		string text = ((dynamic)base.Info.Args).SnType;
		SortedList<string, string> sortedList = Tell(command);
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			int num = ((dynamic)base.Info.Args).Timeout;
			base.CommServer.Timeout = TimeSpan.FromSeconds(num);
		}
		string text2 = sortedList[key];
		text2 = Smart.Convert.CalculateCheckDigit(text2);
		Smart.Log.Info(TAG, $"{text}: {text2}");
		val.Communicating = true;
		val.LastConnected = DateTime.Now;
		base.Log.AddInfo(text, text2);
		if (!text.EndsWith("2"))
		{
			val.SerialNumber = text2;
			if ((bool?)((dynamic)base.Recipe.Info.Args).Options.CopySN == true)
			{
				Smart.File.ClipboardWrite(text2);
			}
		}
		else
		{
			val.SerialNumber2 = text2;
		}
		LogPass();
	}
}
