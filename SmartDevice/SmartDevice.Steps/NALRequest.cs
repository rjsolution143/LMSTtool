using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class NALRequest : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		string empty = string.Empty;
		empty = ((IDevice)((dynamic)base.Recipe.Info.Args).Device).SerialNumber;
		try
		{
			Smart.Log.Info(TAG, $"Requesting NAL info for {empty}");
			SortedList<string, string> miiCertNo = Smart.Web.GetMiiCertNo(empty);
			Smart.Log.Info(TAG, string.Format("Requested NAL is {0} {1}", miiCertNo["miiModel"], miiCertNo["scramblingCode"]));
			base.Cache["nal"] = miiCertNo["miiModel"] + miiCertNo["scramblingCode"];
			LogPass();
		}
		catch (Exception ex)
		{
			Smart.Log.Debug(TAG, ex.Message + ex.StackTrace);
			LogResult((Result)1, "Error occurs while request NAL value");
		}
	}
}
