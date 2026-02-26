using System;
using System.Collections.Generic;
using System.IO;
using ISmart;

namespace SmartDevice;

public class MqsLogger : IResultSubLogger, IDisposable
{
	private const string UNKNOWN_SERIAL_NUMBER = "000000000000000";

	private IDevice device;

	private string logDirectory = string.Empty;

	private DateTime logStart = DateTime.Now;

	private MqsLog mqsLog = new MqsLog();

	private string logPath;

	private string serialNumber = "000000000000000";

	private Dictionary<string, string> dataSets = new Dictionary<string, string>();

	private string TAG => GetType().FullName;

	public string Name => "MQS";

	public UseCase UseCase { get; set; }

	public bool IsOpen
	{
		get
		{
			if (logPath != null)
			{
				return mqsLog != null;
			}
			return false;
		}
	}

	public MqsLogger(IDevice device)
	{
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		Smart.Log.Debug(TAG, "MQS log opened for " + device.ID.ToString());
		string deviceSerialNumber = Smart.Convert.GetDeviceSerialNumber(device);
		this.device = device;
		logDirectory = GetLogPath();
		if (deviceSerialNumber != string.Empty)
		{
			serialNumber = deviceSerialNumber;
		}
		string text;
		if ((text = device.GetLogInfoValue("NewSerialNumber").Trim()) != string.Empty)
		{
			serialNumber = text;
		}
		string arg = DateTime.Now.ToString("MM-dd-yyyy.HH.mm.ss");
		string text2 = $"{serialNumber}.rslt.{arg}";
		logPath = Smart.File.PathJoin(logDirectory, text2);
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		string text3 = $"{((StationDescriptor)(ref stationDescriptor)).RegionId}-{((StationDescriptor)(ref stationDescriptor)).SavedShopId}";
		string stationId = $"{text3}-{((StationDescriptor)(ref stationDescriptor)).StationId}";
		mqsLog.CreateHeader(UseCase, device, text3, stationId, ((StationDescriptor)(ref stationDescriptor)).StationName, ((StationDescriptor)(ref stationDescriptor)).UserName.ToUpperInvariant());
	}

	public void AddResult(string name, SortedList<string, dynamic> details)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if (!IsOpen)
		{
			throw new NotSupportedException("MQS Log is not open for writing");
		}
		UseCase useCase = UseCase;
		bool lastStep = name == ((object)(UseCase)(ref useCase)).ToString();
		Result result = (Result)details["result"];
		_ = new string[3]
		{
			details["name"],
			((object)(Result)(ref result)).ToString(),
			"LastStep: " + lastStep
		};
		mqsLog.AddResult(UseCase, device, details, result, lastStep);
		Smart.File.WriteText(logPath, mqsLog.ToString());
	}

	public void AddInfo(string name, string value)
	{
		if (!IsOpen)
		{
			throw new NotSupportedException("MQS log is not open for writing");
		}
		if (!dataSets.ContainsKey(name))
		{
			dataSets.Add(name, value);
		}
	}

	public void Dispose()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		if (!IsOpen)
		{
			return;
		}
		if ((int)UseCase == 134)
		{
			try
			{
				File.Delete(logPath);
			}
			catch (Exception)
			{
			}
		}
		else
		{
			string deviceSerialNumber = Smart.Convert.GetDeviceSerialNumber(device);
			if (logPath.Contains("000000000000000") && deviceSerialNumber != "000000000000000")
			{
				string destFileName = logPath.Replace("000000000000000", deviceSerialNumber);
				try
				{
					File.Move(logPath, destFileName);
					logPath = destFileName;
				}
				catch (Exception)
				{
				}
			}
			Smart.Rsd.UploadMQSLog(logDirectory, Path.GetFileName(logPath));
		}
		logPath = null;
		mqsLog = null;
		Smart.Log.Info(TAG, "MQS log closed for " + device.ID.ToString());
	}

	private string GetLogPath()
	{
		string result = Smart.File.CommonStorageDir;
		string text = string.Empty;
		IThreadLocked val = Smart.Rsd.LocalOptions();
		try
		{
			dynamic data = val.Data;
			text = data.MQSLogPath;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		if (!string.IsNullOrEmpty(text))
		{
			if (Directory.Exists(text))
			{
				result = text;
			}
			else
			{
				try
				{
					Directory.CreateDirectory(text);
					result = text;
				}
				catch (Exception ex)
				{
					Smart.Log.Error(TAG, $"Failed to create MQS log path {text} - errorMsg: {ex.Message}");
				}
			}
		}
		return result;
	}
}
