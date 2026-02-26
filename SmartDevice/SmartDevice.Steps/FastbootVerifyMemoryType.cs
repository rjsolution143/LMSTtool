using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class FastbootVerifyMemoryType : FastbootStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ded: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0daa: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Result result = (Result)8;
		string text = "ufs";
		if (((dynamic)base.Info.Args).Property != null)
		{
			text = ((dynamic)base.Info.Args).Property;
		}
		string text2 = "getvar " + text.ToLower();
		Smart.Log.Debug(TAG, "command: " + text2);
		int num = 3000;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
			num *= 1000;
		}
		string text3 = "WDC";
		if (((dynamic)base.Info.Args).MemoryType != null)
		{
			text3 = ((dynamic)base.Info.Args).MemoryType;
		}
		string text4 = "FV=1268";
		if (((dynamic)base.Info.Args).ExpectedFV != null)
		{
			text4 = ((dynamic)base.Info.Args).ExpectedFV;
		}
		string text5 = string.Empty;
		if (((dynamic)base.Info.Args).ExpectedPC != null)
		{
			text5 = ((dynamic)base.Info.Args).ExpectedPC;
		}
		bool flag = false;
		if (((dynamic)base.Info.Args).CheckOnly != null)
		{
			flag = ((dynamic)base.Info.Args).CheckOnly;
		}
		string text6 = string.Empty;
		if (((dynamic)base.Info.Args).PromptText != null)
		{
			text6 = ((dynamic)base.Info.Args).PromptText.ToString();
		}
		Smart.Log.Debug(TAG, $"Property: {text}; MemoryType: {text3}; ExpectedFV: {text4}; ExpectedPC: {text5}; CheckOnly: {flag}");
		string filePathName = Smart.Rsd.GetFilePathName("fastbootExe", base.Recipe.Info.UseCase, val);
		int num2 = default(int);
		List<string> resps = Smart.MotoAndroid.Shell(val.ID, text2, num, filePathName, ref num2, 6000, false);
		if (num2 == 0)
		{
			Dictionary<string, string> dictionary = ExtractPropertyValue(resps, text);
			if (dictionary.Count > 0)
			{
				bool flag2 = true;
				if (dictionary["Type"] != text3)
				{
					flag2 = false;
				}
				string text7;
				if (ValueMatched(dictionary["FvValue"], text4))
				{
					text7 = dictionary["Type"] + "+" + text4;
				}
				else
				{
					flag2 = false;
					text7 = dictionary["Type"] + "+" + dictionary["FvValue"];
				}
				if (text5 != string.Empty)
				{
					if (dictionary.TryGetValue("PcValue", out var value))
					{
						if (ValueMatched(value, text5))
						{
							text7 = text7 + "+" + text5;
						}
						else
						{
							flag2 = false;
							text7 = text7 + "+" + value;
							if (text6 != string.Empty)
							{
								text6 = Smart.Locale.Xlate(text6);
								string text8 = Smart.Locale.Xlate(base.Info.Name);
								val.Prompt.MessageBox(text8, text6, (MessageBoxButtons)0, (MessageBoxIcon)64);
							}
						}
					}
					else
					{
						flag2 = false;
					}
				}
				if (!flag2)
				{
					result = (Result)1;
				}
				SetPreCondition(text7);
			}
			else
			{
				result = (Result)1;
				SetPreCondition("Not Supported");
			}
		}
		else
		{
			result = (Result)1;
			SetPreCondition("Not Supported");
		}
		if (flag)
		{
			LogPass();
		}
		else
		{
			LogResult(result);
		}
	}

	private Dictionary<string, string> ExtractPropertyValue(List<string> resps, string property)
	{
		string text = string.Empty;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		List<string> list = new List<string>();
		foreach (string resp in resps)
		{
			if (!resp.Contains("(bootloader)") || resp.Contains(property))
			{
				list.Add(resp);
			}
		}
		if (list.Count > 0)
		{
			if (list[0].StartsWith("(bootloader)"))
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].StartsWith("(bootloader)"))
					{
						string text2 = $"(bootloader) {property}[{i}]: ";
						text += list[i].Substring(text2.Length);
					}
				}
				text = text.Trim();
			}
			else
			{
				string text3 = string.Empty;
				if (list[0].StartsWith("< waiting for"))
				{
					if (list.Count > 1)
					{
						text3 = list[1];
					}
				}
				else
				{
					text3 = list[0];
				}
				text = text3.Replace(property + ":", string.Empty).Trim();
			}
			string[] array = text.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length > 3)
			{
				dictionary.Add("Type", array[1]);
				dictionary.Add("FvValue", array[3]);
				Smart.Log.Debug(TAG, $"Type: {array[1]}; FvValue: {array[3]}");
				if (array.Length > 4)
				{
					dictionary.Add("PcValue", array[4]);
					Smart.Log.Debug(TAG, $"PcValue: {array[4]}");
					base.Log.AddInfo("PcValue", array[4]);
				}
			}
		}
		return dictionary;
	}

	private bool ValueMatched(string value, string expectedValue)
	{
		string[] array = value.Split(new char[1] { '=' });
		int num = int.Parse(array[1]);
		expectedValue = expectedValue.Replace(" ", string.Empty);
		if (string.Compare(array[0], expectedValue.Substring(0, array[0].Length), ignoreCase: true) != 0)
		{
			return false;
		}
		if (expectedValue.Length == array[0].Length)
		{
			return true;
		}
		List<char> list = new List<char>();
		int minOpIndex = array[0].Length - 1;
		int maxOpIndex = array[0].Length + 2;
		ParseOperator(expectedValue, '=', maxOpIndex, minOpIndex, list);
		ParseOperator(expectedValue, '>', maxOpIndex, minOpIndex, list);
		ParseOperator(expectedValue, '<', maxOpIndex, minOpIndex, list);
		if (list.Count == 0)
		{
			string text = $"Invalid \"{expectedValue}\" - no operator specified";
			Smart.Log.Error(TAG, text);
			throw new FormatException(text);
		}
		if (list.Contains('<') && list.Contains('>'))
		{
			string text2 = $"Invalid \"{expectedValue}\" - operators > and < cannot be specified at the same time";
			Smart.Log.Error(TAG, text2);
			throw new FormatException(text2);
		}
		string text3 = expectedValue.Substring(array[0].Length + list.Count);
		int num2;
		try
		{
			num2 = int.Parse(text3);
		}
		catch (Exception)
		{
			Smart.Log.Error(TAG, $"Invalid \"{expectedValue}\" - parsing {text3} failed");
			throw;
		}
		foreach (char item in list)
		{
			if ((item == '=' && num == num2) || (item == '<' && num < num2) || (item == '>' && num > num2))
			{
				Smart.Log.Debug(TAG, $"Match found at {num} {item} {num2}");
				return true;
			}
		}
		return false;
	}

	private void ParseOperator(string value, char op, int maxOpIndex, int minOpIndex, List<char> operators)
	{
		int num = value.IndexOf(op);
		if (num >= 0)
		{
			if (num <= minOpIndex || num >= maxOpIndex)
			{
				string text = $"Invalid \"{value}\" - operator {op} is in wrong position";
				Smart.Log.Error(TAG, text);
				throw new FormatException(text);
			}
			operators.Add(op);
		}
	}
}
