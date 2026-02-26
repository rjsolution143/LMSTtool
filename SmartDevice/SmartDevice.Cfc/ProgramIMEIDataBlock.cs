using System;
using System.Text;

namespace SmartDevice.Cfc;

public class ProgramIMEIDataBlock : ProgramDataBlock
{
	public enum PreConditionEnum
	{
		TestPassed = 0,
		TestSkipped = 3,
		ImeiAlreadyProgrammed = 4,
		ImeiGotProgrammed = 5
	}

	private string TAG => GetType().FullName;

	public string IsUlmaRequiredInDataBlockRequest { get; set; }

	public string ProgramFSGCarrierIDInDataBlockRequest { get; set; }

	public bool ConvertImeiToDMeid { get; set; }

	public string Ulma { get; set; }

	public string FSG_Carrier_ID { get; set; }

	protected override string BuildRequest(string testcommanddata)
	{
		string text = testcommanddata.Substring(ProgramDataBlock.DB_DATA_BLOCK_TYPE_LOCATION * 2, 4);
		string text2 = testcommanddata.Substring(ProgramDataBlock.DB_CID_VERSION_LOCATION * 2, 4);
		int num = 0;
		Smart.Log.Debug(TAG, $"datablocktype {text}, version {text2}");
		if (text.ToLower().Equals("000f") && text2.ToLower().Equals("0002"))
		{
			Smart.Log.Debug(TAG, "found cid version 2, setting offset addr");
			num = 2;
		}
		string text3 = testcommanddata.Substring(2);
		char[] array = text3.ToCharArray();
		int num2 = int.Parse(text3.Substring(0, 2));
		int dEFAULT_DBS_SERIAL_NUMBER_SIZE = ProgramDataBlock.DEFAULT_DBS_SERIAL_NUMBER_SIZE;
		string text4 = text3.Substring(ProgramDataBlock.DB_IMEI_OFFSET * 2, 128);
		char[] array2 = base.NewSerialNumber.ToCharArray();
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2] = array2[0];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 1] = 'A';
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 2] = array2[2];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 3] = array2[1];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 4] = array2[4];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 5] = array2[3];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 6] = array2[6];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 7] = array2[5];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 8] = array2[8];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 9] = array2[7];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 10] = array2[10];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 11] = array2[9];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 12] = array2[12];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 13] = array2[11];
		if (num2 == 3)
		{
			array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 14] = array2[14];
		}
		else
		{
			array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 14] = '0';
		}
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 15] = array2[13];
		char[] array3 = Ulma.ToCharArray();
		for (int i = 0; i < 12; i++)
		{
			array[(ProgramDataBlock.DB_ULMA_LOCATION_OFFSET + num) * 2 + i] = array3[i];
		}
		char[] array4 = "0000".ToCharArray();
		for (int j = 0; j < 4; j++)
		{
			array[(ProgramDataBlock.DB_GSM_IDENTITY_TYPE_LOCATION_OFFSET + num) * 2 + j] = array4[j];
		}
		if (!string.IsNullOrEmpty(FSG_Carrier_ID))
		{
			char[] array5 = FSG_Carrier_ID.ToCharArray();
			for (int k = 2; k < 4; k++)
			{
				array[(ProgramDataBlock.DB_CARRIER_ID_OFFSET + num) * 2 + k] = array5[k];
			}
		}
		text3 = new string(array);
		dEFAULT_DBS_SERIAL_NUMBER_SIZE = base.NewSerialNumber.Length;
		StringBuilder stringBuilder = new StringBuilder();
		char[] array6 = base.NewSerialNumber.ToUpper().ToCharArray();
		foreach (char value in array6)
		{
			stringBuilder.Append(Convert.ToInt32(value).ToString("X2"));
		}
		stringBuilder.Append(new string('0', 128 - stringBuilder.Length));
		text4 = stringBuilder.ToString();
		Smart.Log.Debug(TAG, $"serial number in header: <{text4}>");
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.Append(BuildHeader(int.Parse(text3.Substring(0, 2)), text3.Substring(ProgramDataBlock.DB_NONCE_OFFSET * 2, 32), dEFAULT_DBS_SERIAL_NUMBER_SIZE, text4));
		stringBuilder2.Append(text3.Substring(ProgramDataBlock.DB_REQUEST_TYPE_HEADER_LEN * 2));
		return stringBuilder2.ToString();
	}

	public void Execute(string imei, string ulma, string fsg, Func<string, string, string> testcommand, string originalImei, string logId)
	{
		base.NewSerialNumber = imei;
		if (!string.IsNullOrEmpty(ulma))
		{
			Ulma = ulma;
		}
		else
		{
			Ulma = "000000000000";
		}
		if (!string.IsNullOrEmpty(fsg))
		{
			FSG_Carrier_ID = fsg;
			if (FSG_Carrier_ID.ToLower().StartsWith("0x"))
			{
				FSG_Carrier_ID = FSG_Carrier_ID.Substring(2);
			}
		}
		else
		{
			FSG_Carrier_ID = string.Empty;
		}
		base.DataBlockType = "000F";
		base.WebServiceDataBlockRequestType = "0x00";
		base.SerialNumberType = "00";
		base.OldSerialNumber = imei;
		Execute(testcommand, originalImei, logId);
	}
}
