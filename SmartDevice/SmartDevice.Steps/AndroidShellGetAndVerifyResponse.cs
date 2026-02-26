using System;
using System.Linq;
using System.Text.RegularExpressions;
using ISmart;

namespace SmartDevice.Steps;

public class AndroidShellGetAndVerifyResponse : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_084c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1062: Unknown result type (might be due to invalid IL or missing references)
		//IL_1279: Unknown result type (might be due to invalid IL or missing references)
		//IL_1761: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).Command;
		_ = string.Empty;
		string empty = string.Empty;
		string[] array = new string[0];
		try
		{
			empty = Smart.ADB.Shell(val.ID, text, 10000).Trim();
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error sending ADB shell command: " + ex.Message);
			Smart.Log.Error(TAG, ex.ToString());
			LogResult((Result)4, "Could not send ADB shell command", ex.Message);
			return;
		}
		string text2 = text;
		string text3 = string.Empty;
		if (((dynamic)base.Info.Args).InputName != null && ((dynamic)base.Info.Args).InputName != string.Empty)
		{
			text2 = ((dynamic)base.Info.Args).InputName.ToString();
		}
		if (((dynamic)base.Info.Args).InputName1 != null && ((dynamic)base.Info.Args).InputName1 != string.Empty)
		{
			text3 = ((dynamic)base.Info.Args).InputName1.ToString();
		}
		Smart.Log.Debug(TAG, "response:" + empty);
		if (empty.Contains("\r\n"))
		{
			array = empty.Trim().Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		}
		Result result = (Result)8;
		if (((dynamic)base.Info.Args).CheckResponseStartWith != null)
		{
			string empty2 = string.Empty;
			empty2 = ((dynamic)base.Info.Args).CheckResponseStartWith;
			if (empty2.StartsWith("$"))
			{
				string key = empty2.Substring(1);
				empty2 = base.Cache[key];
			}
			Smart.Log.Debug(TAG, "Check Response StartWith:" + empty2);
			if (!empty.StartsWith(empty2))
			{
				result = (Result)1;
			}
		}
		if (((dynamic)base.Info.Args).CheckResponseEndWith != null)
		{
			string empty3 = string.Empty;
			empty3 = ((dynamic)base.Info.Args).CheckResponseEndWith;
			if (empty3.StartsWith("$"))
			{
				string key2 = empty3.Substring(1);
				empty3 = base.Cache[key2];
			}
			Smart.Log.Debug(TAG, "Check Response EndWith:" + empty3);
			if (!empty.EndsWith(empty3))
			{
				result = (Result)1;
			}
		}
		if (((dynamic)base.Info.Args).CheckResponseContain != null)
		{
			string empty4 = string.Empty;
			empty4 = ((dynamic)base.Info.Args).CheckResponseContain;
			if (empty4.StartsWith("$"))
			{
				string key3 = empty4.Substring(1);
				empty4 = base.Cache[key3];
			}
			Smart.Log.Debug(TAG, "Check Response Contains:" + empty4);
			if (!empty.Contains(empty4))
			{
				result = (Result)1;
			}
		}
		if (((dynamic)base.Info.Args).CheckResponseRegMatch != null)
		{
			string empty5 = string.Empty;
			empty5 = ((dynamic)base.Info.Args).CheckResponseRegMatch;
			Smart.Log.Debug(TAG, "Check Response match with regular expression:" + empty5);
			if (!Regex.IsMatch(empty, empty5))
			{
				result = (Result)1;
			}
		}
		if (((dynamic)base.Info.Args).ResponseLength != null)
		{
			int num = Convert.ToInt32(((dynamic)base.Info.Args).ResponseLength);
			Smart.Log.Debug(TAG, "Check Response length match:" + num);
			if (empty.Length != num)
			{
				result = (Result)1;
			}
		}
		if (((dynamic)base.Info.Args).TrimString != null)
		{
			string empty6 = string.Empty;
			empty6 = ((dynamic)base.Info.Args).TrimString;
			if (empty6.StartsWith("$"))
			{
				string key4 = empty6.Substring(1);
				empty6 = base.Cache[key4];
			}
			empty = Regex.Replace(empty, empty6, "");
		}
		empty = empty.Trim();
		Smart.Log.Info(TAG, "Final response:" + empty);
		if (array.Count() == 2)
		{
			base.Cache[text2] = array[0];
			base.Cache[text3] = array[1];
		}
		else
		{
			base.Cache[text2] = empty;
		}
		if (((dynamic)base.Info.Args).SaveResponseToLogInfo != null && (bool)((dynamic)base.Info.Args).SaveResponseToLogInfo)
		{
			if (array.Count() == 2)
			{
				base.Log.AddInfo(text2, array[0]);
				base.Log.AddInfo(text3, array[1]);
			}
			else
			{
				base.Log.AddInfo(text2, empty);
			}
		}
		if (array.Count() == 2)
		{
			SetPreCondition(array[0]);
			SetPreCondition(array[1]);
		}
		SetPreCondition(empty);
		LogResult(result);
	}
}
