using ISmart;

namespace SmartDevice.Steps;

public class UploadRPK : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		object obj = (object)(IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string serialNumber = ((IDevice)obj).SerialNumber;
		string iD = ((IDevice)obj).ID;
		string logInfoValue = ((IDevice)obj).GetLogInfoValue("googleCsr");
		string logInfoValue2 = ((IDevice)obj).GetLogInfoValue("googleCsr2");
		string logInfoValue3 = ((IDevice)obj).GetLogInfoValue("googleCsr3");
		Smart.Web.Rpk(serialNumber, logInfoValue, logInfoValue2, logInfoValue3, iD);
		LogPass();
	}
}
