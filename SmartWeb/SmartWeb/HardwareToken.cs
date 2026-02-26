using System;
using System.IO;
using ISmart;

namespace SmartWeb;

public class HardwareToken
{
	public enum PkiType
	{
		Cek0118 = 280,
		Cek015B = 347,
		EnterpriseFF01 = 65281,
		Hdcp012A = 298,
		Hdcp012B = 299,
		Iprm00F5 = 245,
		Iprm00F6 = 246,
		Iprm00F7 = 247,
		Iprm0100 = 256,
		Iprm0122 = 290,
		Iprm0123 = 291,
		Iprm015D = 349,
		Janus0018 = 24,
		Janus3014 = 12308,
		Janus5014 = 20500,
		PlayReady0131 = 305,
		PlayReady0132 = 306,
		Sbk00EB = 235,
		Widevine0142 = 322,
		Widevine015F = 351,
		Widevine0172 = 370,
		Wimax04DF = 1247,
		Wimax04A8 = 1192
	}

	private string TAG => GetType().FullName;

	private TokenWrapper token { get; set; }

	public TokenWrapper RawToken => token;

	public TokenInfo Info
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			TokenInfo tokenInfo = TokenInfo.BlankInfo;
			AddExtendedMessage(GenerateTestUnlockMessage(), "testuser", out tokenInfo);
			return tokenInfo;
		}
	}

	public HardwareToken()
	{
		token = new TokenWrapper();
	}

	public byte[] GenerateUnlockMessage(string serialNumber)
	{
		Smart.Log.Debug(TAG, $"Building unlock message for {serialNumber}");
		byte[] array = new byte[1795];
		byte[] array2 = Smart.Convert.UShortToBytes((ushort)1795);
		array[0] = array2[0];
		array[1] = array2[1];
		array[2] = 1;
		array[3] = 5;
		array[4] = 0;
		if (serialNumber.Length > 255)
		{
			Smart.Log.Warning(TAG, $"Invalid serial number size bytes for {serialNumber}: {serialNumber.Length}");
		}
		array[5] = (byte)serialNumber.Length;
		byte[] array3 = Smart.Convert.AsciiToBytes(serialNumber);
		if (array3.Length > 63)
		{
			Smart.Log.Warning(TAG, $"Invalid serial number bytes size for {serialNumber}: {array3.Length}");
		}
		Array.Copy(array3, 0, array, 6, array3.Length);
		array[126] = 0;
		array[127] = 1;
		array[128] = 0;
		array[129] = 102;
		array[130] = 0;
		array[131] = 0;
		array[132] = 0;
		array[133] = 0;
		array[134] = 1;
		array[135] = 48;
		for (int i = 304; i <= 431; i++)
		{
			array[i] = byte.MaxValue;
		}
		array[432] = 5;
		array[433] = 77;
		array[434] = 1;
		array[435] = 1;
		return array;
	}

	public byte[] GenerateTestUnlockMessage()
	{
		return GenerateUnlockMessage("99999999999999");
	}

	public byte[] BuildUnlockMessage(string serialNumber, string userName)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		TokenInfo tokenInfo = TokenInfo.BlankInfo;
		byte[] message = GenerateUnlockMessage(serialNumber);
		byte[] message2 = AddExtendedMessage(message, userName, out tokenInfo);
		return AddCrcToMessage(message2);
	}

	public byte[] GeneratePkiMessage(string serialNumber, string pkiType, string dhKey)
	{
		ushort num = 1594;
		byte[] array = new byte[num];
		byte[] array2 = Smart.Convert.UShortToBytes(num);
		array[0] = array2[0];
		array[1] = array2[1];
		array[2] = 2;
		array[3] = 1;
		array[4] = 0;
		array[5] = 1;
		byte[] array3 = Smart.Convert.HexToBytes(pkiType);
		Smart.Log.Assert(TAG, array3.Length == 2, "PKI type should be two bytes");
		array[6] = array3[0];
		array[7] = array3[1];
		byte[] array4 = Smart.Convert.AsciiToBytes(serialNumber);
		array[38] = (byte)array4.Length;
		Array.Copy(array4, 0, array, 39, array4.Length);
		byte[] array5 = Smart.Convert.HexToBytes(dhKey);
		Smart.Log.Assert(TAG, array5.Length == 128, "Diffie-Hellman key should be 128 bytes");
		Array.Copy(array5, 0, array, 103, array5.Length);
		ushort num2 = 1357;
		byte[] array6 = Smart.Convert.UShortToBytes(num2);
		array[231] = array6[0];
		array[232] = array6[1];
		Smart.Log.Verbose(TAG, Smart.Convert.BytesToHex(array));
		return array;
	}

	public byte[] BuildPkiMessage(string serialNumber, PkiType pkiType, string dhKey, string userName)
	{
		byte[] array = Smart.Convert.UShortToBytes((ushort)pkiType);
		string pkiType2 = Smart.Convert.BytesToHex(array);
		return BuildPkiMessage(serialNumber, pkiType2, dhKey, userName);
	}

	public byte[] BuildPkiMessage(string serialNumber, string pkiType, string dhKey, string userName)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		TokenInfo tokenInfo = TokenInfo.BlankInfo;
		byte[] message = GeneratePkiMessage(serialNumber, pkiType, dhKey);
		byte[] message2 = AddExtendedMessage(message, userName, out tokenInfo);
		return AddCrcToMessage(message2);
	}

	public byte[] BuildDbsMessage(byte[] unsignedMessage, string userName)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		TokenInfo tokenInfo = TokenInfo.BlankInfo;
		byte[] message = AddExtendedMessage(unsignedMessage, userName, out tokenInfo);
		return AddCrcToMessage(message);
	}

	public byte[] AddExtendedMessage(byte[] message, string userName, out TokenInfo tokenInfo)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		byte[] requestMsg = (byte[])message.Clone();
		string ipAddress = "UNKNOWN";
		string location = "UNKNOWN";
		string sourceType = "UNKNOWN";
		string benchId = "UNKNOWN";
		TokenWrapper.TOKEN_ERROR tOKEN_ERROR = token.GenerateDBSRequest(1, ref requestMsg, userName, out ipAddress, out location, out sourceType, out benchId);
		if (tOKEN_ERROR != 0)
		{
			string text = $"Error using hardware token: {tOKEN_ERROR}";
			Smart.Log.Error(TAG, text);
			throw new IOException(text);
		}
		tokenInfo = new TokenInfo(ipAddress, location, sourceType, benchId);
		return requestMsg;
	}

	public byte[] AddCrcToMessage(byte[] message)
	{
		byte[] obj = (byte[])message.Clone();
		int num = 4;
		int num2 = obj.Length - num;
		int num3 = Crc.Compute(obj, 0, num2);
		byte[] array = Smart.Convert.IntToBytes(num3);
		obj[num2] = array[0];
		obj[num2 + 1] = array[1];
		obj[num2 + 2] = array[2];
		obj[num2 + 3] = array[3];
		return obj;
	}
}
