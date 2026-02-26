using System.Collections.Generic;
using System.Text.RegularExpressions;
using ISmart;

namespace SmartDevice.Steps;

public class CheckFirmware : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (!val.Log.Info.ContainsKey("FlashId"))
		{
			LogResult((Result)7, "No FlashId read for connected device");
			return;
		}
		string text = val.Log.Info["FlashId"];
		string modelId = val.ModelId;
		string latestFlashId = Smart.Rsd.GetLatestFlashId(modelId);
		Smart.Log.Debug(TAG, $"FlashID: '{text}', TargetID: '{latestFlashId}'");
		string androidVersion = string.Empty;
		string androidVersion2 = string.Empty;
		string text2 = ParseVersion(text, out androidVersion);
		string text3 = ParseVersion(latestFlashId, out androidVersion2);
		if (text2 == string.Empty)
		{
			LogResult((Result)7, "Could not parse device Flash ID", text);
			Smart.Log.Debug(TAG, $"Could not parse device Flash ID: '{text}'");
			return;
		}
		if (text3 == string.Empty)
		{
			LogResult((Result)7, "Could not parse target Flash ID", latestFlashId);
			Smart.Log.Debug(TAG, $"Could not parse target Flash ID: '{latestFlashId}'");
			return;
		}
		bool flag = false;
		bool flag2 = false;
		if (androidVersion.Length == 1 && androidVersion2.Length == 1)
		{
			char c = androidVersion.ToUpperInvariant()[0];
			char num = androidVersion2.ToUpperInvariant()[0];
			flag = num < c;
			flag2 = num > c;
		}
		else
		{
			int num2 = string.Compare(androidVersion2.ToUpperInvariant(), androidVersion.ToUpperInvariant());
			if (num2 < 0)
			{
				flag = true;
			}
			else if (num2 > 0)
			{
				flag2 = true;
			}
		}
		if (flag)
		{
			LogResult((Result)1, "Target Android version is downgrade from current Android version", $"Android '{androidVersion2}' downgrading from Android '{androidVersion}'");
			Smart.Log.Debug(TAG, $"Android '{androidVersion2}' downgrading from Android '{androidVersion}'");
			return;
		}
		if (flag2)
		{
			LogResult((Result)8, "Target Android version is upgrade from current Android version", $"Android '{androidVersion2}' upgrading from Android '{androidVersion}'");
			Smart.Log.Debug(TAG, $"Android '{androidVersion2}' upgrading from Android '{androidVersion}'");
			return;
		}
		string alphas = string.Empty;
		string alphas2 = string.Empty;
		List<int> current = PullVersionNumbers(text2, out alphas);
		List<int> target = PullVersionNumbers(text3, out alphas2);
		if (alphas.Trim().Length < 2 + androidVersion.Length)
		{
			LogResult((Result)7, "Device Flash ID is not in correct format", text);
			Smart.Log.Debug(TAG, $"Device Flash ID is not in correct format: '{text}'");
		}
		else if (alphas2.Trim().Length < 2 + androidVersion2.Length)
		{
			LogResult((Result)7, "Target Flash ID is not in correct format", latestFlashId);
			Smart.Log.Debug(TAG, $"Target Flash ID is not in correct format: '{latestFlashId}'");
		}
		else if (alphas.Trim().Substring(androidVersion.Length, 2).ToLowerInvariant() != alphas2.Trim().Substring(androidVersion2.Length, 2).ToLowerInvariant())
		{
			LogResult((Result)7, "Device and target Flash ID cannot be compared", $"'{text}' vs '{latestFlashId}'");
			Smart.Log.Debug(TAG, $"Device version ID '{alphas.Trim().Substring(androidVersion.Length, 2)}' does not match Target version ID {alphas2.Trim().Substring(androidVersion2.Length, 2)}");
		}
		else if (IsDownGrade(current, target))
		{
			LogResult((Result)1, "Target Flash ID is a downgrade from current Flash ID", $"'{latestFlashId}' downgrading from '{text}'");
			Smart.Log.Debug(TAG, "Target version number is a downgrade from Device version number");
		}
		else
		{
			LogPass();
		}
	}

	private bool IsDownGrade(List<int> current, List<int> target)
	{
		int count = current.Count;
		for (int i = 0; i < count; i++)
		{
			if (i >= target.Count)
			{
				return true;
			}
			int num = current[i];
			int num2 = target[i];
			if (num2 < num)
			{
				return true;
			}
			if (num2 > num)
			{
				return false;
			}
		}
		return false;
	}

	private string ParseVersion(string input, out string androidVersion)
	{
		androidVersion = string.Empty;
		Regex regex = new Regex("^(?<androidVersion>([a-z]|[r-z][0-9]))[a-z]{2,4}[0-9]{2,3}\\.[a-z]?[0-9]+[a-z]?([-.][a-z]?[0-9]+)*", RegexOptions.IgnoreCase);
		string[] array = input.Split(new char[1] { ' ' });
		foreach (string text in array)
		{
			if (regex.IsMatch(text))
			{
				androidVersion = regex.Match(text).Groups["androidVersion"].Value;
				return text;
			}
		}
		return string.Empty;
	}

	private List<int> PullVersionNumbers(string parsed, out string alphas)
	{
		List<int> list = new List<int>();
		alphas = string.Empty;
		if (parsed.Length < 2)
		{
			return list;
		}
		char c = parsed[1];
		string text = string.Empty;
		if (char.IsDigit(c))
		{
			text = parsed.Substring(0, 2);
			parsed = parsed.Substring(2);
		}
		List<char> list2 = new List<char>();
		list2.Add('.');
		list2.Add('-');
		string text2 = string.Empty;
		string text3 = parsed;
		for (int i = 0; i < text3.Length; i++)
		{
			char c2 = text3[i];
			if (char.IsLetter(c2))
			{
				alphas += c2;
			}
			else if (char.IsDigit(c2))
			{
				text2 += c2;
			}
			else if (list2.Contains(c2) && text2.Length > 0)
			{
				int item = int.Parse(text2);
				list.Add(item);
				text2 = string.Empty;
			}
		}
		alphas = text + alphas;
		if (text2.Length > 0)
		{
			int item2 = int.Parse(text2);
			list.Add(item2);
			text2 = string.Empty;
		}
		return list;
	}
}
