using System;
using System.Collections.Generic;
using System.Linq;
using ISmart;

namespace SmartDevice.Steps;

public class CQASend : CommServerStep
{
	private string errmessage = string.Empty;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0857: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0881: Unknown result type (might be due to invalid IL or missing references)
		//IL_0883: Invalid comparison between Unknown and I4
		//IL_088d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0792: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Result result = (Result)8;
		string text = string.Empty;
		bool flag = false;
		if (((dynamic)base.Info.Args).SaveResponseAsPreCond != null)
		{
			flag = Convert.ToBoolean(((dynamic)base.Info.Args).SaveResponseAsPreCond.ToString());
		}
		try
		{
			if (((dynamic)base.Info.Args).PromptTextBeforeGetTestResults != null)
			{
				string text2 = ((dynamic)base.Info.Args).PromptTextBeforeGetTestResults.ToString();
				text2 = Smart.Locale.Xlate(text2);
				result = (Result)Prompt(((dynamic)base.Info.Args).PromptTypeBeforeGetTestResults.ToString(), text2);
			}
			if (((dynamic)base.Info.Args).Command != null)
			{
				string command = ((dynamic)base.Info.Args).Command;
				SortedList<string, string> sortedList = Tell(command);
				result = VerifyTellResults(sortedList);
				text = string.Join(",", sortedList.Select((KeyValuePair<string, string> kvp) => $"{kvp.Key}={kvp.Value}"));
				Smart.Log.Debug(TAG, "Tell Response " + text);
				Tuple<Result, string> tuple = SaveSingleResponse(sortedList, device);
				result = tuple.Item1;
				text = tuple.Item2;
				result = SaveAllResponse(sortedList, device);
			}
		}
		catch (Exception ex)
		{
			errmessage = ex.Message;
			Smart.Log.Error(TAG, string.Format("Exception errMsg: " + errmessage));
			Smart.Log.Error(TAG, ex.StackTrace);
			result = (Result)4;
		}
		if (flag)
		{
			SetPreCondition(text);
		}
		else
		{
			SetPreCondition(((object)(Result)(ref result)).ToString());
		}
		VerifyOnly(ref result);
		if ((int)result == 8)
		{
			LogPass();
		}
		else
		{
			LogResult(result, "Result Check failed", errmessage);
		}
	}

	private Result SaveAllResponse(SortedList<string, string> results, IDevice device)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a02: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b98: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b13: Invalid comparison between Unknown and I4
		//IL_0b07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b09: Invalid comparison between Unknown and I4
		//IL_0ba9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bab: Invalid comparison between Unknown and I4
		//IL_0b9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba1: Invalid comparison between Unknown and I4
		//IL_0b2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba4: Unknown result type (might be due to invalid IL or missing references)
		Smart.Log.Verbose(TAG, "Enter SaveAllResponse...");
		Result val = (Result)8;
		string empty = string.Empty;
		string[] array = null;
		string[] array2 = null;
		string[] array3 = null;
		string[] array4 = null;
		string[] array5 = null;
		string[] array6 = null;
		if (((dynamic)base.Info.Args).ResultKeyOfAll != null)
		{
			array = ((string)((dynamic)base.Info.Args).ResultKeyOfAll).Split(new char[1] { ',' });
		}
		if (((dynamic)base.Info.Args).ExpectedLengthOfAll != null)
		{
			array2 = ((string)((dynamic)base.Info.Args).ExpectedLengthOfAll).Split(new char[1] { ',' });
		}
		if (((dynamic)base.Info.Args).ExpectedValueEqualOfAll != null)
		{
			array3 = ((string)((dynamic)base.Info.Args).ExpectedValueEqualOfAll).Split(new char[1] { ',' });
		}
		if (((dynamic)base.Info.Args).ExpectedValueContainOfAll != null)
		{
			array4 = ((string)((dynamic)base.Info.Args).ExpectedValueContainOfAll).Split(new char[1] { ',' });
		}
		if (((dynamic)base.Info.Args).LogKeyOfAll != null)
		{
			array5 = ((string)((dynamic)base.Info.Args).LogKeyOfAll).Split(new char[1] { ',' });
		}
		if (((dynamic)base.Info.Args).CacheNameOfAll != null)
		{
			array6 = ((string)((dynamic)base.Info.Args).CacheNameOfAll).Split(new char[1] { ',' });
		}
		if (array != null)
		{
			int num = 0;
			while (num < array.Length)
			{
				string text = array[num];
				if (!results.ContainsKey(text))
				{
					Smart.Log.Debug(TAG, "Tell Response doesn't contain key " + text);
					empty = string.Empty;
					val = (Result)1;
					return (Result)1;
				}
				empty = results[text];
				Smart.Log.Debug(TAG, $"Response of {text} is {empty}");
				if (array2 != null)
				{
					string text2 = array2[num].Trim();
					if (text2 != "IGNORE")
					{
						Smart.Log.Debug(TAG, $"check response length for key {text}");
						if (empty.Trim().Length != Convert.ToInt32(text2))
						{
							val = (Result)1;
							Smart.Log.Debug(TAG, "check failed");
							return val;
						}
						val = (Result)8;
					}
				}
				string text3;
				int num2;
				if (array3 != null)
				{
					text3 = array3[num].Trim();
					if (text3 != "IGNORE")
					{
						if (!text3.StartsWith("!"))
						{
							num2 = 0;
							if (num2 == 0)
							{
								goto IL_0ad4;
							}
						}
						else
						{
							num2 = 1;
						}
						text3 = text3.Substring(1);
						goto IL_0ad4;
					}
				}
				goto IL_0b2c;
				IL_0ad4:
				Smart.Log.Debug(TAG, $"check response fully match expected for {text}");
				val = ((!(empty.Trim() == text3)) ? ((Result)1) : ((Result)8));
				if (num2 != 0)
				{
					val = (((int)val != 8) ? ((Result)8) : ((Result)1));
				}
				if ((int)val == 1)
				{
					Smart.Log.Debug(TAG, "check failed");
					return val;
				}
				goto IL_0b2c;
				IL_0b6c:
				Smart.Log.Debug(TAG, $"check response contain expected for key {text}");
				string text4;
				val = ((!empty.Trim().Contains(text4)) ? ((Result)1) : ((Result)8));
				int num3;
				if (num3 != 0)
				{
					val = (((int)val != 8) ? ((Result)8) : ((Result)1));
				}
				if ((int)val == 1)
				{
					Smart.Log.Debug(TAG, "check failed");
					return val;
				}
				goto IL_0bc4;
				IL_0bc4:
				if (array5 != null)
				{
					Smart.Log.Debug(TAG, "Save result to rsd log...");
					string text5 = array5[num];
					if (text5.Trim() != "IGNORE")
					{
						device.Log.AddInfo(text5, empty);
					}
				}
				if (array6 != null)
				{
					Smart.Log.Debug(TAG, "Save result to Cache...");
					string text6 = array6[num];
					if (text6.Trim() != "IGNORE")
					{
						base.Cache[text6] = empty;
					}
				}
				num++;
				continue;
				IL_0b2c:
				if (array4 != null)
				{
					text4 = array4[num].Trim();
					if (text4 != "IGNORE")
					{
						if (!text4.StartsWith("!"))
						{
							num3 = 0;
							if (num3 == 0)
							{
								goto IL_0b6c;
							}
						}
						else
						{
							num3 = 1;
						}
						text4 = text4.Substring(1);
						goto IL_0b6c;
					}
				}
				goto IL_0bc4;
			}
		}
		Smart.Log.Verbose(TAG, "Exit SaveAllResponse...");
		return val;
	}

	private Tuple<Result, string> SaveSingleResponse(SortedList<string, string> results, IDevice device)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Invalid comparison between Unknown and I4
		//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Invalid comparison between Unknown and I4
		//IL_0817: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a19: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2a: Invalid comparison between Unknown and I4
		//IL_0a1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a20: Invalid comparison between Unknown and I4
		//IL_0a41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a23: Unknown result type (might be due to invalid IL or missing references)
		Smart.Log.Verbose(TAG, "Enter SaveSingleResponse...");
		Result val = (Result)8;
		string text = string.Empty;
		if (((dynamic)base.Info.Args).ResultKey != null)
		{
			string text2 = ((dynamic)base.Info.Args).ResultKey;
			if (!results.ContainsKey(text2))
			{
				Smart.Log.Debug(TAG, "Tell Response doesn't contain key " + text2);
				text = string.Empty;
				val = (Result)1;
			}
			else
			{
				text = results[text2];
			}
		}
		if (((dynamic)base.Info.Args).ResultKeyOfAny != null)
		{
			text = string.Empty;
			string[] array = ((string)((dynamic)base.Info.Args).ResultKeyOfAny).Split(new char[1] { ',' });
			bool flag = false;
			int num = -1;
			string[] array2 = array;
			foreach (string key in array2)
			{
				num++;
				if (results.ContainsKey(key))
				{
					flag = true;
					text = results[key];
					break;
				}
			}
			if (!flag)
			{
				Smart.Log.Debug(TAG, "Not Found any of the key in tell response");
				return new Tuple<Result, string>((Result)1, text);
			}
			Smart.Log.Debug(TAG, $"Found key {array[num]} in tell response with value {text}");
			if (((dynamic)base.Info.Args).ExpectedLengthOfAny != null)
			{
				string text3 = ((string)((dynamic)base.Info.Args).ExpectedLengthOfAny).Split(new char[1] { ',' })[num].Trim();
				if (text3 != "IGNORE")
				{
					Smart.Log.Debug(TAG, "check response length...");
					if (text.Trim().Length != Convert.ToInt32(text3))
					{
						val = (Result)1;
						Smart.Log.Debug(TAG, "check failed");
						return new Tuple<Result, string>(val, text);
					}
					val = (Result)8;
				}
			}
			if (((dynamic)base.Info.Args).ExpectedValueEqualOfAny != null)
			{
				string text4 = ((string)((dynamic)base.Info.Args).ExpectedValueEqualOfAny).Split(new char[1] { ',' })[num].Trim();
				bool flag2 = (text4.StartsWith("!") ? true : false);
				if (flag2)
				{
					text4 = text4.Substring(1);
				}
				if (text4 != "IGNORE")
				{
					Smart.Log.Debug(TAG, "check response fully meet expected...");
					val = ((!(text.Trim() == text4)) ? ((Result)1) : ((Result)8));
					if (flag2)
					{
						val = (((int)val != 8) ? ((Result)8) : ((Result)1));
					}
					if ((int)val == 1)
					{
						Smart.Log.Debug(TAG, "check failed");
						return new Tuple<Result, string>(val, text);
					}
				}
			}
			if (((dynamic)base.Info.Args).ExpectedValueContainOfAny != null)
			{
				string text5 = ((string)((dynamic)base.Info.Args).ExpectedValueContainOfAny).Split(new char[1] { ',' })[num].Trim();
				bool flag3 = (text5.StartsWith("!") ? true : false);
				if (flag3)
				{
					text5 = text5.Substring(1);
				}
				if (text5 != "IGNORE")
				{
					Smart.Log.Debug(TAG, "check response contain expected...");
					val = ((!text.Trim().Contains(text5)) ? ((Result)1) : ((Result)8));
					if (flag3)
					{
						val = (((int)val != 8) ? ((Result)8) : ((Result)1));
					}
					if ((int)val == 1)
					{
						Smart.Log.Debug(TAG, "check failed");
						return new Tuple<Result, string>(val, text);
					}
				}
			}
		}
		Smart.Log.Debug(TAG, "Final found Response " + text);
		if (((dynamic)base.Info.Args).LogKey != null)
		{
			string text6 = ((dynamic)base.Info.Args).LogKey;
			if (!string.IsNullOrEmpty(text6))
			{
				device.Log.AddInfo(text6, text);
			}
		}
		if (((dynamic)base.Info.Args).CacheName != null)
		{
			Smart.Log.Debug(TAG, "Save result to Cache...");
			string text7 = ((dynamic)base.Info.Args).CacheName;
			if (!string.IsNullOrEmpty(text7))
			{
				base.Cache[text7] = text;
			}
		}
		Smart.Log.Verbose(TAG, "Exit SaveSingleResponse...");
		return new Tuple<Result, string>(val, text);
	}

	private Result VerifyTellResults(SortedList<string, string> results)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Invalid comparison between Unknown and I4
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a8: Invalid comparison between Unknown and I4
		//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
		Result val = (Result)1;
		bool flag = false;
		if (((dynamic)base.Info.Args).PartialMatch != null)
		{
			flag = ((dynamic)base.Info.Args).PartialMatch;
		}
		try
		{
			if (((dynamic)base.Info.Args).ExpectedLength != null)
			{
				val = (Result)LengthCheck(results, ((dynamic)base.Info.Args).ExpectedLength);
				if ((int)val != 8)
				{
					return val;
				}
			}
			if (((dynamic)base.Info.Args).Expected != null)
			{
				return (Result)ResultsCheck(results, ((dynamic)base.Info.Args).Expected, false, flag, out errmessage);
			}
			if (((dynamic)base.Info.Args).NotExpected != null)
			{
				val = (Result)ResultsCheck(results, ((dynamic)base.Info.Args).NotExpected, false, flag, out errmessage);
				return (Result)(((int)val == 8) ? 1 : 8);
			}
			return (Result)8;
		}
		catch (Exception ex)
		{
			val = (Result)1;
			Smart.Log.Error(TAG, string.Format(ex.Message + Environment.NewLine + ex.StackTrace));
			return val;
		}
	}

	private Result ResultsCheck(SortedList<string, string> results, dynamic expecteds, bool caseSensitive, bool partial, out string errmessage)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Invalid comparison between Unknown and I4
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		Result val = (Result)8;
		errmessage = string.Empty;
		List<Result> list = new List<Result>();
		foreach (dynamic expected in expecteds)
		{
			Result val2 = (Result)8;
			foreach (dynamic item in expected)
			{
				val2 = (Result)ResultCheck(results, item, caseSensitive, partial, out errmessage);
				if ((int)val2 != 8)
				{
					break;
				}
			}
			list.Add(val2);
		}
		bool flag = true;
		if (((dynamic)base.Info.Args).ResultsCheckLogicAND != null)
		{
			flag = ((dynamic)base.Info.Args).ResultsCheckLogicAND;
		}
		if (flag)
		{
			if (list.All((Result subResult) => (int)subResult == 8))
			{
				return (Result)8;
			}
			return (Result)1;
		}
		if (list.Any((Result subResult) => (int)subResult == 8))
		{
			return (Result)8;
		}
		return (Result)1;
	}

	private Result CheckAnyPhoneResultLengthMeetExpectation(SortedList<string, string> results, string[] resultKeysOfAny)
	{
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		string[] array = null;
		if (((dynamic)base.Info.Args).ExpectedLengthOfAny != null)
		{
			array = ((string)((dynamic)base.Info.Args).ExpectedLengthOfAll).Split(new char[1] { ',' });
			List<Result> list = new List<Result>();
			for (int i = 0; i < resultKeysOfAny.Length; i++)
			{
				string text = resultKeysOfAny[i];
				if (!results.ContainsKey(text))
				{
					Smart.Log.Debug(TAG, "Tell Response doesn't contain key " + text);
					continue;
				}
				string text2 = results[text];
				Smart.Log.Debug(TAG, $"Response of {text} is {text2}");
				Result val = (Result)1;
				string text3 = array[i].Trim();
				if (text3 != "IGNORE")
				{
					Smart.Log.Debug(TAG, $"check response length for key {text} = {text3}");
					val = ((text2.Trim().Length != Convert.ToInt32(text3)) ? ((Result)1) : ((Result)8));
				}
				else
				{
					val = (Result)8;
				}
				list.Add(val);
			}
			if (!list.Any((Result subResult) => (int)subResult == 8))
			{
				Smart.Log.Debug(TAG, "none of the response length equal to expected, fail the test");
				return (Result)1;
			}
			return (Result)8;
		}
		return (Result)8;
	}

	private Result CheckAnyPhoneResultEqualToExpectation(SortedList<string, string> results, string[] resultKeysOfAny)
	{
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Invalid comparison between Unknown and I4
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		string[] array = null;
		if (((dynamic)base.Info.Args).ExpectedValueEqualOfAny != null)
		{
			array = ((string)((dynamic)base.Info.Args).ExpectedValueEqualOfAny).Split(new char[1] { ',' });
			List<Result> list = new List<Result>();
			for (int i = 0; i < resultKeysOfAny.Length; i++)
			{
				string text = resultKeysOfAny[i];
				if (!results.ContainsKey(text))
				{
					Smart.Log.Debug(TAG, "Tell Response doesn't contain key " + text);
					continue;
				}
				string text2 = results[text];
				Smart.Log.Debug(TAG, $"Response of {text} is {text2}");
				Result val = (Result)1;
				string text3 = array[i].Trim();
				int num;
				if (text3 != "IGNORE")
				{
					if (!text3.StartsWith("!"))
					{
						num = 0;
						if (num == 0)
						{
							goto IL_0238;
						}
					}
					else
					{
						num = 1;
					}
					text3 = text3.Substring(1);
					goto IL_0238;
				}
				list.Add((Result)8);
				continue;
				IL_0238:
				Smart.Log.Debug(TAG, "check response fully meet expected...");
				val = ((!(text2.Trim() == text3)) ? ((Result)1) : ((Result)8));
				if (num != 0)
				{
					val = (((int)val != 8) ? ((Result)8) : ((Result)1));
				}
				list.Add(val);
			}
			if (!list.Any((Result subResult) => (int)subResult == 8))
			{
				Smart.Log.Debug(TAG, "none of the response value equal to expected, fail the test");
				return (Result)1;
			}
			return (Result)8;
		}
		return (Result)8;
	}

	private Result CheckAnyPhoneResultContainExpectation(SortedList<string, string> results, string[] resultKeysOfAny)
	{
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Invalid comparison between Unknown and I4
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		string[] array = null;
		if (((dynamic)base.Info.Args).ExpectedValueContainOfAny != null)
		{
			array = ((string)((dynamic)base.Info.Args).ExpectedValueContainOfAny).Split(new char[1] { ',' });
			List<Result> list = new List<Result>();
			for (int i = 0; i < resultKeysOfAny.Length; i++)
			{
				string text = resultKeysOfAny[i];
				if (!results.ContainsKey(text))
				{
					Smart.Log.Debug(TAG, "Tell Response doesn't contain key " + text);
					continue;
				}
				string text2 = results[text];
				Smart.Log.Debug(TAG, $"Response of {text} is {text2}");
				Result val = (Result)1;
				string text3 = array[i].Trim();
				int num;
				if (text3 != "IGNORE")
				{
					if (!text3.StartsWith("!"))
					{
						num = 0;
						if (num == 0)
						{
							goto IL_0238;
						}
					}
					else
					{
						num = 1;
					}
					text3 = text3.Substring(1);
					goto IL_0238;
				}
				val = (Result)8;
				goto IL_0279;
				IL_0279:
				list.Add(val);
				continue;
				IL_0238:
				Smart.Log.Debug(TAG, "check phone response contain expected...");
				val = ((!text2.Trim().Contains(text3)) ? ((Result)1) : ((Result)8));
				if (num != 0)
				{
					val = (((int)val != 8) ? ((Result)8) : ((Result)1));
				}
				goto IL_0279;
			}
			if (!list.Any((Result subResult) => (int)subResult == 8))
			{
				Smart.Log.Debug(TAG, "none of the phone response contain expected, fail the test");
				return (Result)1;
			}
			return (Result)8;
		}
		return (Result)8;
	}
}
