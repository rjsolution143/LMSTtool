using System;
using ISmart;

namespace SmartDevice.Steps;

public class ConfigurationCheck : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		if (!Smart.Security.IntegrityCheck())
		{
			Smart.Log.Error(TAG, "CRITICAL ERROR, INTEGRITY CHECK FAILED, PLEASE CONTACT SUPPORT FOR INSTRUCTIONS");
			LogResult((Result)1, "Local PC configuration is incompatible with LMST please contact support");
			return;
		}
		if (!Smart.Security.HostCheck())
		{
			base.Log.AddInfo("HostCheck", "BadHost");
			Smart.Log.Error(TAG, "HOST CHECK FAILED");
			LogResult((Result)7, "Host Check Failed", "Host Check Failed");
			return;
		}
		bool flag = false;
		try
		{
			flag = Smart.Security.RemoteCheck();
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Remote Check failed");
			Smart.Log.Verbose(TAG, ex.ToString());
		}
		if (flag)
		{
			base.Log.AddInfo("HostCheck", "RemoteDevice");
			Smart.Log.Error(TAG, "REMOTE CHECK FAILED");
			LogResult((Result)7, "Remote Check Failed", "Remote Check Failed");
		}
		else
		{
			LogPass();
		}
	}
}
