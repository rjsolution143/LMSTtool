using ISmart;

namespace SmartDevice.Steps;

public class ReadTracfoneRSUKeyFTM : FtmStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		IFtmResponse val = base.ftm.SendCommand("4B001B00000000", false, false);
		string text = Smart.Convert.BytesToHex(val.Raw);
		if (text.Substring(10, 2).Equals("00") && text.Substring(12, 4).Equals("2000"))
		{
			Smart.Log.Debug(TAG, "Save TFSecret Key to Cache[\"rsuSecretKey\"]");
			base.Cache["rsuSecretKey"] = text.Substring(16, 64);
			LogResult(result);
		}
		else
		{
			Smart.Log.Debug(TAG, "Get RSU Key failed");
			LogResult((Result)1, "Get RSU Key failed");
		}
	}
}
