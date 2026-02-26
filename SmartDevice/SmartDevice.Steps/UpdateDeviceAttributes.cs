using ISmart;

namespace SmartDevice.Steps;

public class UpdateDeviceAttributes : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		DeviceAttributes deviceLatestAttributes = Smart.Rsd.GetDeviceLatestAttributes(val.ModelId);
		if (!string.IsNullOrEmpty(deviceLatestAttributes.BlurId))
		{
			base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup["blur"], deviceLatestAttributes.BlurId);
		}
		if (!string.IsNullOrEmpty(deviceLatestAttributes.FingerPrint))
		{
			base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup["fingerPrint"], deviceLatestAttributes.FingerPrint);
		}
		if (!string.IsNullOrEmpty(deviceLatestAttributes.RoCarrier))
		{
			val.RoCarrier = deviceLatestAttributes.RoCarrier;
			base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup["roCarrier"], deviceLatestAttributes.RoCarrier);
		}
		if (!string.IsNullOrEmpty(deviceLatestAttributes.FsgVersion))
		{
			base.Log.AddInfo(DetectionKey.NameToDisplayStringLookup["fsgVersion"], deviceLatestAttributes.FsgVersion);
		}
		string flashId = deviceLatestAttributes.FlashId;
		if (string.IsNullOrEmpty(flashId))
		{
			flashId = GetFlashId(deviceLatestAttributes.FingerPrint);
		}
		if (!string.IsNullOrEmpty(flashId))
		{
			base.Log.AddInfo("FlashId", flashId);
		}
		if (!string.IsNullOrEmpty(deviceLatestAttributes.FlexId))
		{
			base.Log.AddInfo("FlexId", deviceLatestAttributes.FlexId);
		}
		if (!string.IsNullOrEmpty(deviceLatestAttributes.CarrierSku))
		{
			base.Log.AddInfo("SKU", deviceLatestAttributes.CarrierSku);
		}
		LogPass();
	}

	private string GetFlashId(string fingerPrint)
	{
		string result = string.Empty;
		string[] array = fingerPrint.Split(new char[1] { '/' });
		if (array.Length > 3)
		{
			result = array[3].Trim();
		}
		return result;
	}
}
