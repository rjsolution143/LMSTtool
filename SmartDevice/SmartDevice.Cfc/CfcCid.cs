using System;
using System.Globalization;
using System.Text;
using ISmart;

namespace SmartDevice.Cfc;

public class CfcCid
{
	public class PkiServiceProxyConstants
	{
		public const string LDAP_PASSWORD = "Bs_MIA_001";

		public const int WEB_SERVICE_EXCEPTION = -1;

		public const int INVALID_PARAMS = -2;

		public const int INVALID_IP_ADDRESS = -3;

		public const int INVALID_HW_DONGLE = -4;

		public const int INVALID_SW_CERTIFICATE = -5;

		public const int PKI_MESSAGE_BUILD_ERROR = -6;

		public const int DBS_MESSAGE_BUILD_ERROR = -7;

		public const int DRM_MESSAGE_BUILD_ERROR = -8;

		public const int UNKNOWN_WEB_SERVICE_ERROR = -9;

		public const int INVALID_PKI_TRANSPORT = -10;

		public const int LOCAL_PKI_EXCEPTION = -11;

		public const int UNABLE_TO_CONNECT_TO_PKI_SERVER = -12;

		public const int SERVER_RETURNED_NO_DATA = -13;

		public const int INVALID_SERVER_RESPONSE = -14;

		public const int GSM_PRIMARY_LOCK_CODE = 11;

		public const int GSM_SECONDARY_LOCK_CODE = 12;

		public const int GSM_SERVICE_PASSCODE = 13;

		public const int GSM_LOCK_CODE4 = 14;

		public const int GSM_LOCK_CODE5 = 15;

		public const int GSM_APC_CODE = 17;

		public const int GSM_BLUETOOTH_MAC_ADDRESS = 18;

		public const int CDMA_MASTER_LOCK_CODE = 21;

		public const int CDMA_ONETIME_LOCK_CODE = 22;

		public const int CDMA_SERVICE_PASSCODE = 23;

		public const int CDMA_AKEY1 = 24;

		public const int CDMA_AKEY2 = 25;

		public const int CDMA_EVDO = 26;

		public const int CDMA_APC_CODE = 27;

		public const int CDMA_BLUETOOTH_MAC_ADDRESS = 28;

		public const int PKI_SIGNED_MESSAGE_SIZE = 1795;

		public const int DBS_EXTENDED_LENGTH = 1357;

		public const int DRM_SIGNED_MESSAGE_SIZE = 1594;

		public const int DRM_SIGNED_MESSAGE_SIZE_V3 = 1787;

