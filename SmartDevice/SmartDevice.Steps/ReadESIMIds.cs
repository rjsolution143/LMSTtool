using ISmart;

namespace SmartDevice.Steps;

public class ReadESIMIds : ESimStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		Result val = ReadESimId();
		Result val2 = ReadActiveIccid();
		if ((int)val != 8 || (int)val2 != 8)
		{
			result = (Result)((!ignoreErrors) ? 1 : 7);
		}
		SetPreCondition(((object)(Result)(ref result)).ToString());
		VerifyOnly(ref result);
		LogResult(result);
	}
}
