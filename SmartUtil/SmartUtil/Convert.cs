using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ISmart;

namespace SmartUtil;

public class Convert : IConvert
{
	private const string MEID_IMEI = "^(99)([0-9]{12,13})$";

	private const string IMEI = "^[0-9]{14,15}$";

	private const string MEID = "^[0-9A-Fa-f]{14,15}$";

	private const string MSN = "^[0-9A-Za-z]{8,10}$";

	private const string HSN = "^[0-9A-Za-z]{13}$";

	private const string UUID = "^[0-9A-Fa-f]{32}$";

	private string TAG => GetType().FullName;

	public string BytesToHex(byte[] bytes)
	{
		return BitConverter.ToString(bytes).Replace("-", string.Empty);
	}

	public byte[] HexToBytes(string hex)
	{
		if (hex.Length == 1)
		{
			hex = "0" + hex;
		}
		int length = hex.Length;
		byte[] array = new byte[length / 2];
		for (int i = 0; i < length; i += 2)
		{
			array[i / 2] = System.Convert.ToByte(hex.Substring(i, 2), 16);
		}
		return array;
	}

	public byte[] LongToBytes(long value)
	{
		return ValueToBytes(value, 8);
	}

	public byte[] IntToBytes(int value)
	{
		return ValueToBytes((uint)value, 4);
	}

	public byte[] UShortToBytes(ushort value)
	{
		return ValueToBytes(value, 2);
	}

	public long BytesToLong(byte[] value)
	{
		return BytesToValue(value, 8);
	}

	public int BytesToInt(byte[] value)
	{
		return (int)BytesToValue(value, 4);
	}

	public ushort BytesToUShort(byte[] value)
	{
		return (ushort)BytesToValue(value, 2);
	}

