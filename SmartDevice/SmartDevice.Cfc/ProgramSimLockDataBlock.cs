using System;
using System.Text;

namespace SmartDevice.Cfc;

public class ProgramSimLockDataBlock : ProgramDataBlock
{
	protected enum DataBlockTypes
	{
		IMEI,
		MEID,
		MSN
	}

	private string TAG => GetType().FullName;

	protected DataBlockTypes CurrentDataBlockType { get; set; }

	protected override string BuildRequest(string testcommanddata)
	{
		string empty = string.Empty;
		string text = testcommanddata.Substring(2);
		if (int.Parse(text.Substring(0, 2)) == 3)
		{
			int dEFAULT_DBS_SERIAL_NUMBER_SIZE = ProgramDataBlock.DEFAULT_DBS_SERIAL_NUMBER_SIZE;
			string text2 = text.Substring(ProgramDataBlock.DB_IMEI_OFFSET * 2, 128);
			dEFAULT_DBS_SERIAL_NUMBER_SIZE = base.NewSerialNumber.Length;
			if (CurrentDataBlockType == DataBlockTypes.IMEI)
			{
				StringBuilder stringBuilder = new StringBuilder();
				char[] array = base.NewSerialNumber.ToUpper().ToCharArray();
				foreach (char value in array)
				{
					stringBuilder.Append(Convert.ToInt32(value).ToString("X2"));
				}
				stringBuilder.Append(new string('0', 128 - stringBuilder.Length));
				text2 = stringBuilder.ToString();
				Smart.Log.Debug(TAG, $"serial number in header: <{text2}>");
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append(BuildHeader(int.Parse(text.Substring(0, 2)), text.Substring(ProgramDataBlock.DB_NONCE_OFFSET * 2, 32), dEFAULT_DBS_SERIAL_NUMBER_SIZE, text2));
			stringBuilder2.Append(text.Substring(ProgramDataBlock.DB_REQUEST_TYPE_HEADER_LEN * 2));
			empty = stringBuilder2.ToString();
		}
		else
		{
			empty = base.BuildRequest(testcommanddata);
			Smart.Log.Debug(TAG, $"buildRequest returned from base.BuildRequest: {empty}");
			if (CurrentDataBlockType == DataBlockTypes.IMEI)
			{
				base.NewSerialNumber.ToUpper().ToCharArray();
				char[] array2 = new char[16];
				char[] array3 = ("A" + base.NewSerialNumber.ToUpper()).ToCharArray();
				for (int j = 0; j <= array3.Length - 2; j += 2)
				{
					array2[j] = array3[j + 1];
					array2[j + 1] = array3[j];
				}
				char[] array4 = empty.ToCharArray();
				int num = 0;
				int num2 = 12;
				char[] array = array2;
				foreach (char c in array)
				{
					array4[num2 + num++] = c;
				}
				empty = new string(array4);
			}
		}
		Smart.Log.Debug(TAG, $"buildRequest returned in ProgramSimLockDataBlock: {empty}");
		return empty;
	}

	public void Execute(string oldSerialNumber, string newSerialNumber, string snType, Func<string, string, string> testcommand, string originalImei, string logId, string simLockDataBlockType)
	{
		base.DataBlockType = simLockDataBlockType;
		Smart.Log.Debug(TAG, $"SIMLOCK Datablock Type is 0x{base.DataBlockType}");
		base.WebServiceDataBlockRequestType = "0x01";
		base.SerialNumberType = "00";
		if (string.Compare(snType, "IMEI", ignoreCase: true) == 0)
		{
			string text = newSerialNumber;
			CurrentDataBlockType = DataBlockTypes.IMEI;
			if (string.IsNullOrEmpty(text))
			{
				text = "000000123456782";
				Smart.Log.Debug(TAG, $"no unit data set value for IMEI, take default value {text} as IMEI number");
			}
			if (!string.IsNullOrEmpty(oldSerialNumber))
			{
				base.OldSerialNumber = oldSerialNumber;
				Smart.Log.Debug(TAG, $"the old IMEI value {base.OldSerialNumber}");
			}
			else
			{
				base.OldSerialNumber = text;
				Smart.Log.Debug(TAG, $"take default value {text} as old serial number");
			}
			base.SerialNumberType = "00";
			base.NewSerialNumber = text;
		}
		else if (string.Compare(snType, "MEID", ignoreCase: true) == 0)
		{
			CurrentDataBlockType = DataBlockTypes.MEID;
			Smart.Log.Debug(TAG, $"program sim datablock using MEID value {newSerialNumber}");
			if (!string.IsNullOrEmpty(oldSerialNumber))
			{
				base.OldSerialNumber = oldSerialNumber;
			}
			else
			{
				Smart.Log.Debug(TAG, $"no phone meid read, using unit data set value {newSerialNumber} as old serial number");
				base.OldSerialNumber = newSerialNumber;
			}
			base.SerialNumberType = "05";
			base.NewSerialNumber = newSerialNumber;
		}
		else
		{
			if (string.Compare(snType, "MSN", ignoreCase: true) != 0)
			{
				throw new NotSupportedException("SN Type not supported: " + snType);
			}
			CurrentDataBlockType = DataBlockTypes.MSN;
			Smart.Log.Debug(TAG, $"program sim datablock using MSN value {newSerialNumber}");
			if (!string.IsNullOrEmpty(oldSerialNumber))
			{
				base.OldSerialNumber = oldSerialNumber;
			}
			else
			{
				Smart.Log.Debug(TAG, $"no phone msn read, using unit data set value {newSerialNumber} as old serial number");
				base.OldSerialNumber = newSerialNumber;
			}
			base.SerialNumberType = "04";
			base.NewSerialNumber = newSerialNumber;
		}
		Execute(testcommand, originalImei, logId);
	}
}
