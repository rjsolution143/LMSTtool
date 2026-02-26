using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ISmart;

namespace SmartDevice;

public class Device : IDevice, INotifyPropertyChanged
{
	private IPrompt prompt = Smart.NewPrompt();

	private string id = string.Empty;

	private string serialNumber = string.Empty;

	private string serialNumber2 = string.Empty;

	private string modelId = string.Empty;

	private IResultLogger log;

	private bool locked;

	private bool removed;

	private DateTime lastDisconnect = DateTime.Now.Subtract(TimeSpan.FromSeconds(120.0));

	private string TAG => GetType().FullName;

	public string Unique { get; private set; }

	public IPrompt Prompt => prompt;

	public string ID
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
			OnPropertyChanged("ID");
		}
	}

	public string SerialNumber
	{
		get
		{
			return serialNumber;
		}
		set
		{
			serialNumber = value;
			OnPropertyChanged("SerialNumber");
		}
	}

	public string SerialNumber2
	{
		get
		{
			return serialNumber2;
		}
		set
		{
			serialNumber2 = value;
			OnPropertyChanged("SerialNumber2");
		}
	}

	public DeviceMode Mode { get; protected set; }

	public DeviceMode LastMode { get; protected set; }

	public DeviceMode DetectMode { get; set; }

	public bool UnknownMode
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Invalid comparison between Unknown and I4
			if ((int)Mode != 1)
			{
				return (int)LastMode == 1;
			}
			return true;
		}
	}

	public string ModelId
	{
		get
		{
			return modelId;
		}
		set
		{
			modelId = value;
			OnPropertyChanged("ModelId");
		}
	}

	public bool Locked
	{
		get
		{
			return locked;
		}
		set
		{
			locked = value;
			OnPropertyChanged("Locked");
		}
	}

	public IResultLogger Log
	{
		get
		{
			return log;
		}
		set
		{
			log = value;
			OnPropertyChanged("Log");
		}
	}

	public string RoCarrier { get; set; }

	public string RecordId { get; set; }

	public string IP { get; set; }

	public string ManufacturingDate { get; set; }

	public bool ManualDevice { get; set; }

	public string UserSelectedCarrier { get; set; } = string.Empty;


	public string UserSelectedModel { get; set; } = string.Empty;


	public string UserSerialNumber { get; set; } = string.Empty;


	public DeviceType Type { get; set; }

	public string Group { get; set; } = string.Empty;


	public bool WiFiOnlyDevice { get; set; }

	public string GSN { get; set; } = string.Empty;


	public string PSN { get; set; } = string.Empty;


	public bool Communicating { get; set; }

	public DateTime LastConnected { get; set; } = DateTime.Now;


	public bool MultiUp { get; set; }

	public bool Removed
	{
		get
		{
			return removed;
		}
		set
		{
			removed = value;
			OnPropertyChanged("Removed");
		}
	}

	public string ScannedGsn { get; set; } = string.Empty;


	public string ScannedTrackId { get; set; } = string.Empty;


	public bool ZeroTouchDevice { get; set; }

	public bool OutOfProfile { get; set; }

	public bool InvalidSerialNumber { get; set; }

	public int PortIndex { get; set; }

	public bool Automated { get; set; }

	public string PnpDbccName { get; set; } = string.Empty;


	public event PropertyChangedEventHandler PropertyChanged;

	public Device()
	{
		Unique = Smart.File.Uuid();
		Smart.Log.Verbose(TAG, $"New device created {Unique}");
		prompt.Device = (IDevice)(object)this;
		ManualDevice = false;
		Removed = false;
		Mode = (DeviceMode)1;
		LastMode = (DeviceMode)1;
		DetectMode = (DeviceMode)1;
	}

	protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
	{
		if (this.PropertyChanged != null)
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public void MergeDevice(IDevice otherDevice)
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		ID = otherDevice.ID;
		if (otherDevice.SerialNumber != string.Empty && otherDevice.SerialNumber != "UNKNOWN")
		{
			SerialNumber = otherDevice.SerialNumber;
		}
		if (otherDevice.SerialNumber2 != string.Empty && otherDevice.SerialNumber2 != "UNKNOWN")
		{
			SerialNumber = otherDevice.SerialNumber;
		}
		if (otherDevice.GSN != string.Empty && otherDevice.GSN != "UNKNOWN")
		{
			GSN = otherDevice.GSN;
		}
		if (otherDevice.PSN != string.Empty && otherDevice.PSN != "UNKNOWN")
		{
			PSN = otherDevice.PSN;
		}
		Mode = otherDevice.Mode;
		LastMode = otherDevice.LastMode;
		DetectMode = otherDevice.DetectMode;
		RoCarrier = otherDevice.RoCarrier;
		IP = otherDevice.IP;
		ManufacturingDate = otherDevice.ManufacturingDate;
		ManualDevice = otherDevice.ManualDevice;
		Type = otherDevice.Type;
		WiFiOnlyDevice = otherDevice.WiFiOnlyDevice;
		Communicating = otherDevice.Communicating;
		LastConnected = otherDevice.LastConnected;
		MultiUp = otherDevice.MultiUp;
		Group = otherDevice.Group;
		Removed = false;
		ZeroTouchDevice = otherDevice.ZeroTouchDevice;
		if (otherDevice.OutOfProfile)
		{
			OutOfProfile = true;
		}
		PortIndex = otherDevice.PortIndex;
		InvalidSerialNumber = otherDevice.InvalidSerialNumber;
		Automated = otherDevice.Automated;
		if (Log == null || otherDevice.Log == null)
		{
			return;
		}
		foreach (string key in otherDevice.Log.Info.Keys)
		{
			if (!Log.Info.ContainsKey(key))
			{
				Log.AddInfo(key, otherDevice.Log.Info[key]);
			}
		}
	}

	public void ReportMode(DeviceMode newMode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Invalid comparison between Unknown and I4
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		if ((int)newMode == 1)
		{
			throw new NotSupportedException("Cannot set mode back to UNKNOWN");
		}
		if ((int)newMode == 16)
		{
			if ((int)Mode != 16)
			{
				Mode = newMode;
				lastDisconnect = DateTime.Now;
			}
			return;
		}
		if ((int)LastMode != 1 && (int)Mode != 1 && DateTime.Now.Subtract(lastDisconnect) < TimeSpan.FromSeconds(5.0))
		{
			Smart.Log.Debug(TAG, $"Ignoring new mode {newMode}, last disconnect was less than 5 seconds ago");
			return;
		}
		if (((Enum)newMode).HasFlag((Enum)(object)(DeviceMode)4) && (((Enum)newMode).HasFlag((Enum)(object)(DeviceMode)2) || ((Enum)newMode).HasFlag((Enum)(object)(DeviceMode)8)))
		{
			Smart.Log.Debug(TAG, $"Ignoring invalid mode {newMode}, defaulting to Fastboot mode");
			newMode = (DeviceMode)4;
		}
		if (((Enum)Mode).HasFlag((Enum)(object)(DeviceMode)8) && !((Enum)newMode).HasFlag((Enum)(object)(DeviceMode)8) && ((Enum)newMode).HasFlag((Enum)(object)(DeviceMode)2))
		{
			newMode = (DeviceMode)(newMode | 8);
		}
		Mode = newMode;
		LastMode = newMode;
	}

	public string GetLogInfoValue(string name)
	{
		string value = string.Empty;
		if (log != null && !log.Info.TryGetValue(name, out value))
		{
			value = string.Empty;
		}
		return value;
	}

	public override string ToString()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		string text = $"Device '{ID}' Details:\n";
		text = ((!ManualDevice) ? (text + "Physical device detected\n") : (text + "NOTE: This device was manually registered, no physical device was detected\n"));
		text = ((!Locked || Log == null) ? (text + "UNLOCKED\n") : (text + $"LOCKED in use case {Log.UseCase}\n"));
		if (Removed)
		{
			text += "WARNING: This device has been Removed from DeviceManager and should not be used\n";
		}
		text += $"Unique ID '{Unique}'\n";
		text += $"Serial Number: {SerialNumber} (Serial Number 2: {SerialNumber2})\n";
		text += $"In Mode: {Mode}, IP Address {IP}\n";
		text += $"Last Mode: {LastMode}, IP Address {IP}\n";
		text += $"Model ID: {ModelId}, RoCarrier {RoCarrier}\n";
		if (MultiUp)
		{
			text += "In Multi-Up mode\n";
		}
		text += "\n\n";
		if (Log != null)
		{
			SortedList<string, string> sortedList = new SortedList<string, string>(Log.Info);
			foreach (string key in sortedList.Keys)
			{
				text += $"Log Info {key}: {sortedList[key]}\n";
			}
		}
		return text;
	}
}
