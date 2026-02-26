using System;
using System.Collections.Generic;
using System.Linq;
using ISmart;

namespace SmartDevice.Steps;

public class TestCommandPKIVerifyMulti : TestCommandPKIVerify
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		string text = ((dynamic)base.Info.Args).PKIKeys;
		if (text != null && text != string.Empty && text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		string text2 = ((dynamic)base.Info.Args).ProductFamily;
		if (text2 != null && text2 != string.Empty && text2.StartsWith("$"))
		{
			string key2 = text2.Substring(1);
			text2 = base.Cache[key2];
		}
		List<string> list = text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
		bool flag = true;
		foreach (string item in list)
		{
			string text3 = "Unknown Error during PKI Key verifying";
			Result val = (Result)0;
			try
			{
				bool num = VerifyPKI(item.Trim());
				text3 = "PKI Key verified successfully";
				val = (Result)8;
				if (!num)
				{
					text3 = "PKI Key Type verify failed";
					val = (Result)1;
					flag = false;
				}
			}
			catch (Exception ex)
			{
				text3 = "PKI Key verifying error: " + ex.Message;
				Smart.Log.Error(TAG, text3);
				Smart.Log.Verbose(TAG, ex.ToString());
				val = (Result)4;
				flag = false;
				break;
			}
			finally
			{
				if (!CheckRetest(val))
				{
					base.Log.AddResult(base.Name + " " + item, base.Info.Step, val, text3, "", "", double.MinValue, double.MinValue, double.MinValue, (SortedList<string, object>)null);
				}
			}
		}
		if (flag)
		{
			LogPass();
		}
	}
}
