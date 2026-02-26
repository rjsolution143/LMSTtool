using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class GPPDRequest : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (((dynamic)base.Info.Args).InputName != null && ((dynamic)base.Info.Args).InputName != string.Empty)
		{
			empty = ((dynamic)base.Info.Args).InputName.ToString();
			empty2 = base.Cache[empty];
		}
		else
		{
			empty = "DeviceSerialNumber";
			empty2 = ((IDevice)((dynamic)base.Recipe.Info.Args).Device).SerialNumber;
		}
		Smart.Log.Info(TAG, $"Requesting GPPD info for {empty} {empty2}");
		SortedList<string, string> gppdId = Smart.Web.GetGppdId(empty2);
		base.Cache[empty + "GppdId"] = gppdId["GppdId"];
		base.Cache[empty + "Customer"] = gppdId["Customer"];
		base.Cache[empty + "Protocol"] = gppdId["Protocol"];
		LogPass();
	}
}