	private byte[] ValueToBytes(long value, int expectedBytes)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse((Array)bytes);
		}
		for (int i = 0; i < bytes.Length - expectedBytes; i++)
		{
			if (bytes[i] != 0)
			{
				throw new OverflowException($"Value size of {value} is larger than {expectedBytes} bytes");
			}
		}
		byte[] array = new byte[expectedBytes];
		Array.Copy(bytes, bytes.Length - expectedBytes, array, 0, expectedBytes);
		return array;
	}

	private long BytesToValue(byte[] value, int expectedBytes)
	{
		if (value.Length > expectedBytes)
		{
			for (int i = 0; i < value.Length - expectedBytes; i++)
			{
				if (value[i] != 0)
				{
					throw new OverflowException($"Value size of {value.Length} is larger than {expectedBytes} bytes");
				}
			}
		}
		else if (value.Length < expectedBytes)
		{
			expectedBytes = value.Length;
		}
		byte[] array = new byte[8];
		Array.Copy(value, value.Length - expectedBytes, array, array.Length - expectedBytes, expectedBytes);
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse((Array)array);
		}
		return BitConverter.ToInt64(array, 0);
	}

	public byte[] AsciiToBytes(string asciiString)
	{
		return Encoding.UTF8.GetBytes(asciiString);
	}

	public string BytesToAscii(byte[] asciiBytes)
	{
		return Encoding.UTF8.GetString(asciiBytes);
	}

	public byte[] Base64ToBytes(string base64)
	{
		return System.Convert.FromBase64String(base64);
	}

	public string BytesToBase64(byte[] bytes)
	{
		return System.Convert.ToBase64String(bytes);
	}

	public string LongToBase26(long value)
	{
		int num = 65;
		List<char> list = new List<char>();
		do
		{
			char item = (char)((int)(value % 26) + num);
			list.Insert(0, item);
			value /= 26;
		}
		while (value >= 1);
		return new string(list.ToArray());
	}

	public long Base26ToLong(string base26)
	{
		int num = 65;
		long num2 = 0L;
		for (int i = 0; i < base26.Length; i++)
		{
			int num3 = base26[base26.Length - 1 - i];
			num3 -= num;
			int num4 = (int)Math.Pow(26.0, i);
			num2 += num3 * num4;
		}
		return num2;
	}

	public byte[] StreamToBytes(Stream stream)
	{
		using MemoryStream memoryStream = new MemoryStream();
		MemoryStream memoryStream2 = memoryStream;
		if (typeof(MemoryStream).IsAssignableFrom(stream.GetType()))
		{
			memoryStream2 = (MemoryStream)stream;
		}
		else
		{
			Smart.File.CopyStream(stream, (Stream)memoryStream2);
		}
		return memoryStream2.ToArray();
	}

	public Stream BytesToStream(byte[] bytes)
	{
		return new MemoryStream(bytes);
	}

	public List<EnumType> EnumToValues<EnumType>() where EnumType : struct, IConvertible
	{
		List<EnumType> list = new List<EnumType>();
		foreach (object value in Enum.GetValues(typeof(EnumType)))
		{
			list.Add((EnumType)value);
		}
		return list;
	}

	public string TimeSpanToDisplay(TimeSpan time)
	{
		string text = " years";
		string text2 = " year";
		string text3 = " days";
		string text4 = " day";
		string text5 = " hours";
		string text6 = " hour";
		string text7 = " minutes";
		string text8 = " minute";
		string text9 = " seconds";
		string text10 = " second";
		string text11 = " milliseconds";
		string text12 = " millisecond";
		int num = time.Days / 365;
		int num2 = time.Days % 365;
		string text13 = string.Empty;
		if (num > 0)
		{
			text13 += num;
			text13 = ((num <= 1) ? (text13 + text2) : (text13 + text));
			text13 += " ";
		}
		if (num2 > 0)
		{
			text13 += num2;
			text13 = ((num2 <= 1) ? (text13 + text4) : (text13 + text3));
			text13 += " ";
		}
		if (time.Hours > 0 && num < 1)
		{
			text13 += time.Hours;
			text13 = ((time.Hours <= 1) ? (text13 + text6) : (text13 + text5));
			text13 += " ";
		}
		if (time.Minutes > 0 && time.TotalDays < 1.0)
		{
			text13 += time.Minutes;
			text13 = ((time.Minutes <= 1) ? (text13 + text8) : (text13 + text7));
			text13 += " ";
		}
		if (time.Seconds > 0 && time.TotalMinutes < 5.0)
		{
			text13 += time.Seconds;
			text13 = ((time.Seconds <= 1) ? (text13 + text10) : (text13 + text9));
			text13 += " ";
		}
		if (time.TotalSeconds < 1.0)
		{
			text13 += time.Milliseconds;
			text13 = ((time.Milliseconds <= 1) ? (text13 + text12) : (text13 + text11));
			text13 += " ";
		}
		return text13.TrimEnd(Array.Empty<char>());
	}

	public string ByteSizeToDisplay(long bytes, bool abbreviated = true)
	{
		string[] array = new string[9] { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
		string[] array2 = new string[9] { "bytes", "kilobytes", "megabytes", "gigabytes", "terabytes", "petabytes", "exabytes", "zetabytes", "yottabytes" };
		int num = 0;
		double num2 = bytes;
		while (num2 >= 1024.0 && num < array.Length - 1)
		{
			num++;
			num2 /= 1024.0;
		}
		string text = "{0:0.###}";
		if (num2 > 100.0)
		{
			text = "{0:0.#}";
		}
		else if (num2 > 10.0)
		{
			text = "{0:0.##}";
		}
		text = (abbreviated ? (text + "{1}") : (text + " {2}"));
		return string.Format(text, num2, array[num], array2[num]);
	}

	public int TwosComplement(int data)
	{
		if (data > 32768)
		{
			data -= 65536;
		}
		return data;
	}

	public byte[] ByteSwap(byte[] bytes)
	{
		byte[] array = new byte[bytes.Length];
		Array.Copy(bytes, array, bytes.Length);
		Array.Reverse((Array)array);
		return array;
	}

	public string ToString(string name, IEnumerable<KeyValuePair<string, string>> fields)
	{
		List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
		foreach (KeyValuePair<string, string> field in fields)
		{
			KeyValuePair<string, object> item = new KeyValuePair<string, object>(field.Key, field.Value);
			list.Add(item);
		}
		return ToString(name, list);
	}

	public string ToString(string name, IEnumerable<KeyValuePair<string, object>> fields)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(name);
		foreach (KeyValuePair<string, object> field in fields)
		{
			stringBuilder.Append("    ");
			stringBuilder.Append(field.Key);
			stringBuilder.Append(": ");
			if (field.Value != null)
			{
				stringBuilder.AppendLine(field.Value.ToString());
			}
			else
			{
				stringBuilder.AppendLine("[NULL]");
			}
		}
		return stringBuilder.ToString();
	}

	public string ToCommaSeparated(IEnumerable list)
	{
		string text = string.Empty;
		foreach (object item in list)
		{
			text = text + item.ToString() + ",";
		}
		return text.TrimEnd(new char[1] { ',' });
	}

	public string ToStatusText(string message, double percentageComplete)
	{
		string result = message;
		if (percentageComplete > 0.0 && percentageComplete < 100.0)
		{
			int num = (int)Math.Round(percentageComplete);
			result = $"{message} {num}% complete";
		}
		return result;
	}

	public bool IsSerialNumberValid(string serialNumber, SerialNumberType expectedType = 0)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		SerialNumberType val = ToSerialNumberType(serialNumber);
		if ((int)val == 0)
		{
			return false;
		}
		if ((int)expectedType != 0)
		{
			if ((int)expectedType == 1 || (int)expectedType == 2)
			{
				if ((int)val != 1 && (int)val != 2)
				{
					return false;
				}
			}
			else if (expectedType != val)
			{
				return false;
			}
		}
		return serialNumber.ToLowerInvariant() == CalculateCheckDigit(serialNumber).ToLowerInvariant();
	}

	public string CalculateCheckDigit(string serialNumber)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Invalid comparison between Unknown and I4
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Invalid comparison between Unknown and I4
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Invalid comparison between Unknown and I4
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Invalid comparison between Unknown and I4
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		serialNumber = serialNumber.Trim();
		SerialNumberType val = ToSerialNumberType(serialNumber);
		if ((int)val == 1 || RegMatch(serialNumber, "^(99)([0-9]{12,13})$"))
		{
			Smart.Log.Assert(TAG, serialNumber.Length == 14 || serialNumber.Length == 15, "IMEI length should be 14 (no check digit) or 15 (with check digit)");
			string text = serialNumber;
			if (text.Length > 14)
			{
				text = text.Substring(0, 14);
			}
			int num = 0;
			List<char> list = new List<char>();
			for (int i = 1; i < text.Length; i += 2)
			{
				string text2 = (int.Parse(text[i].ToString()) * 2).ToString();
				list.AddRange(text2.ToCharArray());
			}
			for (int j = 0; j < text.Length; j += 2)
			{
				list.Add(text[j]);
			}
			foreach (char item3 in list)
			{
				num += int.Parse(item3.ToString());
			}
			num %= 10;
			if (num != 0)
			{
				num = 10 - num;
			}
			return text + num;
		}
		if ((int)val == 2)
		{
			Smart.Log.Assert(TAG, serialNumber.Length == 14 || serialNumber.Length == 15, "MEID length should be 14 (no check digit) or 15 (with check digit)");
			char[] array = serialNumber.ToCharArray(0, 14);
			List<char> list2 = new List<char>();
			for (int k = 1; k <= array.Length; k++)
			{
				char item = array[k - 1];
				if (k % 2 == 0)
				{
					byte b = byte.Parse(item.ToString(), NumberStyles.HexNumber);
					char[] array2 = ((byte)(b + b)).ToString("X").ToCharArray();
					foreach (char item2 in array2)
					{
						list2.Add(item2);
					}
				}
				else
				{
					list2.Add(item);
				}
			}
			int num2 = 0;
			foreach (char item4 in list2)
			{
				num2 += byte.Parse(item4.ToString(), NumberStyles.HexNumber);
			}
			int num3 = num2 % 16;
			if (num3 != 0)
			{
				num3 = 16 - num3;
			}
			string text3 = num3.ToString("X");
			Smart.Log.Assert(TAG, text3.Length == 1, "Check digit should be 1 character");
			return new string(array) + text3;
		}
		if ((int)val == 3 || (int)val == 4 || (int)val == 5)
		{
			return serialNumber;
		}
		throw new NotSupportedException(string.Concat("Check digit calculation not supported for ", val, " ('", serialNumber, "')"));
	}

	public SerialNumberType ToSerialNumberType(string serialNumber)
	{
		if (!RegMatch(serialNumber, "^(99)([0-9]{12,13})$"))
		{
			if (!RegMatch(serialNumber, "^[0-9]{14,15}$"))
			{
				if (!RegMatch(serialNumber, "^[0-9A-Fa-f]{14,15}$"))
				{
					if (!RegMatch(serialNumber, "^[0-9A-Za-z]{8,10}$"))
					{
						if (!RegMatch(serialNumber, "^[0-9A-Za-z]{13}$"))
						{
							if (!RegMatch(serialNumber, "^[0-9A-Fa-f]{32}$"))
							{
								return (SerialNumberType)0;
							}
							return (SerialNumberType)5;
						}
						return (SerialNumberType)4;
					}
					return (SerialNumberType)3;
				}
				return (SerialNumberType)2;
			}
			return (SerialNumberType)1;
		}
		return (SerialNumberType)2;
	}

	private bool RegMatch(string input, string pattern)
	{
		return Regex.IsMatch(input, pattern);
	}

	public string ToPEsn(string meid)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		string result = "00000000";
		if (string.IsNullOrEmpty(meid.Trim()))
		{
			return result;
		}
		if ((int)ToSerialNumberType(meid) != 2)
		{
			throw new FormatException("Not a valid MEID: " + meid);
		}
		meid = CalculateCheckDigit(meid);
		meid = meid.Substring(0, meid.Length - 1);
		SHA1Managed sHA1Managed = new SHA1Managed();
		byte[] buffer = HexToBytes(meid);
		buffer = sHA1Managed.ComputeHash(buffer);
		byte[] array = new byte[3];
		Array.Copy(buffer, buffer.Length - array.Length, array, 0, array.Length);
		return "80" + BytesToHex(array).ToUpperInvariant();
	}

	public string GenerateCode(string serialNumber, bool validated)
	{
		if (!serialNumber.All(char.IsDigit))
		{
			return string.Empty;
		}
		return CodeGen.EncryptImei(DateTime.Now, serialNumber, validated);
	}

	public string AsciiStringToUnicodeHexString(string ascii, ByteOrder byteOrder)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Invalid comparison between Unknown and I4
		StringBuilder stringBuilder = new StringBuilder();
		byte[] bytes = Encoding.Unicode.GetBytes(ascii);
		for (int i = 0; i < bytes.Length; i += 2)
		{
			if ((int)byteOrder == 0)
			{
				stringBuilder.Append($"{bytes[i]:X2}");
				stringBuilder.Append($"{bytes[i + 1]:X2}");
			}
			else if ((int)byteOrder == 1)
			{
				stringBuilder.Append($"{bytes[i + 1]:X2}");
				stringBuilder.Append($"{bytes[i]:X2}");
			}
		}
		return stringBuilder.ToString();
	}

	public string AsciiStringToHexString(string ascii)
	{
		StringBuilder stringBuilder = new StringBuilder();
		byte[] bytes = Encoding.UTF8.GetBytes(ascii);
		foreach (byte b in bytes)
		{
			stringBuilder.Append($"{b:X2}");
		}
		return stringBuilder.ToString();
	}

	public string AsciiBytesToHexString(byte[] inputBytes, int nNumberOfBytes)
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		foreach (byte b in inputBytes)
		{
			stringBuilder.Append($"{b:X2}");
			if (++num >= nNumberOfBytes)
			{
				break;
			}
		}
		return stringBuilder.ToString();
	}

	public string HexStringToAsciiString(string hexString)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i <= hexString.Length - 2; i += 2)
		{
			stringBuilder.Append(System.Convert.ToString(System.Convert.ToChar(int.Parse(hexString.Substring(i, 2), NumberStyles.HexNumber))));
		}
		return stringBuilder.ToString();
	}

	public string DecSerialToHex(string serialNumber)
	{
		string result = serialNumber;
		if (serialNumber.Length == 18)
		{
			string serialNumber2 = long.Parse(serialNumber.Substring(0, 10)).ToString("X8") + long.Parse(serialNumber.Substring(10, 8)).ToString("X6");
			result = CalculateCheckDigit(serialNumber2);
		}
		return result;
	}

	public bool ToBool(DialogResult result)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected I4, but got Unknown
		switch ((int)result)
		{
		case 1:
		case 5:
		case 6:
			return true;
		default:
			return false;
		}
	}

	public string AtToTCMD(string atCommand)
	{
		if (atCommand == null)
		{
			return string.Empty;
		}
		string empty = string.Empty;
		StringBuilder stringBuilder = new StringBuilder();
		char[] array = atCommand.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			int num = System.Convert.ToInt32(array[i]);
			stringBuilder.AppendFormat("{0:X2}", num);
		}
		empty = stringBuilder.ToString();
		int num2 = 0;
		num2 = ((atCommand.IndexOf("=") < 0) ? atCommand.Count() : atCommand.Split(new char[1] { '=' })[0].Count());
		return $"{num2:X2}" + empty + "00";
	}

	public string HexToOddOrEven(string hex, bool toEven)
	{
		byte b = HexToBytes(hex.Last().ToString())[0];
		bool flag = b % 2 < 1;
		if ((flag && toEven) || (!flag && !toEven))
		{
			return hex;
		}
		b = (byte)((b + 1) % 16);
		string text = BytesToHex(new byte[1] { b });
		return hex.Substring(0, hex.Length - 1) + text.Substring(1);
	}

	public List<string> Unformat(string format, string text)
	{
		string text2 = Regex.Escape(format);
		int num = 0;
		string newValue = "(.+)";
		string text3 = text2;
		while (true)
		{
			string text4 = "\\{" + num + "}";
			if (!text2.Contains(text4))
			{
				break;
			}
			text3 = text3.Replace(text4, newValue);
			num++;
		}
		RegexOptions options = RegexOptions.IgnoreCase;
		List<string> list = new List<string>();
		if (!Regex.IsMatch(text, text3, options))
		{
			return list;
		}
		Match match = Regex.Match(text, text3, options);
		bool flag = false;
		foreach (Group group in match.Groups)
		{
			if (!flag)
			{
				flag = true;
			}
			else
			{
				list.Add(group.Value);
			}
		}
		return list;
	}

	public DateTime ServerToLocalTime(string timestamp)
	{
		if (timestamp.Contains("."))
		{
			timestamp = timestamp.Substring(0, timestamp.IndexOf("."));
		}
		string format = "yyyy-MM-dd HH:mm:ss";
		DateTime dateTime = DateTime.ParseExact(timestamp, format, CultureInfo.InvariantCulture);
		TimeZoneInfo sourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
		return TimeZoneInfo.ConvertTimeFromUtc(TimeZoneInfo.ConvertTimeToUtc(dateTime, sourceTimeZone), TimeZoneInfo.Local);
	}

	public string QuoteFilePathName(string filePathName)
	{
		string result = filePathName;
		if (filePathName.Contains(" "))
		{
			result = "\"" + filePathName + "\"";
		}
		return result;
	}

	public string GetDeviceSerialNumber(IDevice device)
	{
		if (device.Group == "LST" && !device.WiFiOnlyDevice)
		{
			return device.GetLogInfoValue("IMEI");
		}
		return device.SerialNumber;
	}
}
