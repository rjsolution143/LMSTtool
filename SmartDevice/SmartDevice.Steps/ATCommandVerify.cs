using ISmart;

namespace SmartDevice.Steps;

public class ATCommandVerify : TestATCommandSend
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		string response;
		Result val = SendWitAtCommand(null, null, out response);
		if ((int)val != 8)
		{
			LogResult(val, "AT Command send failed");
			return;
		}
		if (((dynamic)base.Info.Args).Expected != null)
		{
			string text = ((dynamic)base.Info.Args).Expected;
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = base.Cache[key];
			}
			if (response.IndexOf(text) < 0)
			{
				LogResult((Result)1, "Output does not match expected", $"Expected {text}, received {response}");
				return;
			}
			Smart.Log.Debug(TAG, $"Passed: Expected value {text} matched");
			SetPreCondition(text);
		}
		else if (((dynamic)base.Info.Args).NotExpected != null)
		{
			string text = ((dynamic)base.Info.Args).NotExpected;
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = base.Cache[key];
			}
			if (response.IndexOf(text) >= 0)
			{
				Smart.Log.Debug(TAG, $"Failed: NotExpected value {text} matched");
				LogResult((Result)1, "Found unexpected return value", $"NotExpected: {text}");
				return;
			}
		}
		LogPass();
	}
}
