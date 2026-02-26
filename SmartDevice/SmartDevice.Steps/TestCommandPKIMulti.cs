using System;
using System.Collections.Generic;
using System.Linq;
using ISmart;

namespace SmartDevice.Steps;

public class TestCommandPKIMulti : TestCommandPKI
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string rsdLogId = val.Log.RsdLogId;
		string originalImei = val.SerialNumber;
		if (val.Log.Info.ContainsKey("OriginalImei"))
		{
			originalImei = val.Log.Info["OriginalImei"];
		}
		int num = 4000;
		if (((dynamic)base.Info.Args).WaitMilliseconds != null)
		{
			num = ((dynamic)base.Info.Args).WaitMilliseconds;
		}
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
			bool flag2 = false;
			string key3 = $"{val.ID}-{item}";
			string text3 = "Unknown Error during PKI Key programming";
			Result val2 = (Result)0;
			try
			{
				if (base.Cache.ContainsKey(key3))
				{
					text3 = "PKI Key Type programmed previously";
					Smart.Log.Debug(TAG, $"Skipping programming of PKI Type {item} due to already programmed previously");
					string obj = base.Cache[key3];
					flag2 = true;
					if (obj != val.Log.RsdLogId)
					{
						Smart.Log.Error(TAG, "Previously programmed PKI Key was programmed in a previous run, not intended to carry over across runs");
					}
				}
				else
				{
					bool num2 = ProgramPKI(item.Trim(), text2, originalImei, rsdLogId);
					base.Cache[key3] = val.Log.RsdLogId;
					text3 = "PKI Key programmed successfully";
					if (!num2)
					{
						text3 = "PKI Key Type already programmed";
					}
				}
				val2 = (Result)8;
			}
			catch (Exception ex)
			{
				text3 = "PKI Key programming error: " + ex.Message;
				Smart.Log.Error(TAG, text3);
				Smart.Log.Verbose(TAG, ex.ToString());
				val2 = (Result)4;
				flag = false;
				break;
			}
			finally
			{
				if (!flag2)
				{
					Smart.Thread.Wait(TimeSpan.FromMilliseconds(num));
				}
				if (!CheckRetest(val2) && !flag2)
				{
					base.Log.AddResult(base.Name + " " + item, base.Info.Step, val2, text3, "", "", double.MinValue, double.MinValue, double.MinValue, (SortedList<string, object>)null);
				}
				SetPreCondition(((object)(Result)(ref val2)).ToString());
			}
		}
		if (flag)
		{
			LogPass();
		}
	}
}
