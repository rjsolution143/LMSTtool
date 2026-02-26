using System;
using System.Text;

namespace SmartDevice.Cfc;

public class ProgramDualIMEIDataBlock : ProgramDataBlock
{
	private string TAG => GetType().FullName;

	public string IsUlmaRequiredInDataBlockRequest { get; set; }

	public string ProgramFSGCarrierIDInDataBlockRequest { get; set; }

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
		string empty = string.Empty;
		char[] array = text3.ToCharArray();
		char[] array2 = text3.ToCharArray();
		string text4 = text3.Substring(ProgramDataBlock.DB_IMEI_OFFSET * 2, 128);
		int dEFAULT_DBS_SERIAL_NUMBER_SIZE = ProgramDataBlock.DEFAULT_DBS_SERIAL_NUMBER_SIZE;
		int num2 = int.Parse(text3.Substring(0, 2));
		char[] array3 = base.NewSerialNumber.ToCharArray();
		char[] array4 = base.IMEI2.ToCharArray();
		_ = string.Empty;
		_ = string.Empty;
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2] = array3[0];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 1] = 'A';
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 2] = array3[2];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 3] = array3[1];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 4] = array3[4];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 5] = array3[3];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 6] = array3[6];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 7] = array3[5];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 8] = array3[8];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 9] = array3[7];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 10] = array3[10];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 11] = array3[9];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 12] = array3[12];
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 13] = array3[11];
		if (num2 == 3)
		{
			array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 14] = array3[14];
		}
		else
		{
			array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 14] = '0';
		}
		array[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 15] = array3[13];
		array2[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2] = array4[0];
		array2[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 1] = 'A';
		array2[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 2] = array4[2];
		array2[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 3] = array4[1];
		array2[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 4] = array4[4];
		array2[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 5] = array4[3];
		array2[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 6] = array4[6];
		array2[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 7] = array4[5];
		array2[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 8] = array4[8];
		array2[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 9] = array4[7];
		array2[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 10] = array4[10];
		array2[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 11] = array4[9];
		array2[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 12] = array4[12];
		array2[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 13] = array4[11];
		if (num2 == 3)
		{
			array2[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 14] = array4[14];
		}
		else
		{
			array2[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 14] = '0';
		}
		array2[(ProgramDataBlock.DB_IMEI_LOCATION_OFFSET + num) * 2 + 15] = array4[13];
		char[] array5 = Ulma.ToCharArray();
		for (int i = 0; i < 12; i++)
		{
			array[(ProgramDataBlock.DB_ULMA_LOCATION_OFFSET + num) * 2 + i] = array5[i];
			array2[(ProgramDataBlock.DB_ULMA_LOCATION_OFFSET + num) * 2 + i] = array5[i];
		}
		if (!string.IsNullOrEmpty(FSG_Carrier_ID))
		{
			char[] array6 = FSG_Carrier_ID.ToCharArray();
			for (int j = 2; j < 4; j++)
			{
				array[(ProgramDataBlock.DB_CARRIER_ID_OFFSET + num) * 2 + j] = array6[j];
				array2[(ProgramDataBlock.DB_CARRIER_ID_OFFSET + num) * 2 + j] = array6[j];
			}
		}
		char[] array7 = "00".ToCharArray();
		char[] array8 = "01".ToCharArray();
		char[] array9 = "00".ToCharArray();
		char[] array10 = "00".ToCharArray();
		for (int k = 0; k < 2; k++)
		{
			array[(ProgramDataBlock.DB_IDENTITY_NUMBER_LOCATION_OFFSET + num) * 2 + k] = array7[k];
			array[(ProgramDataBlock.DB_IDENTITY_TYPE_LOCATION_OFFSET + num) * 2 + k] = array9[k];
			array2[(ProgramDataBlock.DB_IDENTITY_NUMBER_LOCATION_OFFSET + num) * 2 + k] = array8[k];
			array2[(ProgramDataBlock.DB_IDENTITY_TYPE_LOCATION_OFFSET + num) * 2 + k] = array10[k];
		}
		char[] array11 = "0002".ToCharArray();
		for (int l = 0; l < 4; l++)
		{
			array[ProgramDataBlock.DB_NUM_OF_DATABLOCKS_OFFSET * 2 + l] = array11[l];
		}
		text3 = new string(array);
		empty = new string(array2);
		dEFAULT_DBS_SERIAL_NUMBER_SIZE = base.NewSerialNumber.Length;
		StringBuilder stringBuilder = new StringBuilder();
		char[] array12 = base.NewSerialNumber.ToUpper().ToCharArray();
		foreach (char value in array12)
		{
			stringBuilder.Append(Convert.ToInt32(value).ToString("X2"));
		}
		stringBuilder.Append(new string('0', 128 - stringBuilder.Length));
		text4 = stringBuilder.ToString();
		Smart.Log.Debug(TAG, $"serial number in header: <{text4}>");
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.Append(BuildHeader(int.Parse(text3.Substring(0, 2)), text3.Substring(ProgramDataBlock.DB_NONCE_OFFSET * 2, 32), dEFAULT_DBS_SERIAL_NUMBER_SIZE, text4));
		stringBuilder2.Append(text3.Substring(ProgramDataBlock.DB_NUM_OF_DATABLOCKS_OFFSET * 2));
		stringBuilder2.Append(empty.Substring(ProgramDataBlock.DB_DATABLOCK_TYPE_OFFSET * 2));
		return stringBuilder2.ToString();
	}

	public void Execute(string imei, string imei2, string ulma, string fsg, Func<string, string, string> testcommand, string originalImei, string logId)
	{
		base.NewSerialNumber = imei;
		base.IMEI2 = imei2;
		if (ulma.Trim() != string.Empty)
		{
			Ulma = ulma;
		}
		else
		{
			Ulma = "000000000000";
		}
		if (fsg.Trim() != string.Empty)
		{
			FSG_Carrier_ID = fsg;
			if (FSG_Carrier_ID.ToLower().StartsWith("0x"))
			{
				FSG_Carrier_ID = FSG_Carrier_ID.Substring(2);
			}
		}
		base.DataBlockType = "000F";
		base.WebServiceDataBlockRequestType = "0x00";
		base.SerialNumberType = "00";
		base.OldSerialNumber = base.NewSerialNumber;
		Execute(testcommand, originalImei, logId);
	}
}
