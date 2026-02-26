using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public abstract class BaseStep : IStep
{
	public enum CompareLogic
	{
		None,
		Equal,
		Contain,
		StartWith,
		EndWith,
		Length
	}

	public CompareLogic compareLogic;

	private static readonly Random rand = new Random();

	protected bool audited;

	private string TAG => GetType().FullName;

	public string Name => Info.Name;

	public string FriendlyName => Info.FriendlyName;

	public IRecipe Recipe { get; private set; }

	public IStepInfo Info { get; private set; }

	protected IResultLogger Log => Recipe.Log;

	protected SortedList<string, dynamic> Cache => Recipe.Cache;

	protected SortedList<string, dynamic> CheckedLimits { get; private set; }

	protected dynamic FailedLimits
	{
		get
		{
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Invalid comparison between Unknown and I4
			if (CheckedLimits.Count < 1)
			{
				return null;
			}
			foreach (string key in CheckedLimits.Keys)
			{
				if ((int)(Result)CheckedLimits[key].Result == 1)
				{
					return CheckedLimits[key];
				}
			}
			return null;
		}
	}

	protected string ImageFile
	{
		get
		{
			//IL_024d: Unknown result type (might be due to invalid IL or missing references)
			string text = "DefaultWaiting.gif";
			if (((dynamic)Info.Args)["ImageFile"] != null)
			{
				text = ((dynamic)Info.Args)["ImageFile"];
			}
			IDevice val = (IDevice)((dynamic)Recipe.Info.Args).Device;
			return Smart.Rsd.GetFilePathName(text, Recipe.Info.UseCase, val);
		}
	}

	public void Load(IRecipe recipe, IStepInfo info)
	{
		Recipe = recipe;
		Info = info;
		CheckedLimits = new SortedList<string, object>();
	}

	public virtual void Setup()
	{
	}

	protected virtual void Set(string settingType, dynamic settings)
	{
	}

	public virtual Result Audit()
	{
		//IL_083d: Unknown result type (might be due to invalid IL or missing references)
		//IL_084a: Invalid comparison between Unknown and I4
		//IL_0878: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
		double? num = ((dynamic)Info.Args).AuditPercent;
		if (num.HasValue)
		{
			decimal qualityRating = Smart.UserAccounts.CurrentUser.QualityRating;
			double num2 = num.Value - 0.075;
			num = new double?((10.0 - (double)qualityRating * 2.0) * 0.025) + num2;
			if (num < 0.025)
			{
				num = 0.025;
			}
			if (num > 0.4)
			{
				num = 0.4;
			}
			double num3 = rand.NextDouble();
			Smart.Log.Verbose(TAG, string.Format("Audit percentage {0:P2} (step {1:P2}), audit challenge {2:P2}", num, ((dynamic)Info.Args).AuditPercent, num3));
			if (num3 >= num)
			{
				return (Result)2;
			}
			Smart.Log.Verbose(TAG, $"Audit selected for {Name} ({Info.Step})");
			bool flag = true;
			if (((dynamic)Info.Args).AuditSetup != null)
			{
				flag = ((dynamic)Info.Args).AuditSetup;
			}
			try
			{
				if (flag)
				{
					Setup();
					if (((dynamic)Info.Args).AuditSettings != null)
					{
						Set(((dynamic)Info.Args).SettingsType.ToString(), ((dynamic)Info.Args).AuditSettings);
					}
				}
				string type = ((dynamic)Info.Args).PromptType;
				string text = ((dynamic)Info.Args).PromptText;
				text = Smart.Locale.Xlate(text);
				Result val = Prompt(type, text);
				audited = true;
				if ((int)val == 8)
				{
					Smart.Log.Verbose(TAG, $"Audit failure for {Name} ({Info.Step})");
					return (Result)3;
				}
				Smart.Log.Verbose(TAG, $"Audit complete for {Name} ({Info.Step})");
			}
			finally
			{
				if (flag)
				{
					TearDown();
				}
			}
			return (Result)8;
		}
		return (Result)7;
	}

	public virtual void TearDown()
	{
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Invalid comparison between Unknown and I4
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Invalid comparison between Unknown and I4
		foreach (string key in CheckedLimits.Keys)
		{
			string text = $"{Name}-{key}";
			double num = CheckedLimits[key].Min;
			double num2 = CheckedLimits[key].Max;
			double num3 = CheckedLimits[key].Value;
			Result val = (Result)CheckedLimits[key].Result;
			Smart.Log.Info(TAG, $"{text} - {num.ToString()} < {num3.ToString()} < {num2.ToString()}: {((object)(Result)(ref val)).ToString()}");
			if ((((dynamic)Info.Args).IgnoreFail == true || ((dynamic)Info.Args).VerifyOnly == true) && ((int)val == 1 || (int)val == 4))
			{
				Smart.Log.Debug(TAG, $"Ignoring test status {((object)(Result)(ref val)).ToString()}, skipping result for {Name}");
				val = (Result)7;
			}
			Log.AddResult(text, Info.Step, val, "", "", "", num2, num, num3, (SortedList<string, object>)null);
		}
	}

	public virtual void Restart()
	{
	}

	public abstract void Run();

	public bool CheckRetest(Result result)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Invalid comparison between Unknown and I4
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Invalid comparison between Unknown and I4
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Invalid comparison between Unknown and I4
		//IL_09ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d0: Invalid comparison between Unknown and I4
		if (((int)result == 1 || (int)result == 4) && ((dynamic)Info.Args).RetryLoops != null && ((dynamic)Info.Args).RetryLoops > 0)
		{
			((dynamic)Info.Args).Retesting = true;
			return true;
		}
		if (!((((dynamic)Info.Args).IgnoreRetest != true && (int)result == 1 && ((dynamic)Info.Args).Retest == true && ((((dynamic)Info.Args).RetryLoops != null && ((dynamic)Info.Args).RetryLoops == 0) || ((dynamic)Info.Args).Retesting != true)) ? true : false))
		{
			((dynamic)Info.Args).Retesting = false;
		}
		else if ((int)Prompt("YesNo", "Test failed. Do you want to re-test?") == 8)
		{
			((dynamic)Info.Args).Retesting = true;
			return true;
		}
		return false;
	}

	protected void LogPass()
	{
		LogResult((Result)8);
	}

	protected void LogResult(Result result, bool logged = true)
	{
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Invalid comparison between Unknown and I4
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Invalid comparison between Unknown and I4
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		if (!logged)
		{
			return;
		}
		if (FailedLimits != null)
		{
			LogResult(result, "", FailedLimits.Max, FailedLimits.Min, FailedLimits.Value);
			return;
		}
		if (((dynamic)Info.Args).IgnoreFail == true && ((int)result == 1 || (int)result == 4))
		{
			Smart.Log.Debug(TAG, $"Ignoring test status {((object)(Result)(ref result)).ToString()}, skipping result for {Name}");
			result = (Result)7;
		}
		if (!CheckRetest(result))
		{
			Log.AddResult(Name, Info.Step, result, "", FriendlyName, "", double.MinValue, double.MinValue, double.MinValue, (SortedList<string, object>)null);
		}
	}

	protected void LogResult(Result result, string description)
	{
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Invalid comparison between Unknown and I4
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Invalid comparison between Unknown and I4
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		if (FailedLimits != null)
		{
			LogResult(result, description, FailedLimits.Max, FailedLimits.Min, FailedLimits.Value);
			return;
		}
		if (((dynamic)Info.Args).IgnoreFail == true && ((int)result == 1 || (int)result == 4))
		{
			Smart.Log.Debug(TAG, $"Ignoring test status {((object)(Result)(ref result)).ToString()}, skipping result for {Name}");
			result = (Result)7;
		}
		if (!CheckRetest(result))
		{
			Log.AddResult(Name, Info.Step, result, description, FriendlyName, "", double.MinValue, double.MinValue, double.MinValue, (SortedList<string, object>)null);
		}
	}

	protected void LogResult(Result result, string description, string dynamicError)
	{
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Invalid comparison between Unknown and I4
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Invalid comparison between Unknown and I4
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		if (((dynamic)Info.Args).IgnoreFail == true && ((int)result == 1 || (int)result == 4))
		{
			Smart.Log.Debug(TAG, $"Ignoring test status {((object)(Result)(ref result)).ToString()}, skipping result for {Name}");
			result = (Result)7;
		}
		if (!CheckRetest(result))
		{
			Log.AddResult(Name, Info.Step, result, description, FriendlyName, dynamicError, double.MinValue, double.MinValue, double.MinValue, (SortedList<string, object>)null);
		}
	}

	protected void LogResult(Result result, string description, double max, double min, double value)
	{
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Invalid comparison between Unknown and I4
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Invalid comparison between Unknown and I4
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		if (((dynamic)Info.Args).IgnoreFail == true && ((int)result == 1 || (int)result == 4))
		{
			Smart.Log.Debug(TAG, $"Ignoring test status {((object)(Result)(ref result)).ToString()}, skipping result for {Name}");
			result = (Result)7;
		}
		if (!CheckRetest(result))
		{
			Log.AddResult(Name, Info.Step, result, description, FriendlyName, "", max, min, value, (SortedList<string, object>)null);
		}
	}

	protected string DynamicKey(dynamic item)
	{
		string empty = string.Empty;
		if (item.GetType().GetProperty("Name") != null)
		{
			return item.Name;
		}
		return item.Key;
	}

	protected Result Prompt(string type, string text)
	{
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		string text2 = Smart.Locale.Xlate(Name);
		if (audited)
		{
			text = string.Concat(Smart.Locale.Xlate("Please re-check:") + " ", text);
		}
		return ((IDevice)((dynamic)Recipe.Info.Args).Device).Prompt.CitPrompt(text2, type, text, ImageFile);
	}

	protected Result ColorPrompt(string type, string text, string imagePath)
	{
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		string text2 = Smart.Locale.Xlate(Name);
		if (audited)
		{
			text = string.Concat(Smart.Locale.Xlate("Please re-check:") + " ", text);
		}
		return ((IDevice)((dynamic)Recipe.Info.Args).Device).Prompt.FrontcolorPrompt(text2, type, text, ImageFile, imagePath);
	}

	protected void ProgressUpdate(double progress)
	{
		double num = ((dynamic)Info.Args)["ProgressStart"];
		double num2 = (double)((dynamic)Info.Args)["ProgressEnd"] - num;
		double progress2 = num + num2 * (progress / 100.0);
		Log.Progress = progress2;
	}

	protected Result LimitCheck(SortedList<string, string> results, dynamic limits)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		foreach (dynamic limit in limits)
		{
			string key = DynamicKey(limit);
			if (!results.ContainsKey(key))
			{
				result = (Result)1;
				continue;
			}
			double num = limit.Value.Min;
			double num2 = limit.Value.Max;
			double num3 = double.Parse(results[key]);
			dynamic val = new ExpandoObject();
			val.Min = num;
			val.Max = num2;
			val.Value = num3;
			if (!(num > num3) && !(num3 > num2))
			{
				val.Result = (Result)8;
			}
			else
			{
				result = (Result)1;
				val.Result = (Result)1;
			}
			CheckedLimits[key] = (object)val;
		}
		return result;
	}

	protected Result ResultCheck(SortedList<string, string> results, dynamic expecteds)
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		string empty = string.Empty;
		return (Result)ResultCheck(results, expecteds, false, false, out empty);
	}

	protected Result ResultCheck(SortedList<string, string> results, dynamic expecteds, bool caseSensitive, bool partial)
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		string empty = string.Empty;
		return (Result)ResultCheck(results, expecteds, caseSensitive, partial, out empty);
	}

	protected Result ResultCheck(SortedList<string, string> results, dynamic expecteds, bool caseSensitive, bool partial, out string errmessage)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		errmessage = string.Empty;
		foreach (dynamic expected in expecteds)
		{
			string text = DynamicKey(expected);
			if (!results.ContainsKey(text))
			{
				Smart.Log.Info(TAG, $"No result found for {text}");
				result = (Result)1;
				continue;
			}
			string text2 = expected.Value;
			char[] separator = new char[1] { ',' };
			string[] array = text2.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			string text3 = results[text];
			bool flag = false;
			string[] array2 = array;
			foreach (string obj in array2)
			{
				string text4 = text3.Trim();
				string text5 = obj.Trim();
				if (!caseSensitive)
				{
					text4 = text4.ToLowerInvariant();
					text5 = text5.ToLowerInvariant();
				}
				bool flag2 = text4 == text5;
				bool flag3 = text4.Contains(text5);
				if (flag2 || (partial && flag3))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Smart.Log.Info(TAG, $"Result found for {text}: {text3} does not match value");
				errmessage = $"Result found for {text}: {text3} does not match value";
				result = (Result)1;
			}
		}
		return result;
	}

	protected Result LengthCheck(SortedList<string, string> results, dynamic lengths)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		foreach (dynamic length in lengths)
		{
			string text = DynamicKey(length);
			if (!results.ContainsKey(text))
			{
				Smart.Log.Info(TAG, $"No result found for {text}");
				result = (Result)1;
				continue;
			}
			int num = length.Value;
			string text2 = results[text];
			text2 = text2.Trim();
			if (text2.Length != num)
			{
				Smart.Log.Info(TAG, $"Result found for {text}: '{text2}' does not match expected legnth {num}");
				result = (Result)1;
			}
		}
		return result;
	}

	protected void SetPreCondition(string value)
	{
		if ((((dynamic)Info.Args).SetPreCond != null) && (bool)((dynamic)Info.Args).SetPreCond && Name != null)
		{
			string text = Name.Trim();
			if (text != string.Empty)
			{
				value = value.Trim();
				Smart.Log.Debug(TAG, $"Add pair ({text}, {value}) to Cache");
				Cache[text] = value;
			}
		}
	}

	public bool VerifyPreContionMet()
	{
		if (((dynamic)Info.Args).PreCondTest != null)
		{
			string text = (string)((dynamic)Info.Args).PreCondTest;
			string[] array = text.Split(new char[1] { '&' });
			string[] array2 = new string[0];
			bool flag = false;
			bool flag2 = true;
			if (((dynamic)Info.Args).LogicAND != null)
			{
				flag2 = ((dynamic)Info.Args).LogicAND;
			}
			string text2 = string.Empty;
			if (((dynamic)Info.Args).PreCondValue != null)
			{
				text2 = (string)((dynamic)Info.Args).PreCondValue;
				array2 = text2.Split(new char[1] { '&' });
				compareLogic = CompareLogic.Equal;
			}
			else if (((dynamic)Info.Args).PreCondValueContain != null)
			{
				text2 = (string)((dynamic)Info.Args).PreCondValueContain;
				array2 = text2.Split(new char[1] { '&' });
				compareLogic = CompareLogic.Contain;
			}
			else if (((dynamic)Info.Args).PreCondValueStartWith != null)
			{
				text2 = (string)((dynamic)Info.Args).PreCondValueStartWith;
				array2 = text2.Split(new char[1] { '&' });
				compareLogic = CompareLogic.StartWith;
			}
			else if (((dynamic)Info.Args).PreCondValueEndWith != null)
			{
				text2 = (string)((dynamic)Info.Args).PreCondValueEndWith;
				array2 = text2.Split(new char[1] { '&' });
				compareLogic = CompareLogic.EndWith;
			}
			bool flag3 = true;
			if (((dynamic)Info.Args).SkipOnPrecondSkipped != null)
			{
				flag3 = ((dynamic)Info.Args).SkipOnPrecondSkipped;
			}
			bool flag4 = false;
			List<string> list = null;
			if (((dynamic)Info.Args).PreCondEqual != null && (bool)((dynamic)Info.Args).PreCondEqual)
			{
				list = new List<string>();
			}
			if (((dynamic)Info.Args).PreCondDiff != null && (bool)((dynamic)Info.Args).PreCondDiff)
			{
				list = new List<string>();
				flag4 = true;
			}
			for (int i = 0; i < array.Length; i++)
			{
				string text3 = array[i].Trim();
				if (Cache.TryGetValue(text3, out var value))
				{
					string text4 = (string)value;
					list?.Add(text4);
					string[] array3 = text4.Split(new char[1] { ',' });
					foreach (string value2 in array3)
					{
						if (array2.Length < 1)
						{
							break;
						}
						string[] array4 = array2[i].Split(new char[1] { ',' });
						foreach (string text5 in array4)
						{
							flag = IsMatched(text5, value2);
							if (flag)
							{
								Smart.Log.Debug(TAG, $"PreCondTest: \"{text3}\" PreCondValue: \"{text5}\" is met");
								if (flag2)
								{
									break;
								}
								Smart.Log.Debug(TAG, $"Test \"{Name}\" is executed (logic OR)");
								return true;
							}
						}
						if (flag)
						{
							break;
						}
					}
					if (!flag && flag2 && list == null)
					{
						Smart.Log.Debug(TAG, $"PreCondTest: \"{text3}\" and PreCondValue: \"{array2[i]}\" is not met. Test \"{Name}\" is skipped (logic AND)");
						LogSkippedResult();
						return false;
					}
				}
				else if (flag3)
				{
					Smart.Log.Debug(TAG, $"PreCondtest \"{text3}\" not in Cache");
					if (flag2)
					{
						Smart.Log.Debug(TAG, $"skipOnPrecondSkipped: {flag3} Test \"{Name}\" is skipped (logic AND)");
						LogSkippedResult();
						return false;
					}
					flag = false;
				}
				else
				{
					Smart.Log.Debug(TAG, $"PreCondtest \"{text3}\" not in Cache");
					if (!flag2)
					{
						Smart.Log.Debug(TAG, $"skipOnPrecondSkipped: {flag3} logic OR. Test \"{Name}\" is executed");
						return true;
					}
					flag = true;
				}
			}
			if (list != null)
			{
				bool flag5 = true;
				string text6 = string.Empty;
				string text7 = string.Empty;
				foreach (string item in list)
				{
					text7 = text7 + "'" + item + "' ";
					if (text6 == string.Empty)
					{
						text6 = item;
					}
					else if (text6 != item)
					{
						flag5 = false;
					}
				}
				if (flag4 && !flag5)
				{
					Smart.Log.Debug(TAG, $"PreCondTest: \"{text}\" did not have equal values ({text7}), Test \"{Name}\" is executed");
					return true;
				}
				if (!flag4 && flag5)
				{
					Smart.Log.Debug(TAG, $"PreCondTest: \"{text}\" have equal values ({text7}), Test \"{Name}\" is executed");
					return true;
				}
				if (flag5)
				{
					Smart.Log.Debug(TAG, $"PreCondTest: \"{text}\" have equal values ({text7})");
				}
				else if (!flag5)
				{
					Smart.Log.Debug(TAG, $"PreCondTest: \"{text}\" did not have equal values ({text7})");
				}
			}
			if (!flag)
			{
				Smart.Log.Debug(TAG, $"PreCondTest: \"{text}\" and PreCondValue: \"{text2}\" is not met Test \"{Name}\" is skipped");
				LogSkippedResult();
				return false;
			}
		}
		return true;
	}

	public bool IsMatched(string val, string value)
	{
		string text = val.Trim();
		bool result = false;
		if (text.StartsWith("!"))
		{
			text = ((text.Length != 1) ? text.Substring(1).Trim() : string.Empty);
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = Cache[key];
			}
			if (text == null || value == null)
			{
				Smart.Log.Error(TAG, "Cannot compare null value");
				return false;
			}
			switch (compareLogic)
			{
			case CompareLogic.Equal:
				result = string.Compare(text, value, ignoreCase: true) != 0;
				break;
			case CompareLogic.Contain:
				result = !value.ToLowerInvariant().Contains(text.ToLowerInvariant());
				break;
			case CompareLogic.StartWith:
				result = !value.ToLowerInvariant().StartsWith(text.ToLowerInvariant());
				break;
			case CompareLogic.EndWith:
				result = !value.ToLowerInvariant().EndsWith(text.ToLowerInvariant());
				break;
			case CompareLogic.Length:
			{
				int result2 = 0;
				if (int.TryParse(text, out result2))
				{
					result = value.Length != result2;
				}
				break;
			}
			}
		}
		else
		{
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = Cache[key];
			}
			if (text == null || value == null)
			{
				Smart.Log.Error(TAG, "Cannot compare null value");
				return false;
			}
			switch (compareLogic)
			{
			case CompareLogic.Equal:
				result = string.Compare(text, value, ignoreCase: true) == 0;
				break;
			case CompareLogic.Contain:
				result = value.ToLowerInvariant().Contains(text.ToLowerInvariant());
				break;
			case CompareLogic.StartWith:
				result = value.ToLowerInvariant().StartsWith(text.ToLowerInvariant());
				break;
			case CompareLogic.EndWith:
				result = value.ToLowerInvariant().EndsWith(text.ToLowerInvariant());
				break;
			case CompareLogic.Length:
			{
				int result3 = 0;
				if (int.TryParse(text, out result3))
				{
					result = value.Length == result3;
				}
				break;
			}
			}
		}
		return result;
	}

	private void LogSkippedResult()
	{
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (((dynamic)Info.Args).VerifyOnly != null)
		{
			flag = ((dynamic)Info.Args).VerifyOnly;
		}
		if (flag)
		{
			Result val = (Result)7;
			SetPreCondition(((object)(Result)(ref val)).ToString());
		}
		LogResult((Result)7);
	}

	protected Result VerifyPropertyValue(string propValue, bool logOnFailed = false, string propName = "value", bool containedAll = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		bool flag = false;
		if (((dynamic)Info.Args).Expected != null)
		{
			string[] array = ParseArgument(((dynamic)Info.Args).Expected);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (text.StartsWith("$"))
				{
					string key = text.Substring(1);
					text = Cache[key];
				}
				Smart.Log.Debug(TAG, $"Expected value: {text}");
				if (string.Compare(text, propValue, ignoreCase: true) == 0)
				{
					Smart.Log.Debug(TAG, $"Passed: Expected value {text} matched");
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				result = (Result)1;
				if (logOnFailed)
				{
					LogResult((Result)1, $"Expected {propName} not found");
				}
			}
		}
		else if (((dynamic)Info.Args).NotExpected != null)
		{
			string[] array = ParseArgument(((dynamic)Info.Args).NotExpected);
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i].Trim();
				if (text2.StartsWith("$"))
				{
					string key2 = text2.Substring(1);
					text2 = Cache[key2];
				}
				Smart.Log.Debug(TAG, $"NotExpected value: {text2}");
				if (string.Compare(text2, propValue, ignoreCase: true) == 0)
				{
					Smart.Log.Debug(TAG, $"Failed: NotExpected value {text2} matched");
					flag = true;
					break;
				}
			}
			if (flag)
			{
				result = (Result)1;
				if (logOnFailed)
				{
					LogResult((Result)1, $"Unexpected {propName} found");
				}
			}
		}
		else
		{
			string contained = ((dynamic)Info.Args).Contained;
			string notContained = ((dynamic)Info.Args).NotContained;
			result = VerifyContainedPropertyValue(contained, notContained, propValue, logOnFailed, propName, containedAll);
		}
		return result;
	}

	protected Result VerifyContainedPropertyValue(string contained, string notContained, string propValue, bool logOnFailed = false, string propName = "value", bool containedAll = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		bool flag = false;
		if (contained != null)
		{
			string[] array = ParseArgument(contained);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (text.StartsWith("$"))
				{
					string key = text.Substring(1);
					text = Cache[key];
				}
				Smart.Log.Debug(TAG, $"Check phone response contained value: {text}");
				if (text == string.Empty && propValue != string.Empty)
				{
					continue;
				}
				if (propValue.ToLower().Contains(text.ToLower()))
				{
					flag = true;
					if (!containedAll)
					{
						Smart.Log.Debug(TAG, $"Passed: Contained value '{text}' is in '{propValue}'");
						break;
					}
					Smart.Log.Debug(TAG, string.Format("Value '{0}' is contained in '{1}'", text, "propValue"));
				}
				else if (containedAll)
				{
					Smart.Log.Debug(TAG, string.Format("Failed: Contained value '{0}' is NOT in '{1}'", text, "propValue"));
					flag = false;
					break;
				}
			}
			if (!flag)
			{
				result = (Result)1;
				Smart.Log.Error(TAG, string.Format("Could not find contained value {0} in '{1}'", contained, containedAll ? "propValue" : propValue));
				if (logOnFailed)
				{
					LogResult((Result)1, $"Contained value {contained} not found in {propName}");
				}
			}
		}
		else if (notContained != null)
		{
			string[] array = ParseArgument(notContained);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (text.StartsWith("$"))
				{
					string key = text.Substring(1);
					text = Cache[key];
				}
				Smart.Log.Debug(TAG, $"NotContained value: {text}");
				if ((!(text == string.Empty) || !(propValue != string.Empty)) && propValue.ToLower().Contains(text.ToLower()))
				{
					Smart.Log.Debug(TAG, $"Failed: NotContained value '{text}' is in '{propValue}'");
					flag = true;
					break;
				}
			}
			if (flag)
			{
				result = (Result)1;
				Smart.Log.Error(TAG, $"Found not notContained value {notContained} in propValue '{propValue}'");
				if (logOnFailed)
				{
					LogResult((Result)1, $"NotContained {notContained} is found in {propName}");
				}
			}
		}
		return result;
	}

	protected string[] ParseArgument(dynamic argument)
	{
		string[] result = new string[0];
		if (argument == null)
		{
			return result;
		}
		try
		{
			return ((string)argument).Split(new char[1] { ',' });
		}
		catch (Exception)
		{
			List<string> list = new List<string>();
			foreach (object item2 in argument)
			{
				string item = (string)(dynamic)item2;
				list.Add(item);
			}
			return list.ToArray();
		}
	}

	protected void VerifyOnly(ref Result result)
	{
		bool flag = false;
		if (((dynamic)Info.Args).VerifyOnly != null)
		{
			flag = ((dynamic)Info.Args).VerifyOnly;
		}
		if (flag)
		{
			Smart.Log.Verbose(TAG, $"Setting result to Passed and keeping original result {result} as verify only");
			SetPreCondition(((object)(Result)(ref result)).ToString());
			result = (Result)8;
		}
	}

	protected bool MessageToAction(string logMessage, List<string> mLogMsgToClosePrompts, List<List<string>> mLogMsgToPrompts, List<List<string>> mLogMsgToCommands, List<List<string>> mLogMsgToExtractValues)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		MessageBoxButtons buttonType = (MessageBoxButtons)0;
		MessageBoxIcon iconType = (MessageBoxIcon)64;
		IDevice device = (IDevice)((dynamic)Recipe.Info.Args).Device;
		bool result = true;
		logMessage = logMessage.Trim().Replace(" ", string.Empty).ToLower();
		if (string.IsNullOrEmpty(logMessage))
		{
			return result;
		}
		if (mLogMsgToClosePrompts != null)
		{
			foreach (string mLogMsgToClosePrompt in mLogMsgToClosePrompts)
			{
				if (logMessage.Contains(mLogMsgToClosePrompt.Replace(" ", string.Empty).ToLower()))
				{
					Smart.Log.Debug(TAG, $"Message: {mLogMsgToClosePrompt} => close current prompt");
					device.Prompt.CloseMessageBox();
					break;
				}
			}
		}
		if (mLogMsgToPrompts != null)
		{
			foreach (List<string> msgToPrompt in mLogMsgToPrompts)
			{
				if (!logMessage.Contains(msgToPrompt[0].Replace(" ", string.Empty).ToLower()))
				{
					continue;
				}
				Smart.Log.Debug(TAG, $"Message: {msgToPrompt[0]} => PromptText: {msgToPrompt[1]}");
				Task.Run(delegate
				{
					//IL_0199: Unknown result type (might be due to invalid IL or missing references)
					//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
					//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
					//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
					//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
					//IL_0076: Unknown result type (might be due to invalid IL or missing references)
					//IL_0077: Unknown result type (might be due to invalid IL or missing references)
					//IL_011d: Unknown result type (might be due to invalid IL or missing references)
					//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
					device.Prompt.CloseMessageBox();
					string text5 = Smart.Locale.Xlate(msgToPrompt[1]);
					string text6 = Smart.Locale.Xlate(Info.Name);
					if (msgToPrompt.Count > 2)
					{
						if (Enum.TryParse<MessageBoxIcon>(msgToPrompt[2], ignoreCase: true, out MessageBoxIcon result2))
						{
							iconType = result2;
						}
						else
						{
							Smart.Log.Error(TAG, $"Unknown {msgToPrompt[2]} IconType. Use default {iconType}");
						}
					}
					if (msgToPrompt.Count > 3)
					{
						if (Enum.TryParse<MessageBoxButtons>(msgToPrompt[3], ignoreCase: true, out MessageBoxButtons result3))
						{
							buttonType = result3;
						}
						else
						{
							Smart.Log.Error(TAG, $"Unknown {msgToPrompt[3]} ButtonType. Use default {buttonType}");
						}
					}
					Smart.Log.Debug(TAG, $"Icon: {((object)(MessageBoxIcon)(ref iconType)).ToString()}, Button {((object)(MessageBoxButtons)(ref buttonType)).ToString()}");
					DialogResult val = device.Prompt.MessageBox(text6, text5, buttonType, iconType);
					Smart.Log.Debug(TAG, $"User response {((object)(DialogResult)(ref val)).ToString()}");
				});
				break;
			}
		}
		if (mLogMsgToCommands != null)
		{
			int num = 30000;
			if (((dynamic)Info.Args).ActionTimeOut != null)
			{
				num = ((dynamic)Info.Args).ActionTimeOut;
				num = 1000 * num;
			}
			int num2 = default(int);
			foreach (List<string> mLogMsgToCommand in mLogMsgToCommands)
			{
				if (!logMessage.Contains(mLogMsgToCommand[0].Replace(" ", string.Empty).ToLower()))
				{
					continue;
				}
				string filePathName = Smart.Rsd.GetFilePathName(mLogMsgToCommand[1], Recipe.Info.UseCase, device);
				string text = string.Empty;
				List<string> list = new List<string>();
				for (int i = 2; i < mLogMsgToCommand.Count; i++)
				{
					if (i == 2)
					{
						text = mLogMsgToCommand[i];
					}
					else if (i > 2)
					{
						string text2 = mLogMsgToCommand[i];
						if (text2.StartsWith("$"))
						{
							string key = text2.Substring(1);
							text2 = Cache[key];
						}
						list.Add(text2);
					}
				}
				if (list.Count > 0 && text != string.Empty)
				{
					text = string.Format(text, list.ToArray());
				}
				Smart.MotoAndroid.Shell(device.ID, text, num, filePathName, ref num2, 6000, true);
				if (num2 != 0)
				{
					result = false;
				}
			}
		}
		if (mLogMsgToExtractValues != null)
		{
			foreach (List<string> mLogMsgToExtractValue in mLogMsgToExtractValues)
			{
				string text3 = mLogMsgToExtractValue[0].Replace(" ", string.Empty).ToLower();
				int num3 = logMessage.IndexOf(text3);
				if (num3 >= 0)
				{
					int length = int.Parse(mLogMsgToExtractValue[1]);
					string text4 = logMessage.Substring(num3 + text3.Length, length);
					if (mLogMsgToExtractValue[2] == "PreCondValue")
					{
						Smart.Log.Verbose(TAG, $"Set {text4} as PreCondValue");
						SetPreCondition(text4);
					}
					else
					{
						Smart.Log.Verbose(TAG, $"Add {text4} to Cache[{mLogMsgToExtractValue[2]}]");
						Cache[mLogMsgToExtractValue[2]] = text4;
					}
				}
			}
		}
		return result;
	}

	public static string VariableSubstitution(string Variable)
	{
		string text = Variable;
		if (Variable.Contains("<") && Variable.Contains(">"))
		{
			text = text.Replace("<DateNow>", DateTime.Now.ToString("MM-dd-yyyy"));
			text = text.Replace("<DateNowLamu>", DateTime.Now.ToString("yyyyMMdd"));
		}
		return text;
	}
}
