using System;
using System.Globalization;
using System.IO;
using System.Text;
using ISmart;

namespace SmartDevice.Cfc;

public class ProgramPKI : BaseTest
{
	protected static int PKI_SERIAL_NUM_OFFSET = 34;

	protected static int PKI_DATA_RESPONSE_LEN = 228;

	protected static int PKI_DATA_REQUEST_TOTAL_LEN = 237;

	protected static int PKI_PROTOCOL_VERSION_OFFSET = 32;

	protected static int PKI_DATA_TYPES_LEN = 32;

	protected static int PKI_SERIAL_NUM_LEN = 64;

	private string TAG => GetType().FullName;

	public bool Execute(string pkiKeyType, Func<string, string, string> testcommand, bool production, string originalImei, string logId)
	{
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
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
		empty = "05" + empty + pkiKeyType;
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
		empty = text.Substring(4);
		string value = pkiKeyType + new string('0', 60);
		int pKI_DATA_REQUEST_TOTAL_LEN = PKI_DATA_REQUEST_TOTAL_LEN;
		int num2 = 1;
		string empty3 = string.Empty;
		string empty4 = string.Empty;
		_ = string.Empty;
		string ServerId = string.Empty;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(string.Format("{0}{1}{2}{3}{4}", pKI_DATA_REQUEST_TOTAL_LEN.ToString("X4", CultureInfo.InvariantCulture), empty.Substring(PKI_PROTOCOL_VERSION_OFFSET * 2, 2), num2.ToString("X2", CultureInfo.InvariantCulture), "00", "01"));
		stringBuilder.Append(value);
		stringBuilder.Append(empty.Substring((PKI_DATA_TYPES_LEN + 1) * 2));
		stringBuilder.Append("0000");
		int num3 = int.Parse(empty.Substring(PKI_SERIAL_NUM_OFFSET * 2 - 2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
		empty4 = empty.Substring(PKI_SERIAL_NUM_OFFSET * 2, num3 * 2);
		int num4 = 0;
		byte[] array = new byte[empty4.Length / 2];
		for (int i = 0; i <= empty4.Length - 2; i += 2)
		{
			array[num4++] = (byte)Convert.ToChar(int.Parse(empty4.Substring(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
		}
		empty4 = Encoding.ASCII.GetString(array);
		if (empty4.StartsWith("I+U"))
		{
			empty4.Substring(18, empty4.Length - 18);
			empty4 = empty4.Substring(3, 15);
		}
		if (empty4.StartsWith("M+U"))
		{
			empty4.Substring(17, empty4.Length - 17);
			empty4 = empty4.Substring(3, 14);
			if (empty3.Length > 14)
			{
				empty3 = empty3.Substring(0, 14);
			}
		}
		empty3 = empty4;
		empty2 = stringBuilder.ToString();
		byte[] array2 = new byte[empty2.Length / 2];
		for (int j = 0; j <= empty2.Length - 2; j += 2)
		{
			array2[num++] = (byte)Convert.ToChar(int.Parse(empty2.Substring(j, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
		}
		array2[0] = (byte)(array2.Length / 256);
		array2[1] = (byte)(array2.Length % 256);
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
		StringBuilder stringBuilder2 = new StringBuilder("0101");
		if ((dbsResponse[4] << 8) + dbsResponse[5] % 256 != 0)
		{
			byte[] array3 = new byte[64];
			Array.Copy(dbsResponse, 6, array3, 0, 64);
			sErrorMessage = Encoding.ASCII.GetString(array3);
			int num5 = sErrorMessage.IndexOf('\0');
			if (num5 != -1)
			{
				sErrorMessage = sErrorMessage.Substring(0, num5);
			}
		}
		int num6 = 0;
		byte[] array4 = new byte[1];
		Array.Copy(dbsResponse, 139, array4, 0, 1);
		num6 = array4[0] % 256;
		byte[] array5 = new byte[num6 * 2];
		Array.Copy(dbsResponse, 140, array5, 0, array5.Length);
		StringBuilder stringBuilder3 = new StringBuilder();
		for (int k = 0; k < num6; k++)
		{
			stringBuilder3.Append(Convert.ToInt32(array5[k]).ToString("X2", CultureInfo.InvariantCulture));
		}
		stringBuilder3.ToString();
		for (int l = 0; l < nDbsResponseSize; l++)
		{
			char[] array6 = dbsResponse[l].ToString("X2", CultureInfo.InvariantCulture).ToCharArray();
			stringBuilder2.Append(array6[0]);
			stringBuilder2.Append(array6[1]);
		}
		text = testcommand("006A", stringBuilder2.ToString());
		if (!text.Substring(2, 2).Equals("00"))
		{
			throw new IOException("PKI programming response is invalid");
		}
		return true;
	}
}
