using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ISmart;

namespace SmartDevice.Cfc;

public class ProgramCIDViaFastboot : BaseTest
{
	protected enum DataBlockTypes
	{
		IMEI,
		MEID,
		MSN
	}

	protected object _pkilock = new object();

	protected static int DB_NONCE_OFFSET = 65;

	protected static int DB_IMEI_OFFSET = 1;

	protected static int DB_REQUEST_TYPE_HEADER_LEN = 81;

	protected static int DB_REQUEST_HEADER_LEN = 160;

	protected static int DB_IMEI_LOCATION_OFFSET = 123;

	protected static int DB_ULMA_LOCATION_OFFSET = 131;

	protected static int DB_PSN_TYPE_LOCATION_OFFSET = 127;

	protected static int DB_PSN_LOCATION_OFFSET = 129;

	protected static int DB_DATA_BLOCK_TYPE_LOCATION = 84;

	protected static int DB_CID_VERSION_LOCATION = 86;

	protected static int DEFAULT_DBS_SERIAL_NUMBER_SIZE = 8;

	protected static int DB_CHANNEL_ID_OFFSET = 123;

	protected static int DBS_CIDV2_OFFSET = 2;

	private bool _updatepsnindatablockrequest;

	public string prov_req_command_string = "cid_prov_req";

	private const string CharList = "0123456789abcdefghijklmnopqrstuvwxyz";

	private string TAG => GetType().FullName;

	public string cidprovparameter { get; set; }

	public string ProgramChannelIDInDataBlockRequest { get; set; }

	public string WebServiceDataBlockRequestType { get; set; }

	public string DataBlockType { get; set; }

	public string SerialNumberType { get; set; }

	public string OldSerialNumber { get; set; }

	public string NewSerialNumber { get; set; }

	public string SubsidyLockFromWebService { get; set; }

	public bool DidWebServiceReadSubsidyLock { get; set; }

	protected DataBlockTypes CurrentDataBlockType { get; set; }

	public bool UpdatePSNInDataBlockRequest
	{
		get
		{
			return _updatepsnindatablockrequest;
		}
		set
		{
			_updatepsnindatablockrequest = value;
		}
	}

	public string Channel_ID { get; set; }

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