		public static uint[] CRC_TABLE = new uint[256]
		{
			0u, 1996959894u, 3993919788u, 2567524794u, 124634137u, 1886057615u, 3915621685u, 2657392035u, 249268274u, 2044508324u,
			3772115230u, 2547177864u, 162941995u, 2125561021u, 3887607047u, 2428444049u, 498536548u, 1789927666u, 4089016648u, 2227061214u,
			450548861u, 1843258603u, 4107580753u, 2211677639u, 325883990u, 1684777152u, 4251122042u, 2321926636u, 335633487u, 1661365465u,
			4195302755u, 2366115317u, 997073096u, 1281953886u, 3579855332u, 2724688242u, 1006888145u, 1258607687u, 3524101629u, 2768942443u,
			901097722u, 1119000684u, 3686517206u, 2898065728u, 853044451u, 1172266101u, 3705015759u, 2882616665u, 651767980u, 1373503546u,
			3369554304u, 3218104598u, 565507253u, 1454621731u, 3485111705u, 3099436303u, 671266974u, 1594198024u, 3322730930u, 2970347812u,
			795835527u, 1483230225u, 3244367275u, 3060149565u, 1994146192u, 31158534u, 2563907772u, 4023717930u, 1907459465u, 112637215u,
			2680153253u, 3904427059u, 2013776290u, 251722036u, 2517215374u, 3775830040u, 2137656763u, 141376813u, 2439277719u, 3865271297u,
			1802195444u, 476864866u, 2238001368u, 4066508878u, 1812370925u, 453092731u, 2181625025u, 4111451223u, 1706088902u, 314042704u,
			2344532202u, 4240017532u, 1658658271u, 366619977u, 2362670323u, 4224994405u, 1303535960u, 984961486u, 2747007092u, 3569037538u,
			1256170817u, 1037604311u, 2765210733u, 3554079995u, 1131014506u, 879679996u, 2909243462u, 3663771856u, 1141124467u, 855842277u,
			2852801631u, 3708648649u, 1342533948u, 654459306u, 3188396048u, 3373015174u, 1466479909u, 544179635u, 3110523913u, 3462522015u,
			1591671054u, 702138776u, 2966460450u, 3352799412u, 1504918807u, 783551873u, 3082640443u, 3233442989u, 3988292384u, 2596254646u,
			62317068u, 1957810842u, 3939845945u, 2647816111u, 81470997u, 1943803523u, 3814918930u, 2489596804u, 225274430u, 2053790376u,
			3826175755u, 2466906013u, 167816743u, 2097651377u, 4027552580u, 2265490386u, 503444072u, 1762050814u, 4150417245u, 2154129355u,
			426522225u, 1852507879u, 4275313526u, 2312317920u, 282753626u, 1742555852u, 4189708143u, 2394877945u, 397917763u, 1622183637u,
			3604390888u, 2714866558u, 953729732u, 1340076626u, 3518719985u, 2797360999u, 1068828381u, 1219638859u, 3624741850u, 2936675148u,
			906185462u, 1090812512u, 3747672003u, 2825379669u, 829329135u, 1181335161u, 3412177804u, 3160834842u, 628085408u, 1382605366u,
			3423369109u, 3138078467u, 570562233u, 1426400815u, 3317316542u, 2998733608u, 733239954u, 1555261956u, 3268935591u, 3050360625u,
			752459403u, 1541320221u, 2607071920u, 3965973030u, 1969922972u, 40735498u, 2617837225u, 3943577151u, 1913087877u, 83908371u,
			2512341634u, 3803740692u, 2075208622u, 213261112u, 2463272603u, 3855990285u, 2094854071u, 198958881u, 2262029012u, 4057260610u,
			1759359992u, 534414190u, 2176718541u, 4139329115u, 1873836001u, 414664567u, 2282248934u, 4279200368u, 1711684554u, 285281116u,
			2405801727u, 4167216745u, 1634467795u, 376229701u, 2685067896u, 3608007406u, 1308918612u, 956543938u, 2808555105u, 3495958263u,
			1231636301u, 1047427035u, 2932959818u, 3654703836u, 1088359270u, 936918000u, 2847714899u, 3736837829u, 1202900863u, 817233897u,
			3183342108u, 3401237130u, 1404277552u, 615818150u, 3134207493u, 3453421203u, 1423857449u, 601450431u, 3009837614u, 3294710456u,
			1567103746u, 711928724u, 3020668471u, 3272380065u, 1510334235u, 755167117u
		};
	}

	private const string CharList = "0123456789abcdefghijklmnopqrstuvwxyz";

	private static string TAG => "CfcCid";

	public static bool CalculateCRC32(byte[] inputArray, int nSize, out uint nCrcValue)
	{
		uint num = uint.MaxValue;
		for (uint num2 = 0u; num2 < nSize; num2++)
		{
			num = (num >> 8) ^ PkiServiceProxyConstants.CRC_TABLE[(inputArray[num2] ^ num) & 0xFF];
		}
		nCrcValue = ~num;
		return true;
	}

	public static byte[] HexStringToByteArray(string str)
	{
		string text = "0123456789ABCDEF";
		if (str.Length % 2 != 0)
		{
			str += "0";
		}
		byte[] array = new byte[str.Length >> 1];
		for (int i = 0; i < str.Length; i += 2)
		{
			int num = text.IndexOf(char.ToUpperInvariant(str[i]));
			int num2 = text.IndexOf(char.ToUpperInvariant(str[i + 1]));
			if (num == -1 || num2 == -1)
			{
				throw new ArgumentException("The string contains an invalid digit.", "s");
			}
			array[i >> 1] = (byte)((num << 4) | num2);
		}
		return array;
	}

