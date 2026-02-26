using System;
using System.Globalization;
using System.Text;
using ISmart;

namespace SmartDevice.Cfc;

public class ProgramDataBlock : BaseTest
{
	protected object _pkilock = new object();

	protected static int DB_REQUEST_TYPE_HEADER_LEN = 81;

	protected static int DB_REQUEST_HEADER_LEN = 160;

	protected static int DEFAULT_DBS_SERIAL_NUMBER_SIZE = 8;

	protected static int DB_PROTOCOL_VERSION_OFFSET = 0;

	protected static int DB_IMEI_OFFSET = 1;

	protected static int DB_NONCE_OFFSET = 65;

	protected static int DB_NUM_OF_DATABLOCKS_OFFSET = 81;

	protected static int DB_DATABLOCK_TYPE_OFFSET = 83;

	protected static int DB_FORMAT_VERSION_OFFSET = 85;

	protected static int DB_LENGTH_OFFSET = 87;

	protected static int DB_PROCESSOR_UID_OFFSET = 91;

	protected static int DB_FLASH_UID_OFFSET = 107;

	protected static int DB_IMEI_LOCATION_OFFSET = 123;

	protected static int DB_ULMA_LOCATION_OFFSET = 131;

	protected static int DB_WIFI_MAC_OFFSET = 137;

	protected static int DB_SW_TYPE_OFFSET = 143;

	protected static int DB_CARRIER_ID_OFFSET = 147;

	protected static int DB_IDENTITY_NUMBER_LOCATION_OFFSET = 149;

	protected static int DB_IDENTITY_TYPE_LOCATION_OFFSET = 150;

	protected static int DB_CHANNEL_ID_OFFSET = 123;

	protected static int DB_CID_VALUE_OFFSET = 125;

	protected static int DB_PSN_TYPE_LOCATION_OFFSET = 127;

	protected static int DB_PSN_LOCATION_OFFSET = 129;

	protected static int DB_SSN_TYPE_OFFSET = 137;

	protected static int DB_SSN_OFFSET = 139;

	protected static int DB_PASSWORD_UPDATE_FLAG_OFFSET = 159;

	protected static int DB_PASSWORD_HASH_OFFSET = 161;

	protected static int DB_CIDV2_RESERVED_DATA_OFFSET = 181;

	protected static int DBS_CIDV2_OFFSET = 2;

	protected static int DB_GSM_IDENTITY_TYPE_LOCATION_OFFSET = 149;

	protected static int DB_DATA_BLOCK_TYPE_LOCATION = 84;

	protected static int DB_CID_VERSION_LOCATION = 86;

	private string TAG => GetType().FullName;

	public string WebServiceDataBlockRequestType { get; set; }

	public string DataBlockType { get; set; }

	public string SerialNumberType { get; set; }

	public string OldSerialNumber { get; set; }

	public string NewSerialNumber { get; set; }

	public string IMEI2 { get; set; }

	public string SubsidyLockFromWebService { get; set; }

	public bool DidWebServiceReadSubsidyLock { get; set; }

