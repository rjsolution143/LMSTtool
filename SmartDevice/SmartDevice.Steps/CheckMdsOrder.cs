using System;
using ISmart;

namespace SmartDevice.Steps;

public class CheckMdsOrder : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Invalid comparison between Unknown and I4
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Result val2 = (Result)1;
		string serialNumber = val.SerialNumber;
		string iD = val.ID;
		string text = null;
		if (val.Log.Info.ContainsKey("OriginalImei"))
		{
			text = val.Log.Info["OriginalImei"];
		}
		string text2 = "MDS Order required but not found";
		bool flag = false;
		bool flag2 = false;
		try
		{
			flag = Smart.Web.CheckMdsOrder(serialNumber, iD, text);
		}
		catch (Exception ex)
		{
			text2 = "Error during CheckMdsOrder: " + ex.Message;
			Smart.Log.Error(TAG, text2);
			Smart.Log.Debug(TAG, ex.ToString());
			flag = false;
			flag2 = true;
		}
		if (!flag && !flag2 && text == null)
		{
			string text3 = null;
			while (text3 == null)
			{
				text3 = Smart.User.InputBox(Smart.Locale.Xlate("Input Serial Number"), Smart.Locale.Xlate("Scan Customer Serial Number"), (string)null);
				if (text3 == string.Empty || text3 == null)
				{
					break;
				}
				text3 = text3.ToUpperInvariant().Trim();
				if ((int)Smart.Convert.ToSerialNumberType(text3) == 1)
				{
					break;
				}
			}
			if (text3 == null || text3 == string.Empty)
			{
				val2 = (Result)1;
				VerifyOnly(ref val2);
				LogResult(val2, "User did not enter Customer IMEI for MDS Order Info");
				return;
			}
			Smart.Log.Debug(TAG, $"User entered Original IMEI: {text3}");
			val.Log.AddInfo("OriginalImei", text3);
			text = text3;
			try
			{
				flag = Smart.Web.CheckMdsOrder(serialNumber, iD, text);
			}
			catch (Exception ex2)
			{
				text2 = "Error during CheckMdsOrder: " + ex2.Message;
				Smart.Log.Error(TAG, text2);
				Smart.Log.Debug(TAG, ex2.ToString());
				flag = false;
				flag2 = true;
			}
		}
		if (!flag)
		{
			val2 = (Result)1;
			VerifyOnly(ref val2);
			LogResult(val2, text2);
		}
		else
		{
			Result val3 = (Result)8;
			SetPreCondition(((object)(Result)(ref val3)).ToString());
			LogPass();
		}
	}
}
