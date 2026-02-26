using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ISmart;

public interface IConvert
{
	string BytesToHex(byte[] bytes);

	byte[] HexToBytes(string hex);

	byte[] LongToBytes(long value);

	byte[] IntToBytes(int value);

	byte[] UShortToBytes(ushort value);

	long BytesToLong(byte[] value);

	int BytesToInt(byte[] value);

	ushort BytesToUShort(byte[] value);

	byte[] AsciiToBytes(string asciiString);

	string BytesToAscii(byte[] asciiBytes);

	byte[] Base64ToBytes(string base64);

	string BytesToBase64(byte[] bytes);

	string LongToBase26(long value);

	long Base26ToLong(string base26);

	byte[] StreamToBytes(Stream stream);

	Stream BytesToStream(byte[] bytes);

	List<EnumType> EnumToValues<EnumType>() where EnumType : struct, IConvertible;

	string TimeSpanToDisplay(TimeSpan time);

	string ByteSizeToDisplay(long bytes, bool abbreviated = true);

	string CalculateCheckDigit(string serialNumber);

	bool IsSerialNumberValid(string serialNumber, SerialNumberType expectedType = SerialNumberType.Unknown);

	SerialNumberType ToSerialNumberType(string serialNumber);

	string ToPEsn(string meid);

	int TwosComplement(int data);

	byte[] ByteSwap(byte[] bytes);

	string ToString(string name, IEnumerable<KeyValuePair<string, object>> fields);

	string ToString(string name, IEnumerable<KeyValuePair<string, string>> fields);

	string ToCommaSeparated(IEnumerable list);

	string ToStatusText(string message, double percentageComplete);

	string GenerateCode(string serialNumber, bool validated);

	string AsciiStringToUnicodeHexString(string ascii, ByteOrder byteOrder);

	string AsciiStringToHexString(string ascii);

	string AsciiBytesToHexString(byte[] inputBytes, int nNumberOfBytes);

	string HexStringToAsciiString(string hexString);

	string DecSerialToHex(string serialNumber);

	bool ToBool(DialogResult result);

	string AtToTCMD(string atCommand);

	string HexToOddOrEven(string hex, bool toEven);

	List<string> Unformat(string format, string text);

	DateTime ServerToLocalTime(string timestamp);

	string QuoteFilePathName(string filePathName);

	string GetDeviceSerialNumber(IDevice device);
}