	protected virtual string BuildHeader(int nProtocolVersion, string sNonce, int nSerialNumberSize, string sSerialNumber)
	{
		int num = 5;
		int num2 = 1;
		char[] array = new char[78];
		string text = "";
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = '0';
		}
		if (nProtocolVersion == 3)
		{
			return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}", "0000", nProtocolVersion.ToString("X2", CultureInfo.InvariantCulture), num.ToString("X2", CultureInfo.InvariantCulture), SerialNumberType, nSerialNumberSize.ToString("X2", CultureInfo.InvariantCulture), sSerialNumber, sNonce, num2.ToString("X2", CultureInfo.InvariantCulture), new string(array), "0001", text.PadLeft(60, '0'));
		}
		return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", "0000", nProtocolVersion.ToString("X2", CultureInfo.InvariantCulture), num.ToString("X2", CultureInfo.InvariantCulture), SerialNumberType, nSerialNumberSize.ToString("X2", CultureInfo.InvariantCulture), sSerialNumber, sNonce, num2.ToString("X2", CultureInfo.InvariantCulture), new string(array));
	}

	protected virtual string BuildRequest(string testcommanddata)
	{
		string text = testcommanddata.Substring(2);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(BuildHeader(int.Parse(text.Substring(0, 2)), text.Substring(DB_NONCE_OFFSET * 2, 32), DEFAULT_DBS_SERIAL_NUMBER_SIZE, text.Substring(DB_IMEI_OFFSET * 2, 128)));
		stringBuilder.Append(text.Substring(DB_REQUEST_TYPE_HEADER_LEN * 2));
		return stringBuilder.ToString();
	}

	public void Execute(Func<string, string, string> testcommand, string originalImei, string logId)
	{
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		SubsidyLockFromWebService = string.Empty;
		try
		{
			lock (_pkilock)
			{
				string text = testcommand("0C31", "00" + DataBlockType);
				if (!text.StartsWith("00"))
				{
					string text2 = $"datablock response {text} is invalid";
					Smart.Log.Error(TAG, text2);
					throw new NotSupportedException(text2);
				}
				string text3 = BuildRequest(text);
				Smart.Log.Verbose(TAG, $"DBS request string {text3}");
				byte[] array = new byte[text3.Length / 2];
				for (int i = 0; i <= text3.Length - 2; i += 2)
				{
					array[num++] = (byte)Convert.ToChar(int.Parse(text3.Substring(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
				}
				array[0] = (byte)(array.Length / 256);
				array[1] = (byte)(array.Length % 256);
				byte[] dbsResponse = null;
				int nDbsResponseSize = 0;
				string sSubsidyLock = string.Empty;
				int nErrorCode = 0;
				string sErrorMessage = string.Empty;
				bool bReadSubsidyLock = false;
				DidWebServiceReadSubsidyLock = false;
				string ServerId = string.Empty;
				Login login = Smart.Rsd.Login;
				CfcCid.RetrieveDbsResponseFor(((Login)(ref login)).UserName, productiontype: true, OldSerialNumber, NewSerialNumber, originalImei, WebServiceDataBlockRequestType, "0x00", array, logId, out bReadSubsidyLock, out sSubsidyLock, out dbsResponse, out nDbsResponseSize, out ServerId, out nErrorCode, out sErrorMessage);
				SubsidyLockFromWebService = sSubsidyLock;
				DidWebServiceReadSubsidyLock = bReadSubsidyLock;
				if ((dbsResponse[4] << 8) + dbsResponse[5] % 256 != 0)
				{
					byte[] array2 = new byte[64];
					Array.Copy(dbsResponse, 6, array2, 0, 64);
					sErrorMessage = Encoding.UTF8.GetString(array2);
					Smart.Log.Error(TAG, $"datablock response status is invalid, status bytes is {sErrorMessage}");
					throw new NotSupportedException(sErrorMessage);
				}
				StringBuilder stringBuilder = new StringBuilder("01" + DataBlockType);
				for (int j = 0; j < nDbsResponseSize; j++)
				{
					char[] array3 = dbsResponse[j].ToString("X2", CultureInfo.InvariantCulture).ToCharArray();
					stringBuilder.Append(array3[0]);
					stringBuilder.Append(array3[1]);
				}
				text = testcommand("0C31", stringBuilder.ToString());
				if (!text.Equals("00"))
				{
					Smart.Log.Error(TAG, $"program datablock failed response {text}, is invalid");
					throw new NotSupportedException("Program datablock failed");
				}
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, $"Exception raised {ex.Message}");
			string text4 = $"Exception Message {ex.Message}, Stack Trace {ex.StackTrace}";
			Smart.Log.Verbose(TAG, text4);
			throw;
		}
	}
}
