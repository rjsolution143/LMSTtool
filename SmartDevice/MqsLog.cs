using System;
using System.Collections.Generic;
using System.Linq;
using ISmart;

namespace SmartDevice;

public class MqsLog
{
	private const string DEFAULT_TRACK_ID = "XXXXXXXXXX";

	private List<List<KeyValuePair<string, string>>> records = new List<List<KeyValuePair<string, string>>>();

	private List<KeyValuePair<string, string>> header;

	private DateTime logStart = DateTime.Now;

	private DateTime lastResult = DateTime.Now;

	private string trackId = "XXXXXXXXXX";

	private int sequenceNumber;

	private string currentModelNumber = string.Empty;

	private bool firstStepFailed = true;

	private string TAG => GetType().FullName;

	public List<KeyValuePair<string, string>> Header
	{
		get
		{
			return header;
		}
		set
		{
			header = value;
		}
	}

	public List<List<KeyValuePair<string, string>>> Records => records;

	public void CreateHeader(UseCase useCase, IDevice device, string mascId, string stationId, string computerName, string userId)
	{
		string unitId = Smart.Convert.GetDeviceSerialNumber(device);
		string logInfoValue;
		if ((logInfoValue = device.GetLogInfoValue("SerialNumberIn")) != string.Empty)
		{
			unitId = logInfoValue;
		}
		if (trackId == "XXXXXXXXXX")
		{
			trackId = device.ID;
		}
		if (trackId == "UNKNOWN")
		{
			trackId = unitId;
		}
		DateTime time = logStart;
		string process = ((object)(UseCase)(ref useCase)).ToString();
		string result = "I";
		TimeSpan processTime = TimeSpan.FromMilliseconds(0.0);
		string version = Smart.App.Version;
		header = Th3(trackId, string.Empty, time, process, mascId, stationId, string.Empty, processTime, result, string.Empty, unitId, userId, computerName, string.Empty, version, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
		UpdateDeviceDetails(device);
	}

	public void AddResult(UseCase useCase, IDevice device, SortedList<string, dynamic> details, Result result, bool lastStep)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Invalid comparison between Unknown and I4
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Invalid comparison between Unknown and I4
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Invalid comparison between Unknown and I4
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Invalid comparison between Unknown and I4
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Invalid comparison between Unknown and I4
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Invalid comparison between Unknown and I4
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Invalid comparison between Unknown and I4
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Invalid comparison between Unknown and I4
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Invalid comparison between Unknown and I4
		sequenceNumber++;
		TimeSpan timeSpan = DateTime.Now.Subtract(lastResult);
		if (trackId == "XXXXXXXXXX")
		{
			trackId = device.ID;
		}
		string unitId = Smart.Convert.GetDeviceSerialNumber(device);
		string logInfoValue;
		if ((logInfoValue = device.GetLogInfoValue("SerialNumberIn")) != string.Empty)
		{
			unitId = logInfoValue;
		}
		if (trackId == "UNKNOWN")
		{
			trackId = unitId;
		}
		string text = "F";
		if ((int)result == 8)
		{
			text = "P";
		}
		else if ((int)result == 7 || (int)result == 2)
		{
			text = "I";
		}
		else if ((int)result == 3)
		{
			text = "F";
		}
		string text2 = details["name"];
		string text3 = text2;
		string empty = string.Empty;
		string empty2 = string.Empty;
		if ((int)result == 1 || (int)result == 4 || (int)result == 3 || (int)result == 5)
		{
			_ = (string)(details["name"] + " failed");
			if ((int)result == 3)
			{
				_ = (string)("Audit failed for " + details["name"]);
			}
			string failDescription = details["description"];
			if (firstStepFailed)
			{
				firstStepFailed = false;
			}
			else
			{
				text2 = string.Empty;
				_ = string.Empty;
				failDescription = string.Empty;
			}
			List<KeyValuePair<string, string>> newRecord = Th3(trackId, string.Empty, logStart, ((object)(UseCase)(ref useCase)).ToString(), string.Empty, string.Empty, string.Empty, DateTime.Now.Subtract(logStart), text, string.Empty, unitId, string.Empty, string.Empty, text2, Smart.App.Version, failDescription, string.Empty, string.Empty, string.Empty, empty, empty2, string.Empty);
			header = UpdateRecord(header, newRecord);
		}
		UpdateDeviceDetails(device);
		if (lastStep)
		{
			List<KeyValuePair<string, string>> newRecord2 = Th3(trackId, string.Empty, logStart, ((object)(UseCase)(ref useCase)).ToString(), string.Empty, string.Empty, string.Empty, DateTime.Now.Subtract(logStart), text, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
			header = UpdateRecord(header, newRecord2);
		}
		double num = 0.0;
		double num2 = 0.0;
		double num3 = 0.0;
		if (details.ContainsKey("upperLimit"))
		{
			num = details["upperLimit"];
		}
		if (details.ContainsKey("lowerLimit"))
		{
			num2 = details["lowerLimit"];
		}
		if (details.ContainsKey("value"))
		{
			num3 = details["value"];
		}
		List<KeyValuePair<string, string>> item = Tr1(trackId, logStart, text3, 0, num3, 0, text, 0, num, num2, timeSpan, 1, 1, 0, sequenceNumber, details["description"], empty, empty2, string.Empty);
		records.Add(item);
		lastResult = DateTime.Now;
	}

	private void UpdateDeviceDetails(IDevice device)
	{
		string model = GetModel(device);
		if (model != currentModelNumber)
		{
			currentModelNumber = model;
			List<KeyValuePair<string, string>> newRecord = Th3(string.Empty, string.Empty, logStart, string.Empty, string.Empty, string.Empty, string.Empty, DateTime.Now.Subtract(logStart), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
			header = UpdateRecord(header, newRecord);
		}
		string unitId = Smart.Convert.GetDeviceSerialNumber(device);
		string logInfoValue;
		if ((logInfoValue = device.GetLogInfoValue("SerialNumberIn")) != string.Empty)
		{
			unitId = logInfoValue;
		}
		List<KeyValuePair<string, string>> newRecord2 = Th3(trackId, string.Empty, logStart, string.Empty, string.Empty, string.Empty, model, DateTime.Now.Subtract(logStart), string.Empty, string.Empty, unitId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
		header = UpdateRecord(header, newRecord2);
	}

	private List<KeyValuePair<string, string>> UpdateRecord(List<KeyValuePair<string, string>> oldRecord, List<KeyValuePair<string, string>> newRecord)
	{
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		foreach (KeyValuePair<string, string> item2 in oldRecord)
		{
			KeyValuePair<string, string> item = item2;
			foreach (KeyValuePair<string, string> item3 in newRecord)
			{
				if (item3.Key.ToLowerInvariant() == item2.Key.ToLowerInvariant())
				{
					string value = item3.Value;
					if (value != null && value != string.Empty)
					{
						item = new KeyValuePair<string, string>(item2.Key, item3.Value);
					}
					break;
				}
			}
			list.Add(item);
		}
		return list;
	}

	private List<KeyValuePair<string, string>> Tr1(string trackId, DateTime time, string test, int channel, double value, int frequency, string result, int voltage, double upperLimit, double lowerLimit, TimeSpan testTime, int attempts, int units, int retest, int sequenceNumber, string detailedCode, string level2Code, string level1Code, string testValue)
	{
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		AddField("TYPE", "TR1", 3, list);
		AddField("TRACKID", trackId, 50, list);
		string fieldValue = time.ToString("MM/dd/yy HH:mm:ss");
		AddField("TIME", fieldValue, 17, list);
		AddField("TESTCODE", test, 100, list);
		AddField("CHANNEL", channel.ToString(), 38, list);
		AddField("VALUE", value.ToString(), 38, list);
		AddField("FREQUENCY", frequency.ToString(), 38, list);
		AddField("RESULT", result, 1, list);
		AddField("VOLTAGE", voltage.ToString(), 38, list);
		AddField("LOWERLIMIT", lowerLimit.ToString(), 38, list);
		AddField("UPPERLIMIT", upperLimit.ToString(), 38, list);
		AddField("TESTTIME", testTime.TotalSeconds.ToString(), 38, list);
		AddField("ATTEMPTS", attempts.ToString(), 38, list);
		AddField("UNITS", units.ToString(), 38, list);
		AddField("RETEST", retest.ToString(), 1, list);
		AddField("SEQUENCE", sequenceNumber.ToString(), 38, list);
		AddField("DETAILEDCODE", detailedCode, 255, list);
		AddField("LEVEL2CODE", level2Code, 100, list);
		AddField("LEVEL1CODE", level1Code, 100, list);
		AddField("TESTVALUE", testValue, 60, list);
		return list;
	}

	private List<KeyValuePair<string, string>> Th3(string trackId, string subAssm, DateTime time, string process, string station, string fixture, string modelNumber, TimeSpan processTime, string result, string shopOrder, string unitId, string carrierId, string carrierSite, string failCode, string clientVersion, string failDescription, string level2Model, string level1Model, string detailedCode, string level2Code, string level1Code, string errorCode)
	{
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		AddField("TYPE", "TH3", 3, list);
		AddField("TRACKID", trackId, 50, list);
		AddField("SUBASSM", subAssm, 50, list);
		string fieldValue = time.ToString("MM/dd/yy HH:mm:ss");
		AddField("TIME", fieldValue, 17, list);
		AddField("PROCESS", process, 50, list);
		AddField("STATION", station, 50, list);
		AddField("FIXTURE", fixture, 50, list);
		if (modelNumber != null && modelNumber.Contains("/"))
		{
			modelNumber = modelNumber[..modelNumber.IndexOf("/")];
		}
		AddField("MODELNO", modelNumber, 50, list);
		AddField("PROCESSTIME", processTime.TotalSeconds.ToString(), 38, list);
		AddField("RESULT", result, 1, list);
		AddField("SHOPORDER", shopOrder, 50, list);
		AddField("UNITID", unitId, 50, list);
		AddField("CARRIERID", carrierId, 50, list);
		AddField("CARRIERSITE", carrierSite, 50, list);
		AddField("FAILCODE", failCode, 100, list);
		AddField("CLIENTVER", clientVersion, 50, list);
		AddField("FAILDESC", failDescription, 255, list);
		AddField("LEVEL2MODEL", level2Model, 50, list);
		AddField("LEVEL1MODEL", level1Model, 50, list);
		AddField("DETAILEDCODE", detailedCode, 255, list);
		AddField("LEVEL2CODE", level2Code, 100, list);
		AddField("LEVEL1CODE", level1Code, 100, list);
		AddField("ERRORCODE", errorCode, 60, list);
		return list;
	}

	private void AddField(string fieldName, string fieldValue, int maxLength, List<KeyValuePair<string, string>> record)
	{
		if (fieldValue.Length > maxLength)
		{
			Smart.Log.Warning(TAG, $"{fieldName} should be less than {maxLength} characters: '{fieldValue}'");
			fieldValue = fieldValue.Substring(0, maxLength);
		}
		if (Enumerable.Contains(fieldValue, ','))
		{
			Smart.Log.Warning(TAG, "MQS field contains illegal comma: " + fieldValue);
			fieldValue = fieldValue.Replace(',', '.');
		}
		record.Add(new KeyValuePair<string, string>(fieldName, fieldValue));
	}

	public override string ToString()
	{
		string text = string.Empty;
		foreach (List<KeyValuePair<string, string>> record in records)
		{
			string text2 = RecordToString(record);
			text = text + text2 + "\n";
		}
		return text + RecordToString(header);
	}

	private string RecordToString(List<KeyValuePair<string, string>> record)
	{
		string text = string.Empty;
		foreach (KeyValuePair<string, string> item in record)
		{
			text = text + item.Value + ",";
		}
		if (text.Length > 0)
		{
			text = text.Substring(0, text.Length - 1);
		}
		return text;
	}

	private string GetModel(IDevice device)
	{
		string result = string.Empty;
		if (device.ModelId != string.Empty)
		{
			result = device.ModelId.Split(new char[1] { '|' })[0];
		}
		return result;
	}
}
