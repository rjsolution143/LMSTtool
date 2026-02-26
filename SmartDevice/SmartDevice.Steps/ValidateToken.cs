using System;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class ValidateToken : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Invalid comparison between Unknown and I4
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Invalid comparison between Unknown and I4
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Invalid comparison between Unknown and I4
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		bool flag = false;
		string description = string.Empty;
		string text = string.Empty;
		try
		{
			flag = Smart.Web.ValidateToken();
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, ex.Message);
			flag = false;
			result = (Result)4;
			text = ex.Message;
		}
		if (flag)
		{
			TokenInfo val = Smart.Web.TokenInfo();
			Smart.Log.Debug(TAG, ((object)(TokenInfo)(ref val)).ToString());
			base.Log.AddInfo("HardwareTokenIP", ((TokenInfo)(ref val)).HwDongleIp);
		}
		else
		{
			description = "eToken is missing or invalid";
			if (text == string.Empty)
			{
				TokenStatus tokenStatus = Smart.Web.TokenStatus;
				text = ((object)(TokenStatus)(ref tokenStatus)).ToString();
			}
			if ((int)Smart.Web.TokenStatus == 4)
			{
				string text2 = Smart.Locale.Xlate("Corrupt Hardware Token");
				string text3 = Smart.Locale.Xlate("The eToken connected to the pc is no longer usable. Please replace with a new eToken before re-testing.");
				Smart.User.MessageBox(text2, text3, (MessageBoxButtons)0, (MessageBoxIcon)48);
				description = "eToken is damaged or corrupt";
				if (((dynamic)base.Info.Args).RetryLoops != null && ((dynamic)base.Info.Args).RetryLoops > 0)
				{
					((dynamic)base.Info.Args).RetryLoops = 0;
				}
			}
			if ((int)result == 8)
			{
				result = (Result)1;
			}
		}
		VerifyOnly(ref result);
		if ((int)result == 8)
		{
			LogPass();
		}
		else
		{
			LogResult(result, description, text);
		}
	}
}
