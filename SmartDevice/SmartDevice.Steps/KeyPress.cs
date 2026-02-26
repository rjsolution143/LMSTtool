using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class KeyPress : CommServerStep
{
	private List<string> keysDetected = new List<string>();

	private List<string> expected = new List<string>();

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		keysDetected = new List<string>();
		expected = new List<string>();
		foreach (object item in ((dynamic)base.Info.Args).KeyCodes)
		{
			string text = (string)(dynamic)item;
			expected.Add(text + "00");
			expected.Add(text + "01");
		}
		string command = ((dynamic)base.Info.Args).StartCommand;
		Tell(command);
		string type = ((dynamic)base.Info.Args).PromptType;
		string text2 = ((dynamic)base.Info.Args).PromptText;
		text2 = Smart.Locale.Xlate(text2);
		Prompt(type, text2);
		double value = ((dynamic)base.Info.Args).Timeout;
		Result result = Smart.Thread.Wait<Result>(TimeSpan.FromSeconds(value), (Checker<Result>)VerifyKeys, (Result)8);
		string command2 = ((dynamic)base.Info.Args).StopCommand;
		Tell(command2);
		Smart.Log.Debug(TAG, $"Expected {expected.Count} key actions, detected {keysDetected.Count} key actions");
		LogResult(result);
	}

	private Result VerifyKeys()
	{
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		string command = ((dynamic)base.Info.Args).ResultCommand;
		SortedList<string, string> sortedList = Tell(command);
		if (sortedList.ContainsKey("KEYS"))
		{
			string[] array = sortedList["KEYS"].Split(new char[1] { ',' });
			foreach (string text in array)
			{
				Smart.Log.Assert(TAG, text.Length == 4, "Key action string should be 4 digits: " + text);
				Smart.Log.Assert(TAG, expected.Contains(text), $"Key code {text} should be in the key list");
				if (!keysDetected.Contains(text))
				{
					keysDetected.Add(text);
				}
			}
			if (keysDetected.Count >= expected.Count)
			{
				foreach (string item in expected)
				{
					if (!keysDetected.Contains(item))
					{
						return (Result)1;
					}
				}
				return (Result)8;
			}
			return (Result)1;
		}
		return (Result)1;
	}
}
