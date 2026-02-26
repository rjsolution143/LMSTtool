using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class CheckChannelIdInfo : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Result val2 = (Result)1;
		string serialNumber = val.SerialNumber;
		string iD = val.ID;
		string text = serialNumber;
		if (text == null || text == string.Empty || text == "UNKNOWN")
		{
			text = iD;
		}
		SortedList<string, string> sortedList = Smart.Web.ArgoInfo(text);
		string text2 = sortedList["Imei"];
		string text3 = sortedList["EnterpriseEdition"];
		string text4 = sortedList["ChannelId"];
		if (text4 == null || text4 == string.Empty || text3 == null || serialNumber == null)
		{
			val2 = (Result)1;
			VerifyOnly(ref val2);
			LogResult(val2, "Channel ID info not found");
			return;
		}
		val.Log.AddInfo("OtaChannelId", text4);
		val.Log.AddInfo("ChannelIdImei", text2);
		val.Log.AddInfo("EnterpriseEdition", text3);
		Result val3 = (Result)8;
		SetPreCondition(((object)(Result)(ref val3)).ToString());
		LogPass();
	}
}