	public static byte[] BuildDbsSignedMessage(byte[] dbsRequest, string sUserId, out string sMascId, out string sLocalIpAddress, out string sSourceType, out string sBenchId, out TOKEN_ERROR tokenError)
	{
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Expected I4, but got Unknown
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Expected I4, but got Unknown
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		sMascId = string.Empty;
		sLocalIpAddress = string.Empty;
		sSourceType = string.Empty;
		sBenchId = string.Empty;
		int num = 1357;
		int num2 = dbsRequest[2];
		int num3 = ((num2 != 3) ? 1 : 2);
		Smart.Log.Debug(TAG, $"DBS Data Requset has protocol_version = {num2}, authenticator = {num3}");
		int num4 = dbsRequest[0] * 256 + dbsRequest[1];
		if (num2 == 3)
		{
			num = Smart.Web.TokenGetExtendFieldLen(num3);
			if (num <= 0)
			{
				Smart.Log.Debug(TAG, $"Extended Field Length invalid!!!");
				throw new NotSupportedException("Extended Field Length invalid");
			}
		}
		int num5 = num4 + num + 2 + 4;
		Smart.Log.Debug(TAG, $"passed in dbsRequest data length = {num4} bytes, extended length = {num}");
		Smart.Log.Debug(TAG, $"the expected total DbsSignedMessageLength = {num5} bytes");
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder = new StringBuilder();
		for (int i = 0; i < num4; i++)
		{
			int num6 = dbsRequest[i];
			string value = string.Format("{0}", num6.ToString("X2", CultureInfo.InvariantCulture));
			stringBuilder.Append(value);
		}
		Smart.Log.Debug(TAG, $"passed in raw request data: length update = {stringBuilder.Length / 2} bytes \n <{stringBuilder.ToString()}>");
		string text = num5.ToString("X4", CultureInfo.InvariantCulture);
		Smart.Log.Debug(TAG, $"building the total dbs signed messsage with length = 0x{text} (={num5}) bytes");
		byte[] array = new byte[num5];
		for (int j = 0; j < num5; j++)
		{
			array[j] = 0;
		}
		array[0] = (byte)int.Parse(text.Substring(0, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
		array[1] = (byte)int.Parse(text.Substring(2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
		for (int k = 2; k < num4; k++)
		{
			array[k] = dbsRequest[k];
		}
		if (num2 == 3)
		{
			byte[] bytes = BitConverter.GetBytes(num);
			array[num4] = bytes[1];
			array[num4 + 1] = bytes[0];
			array[num4 + 2] = 1;
			array[num4 + 3] = (byte)num3;
			Smart.Log.Debug(TAG, $"Insert the extended data lenghth of {num} at {num4}-{num4 + 1} byte location");
			Smart.Log.Debug(TAG, string.Format("Insert the Syntax_version = 1 and Type = Authenticator #{0} of {0} at {1}-{2} byte location", num3, num4 + 2, num4 + 3));
			StringBuilder stringBuilder2 = new StringBuilder();
			for (int l = 0; l < num4 + 4; l++)
			{
				int num7 = array[l];
				string value2 = string.Format("{0}", num7.ToString("X2", CultureInfo.InvariantCulture));
				stringBuilder2.Append(value2);
			}
			Smart.Log.Debug(TAG, $"request data after extended data length and authenticator updated = {stringBuilder2.Length / 2} bytes <{stringBuilder2.ToString()}>");
		}
		else
		{
			array[num4] = 5;
			array[num4 + 1] = 77;
			array[num4 + 2] = 1;
			array[num4 + 3] = 1;
		}
		StringBuilder stringBuilder3 = new StringBuilder();
		stringBuilder3 = new StringBuilder();
		for (int m = 0; m < num5; m++)
		{
			int num8 = array[m];
			string value3 = string.Format("{0}", num8.ToString("X2", CultureInfo.InvariantCulture));
			stringBuilder3.Append(value3);
		}
		Smart.Log.Debug(TAG, $"request data after extended data built wiht {stringBuilder3.Length / 2} byte: \n <{stringBuilder3.ToString()}>");
		string empty = string.Empty;
		string empty2 = string.Empty;
		string empty3 = string.Empty;
		if (num2 == 3)
		{
			tokenError = (TOKEN_ERROR)(int)Smart.Web.GenExtendFieldType2(1, ref array, sUserId);
		}
		else
		{
			tokenError = (TOKEN_ERROR)(int)Smart.Web.GenerateDBSRequest(1, ref array, sUserId);
		}
		if (tokenError)
		{
			string text2 = "tokenError from GenerateDBSRequest is ";
			text2 += ((object)(TOKEN_ERROR)(ref tokenError)).ToString();
			Smart.Log.Debug(TAG, text2);
			throw new NotSupportedException(text2);
		}
		TokenInfo val = Smart.Web.TokenInfo();
		empty = ((TokenInfo)(ref val)).HwDongleIp;
		empty2 = ((TokenInfo)(ref val)).PkiSource;
		empty3 = ((TokenInfo)(ref val)).PkiSourceType;
		sBenchId = ((TokenInfo)(ref val)).BenchId;
		sMascId = empty2;
		sLocalIpAddress = empty;
		sBenchId = empty3;
		uint nCrcValue = 0u;
		if (!CalculateCRC32(array, num5 - 4, out nCrcValue))
		{
			throw new NotSupportedException("CalculateCRC32 failed");
		}
		string text3 = nCrcValue.ToString("X8", CultureInfo.InvariantCulture);
		Smart.Log.Debug(TAG, $"CRC: {text3}");
		if (num2 == 3)
		{
			array[num5 - 4] = (byte)int.Parse(text3.Substring(0, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
			array[num5 - 3] = (byte)int.Parse(text3.Substring(2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
			array[num5 - 2] = (byte)int.Parse(text3.Substring(4, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
			array[num5 - 1] = (byte)int.Parse(text3.Substring(6, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
		}
		else
		{
			array[num5 - 4] = (byte)int.Parse(text3.Substring(0, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
			array[num5 - 3] = (byte)int.Parse(text3.Substring(2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
			array[num5 - 2] = (byte)int.Parse(text3.Substring(4, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
			array[num5 - 1] = (byte)int.Parse(text3.Substring(6, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
		}
		StringBuilder stringBuilder4 = new StringBuilder();
		stringBuilder4 = new StringBuilder();
		for (int n = 0; n < num5; n++)
		{
			int num9 = array[n];
			string value4 = string.Format("{0}", num9.ToString("X2", CultureInfo.InvariantCulture));
			stringBuilder4.Append(value4);
		}
		Smart.Log.Debug(TAG, $"final request data after etoken signed and crc calculation with {stringBuilder4.Length / 2} byte data: \n <{stringBuilder4.ToString()}>");
		return array;
	}

	public static byte[] BuildDrmSignedMessage(byte[] drmRequest, string sUserId, out string sMascId, out string sLocalIpAddress, out string sSourceType, out string sBenchId, out TOKEN_ERROR tokenError)
	{
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Expected I4, but got Unknown
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		int num = drmRequest[2];
		Smart.Log.Debug(TAG, $"PKI Data Requset has protocl_version = {num}");
		int num2 = 1594;
		if (num == 3)
		{
			num2 = 1787;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (int num3 in drmRequest)
			{
				string value = string.Format("{0}", num3.ToString("X2", CultureInfo.InvariantCulture));
				stringBuilder.Append(value);
			}
			Smart.Log.Debug(TAG, $"requested data input byte array before signed has = {stringBuilder.Length / 2} byte data: <{stringBuilder.ToString()}>");
		}
		string text = num2.ToString("X4", CultureInfo.InvariantCulture);
		byte[] array = new byte[num2];
		for (int j = 0; j < num2; j++)
		{
			array[j] = 0;
		}
		array[0] = (byte)int.Parse(text.Substring(0, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
		array[1] = (byte)int.Parse(text.Substring(2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
		for (int k = 2; k < drmRequest.Length; k++)
		{
			array[k] = drmRequest[k];
		}
		if (num == 3)
		{
			StringBuilder stringBuilder2 = new StringBuilder();
			array[drmRequest.Length - 2] = 5;
			array[drmRequest.Length - 1] = 77;
			Smart.Log.Debug(TAG, $"Insert the extended data lenghth of 1357 (0x0577) at {drmRequest.Length - 2}-{drmRequest.Length - 1} byte location");
			stringBuilder2 = new StringBuilder();
			for (int l = 0; l < drmRequest.Length; l++)
			{
				int num4 = array[l];
				string value2 = string.Format("{0}", num4.ToString("X2", CultureInfo.InvariantCulture));
				stringBuilder2.Append(value2);
			}
			Smart.Log.Debug(TAG, $"request data after extended data length update = {stringBuilder2.Length / 2} bytes <{stringBuilder2.ToString()}>");
		}
		else
		{
			array[231] = 5;
			array[232] = 77;
		}
		string text2 = "";
		string text3 = "";
		string text4 = "";
		Login login = Smart.Rsd.Login;
		sUserId = ((Login)(ref login)).UserName;
		tokenError = (TOKEN_ERROR)(int)Smart.Web.GenExtendFieldType1(1, ref array, sUserId);
		if (tokenError)
		{
			string text5 = "tokenError from GenExtendFieldType1 is ";
			text5 += ((object)(TOKEN_ERROR)(ref tokenError)).ToString();
			Smart.Log.Debug(TAG, text5);
			throw new NotSupportedException(text5);
		}
		TokenInfo val = Smart.Web.TokenInfo();
		text2 = ((TokenInfo)(ref val)).HwDongleIp;
		text3 = ((TokenInfo)(ref val)).PkiSource;
		text4 = ((TokenInfo)(ref val)).PkiSourceType;
		sSourceType = ((TokenInfo)(ref val)).PkiSourceType;
		sBenchId = ((TokenInfo)(ref val)).BenchId;
		StringBuilder stringBuilder3 = new StringBuilder();
		foreach (int num5 in array)
		{
			string value3 = string.Format("{0}", num5.ToString("X2", CultureInfo.InvariantCulture));
			stringBuilder3.Append(value3);
		}
		Smart.Log.Debug(TAG, $"requested data signed = {stringBuilder3.Length / 2} bytes <{stringBuilder3.ToString()}>");
		uint nCrcValue = 0u;
		if (!CalculateCRC32(array, num2 - 4, out nCrcValue))
		{
			throw new NotSupportedException("Token CRC calculation failed");
		}
		string text6 = nCrcValue.ToString("X8", CultureInfo.InvariantCulture);
		Smart.Log.Debug(TAG, $"CRC: {text6}");
		if (num == 3)
		{
			array[num2 - 4] = (byte)int.Parse(text6.Substring(0, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
			array[num2 - 3] = (byte)int.Parse(text6.Substring(2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
			array[num2 - 2] = (byte)int.Parse(text6.Substring(4, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
			array[num2 - 1] = (byte)int.Parse(text6.Substring(6, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
		}
		else
		{
			array[1590] = (byte)int.Parse(text6.Substring(0, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
			array[1591] = (byte)int.Parse(text6.Substring(2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
			array[1592] = (byte)int.Parse(text6.Substring(4, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
			array[1593] = (byte)int.Parse(text6.Substring(6, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
		}
		sMascId = text3;
		sLocalIpAddress = text2;
		sBenchId = text4;
		StringBuilder stringBuilder4 = new StringBuilder();
		foreach (int num6 in array)
		{
			string value4 = string.Format("{0}", num6.ToString("X2", CultureInfo.InvariantCulture));
			stringBuilder4.Append(value4);
		}
		Smart.Log.Debug(TAG, $"requested data signed with final length = {stringBuilder4.Length / 2} bytes <{stringBuilder4.ToString()}>");
		return array;
	}

	public static void RetrieveDbsResponseFor(string sUserId, bool productiontype, string sOldImei, string sNewImei, string sOriginalImei, string sClientRequestType, string sPasswordChangeReq, byte[] dbsRequest, string logId, out bool bReadSubsidyLock, out string sSubsidyLock, out byte[] dbsResponse, out int nDbsResponseSize, out string ServerId, out int nErrorCode, out string sErrorMessage)
	{
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		Smart.Log.Debug(TAG, $"retrieve dbs response for Old IMEI {sOldImei}, New Imei {sNewImei}, Client Request Type {sClientRequestType}, Password Change Req {sPasswordChangeReq}");
		dbsResponse = null;
		nDbsResponseSize = 0;
		sErrorMessage = string.Empty;
		sSubsidyLock = string.Empty;
		bReadSubsidyLock = false;
		ServerId = string.Empty;
		StringBuilder stringBuilder = new StringBuilder("");
		if (string.IsNullOrEmpty(sUserId))
		{
			stringBuilder.Append("Userid ");
		}
		if (string.IsNullOrEmpty(sOldImei))
		{
			stringBuilder.Append("Old Imei ");
		}
		if (string.IsNullOrEmpty(sNewImei))
		{
			stringBuilder.Append("New Imei ");
		}
		if (string.IsNullOrEmpty(sClientRequestType))
		{
			stringBuilder.Append("Client Request Type ");
		}
		if (string.IsNullOrEmpty(sPasswordChangeReq))
		{
			stringBuilder.Append("Password Change Request ");
		}
		if (dbsRequest == null)
		{
			stringBuilder.Append("Dbs Request Param ");
		}
		if (stringBuilder.Length != 0)
		{
			sErrorMessage = "Invalid input param(s)";
			throw new NotSupportedException(sErrorMessage);
		}
		string sLocalIpAddress = string.Empty;
		string sMascId = string.Empty;
		string sSourceType = string.Empty;
		string sBenchId = string.Empty;
		byte[] array = null;
		int num = 0;
		int num2 = 5;
		TOKEN_ERROR tokenError = (TOKEN_ERROR)3;
		string arg = "Dbs";
		do
		{
			if (num > 0)
			{
				Smart.Thread.Wait(TimeSpan.FromSeconds(1.0));
			}
			if (sClientRequestType.Equals("0x05"))
			{
				arg = "Drm";
				array = BuildDrmSignedMessage(dbsRequest, sUserId, out sMascId, out sLocalIpAddress, out sSourceType, out sBenchId, out tokenError);
			}
			else
			{
				array = BuildDbsSignedMessage(dbsRequest, sUserId, out sMascId, out sLocalIpAddress, out sSourceType, out sBenchId, out tokenError);
			}
		}
		while (array == null && ++num < num2);
		if (array == null)
		{
			nErrorCode = -7;
			sErrorMessage = $"Failure to build {arg} Signed Message, reason {((object)(TOKEN_ERROR)(ref tokenError)).ToString()}";
			throw new NotSupportedException(sErrorMessage);
		}
		if ("no".Equals("yes", StringComparison.OrdinalIgnoreCase) && sClientRequestType.Equals("0x01"))
		{
			sPasswordChangeReq = "0x01";
			Smart.Log.Debug(TAG, $"setting password change req to 0x01, Read Subsidy Lock to 0x01");
		}
		byte[] array2 = null;
		num = 0;
		do
		{
			array2 = Smart.Web.DbsRequest(sLocalIpAddress, sMascId, sClientRequestType, productiontype, sOldImei, sNewImei, sOriginalImei, sPasswordChangeReq, array, logId, ref ServerId, ref nErrorCode, ref sErrorMessage);
			if (sClientRequestType.Equals("0x00") && nErrorCode == 8011)
			{
				Smart.Log.Error(TAG, $"error code 8011 is returned, changing old imei {sOldImei} to match new imei {sNewImei} in subsequent requests");
				sOldImei = sNewImei;
			}
		}
		while (array2 == null && ++num < num2);
		if (nErrorCode != 0)
		{
			if (string.IsNullOrEmpty(sErrorMessage))
			{
				nErrorCode = -9;
				sErrorMessage = "Failure to make web service call";
			}
			Smart.Log.Error(TAG, sErrorMessage);
			throw new NotSupportedException(sErrorMessage);
		}
		nDbsResponseSize = array2.Length;
		dbsResponse = array2;
		Smart.Log.Info(TAG, $"success. IMEI <{sNewImei}> sClientRequestType <{sClientRequestType}>, server data length {nDbsResponseSize}");
	}
}
