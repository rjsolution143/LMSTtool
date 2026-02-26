using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SmartDevice.Cfc;

public class ProgramRSUKey : BaseTest
{
	public static char[] hexArray = "0123456789ABCDEF".ToCharArray();

	private string TAG => GetType().FullName;

	private static string Base64Encode(string plainText)
	{
		return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
	}

	private static string Base64Decode(string base64EncodedData)
	{
		byte[] bytes = Convert.FromBase64String(base64EncodedData);
		return Encoding.UTF8.GetString(bytes);
	}

	public static string bytesToHex(byte[] bytes)
	{
		char[] array = new char[bytes.Length * 2];
		for (int i = 0; i < bytes.Length; i++)
		{
			int num = bytes[i] & 0xFF;
			array[i * 2] = hexArray[num >> 4];
			array[i * 2 + 1] = hexArray[num & 0xF];
		}
		return new string(array);
	}

	public string GetTestCommandDataStringAscii(string responseHex, int offset)
	{
		if (responseHex != null && !string.IsNullOrEmpty(responseHex))
		{
			string text = HexStringToAsciiString(responseHex);
			if (text.Length > offset)
			{
				return text.Substring(offset);
			}
			return null;
		}
		return null;
	}

	public static string HexStringToAsciiString(string hexString)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i <= hexString.Length - 2; i += 2)
		{
			stringBuilder.Append(Convert.ToString(Convert.ToChar(int.Parse(hexString.Substring(i, 2), NumberStyles.HexNumber))));
		}
		return stringBuilder.ToString();
	}

	public Tuple<string, string> Execute(string deviceModel, string operatorValue, string socModel, string sip, Func<string, string, string> testcommand)
	{
		string text = "777984792211";
		string text2 = "3000000014F0000049EC3734E6BD4F207C58F0703AC4DBFD194A42F8D1EEF2F75025FF8AD368FF63D874CF88D51CCA4C63EFCA6476108396A838041F388FB33863943AE7AB2A3BABE7D841552C48E34F4F283BF1F9217253153BCF533023E0A462FA617B1886CBD59D07597EA2019043A92FC70EF79AF40FE808FE1B5DAB10B298D58324E7B1EDBCD09C43C79EA7A59C5E84359E935347EC7C5CBAB77DD265F50918C575FDBFA5D884BDE22CD5C6DA9201E8A3F9E8532C29815286F75CAD126052184CDB5EA481C2AF269F778EDA77C4D270AF3E188214EE43A57902FE299C3FE5C4921B2BE295EB84962683E1BCF0321BEE6AC1A4ED400AA90C3B42E29904ADC12554A1641015DD11111111111111112222222222222222200000000000000000000000000000000000000000000000000000000000000000";
		string text3 = deviceModel;
		string text4 = socModel;
		string text5 = sip;
		string empty = string.Empty;
		string empty2 = string.Empty;
		if (!string.IsNullOrEmpty(text3))
		{
			text3 = text3.ToUpper();
		}
		if (string.IsNullOrEmpty(operatorValue))
		{
			Smart.Log.Debug(TAG, "RSU_OPERATOR VERSION field is mandatory");
			throw new MissingFieldException("RSU Operator not supplied");
		}
		operatorValue.ToUpper();
		if (!string.IsNullOrEmpty(text4))
		{
			text4 = text4.ToUpper();
		}
		if (!string.IsNullOrEmpty(text5))
		{
			text5 = text5.ToUpper();
		}
		Smart.Log.Debug(TAG, "triggering generation of cmp_genbindinkey.bin");
		testcommand("0800", "00" + text2.Length.ToString("X4") + text2);
		DateTime value = new DateTime(1970, 1, 1, 0, 0, 0);
		int num = (int)DateTime.UtcNow.Subtract(value).TotalSeconds;
		byte[] array = new byte[32];
		RandomNumberGenerator.Create().GetBytes(array);
		StringBuilder stringBuilder = new StringBuilder();
		byte[] array2 = array;
		foreach (byte b in array2)
		{
			stringBuilder.AppendFormat("{0:x2}", b);
		}
		string text6 = stringBuilder.ToString();
		Smart.Log.Verbose(TAG, string.Format("Serial Number Length {0}, Serial Number {1}", (text.Length / 2).ToString("X2"), text));
		Smart.Log.Verbose(TAG, string.Format("Epoch Elapsed Time Hex {0}, Normal {1}", num.ToString("X4"), num));
		Smart.Log.Verbose(TAG, $"Entropy {text6}");
		testcommand("0800", "01" + (text.Length / 2).ToString("X2") + text + num.ToString("X4") + text6);
		string text7 = testcommand("0800", "02");
		if (!text7.StartsWith("00"))
		{
			Smart.Log.Debug(TAG, $"read rsu uid command failed {text7}");
			throw new IOException("read rsu uid command failed");
		}
		empty2 = GetTestCommandDataStringAscii(text7, 3);
		int num2 = int.Parse(text7.Substring(2, 4), NumberStyles.HexNumber);
		Smart.Log.Verbose(TAG, $"suid {empty2}");
		Smart.Log.Verbose(TAG, $"expected length of suid {num2}");
		if (num2 != empty2.Length)
		{
			Smart.Log.Verbose(TAG, $"phone returned suid {empty2}, length {empty2.Length} does not match length {num2}");
			throw new FormatException("Device data and returned suid data length do not match");
		}
		text7 = testcommand("0800", "03");
		if (!text7.StartsWith("00"))
		{
			Smart.Log.Debug(TAG, $"read rsu key command failed {text7}");
			throw new IOException("read rsu key command failed");
		}
		empty = GetTestCommandDataStringAscii(text7, 3);
		int num3 = int.Parse(text7.Substring(2, 4), NumberStyles.HexNumber);
		if (num3 != empty.Length)
		{
			Smart.Log.Debug(TAG, $"phone returned rsukey {empty}, length {empty.Length} does not match length {num3}");
			throw new FormatException("Device data and returned rsukey data length do not match");
		}
		testcommand("0800", "04");
		return new Tuple<string, string>(empty, empty2);
	}
}
