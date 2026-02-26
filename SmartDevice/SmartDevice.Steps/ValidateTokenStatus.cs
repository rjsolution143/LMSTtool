using ISmart;

namespace SmartDevice.Steps;

public class ValidateTokenStatus : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		TokenInfo val = Smart.Web.TokenInfo();
		TokenStatus tokenStatus = Smart.Web.GetTokenStatus(((TokenInfo)(ref val)).HwDongleIp);
		if ((int)tokenStatus == 1)
		{
			Smart.Log.Debug(TAG, "Hardware token status is valid");
			LogPass();
		}
		else
		{
			LogResult((Result)1, $"Hardware token status in not valid: {tokenStatus}");
		}
	}
}
