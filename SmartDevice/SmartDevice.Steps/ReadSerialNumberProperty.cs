using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class ReadSerialNumberProperty : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		SortedList<string, string> sortedList = base.Cache["props"];
		string text = ((dynamic)base.Info.Args).IMEI;
		string value = string.Empty;
		if (text != null)
		{
			sortedList.TryGetValue(text, out value);
		}
		Smart.Log.Info(TAG, $"IMEI: {value}");
		val.SerialNumber = value;
		if ((bool?)((dynamic)base.Recipe.Info.Args).Options.CopySN == true)
		{
			Smart.File.ClipboardWrite(value);
		}
		base.Log.AddInfo("IMEI", value);
		string text2 = ((dynamic)base.Info.Args).IMEI2;
		string value2 = string.Empty;
		if (text2 != null)
		{
			sortedList.TryGetValue(text2, out value2);
		}
		Smart.Log.Info(TAG, $"IMEI2: {value2}");
		if (!string.IsNullOrEmpty(value2))
		{
			base.Log.AddInfo("IMEI2", value2);
			val.SerialNumber2 = value2;
		}
		LogPass();
	}
}
