using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SmartRsd;

public class ModelPropertyBase<T> where T : IComparable
{
	protected Dictionary<string, List<T>> mModelToProperties;

	protected int mOffset;

	protected const string RSD_CARRIER = "rsdcarrier";

	protected int mRsdCarrierIndex;

	protected static bool FormatMatched(string csvFilePathName, int skipEmptyOn, Dictionary<int, string> formats)
	{
		bool result = false;
		if (File.Exists(csvFilePathName))
		{
			string[] array = File.ReadAllText(csvFilePathName, Encoding.UTF8).Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			List<string> list = new List<string>();
			int num = 0;
			int index = 0;
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				list = ParseLine(array2[i].Replace('\n', ' '));
				if (FindFieldName(list, "rsdcarrier", out index))
				{
					num = 1;
				}
				if (list[skipEmptyOn + ((skipEmptyOn >= index) ? num : 0)].Trim() != string.Empty)
				{
					break;
				}
			}
			if (list.Count == 0)
			{
				Smart.Log.Debug("FormatMatched", $"File {csvFilePathName} is empty");
				return false;
			}
			result = true;
			foreach (int key in formats.Keys)
			{
				int num2 = key + ((key >= index) ? num : 0);
				if (num2 < list.Count)
				{
					if (string.Compare(list[num2].Trim().Replace(" ", string.Empty), formats[key], ignoreCase: true) != 0)
					{
						result = false;
						break;
					}
					continue;
				}
				result = false;
				break;
			}
		}
		return result;
	}

	protected ModelPropertyBase(string csvFilePathName)
	{
		mModelToProperties = new Dictionary<string, List<T>>();
		if (File.Exists(csvFilePathName))
		{
			string[] lines = File.ReadAllText(csvFilePathName, Encoding.UTF8).Split(new string[1] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			Parse(lines);
		}
	}

	public List<string> GetCarrierParameters(string phoneModel, string rsdCarrier)
	{
		List<string> list = new List<string>();
		Smart.Log.Debug("GetCarrierParameters", $"phoneModel {phoneModel} rsdCarrier {rsdCarrier}");
		if (mModelToProperties.ContainsKey(phoneModel))
		{
			List<T> list2 = mModelToProperties[phoneModel];
			Smart.Log.Debug("GetCarrierParameters", $"propertyList.Count = {list2.Count}");
			for (int i = 0; i < list2.Count; i++)
			{
				string text = BuildCarrierString(list2[i], i, rsdCarrier);
				if (text != string.Empty)
				{
					list.Add(text);
				}
			}
		}
		else
		{
			Smart.Log.Debug("GetCarrierParameters", $"phoneModel: {phoneModel} is not in mModelToProperties list");
		}
		Smart.Log.Debug("GetCarrierParameters", $"Found {list.Count} carrier.");
		return list;
	}

	public T GetProperties(string phoneModel, int index)
	{
		T result = default(T);
		if (mModelToProperties.ContainsKey(phoneModel))
		{
			List<T> list = mModelToProperties[phoneModel];
			if (index < list.Count)
			{
				return list[index];
			}
		}
		return result;
	}

	private void Parse(string[] lines)
	{
		mModelToProperties.Clear();
		for (int i = 0; i < lines.Length; i++)
		{
			List<string> fields = ParseLine(lines[i].Replace('\n', ' '));
			AddProperty(fields);
		}
		foreach (string key in mModelToProperties.Keys)
		{
			mModelToProperties[key].Sort();
		}
	}

	protected static List<string> ParseLine(string csvLine)
	{
		char[] array = csvLine.ToCharArray();
		List<string> list = new List<string>();
		char c = ',';
		char c2 = '"';
		bool flag = false;
		List<char> list2 = new List<char>();
		for (int i = 0; i < array.Length; i++)
		{
			char c3 = array[i];
			if (!flag && c3 == c2)
			{
				flag = true;
			}
			else if (flag && i < array.Length - 1 && c3 == c2 && array[i + 1] == c2)
			{
				list2.Add(c3);
				i++;
			}
			else if (flag && c3 == c2)
			{
				flag = false;
			}
			else if (!flag && c3 == c)
			{
				list.Add(new string(list2.ToArray()));
				list2 = new List<char>();
			}
			else
			{
				list2.Add(c3);
			}
		}
		list.Add(new string(list2.ToArray()));
		return list;
	}

	protected virtual void AddProperty(List<string> fields)
	{
		throw new NotImplementedException();
	}

	protected virtual string BuildCarrierString(T property, int i, string rsdCarrier)
	{
		throw new NotImplementedException();
	}

	protected static bool FindFieldName(List<string> fields, string fieldName, out int index)
	{
		index = 0;
		bool result = false;
		string text = fieldName.Replace(" ", string.Empty).ToLower();
		for (int i = 0; i < fields.Count; i++)
		{
			if (fields[i].Replace(" ", string.Empty).ToLower() == text)
			{
				index = i;
				result = true;
				break;
			}
		}
		return result;
	}
}
