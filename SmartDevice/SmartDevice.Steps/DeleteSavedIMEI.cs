using ISmart;

namespace SmartDevice.Steps;

public class DeleteSavedIMEI : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		Smart.Session.Delete((SessionType)1);
		LogPass();
	}
}
