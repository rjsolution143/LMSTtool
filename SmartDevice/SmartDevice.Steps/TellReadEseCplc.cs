using System;
using ISmart;

namespace SmartDevice.Steps;

public class TellReadEseCplc : CommServerStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Result result = (Result)8;
		try
		{
			string command = "GET_CPLC";
			if (((dynamic)base.Info.Args).Command != null)
			{
				command = ((dynamic)base.Info.Args).Command;
			}
			string text = Tell(command)["CPLC"];
			val.Log.AddInfo("eSECPLC", text);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, string.Format("Exception errMsg: " + ex.Message));
			Smart.Log.Error(TAG, ex.StackTrace);
			result = (Result)4;
		}
		LogResult(result);
	}
}
