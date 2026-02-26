using System;
using ISmart;

namespace SmartDevice.Steps;

public class AndroidShellVerify : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).Command;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = string.Empty;
		if (text.ToLower() == "androidversion")
		{
			text4 = "androidversion";
			text = "getprop ro.build.fingerprint";
		}
		if (((dynamic)base.Info.Args).Ref != null)
		{
			text2 = ((dynamic)base.Info.Args).Ref;
		}
		if (((dynamic)base.Info.Args).RefType != null)
		{
			text3 = ((dynamic)base.Info.Args).RefType;
		}
		if (text2.StartsWith("$"))
		{
			string key = text2.Substring(1);
			text2 = base.Cache[key];
		}
		string empty = string.Empty;
		try
		{
			empty = Smart.ADB.Shell(val.ID, text, 10000).Trim();
			if (text4 == "androidversion")
			{
				empty = GetAndroidVersion(empty);
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error sending ADB shell command: " + ex.Message);
			Smart.Log.Error(TAG, ex.ToString());
			LogResult((Result)4, "Could not send ADB shell command", ex.Message);
			return;
		}
		Smart.Log.Verbose(TAG, "Return string: " + empty);
		bool flag = false;
		if (text2 == string.Empty)
		{
			flag = empty != string.Empty;
		}
		else if (text3 != string.Empty)
		{
			compareLogic = (CompareLogic)Enum.Parse(typeof(CompareLogic), text3);
			flag = IsMatched(text2, empty);
		}
		else
		{
			flag = empty.Contains(text2);
		}
		Result result = (Result)((!flag) ? 1 : 8);
		string text5 = $"Expected value \"{text2}\". Found value \"{empty}\"";
		Smart.Log.Verbose(TAG, "Result =" + ((object)(Result)(ref result)).ToString() + "," + text5);
		if (((dynamic)base.Info.Args).Description != null)
		{
			text5 = ((dynamic)base.Info.Args).Description;
		}
		if (text2 == string.Empty)
		{
			SetPreCondition(empty);
		}
		else
		{
			SetPreCondition(text2);
		}
		VerifyOnly(ref result);
		LogResult(result, text5);
	}

	private string GetAndroidVersion(string shelled)
	{
		string result = string.Empty;
		int num = 1;
		if (((dynamic)base.Info.Args).VersionSize != null)
		{
			num = ((dynamic)base.Info.Args).VersionSize;
		}
		string[] array = shelled.Split(new char[1] { '/' });
		if (array.Length > 3)
		{
			string text = array[3].Trim();
			result = ((text.Length >= num) ? text.Substring(0, num) : text);
		}
		return result;
	}
}
