using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class FindBluetooth : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		Tell("ENABLE_BLUETOOTH");
		try
		{
			Smart.Thread.Wait(TimeSpan.FromMilliseconds(500.0));
			SortedList<string, string> sortedList = Tell("GET_BLUETOOTH_MAC");
			if (!sortedList.ContainsKey("MAC_ADDRESS"))
			{
				throw new NotSupportedException("Could not read Bluetooth address");
			}
			sortedList["MAC_ADDRESS"].ToUpperInvariant();
			string text = ((dynamic)base.Info.Args).ClearCommand;
			if (text != null)
			{
				Tell(text);
			}
			Smart.Thread.Wait(TimeSpan.FromMilliseconds(100.0));
			Tell("START_BLUETOOTH_SCAN");
			double value = ((dynamic)base.Info.Args).Timeout;
			bool num = Smart.Thread.Wait(TimeSpan.FromSeconds(value), (Checker<bool>)FindReading);
			Result result = (Result)1;
			if (num)
			{
				result = (Result)8;
			}
			Tell("STOP_BLUETOOTH_SCAN");
			if (text != null)
			{
				Tell(text);
			}
			LogResult(result);
		}
		finally
		{
			Tell("DISABLE_BLUETOOTH");
		}
	}

	private bool FindReading()
	{
		string command = ((dynamic)base.Info.Args).ReadingCommand;
		return Tell(command).Count > 0;
	}
}
