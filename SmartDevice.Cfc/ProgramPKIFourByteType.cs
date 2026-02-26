using System;
using System.Globalization;
using System.IO;
using System.Text;
using ISmart;

namespace SmartDevice.Cfc;

public class ProgramPKIFourByteType : BaseTest
{
	protected static int PKI_SERIAL_NUM_OFFSET = 6;

	protected static int PKI_DATA_RESPONSE_LEN = 327;

	protected static int PKI_DATA_REQUEST_TOTAL_LEN = 430;

	protected static int PKI_PROTOCOL_VERSION_OFFSET = 4;

	protected static int PKI_DATA_TYPES_LEN = 4;

	protected static int PKI_SERIAL_NUM_LEN = 64;

	private string TAG => GetType().FullName;

	public bool Execute(string pkiKeyType, Func<string, string, string> testcommand, bool production, string productFamily, string originalImei, string logId)
	{
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		string empty = string.Empty;
		int num = 0;
		string empty2 = string.Empty;
		bool productiontype = true;
		if (!production)
		{
			empty = "02";
			productiontype = false;
		}
		else
		{
			empty = "01";
		}
		if (string.IsNullOrEmpty(pkiKeyType))
		{
			throw new NotSupportedException("PKI key type value must be non-empty");
		}
		empty = "08" + empty + pkiKeyType;
		string text = testcommand("006A", empty);
		if (text.StartsWith("02"))
		{
			Smart.Log.Debug(TAG, "PKI key already programmed");
			return false;
		}
		if (!text.StartsWith("00"))
		{
			string arg = Smart.Convert.HexStringToAsciiString(text);
			Smart.Log.Error(TAG, $"Hex return data: {arg}");
			throw new IOException("PKI provision response is invalid");
		}
		if (text.Length < PKI_DATA_RESPONSE_LEN * 2)
		{
			Smart.Log.Debug(TAG, $"PKI response length {text.Length}, expected length {PKI_DATA_RESPONSE_LEN * 2}");
			throw new IOException("PKI response is invalid (too short response)");
		}
		empty = text.Substring(2);
		string value = pkiKeyType + new string('0', 120);
		int pKI_DATA_REQUEST_TOTAL_LEN = PKI_DATA_REQUEST_TOTAL_LEN;
		int num2 = 1;
		string empty3 = string.Empty;
		string empty4 = string.Empty;
		string arg2 = string.Empty;
		string ServerId = string.Empty;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(string.Format("{0}{1}{2}{3}{4}", pKI_DATA_REQUEST_TOTAL_LEN.ToString("X4", CultureInfo.InvariantCulture), empty.Substring(PKI_PROTOCOL_VERSION_OFFSET * 2, 2), num2.ToString("X2", CultureInfo.InvariantCulture), "00", "01"));
		Smart.Log.Verbose(TAG, $"Add 6 byte data: 2B-total_length, 1B-protocol_version, 1B-message_id, 1B-test_mode Normal");
		Smart.Log.Verbose(TAG, $"Build request {stringBuilder.Length / 2}-byte data: <{stringBuilder}>");
		stringBuilder.Append(value);
		Smart.Log.Verbose(TAG, $"Add 64 byte data: PKI Data type, so far there are 70 bytes in total");
		Smart.Log.Verbose(TAG, $"Build request {stringBuilder.Length / 2}-byte data: <{stringBuilder}>");
		string text2 = empty.Substring((PKI_DATA_TYPES_LEN + 1) * 2, 130);
		stringBuilder.Append(text2);
		Smart.Log.Verbose(TAG, $"Add 65 byte data: serial number and size, so far there are 135 bytes in total");
		Smart.Log.Verbose(TAG, $"Build request {stringBuilder.Length / 2}-byte data: <{stringBuilder}>");
		string text3 = "02";
		stringBuilder.Append(text3);
		Smart.Log.Verbose(TAG, $"Add 1 byte data: key_agreement_type = {text3}, so far there are 136 bytes in total");
		Smart.Log.Verbose(TAG, $"Build request {stringBuilder.Length / 2}-byte data: <{stringBuilder}>");
		string value2 = empty.Substring((PKI_DATA_TYPES_LEN + 1) * 2 + text2.Length);
		stringBuilder.Append(value2);
		Smart.Log.Verbose(TAG, $"Add 256 byte data: Public key 256 byte for v3, so far there are 392 bytes in total");
		Smart.Log.Verbose(TAG, $"Build request {stringBuilder.Length / 2}-byte data: <{stringBuilder}>");
		stringBuilder.Append("001E");
		int num3 = 6;
		string text4 = "LENOVO";
		string text5 = "454E562E00000016000100270000000E0001002900000006";
		if (productFamily != null && productFamily != string.Empty)
		{
			text4 = productFamily;
		}
		StringBuilder stringBuilder2 = new StringBuilder();
		for (int i = 0; i < num3; i++)
		{
			string value3 = "30";
			if (i < text4.Length)
			{
				value3 = string.Format("{0}", ((int)text4[i]).ToString("X2", CultureInfo.InvariantCulture));
			}
			stringBuilder2.Append(value3);
		}
		text5 += stringBuilder2.ToString().Substring(0, num3 * 2);
		stringBuilder.Append(text5);
		Smart.Log.Verbose(TAG, $"additional data has {text5.Length / 2} bytes <{text5}>");
		Smart.Log.Verbose(TAG, $"Add additional data: 2B-data_length and 30B-data, so far there are 424 bytes in total");
		Smart.Log.Verbose(TAG, $"Build request {stringBuilder.Length / 2}-byte data: <{stringBuilder}>");
		stringBuilder.Append("0000");
		Smart.Log.Verbose(TAG, $"Add 2 byte zero for extended data length: , so far there are 426 bytes in total");
		Smart.Log.Verbose(TAG, $"Build request {stringBuilder.Length / 2}-byte data: <{stringBuilder}>");
		empty3 = string.Empty;
		int num4 = int.Parse(empty.Substring(PKI_SERIAL_NUM_OFFSET * 2 - 2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
		empty4 = empty.Substring(PKI_SERIAL_NUM_OFFSET * 2, num4 * 2);
		int num5 = 0;
		byte[] array = new byte[empty4.Length / 2];
		for (int j = 0; j <= empty4.Length - 2; j += 2)
		{
			array[num5++] = (byte)Convert.ToChar(int.Parse(empty4.Substring(j, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
		}
		empty4 = Encoding.ASCII.GetString(array);
		if (empty4.StartsWith("I+U"))
		{
			arg2 = empty4.Substring(18, empty4.Length - 18);
			empty4 = empty4.Substring(3, 15);
		}
		if (empty4.StartsWith("M+U"))
		{
			arg2 = empty4.Substring(17, empty4.Length - 17);
			empty4 = empty4.Substring(3, 14);
			if (empty3.Length > 14)
			{
				empty3 = empty3.Substring(0, 14);
			}
		}
		if (string.IsNullOrEmpty(empty3))
		{
			empty3 = empty4;
		}
		Smart.Log.Verbose(TAG, $"Using Business Serial Number{empty3}, Pki Serial Number {empty4}, Pki UID {arg2}");
		empty2 = stringBuilder.ToString();
		byte[] array2 = new byte[empty2.Length / 2];
		for (int k = 0; k <= empty2.Length - 2; k += 2)
		{
			array2[num++] = (byte)Convert.ToChar(int.Parse(empty2.Substring(k, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
		}
		array2[0] = (byte)(array2.Length / 256);
		array2[1] = (byte)(array2.Length % 256);
		StringBuilder stringBuilder3 = new StringBuilder();
		foreach (int num6 in array2)
		{
			string value4 = string.Format("{0}", num6.ToString("X2", CultureInfo.InvariantCulture));
			stringBuilder3.Append(value4);
		}
		Smart.Log.Verbose(TAG, $"total data length = {array2[0] * 256 + array2[1]} in byte array (No 4B-CRC) before token signing and request");
		Smart.Log.Verbose(TAG, $"Build request {stringBuilder3.Length / 2}-byte data before signing: <{stringBuilder3.ToString()}>");
		byte[] dbsResponse = null;
		int nDbsResponseSize = 0;
		string sSubsidyLock = string.Empty;
		int nErrorCode = 0;
		string sErrorMessage = string.Empty;
		_ = string.Empty;
		_ = string.Empty;
		bool bReadSubsidyLock = false;
		Login login = Smart.Rsd.Login;
		CfcCid.RetrieveDbsResponseFor(((Login)(ref login)).UserName, productiontype, empty3, empty3, originalImei, "0x05", "0x00", array2, logId, out bReadSubsidyLock, out sSubsidyLock, out dbsResponse, out nDbsResponseSize, out ServerId, out nErrorCode, out sErrorMessage);
		_ = string.Empty;
		StringBuilder stringBuilder4 = new StringBuilder("0101");
		if ((dbsResponse[4] << 8) + dbsResponse[5] % 256 != 0)
		{
			byte[] array3 = new byte[64];
			Array.Copy(dbsResponse, 6, array3, 0, 64);
			sErrorMessage = Encoding.ASCII.GetString(array3);
			int num7 = sErrorMessage.IndexOf('\0');
			if (num7 != -1)
			{
				sErrorMessage = sErrorMessage.Substring(0, num7);
			}
			Smart.Log.Error(TAG, $"pki response status from pki server is invalid, status bytes is {sErrorMessage}");
			throw new IOException("PKI programming response is invalid");
		}
		int num8 = 0;
		byte[] array4 = new byte[1];
		Array.Copy(dbsResponse, 141, array4, 0, 1);
		num8 = array4[0] % 256;
		byte[] array5 = new byte[num8 * 2];
		Array.Copy(dbsResponse, 142, array5, 0, array5.Length);
		StringBuilder stringBuilder5 = new StringBuilder();
		for (int m = 0; m < num8; m++)
		{
			stringBuilder5.Append(Convert.ToInt32(array5[m]).ToString("X2", CultureInfo.InvariantCulture));
		}
		stringBuilder5.ToString();
		for (int n = 0; n < nDbsResponseSize; n++)
		{
			char[] array6 = dbsResponse[n].ToString("X2", CultureInfo.InvariantCulture).ToCharArray();
			stringBuilder4.Append(array6[0]);
			stringBuilder4.Append(array6[1]);
		}
		text = testcommand("006A", stringBuilder4.ToString());
		if (!text.Substring(2, 2).Equals("00"))
		{
			Smart.Log.Error(TAG, $"program pki response {text}, is invalid");
			throw new IOException("PKI program response is invalid");
		}
		return true;
	}
}
