using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class LoadSavedIMEI : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		_ = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (!Smart.Session.IsSaved((SessionType)1))
		{
			throw new NotSupportedException("No saved IMEI info to load!");
		}
		SortedList<string, string> sortedList = Smart.Session.Load((SessionType)1);
		string text = sortedList["SerialNumber"];
		string text2 = sortedList["SerialNumberDual"];
		string arg = sortedList["ModelId"];
		Smart.Log.Debug(TAG, $"Loading Saved SN: {text}");
		Smart.Log.Debug(TAG, $"Loading Saved SN Dual: {text2}");
		Smart.Log.Debug(TAG, $"Loading Saved SN Dual: {arg}");
		if (string.IsNullOrEmpty(text) || text == "UNKNOWN" || text == "000000000000000")
		{
			throw new NotSupportedException("Loaded serial number is invalid");
		}
		base.Cache["SavedSerialNumber"] = text;
		if (!string.IsNullOrEmpty(text2) && text2 != "UNKNOWN" && text2 != "000000000000000")
		{
			base.Cache["SavedSerialNumberDual"] = text;
		}
		LogPass();
	}
}
