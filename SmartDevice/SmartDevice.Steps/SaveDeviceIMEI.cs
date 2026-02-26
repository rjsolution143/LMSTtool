using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class SaveDeviceIMEI : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		object obj = (object)(IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string modelId = ((IDevice)obj).ModelId;
		string text = ((IDevice)obj).Log.Info["PhysicalSnStart"];
		string text2 = ((IDevice)obj).Log.Info["PhysicalSnStart2"];
		Smart.Log.Debug(TAG, $"Reading Physical SN: {text}");
		Smart.Log.Debug(TAG, $"Reading Physical SN Dual: {text2}");
		if (string.IsNullOrEmpty(text) || text == "UNKNOWN" || text == "000000000000000")
		{
			throw new NotSupportedException("Device must have a valid serial number");
		}
		if (Smart.Session.IsSaved((SessionType)1))
		{
			Smart.Log.Debug(TAG, "SN Wipe Data already exists! Deleting to save current data");
			Smart.Session.Delete((SessionType)1);
		}
		SortedList<string, string> sortedList = new SortedList<string, string>();
		sortedList["SerialNumber"] = text;
		sortedList["SerialNumberDual"] = text2;
		sortedList["ModelId"] = modelId;
		Smart.Session.Save((SessionType)1, sortedList);
		LogPass();
	}
}