	public static string DecodeBase36AsString(string input)
	{
		IEnumerable<char> enumerable = input.ToLower().Reverse();
		long num = 0L;
		int num2 = 0;
		foreach (char item in enumerable)
		{
			num += "0123456789abcdefghijklmnopqrstuvwxyz".IndexOf(item) * (long)Math.Pow(36.0, num2);
			num2++;
		}
		byte[] bytes = BitConverter.GetBytes(num);
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse((Array)bytes);
		}
		StringBuilder stringBuilder = new StringBuilder();
		byte[] array = bytes;
		foreach (byte b in array)
		{
			stringBuilder.Append(b.ToString("X2"));
		}
		return stringBuilder.ToString();
	}

	protected string BuildRequest(string testcommanddata)
	{
		string text = testcommanddata.Substring(2);
		int num = int.Parse(text.Substring(0, 2));
		Smart.Log.Debug(TAG, $"nProtocolVersion = {num}");
		string text2 = text.Substring(DB_IMEI_OFFSET * 2, 128);
		int nSerialNumberSize = DEFAULT_DBS_SERIAL_NUMBER_SIZE;
		string text3 = testcommanddata.Substring(DB_DATA_BLOCK_TYPE_LOCATION * 2, 4);
		string text4 = testcommanddata.Substring(DB_CID_VERSION_LOCATION * 2, 4);
		if (text3.ToLower().Equals("00f0") && text4.ToLower().Equals("0000"))
		{
			UpdatePSNInDataBlockRequest = false;
		}
		else
		{
			UpdatePSNInDataBlockRequest = true;
		}
		Smart.Log.Debug(TAG, $"datablock type: {text3}, cid version: {text4}, update PSN: {UpdatePSNInDataBlockRequest}");
		if (UpdatePSNInDataBlockRequest)
		{
			char[] array = text.ToCharArray();
			char[] array2 = null;
			char[] array3 = null;
			nSerialNumberSize = NewSerialNumber.Length;
			if (CurrentDataBlockType == DataBlockTypes.IMEI)
			{
				array2 = NewSerialNumber.ToUpper().ToCharArray();
				char[] array4 = new char[16];
				char[] array5 = ("A" + NewSerialNumber.ToUpper()).ToCharArray();
				for (int i = 0; i <= array5.Length - 2; i += 2)
				{
					array4[i] = array5[i + 1];
					array4[i + 1] = array5[i];
				}
				array2 = array4;
				array3 = ("00" + SerialNumberType).ToCharArray();
			}
			else if (CurrentDataBlockType == DataBlockTypes.MEID)
			{
				array2 = ((NewSerialNumber.Length == 16) ? NewSerialNumber.ToUpper().ToCharArray() : (NewSerialNumber.ToUpper() + new string('0', 16 - NewSerialNumber.Length)).ToCharArray());
				array3 = ("00" + SerialNumberType).ToCharArray();
			}
			else if (CurrentDataBlockType == DataBlockTypes.MSN)
			{
				array2 = DecodeBase36AsString(NewSerialNumber).ToUpper().ToCharArray();
				array3 = ("00" + SerialNumberType).ToCharArray();
			}
			int num2 = 0;
			int num3 = 0;
			if (text4.Equals("0002"))
			{
				num3 = DBS_CIDV2_OFFSET;
				Smart.Log.Debug(TAG, $"datablock type: {text3}, cid version: {text4}, update PSN: {UpdatePSNInDataBlockRequest}, offset: {num3}");
			}
			char[] array6 = array3;
			foreach (char c in array6)
			{
				array[(DB_PSN_TYPE_LOCATION_OFFSET + num3) * 2 + num2++] = c;
			}
			int num4 = 0;
			array6 = array2;
			foreach (char c2 in array6)
			{
				array[(DB_PSN_LOCATION_OFFSET + num3) * 2 + num4++] = c2;
			}
			Smart.Log.Debug(TAG, $"Setting Channel ID to: {Channel_ID}");
			if (!string.IsNullOrEmpty(Channel_ID))
			{
				num4 = 0;
				array6 = Channel_ID.ToCharArray();
				foreach (char c3 in array6)
				{
					array[(DB_CHANNEL_ID_OFFSET + num3) * 2 + num4++] = c3;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			array6 = NewSerialNumber.ToUpper().ToCharArray();
			foreach (char value in array6)
			{
				stringBuilder.Append(Convert.ToInt32(value).ToString("X2"));
			}
			stringBuilder.Append(new string('0', 128 - stringBuilder.Length));
			text2 = stringBuilder.ToString();
			Smart.Log.Debug(TAG, $"serial number in header: <{text2}>");
			text = new string(array);
		}
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.Append(BuildHeader(num, text.Substring(DB_NONCE_OFFSET * 2, 32), nSerialNumberSize, text2));
		stringBuilder2.Append(text.Substring(DB_REQUEST_TYPE_HEADER_LEN * 2));
		return stringBuilder2.ToString();
	}

	public virtual void Execute(Func<string, string> fastboot, string originalImei, string logId)
	{
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		SubsidyLockFromWebService = string.Empty;
		string empty = string.Empty;
		string text = Smart.File.TempFolder();
		string text2 = string.Format("{0}_{1}_cid_prov", NewSerialNumber, DateTime.Now.ToString("yyyyMMddHHmmss"));
		string text3 = Smart.File.PathJoin(text, text2);
		Directory.CreateDirectory(Path.GetDirectoryName(text3));
		FileStream fileStream = new FileStream(text3, FileMode.Create, FileAccess.Write);
		try
		{
			lock (_pkilock)
			{
				string empty2 = string.Empty;
				empty2 = (string.IsNullOrEmpty(cidprovparameter) ? prov_req_command_string : (prov_req_command_string + " " + cidprovparameter));
				string response = fastboot("oem " + empty2);
				response = GetOemDataFromResponse(response);
				empty = response.Replace("(bootloader) ", string.Empty);
				try
				{
					Smart.Log.Verbose(TAG, $"phone provision response: {empty}");
				}
				catch (Exception)
				{
					Smart.Log.Error(TAG, $"fastboot datablock request response is invalid");
					throw;
				}
				if (!empty.StartsWith("00"))
				{
					throw new NotSupportedException($"datablock response {empty} is invalid");
				}
				int num2 = int.Parse(empty.Substring(2, 2));
				Smart.Log.Debug(TAG, $"nProtocolVersion = {num2}");
				string text4 = BuildRequest(empty);
				byte[] array = new byte[text4.Length / 2];
				for (int i = 0; i <= text4.Length - 2; i += 2)
				{
					array[num++] = (byte)Convert.ToChar(int.Parse(text4.Substring(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
				}
				array[0] = (byte)(array.Length / 256);
				array[1] = (byte)(array.Length % 256);
				Smart.Log.Debug(TAG, $"DBS request string has total data length: {text4.Length / 2} bytes after BuildRequest.");
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
				Smart.Log.Verbose(TAG, $"server DBS response: {stringBuilder.ToString()}");
				byte[] array4 = CfcCid.HexStringToByteArray(stringBuilder.ToString());
				fileStream.Write(array4, 0, array4.Length);
				fileStream.Close();
				string text5 = fastboot($"flash cid_prov \"{text3}\"");
				Smart.Log.Verbose(TAG, text5);
			}
		}
		catch (Exception ex2)
		{
			Smart.Log.Error(TAG, $"Exception raised {ex2.Message}");
			string text6 = $"Exception Message {ex2.Message}, Stack Trace {ex2.StackTrace}";
			Smart.Log.Verbose(TAG, text6);
			throw;
		}
		finally
		{
			if (fileStream != null)
			{
				fileStream.Close();
				fileStream = null;
			}
			Directory.Delete(Path.GetDirectoryName(text3), recursive: true);
		}
	}

	private string GetOemDataFromResponse(string response)
	{
		string[] array = response.Split(new string[1] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
		Regex regex = new Regex("\\(bootloader\\) [0-9a-fA-F]+", RegexOptions.Compiled);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			if (regex.IsMatch(array[i]))
			{
				stringBuilder.Append(array[i]);
			}
		}
		return stringBuilder.ToString();
	}
}
