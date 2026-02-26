using ISmart;

namespace SmartDevice.Steps;

public class CheckFirmwareLatest : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Invalid comparison between Unknown and I4
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string serialNumber = val.SerialNumber;
		string text = string.Empty;
		if (!val.ManualDevice)
		{
			text = val.GetLogInfoValue(DetectionKey.NameToDisplayStringLookup["fingerPrint"]);
		}
		string latestSoftwareHistory = Smart.Rsd.GetLatestSoftwareHistory(serialNumber);
		dynamic val2 = Smart.Json.Load(latestSoftwareHistory);
		string text2 = val2["status"];
		if (text2.Trim().ToLowerInvariant() != "success")
		{
			string arg = val2["message"];
			string text3 = $"{text2} - {arg}";
			Smart.Log.Error(TAG, "Error during downloading latest software history: " + text3);
			LogResult((Result)2, text3);
			return;
		}
		string text4 = val2["serialno"];
		if (text4.Trim() != serialNumber)
		{
			Smart.Log.Warning(TAG, $"Response SN {text4} does not match input SN {serialNumber}");
		}
		if (text == string.Empty)
		{
			text = val2["fingerprint"];
		}
		Smart.Log.Debug(TAG, "Device fingerprint: " + text);
		if (string.IsNullOrEmpty(text))
		{
			string text5 = "Device fingerprint is empty";
			Smart.Log.Error(TAG, text5);
			LogResult((Result)2, text5);
			return;
		}
		string text6 = val2["releasedfingerprint"];
		Smart.Log.Debug(TAG, "Release fingerprint: " + text6);
		string text7 = ((text == text6) ? "Yes" : "No");
		Smart.Log.Debug(TAG, "firmwareStatus: " + text7);
		base.Log.AddInfo("FirmwareLatest", text7);
		SetPreCondition(text7);
		if ((int)VerifyPropertyValue(text7, logOnFailed: true, "FirmwareLatest") == 8)
		{
			LogPass();
		}
	}
}
