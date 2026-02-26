using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ISmart;

namespace SmartUtil;

public class CsvFile : ICsvFile, IEnumerable<SortedList<string, string>>, IEnumerable
{
	private List<string> headers = new List<string>();

	private List<List<string>> rows = new List<List<string>>();

	private char comma = ',';

	private string TAG => GetType().FullName;

	public List<string> Headers
	{
		get
		{
			return headers;
		}
		set
		{
			headers = value;
		}
	}

	public List<List<string>> Rows
	{
		get
		{
			return rows;
		}
		private set
		{
			rows = value;
		}
	}

	public SortedList<string, string> this[int index]
	{
		get
		{
			SortedList<string, string> sortedList = new SortedList<string, string>();
			List<string> list = Rows[index];
			Smart.Log.Assert(TAG, list.Count <= headers.Count, "Row and header count should be the same");
			foreach (int item in Enumerable.Range(0, headers.Count))
			{
				if (item >= list.Count)
				{
					sortedList[headers[item]] = string.Empty;
				}
				else
				{
					sortedList[headers[item]] = list[item];
				}
			}
			return sortedList;
		}
	}

	public void LoadFile(string fileName, char separator = ',')
	{
		Load(Smart.File.ReadText(fileName), separator);
	}

	public void Load(string csvText, char separator = ',')
	{
		comma = separator;
		string[] array = csvText.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string csvLine in array)
		{
			List<string> item = ParseLine(csvLine);
			if (headers.Count == 0)
			{
				headers = item;
			}
			rows.Add(item);
		}
	}

	private List<string> ParseLine(string csvLine)
	{
		char[] array = csvLine.ToCharArray();
		List<string> list = new List<string>();
		char c = '"';
		bool flag = false;
		List<char> list2 = new List<char>();
		for (int i = 0; i < array.Length; i++)
		{
			char c2 = array[i];
			if (!flag && c2 == c)
			{
				flag = true;
			}
			else if (flag && i < array.Length - 1 && c2 == c && array[i + 1] == c)
			{
				list2.Add(c2);
				i++;
			}
			else if (flag && c2 == c)
			{
				flag = false;
			}
			else if (!flag && c2 == comma)
			{
				list.Add(new string(list2.ToArray()));
				list2 = new List<char>();
			}
			else
			{
				list2.Add(c2);
			}
		}
		list.Add(new string(list2.ToArray()));
		return list;
	}

	public void SaveFile(string fileName)
	{
		SaveFile(fileName, writeHeaders: true);
	}

	public void SaveFile(string fileName, bool writeHeaders)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (writeHeaders)
		{
			foreach (string header in Headers)
			{
				stringBuilder.Append(EscapeAndQuote(header));
				stringBuilder.Append(comma);
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.AppendLine();
		}
		foreach (List<string> row in Rows)
		{
			foreach (string item in row)
			{
				stringBuilder.Append(EscapeAndQuote(item));
				stringBuilder.Append(comma);
			}
			if (row.Count > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			stringBuilder.AppendLine();
		}
		Smart.File.WriteText(fileName, stringBuilder.ToString());
	}

	private string EscapeAndQuote(string field)
	{
		char value = '"';
		field = field.Replace(value.ToString(), value.ToString() + value);
		if (Enumerable.Contains(field, value) || Enumerable.Contains(field, comma))
		{
			field = value + field + value;
		}
		return field;
	}

	public IEnumerator<SortedList<string, string>> GetEnumerator()
	{
		for (int i = 0; i < Rows.Count; i++)
		{
			yield return this[i];
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
