using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartDevice.Cfc;

public class ProgramCidLockDataBlock : ProgramDataBlock
{
	protected enum DataBlockTypes
	{
		IMEI,
		MEID,
		MSN
	}

	private bool _updatepsnindatablockrequest;

	private bool _usedefaultserialnumbervalues;

	private const string CharList = "0123456789abcdefghijklmnopqrstuvwxyz";

	private string TAG => GetType().FullName;

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

	public bool UseDefaultSerialNumberValues
	{
		get
		{
			return _usedefaultserialnumbervalues;
		}
		set
		{
			_usedefaultserialnumbervalues = value;
		}
	}

	public string Channel_ID { get; set; }

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

	protected override string BuildRequest(string testcommanddata)
	{
		string text = testcommanddata.Substring(2);
		string text2 = text.Substring(ProgramDataBlock.DB_IMEI_OFFSET * 2, 128);
		int nSerialNumberSize = ProgramDataBlock.DEFAULT_DBS_SERIAL_NUMBER_SIZE;
		string text3 = testcommanddata.Substring(ProgramDataBlock.DB_DATA_BLOCK_TYPE_LOCATION * 2, 4);
		string text4 = testcommanddata.Substring(ProgramDataBlock.DB_CID_VERSION_LOCATION * 2, 4);
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
			nSerialNumberSize = base.NewSerialNumber.Length;
			if (CurrentDataBlockType == DataBlockTypes.IMEI)
			{
				array2 = base.NewSerialNumber.ToUpper().ToCharArray();
				char[] array4 = new char[16];
				char[] array5 = ("A" + base.NewSerialNumber.ToUpper()).ToCharArray();
				for (int i = 0; i <= array5.Length - 2; i += 2)
				{
					array4[i] = array5[i + 1];
					array4[i + 1] = array5[i];
				}
				array2 = array4;
				array3 = ("00" + base.SerialNumberType).ToCharArray();
			}
			else if (CurrentDataBlockType == DataBlockTypes.MEID)
			{
				array2 = ((base.NewSerialNumber.Length == 16) ? base.NewSerialNumber.ToUpper().ToCharArray() : (base.NewSerialNumber.ToUpper() + new string('0', 16 - base.NewSerialNumber.Length)).ToCharArray());
				array3 = ("00" + base.SerialNumberType).ToCharArray();
			}
			else if (CurrentDataBlockType == DataBlockTypes.MSN)
			{
				array2 = DecodeBase36AsString(base.NewSerialNumber).ToUpper().ToCharArray();
				array3 = ("00" + base.SerialNumberType).ToCharArray();
			}
			int num = 0;
			int num2 = 0;
			if (text4.Equals("0002"))
			{
				num2 = ProgramDataBlock.DBS_CIDV2_OFFSET;
				Smart.Log.Debug(TAG, $"datablock type: {text3}, cid version: {text4}, update PSN: {UpdatePSNInDataBlockRequest}, offset: {num2}");
			}
			char[] array6 = array3;
			foreach (char c in array6)
			{
				array[(ProgramDataBlock.DB_PSN_TYPE_LOCATION_OFFSET + num2) * 2 + num++] = c;
			}
			int num3 = 0;
			array6 = array2;
			foreach (char c2 in array6)
			{
				array[(ProgramDataBlock.DB_PSN_LOCATION_OFFSET + num2) * 2 + num3++] = c2;
			}
			Smart.Log.Debug(TAG, $"Setting Channel ID to: {Channel_ID}");
			if (!string.IsNullOrEmpty(Channel_ID))
			{
				num3 = 0;
				array6 = Channel_ID.ToCharArray();
				foreach (char c3 in array6)
				{
					array[(ProgramDataBlock.DB_CHANNEL_ID_OFFSET + num2) * 2 + num3++] = c3;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			array6 = base.NewSerialNumber.ToUpper().ToCharArray();
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
		stringBuilder2.Append(BuildHeader(int.Parse(text.Substring(0, 2)), text.Substring(ProgramDataBlock.DB_NONCE_OFFSET * 2, 32), nSerialNumberSize, text2));
		stringBuilder2.Append(text.Substring(ProgramDataBlock.DB_REQUEST_TYPE_HEADER_LEN * 2));
		return stringBuilder2.ToString();
	}

	public void Execute(string imei, string deviceImei, Func<string, string, string> testcommand, string originalImei, string logId)
	{
		base.DataBlockType = "00F0";
		base.WebServiceDataBlockRequestType = "0x02";
		base.SerialNumberType = "00";
		if (CurrentDataBlockType == DataBlockTypes.IMEI)
		{
			base.SerialNumberType = "00";
		}
		if (CurrentDataBlockType == DataBlockTypes.MEID)
		{
			base.SerialNumberType = "05";
		}
		if (CurrentDataBlockType == DataBlockTypes.MSN)
		{
			base.SerialNumberType = "04";
		}
		Smart.Log.Debug(TAG, $"program cid test using IMEI value {imei}");
		if (!string.IsNullOrEmpty(deviceImei))
		{
			base.OldSerialNumber = deviceImei;
		}
		else
		{
			Smart.Log.Debug(TAG, $"no phone imei read, using unit data set value {imei} as old serial number");
			base.OldSerialNumber = imei;
		}
		if (!UseDefaultSerialNumberValues)
		{
			base.NewSerialNumber = imei;
		}
		Execute(testcommand, originalImei, logId);
	}
}
