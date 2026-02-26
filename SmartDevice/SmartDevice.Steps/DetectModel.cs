using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class DetectModel : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		DetectionKey val2 = new DetectionKey((SortedList<string, string>)(object)base.Cache["props"], string.Empty, true, val.Group);
		string text = Smart.Rsd.DetectModel(val2, val);
		string[] array = text.Split(new char[1] { '|' });
		string text2 = "Unknown";
		if (text == string.Empty)
		{
			Smart.Log.Error(TAG, "Fail to detect modelId");
			array[0] = "UNKNOWN";
			text2 = "Unknown";
			LogResult((Result)1, "Could not detect device modelId");
		}
		else
		{
			Smart.Log.Info(TAG, $"Detected modelId: {text}");
			DeviceAttributes deviceLatestAttributes = Smart.Rsd.GetDeviceLatestAttributes(text);
			text2 = ((val2.GetValue("fingerPrint") == deviceLatestAttributes.FingerPrint) ? "Yes" : "No");
			val.Log.AddInfo("Detection", "ADB");
			LogPass();
		}
		base.Log.AddInfo("FirmwareLatest", text2);
		base.Log.AddInfo("Model", array[0]);
		val.ModelId = text;
	}
}
