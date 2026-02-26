using ISmart;

namespace SmartDevice.Steps;

public class CheckUserCredentials : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Login login = Smart.Rsd.Login;
		string userName = ((Login)(ref login)).UserName;
		if (userName == null || userName.Length < 1)
		{
			LogResult((Result)1, "No RSD username set");
			return;
		}
		Smart.Log.Debug(TAG, "Found RSD username " + userName);
		LogPass();
	}
}
