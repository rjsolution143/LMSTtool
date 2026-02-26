using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SmartUtil;

public class CodeGen
{
	private static string _TimeZone;

	private static readonly string BaseTable = "Z123456789ABCDEFGHJKLMNPQRSTUVWXY0";

	private static readonly DateTime StandardTime = DateTime.Parse("2016-11-28 00:00:00");

	private static Random rd = new Random(DateTime.Now.Millisecond);

	private static string TAG => "CodeGen";

	private static string TimeZone
	{
		get
		{
			TimeZoneInfo local = TimeZoneInfo.Local;
			_TimeZone = new Regex("^(\\(.+\\)).+$").Match(local.DisplayName).Groups[1].ToString();
			_TimeZone = _TimeZone.Substring(1, _TimeZone.Length - 2);
			return _TimeZone;
		}
	}

	public static string EncryptImei(DateTime dateTime, string imei, bool high)
	{
		return Encrypt(dateTime, imei, imei: true, high);
	}

	public static string EncryptSn(DateTime dateTime, string sn)
	{
		return Encrypt(dateTime, sn, imei: false, high: false);
	}

	private static string Encrypt(DateTime dateTime, string source, bool imei, bool high)
	{
		source = source.Trim();
		source = ((!imei) ? ConvertAsciiString(source.ToUpper()) : source.PadLeft(16, '0'));
		source = ConvertDateTime(dateTime) + source;
		string source2 = ConvertBaseN(source).PadLeft(17, 'Z');
		source2 = Garble(source2);
		int num = GenerateCheckDigit(source2);
		source2 = ((num != 0) ? (num + source2) : ("X" + source2));
		string value = Rando(!(source.Substring(source.Length - 16, 1) == "0"), high);
		return source2.Insert(++num, value);
	}

	private static string ConvertDateTime(DateTime dateTime)
	{
		DateTime dateTime2 = dateTime;
		string text = dateTime2.Subtract(StandardTime).Days.ToString();
		string text2 = dateTime2.ToString("HHmmss");
		return text + text2;
	}

	private static string ConvertAsciiString(string source)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(source);
		string text = string.Empty;
		byte[] array = bytes;
		foreach (byte b in array)
		{
			string text2 = text;
			int num = b;
			text = text2 + num;
		}
		return text;
	}

	private static string ConvertBaseN(string sourceValue)
	{
		return ConvertBaseN(sourceValue, BaseTable);
	}

	private static string ConvertBaseN(string sourceValue, string newBaseChars)
	{
		int length = newBaseChars.Length;
		List<int> list = new List<int>();
		list.AddRange(sourceValue.Select((char n) => int.Parse(n.ToString())));
		Stack<int> stack = new Stack<int>();
		while ((list.Count != 1 || list[0] >= length) && list.Count != 0)
		{
			List<int> list2 = new List<int>();
			int num = 0;
			foreach (int item in list)
			{
				num = num * 10 + item;
				list2.Add(num / length);
				num %= length;
			}
			stack.Push(num);
			bool flag = false;
			list.Clear();
			foreach (int item2 in list2.Where((int a) => a != 0 || flag))
			{
				flag = true;
				list.Add(item2);
			}
		}
		if (list.Count > 0)
		{
			stack.Push(list[0]);
		}
		StringBuilder stringBuilder = new StringBuilder(stack.Count);
		while (stack.Count > 0)
		{
			stringBuilder.Append(newBaseChars[stack.Pop()]);
		}
		return stringBuilder.ToString();
	}

	private static string Garble(string source)
	{
		int length = source.Length;
		char[] array = source.ToCharArray();
		for (int i = 0; i < length / 4; i++)
		{
			if (array[i * 2] != array[length - i * 2 - 1])
			{
				char c = ' ';
				c = array[i * 2];
				array[i * 2] = array[length - i * 2 - 1];
				array[length - i * 2 - 1] = c;
			}
		}
		return new string(array);
	}

	private static int GenerateCheckDigit(string source)
	{
		char[] array = source.ToCharArray();
		StringBuilder stringBuilder = new StringBuilder(array.Length / 2);
		for (int i = 0; i < array.Length / 2; i++)
		{
			stringBuilder.Append(array[2 * i + 1].ToString());
		}
		IEnumerable<int> enumerable = from n in stringBuilder.ToString()
			select BaseTable.IndexOf(n);
		foreach (int item in enumerable)
		{
			_ = item;
		}
		return enumerable.Sum() % 10;
	}

	private static string Rando(bool odd, bool high)
	{
		int num = rd.Next(0, 10);
		if (high)
		{
			if (num < 5)
			{
				num = 9 - num;
			}
		}
		else if (num > 4)
		{
			num -= 5;
		}
		if (odd)
		{
			if (num % 2 == 0)
			{
				num++;
			}
			if (!high && num > 4)
			{
				num = 3;
			}
		}
		else
		{
			if (num % 2 == 1)
			{
				num--;
			}
			if (high && num < 5)
			{
				num = 6;
			}
		}
		return num.ToString();
	}
}
