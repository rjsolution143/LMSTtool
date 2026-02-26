using System;
using System.Collections.Generic;
using ISmart;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace SmartDevice;

public class UsbPorts : IUsbPorts
{
	private DateTime last = DateTime.Now.Subtract(TimeSpan.FromSeconds(10.0));

	private string locationKey = "USBPortPhysicalLocation";

	private string TAG => GetType().FullName;

	public SortedList<int, UsbPortStatus> PortStatus { get; private set; } = new SortedList<int, UsbPortStatus>();


	protected SortedList<int, string> PortLocations { get; private set; } = new SortedList<int, string>();


	public UsbPorts()
	{
		LoadPorts();
		for (int i = 0; i < 12; i++)
		{
			int key = i + 1;
			if (PortLocations.ContainsKey(key))
			{
				PortStatus[key] = (UsbPortStatus)1;
			}
			else
			{
				PortStatus[key] = (UsbPortStatus)0;
			}
		}
	}

	private void LoadPorts()
	{
		Smart.Log.Debug(TAG, "Loading port assignments");
		string text = Smart.File.ResourceNameToFilePath("usbports", ".json");
		if (!Smart.File.Exists(text))
		{
			Smart.Log.Debug(TAG, "No ports assigned");
			return;
		}
		string text2 = Smart.File.ReadText(text);
		dynamic val = Smart.Json.Load(text2);
		_ = val.PortLocations;
		if (PortLocations == null)
		{
			Smart.Log.Error(TAG, "Port assignment file is corrupt");
			return;
		}
		SortedList<int, string> sortedList = val.PortLocations.ToObject<SortedList<int, string>>();
		foreach (int key in sortedList.Keys)
		{
			PortLocations[key] = sortedList[key];
		}
	}

	private void SavePorts()
	{
		Smart.Log.Debug(TAG, "Saving port assignments");
		string text = Smart.File.ResourceNameToFilePath("usbports", ".json");
		SortedList<string, object> sortedList = new SortedList<string, object>();
		sortedList["Version"] = "1.0";
		sortedList["LastUpdate"] = DateTime.Now.ToString("yyyyMMddHHmm");
		sortedList["PortLocations"] = PortLocations;
		string text2 = Smart.Json.Dump((object)sortedList);
		Smart.File.WriteText(text, text2);
	}

	public void ClearPorts()
	{
		Smart.Log.Debug(TAG, "Clearing all port assignments");
		PortLocations = new SortedList<int, string>();
		SavePorts();
		foreach (IDevice value in Smart.DeviceManager.Devices.Values)
		{
			value.PortIndex = 0;
		}
	}

	public void PortRefresh()
	{
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		Smart.Log.Info(TAG, "Refreshing USB port status");
		SortedList<string, SortedList<string, string>> fastbootDevicesInfo = Smart.DeviceListener.GetFastbootDevicesInfo();
		for (int i = 0; i < 12; i++)
		{
			int num = i + 1;
			if (!PortLocations.ContainsKey(num))
			{
				PortStatus[num] = (UsbPortStatus)0;
				continue;
			}
			string text = PortLocations[num];
			bool flag = false;
			string text2 = string.Empty;
			foreach (SortedList<string, string> value in fastbootDevicesInfo.Values)
			{
				if (!value.ContainsKey(locationKey))
				{
					continue;
				}
				string text3 = value[locationKey];
				if (text3.ToLowerInvariant().Trim() == text.ToLowerInvariant().Trim() || text3.ToLowerInvariant().Substring(0, text3.LastIndexOf('&')) == text.ToLowerInvariant().Substring(0, text.LastIndexOf('&')))
				{
					flag = true;
					if (value.ContainsKey("SerialNo"))
					{
						text2 = value["SerialNo"];
					}
					break;
				}
			}
			if (!flag)
			{
				PortStatus[num] = (UsbPortStatus)1;
				continue;
			}
			if (flag && text2 == string.Empty)
			{
				Smart.Log.Debug(TAG, $"Found device on Port {num} with status {PortStatus[num]} but SerialNo is empty");
				PortStatus[num] = (UsbPortStatus)2;
				continue;
			}
			PortStatus[num] = (UsbPortStatus)2;
			foreach (IDevice value2 in Smart.DeviceManager.Devices.Values)
			{
				if (!(value2.ID.ToLowerInvariant().Trim() != text2.ToLowerInvariant().Trim()))
				{
					PortStatus[num] = (UsbPortStatus)3;
					if (value2.PortIndex < 1)
					{
						Smart.Log.Debug(TAG, $"Assigning device {value2.ID} to port/Fixture {num}");
						value2.PortIndex = num;
					}
					else if (value2.PortIndex != num)
					{
						Smart.Log.Error(TAG, $"ERROR: Device {value2.ID} assigned to port {value2.PortIndex} but found on port {num}");
						PortStatus[num] = (UsbPortStatus)(-1);
					}
					break;
				}
			}
		}
	}

	private int GetPortRefreshFreqFromLocalOptionsFile()
	{
		int result = 0;
		IThreadLocked val = Smart.Rsd.LocalOptions();
		try
		{
			dynamic data = val.Data;
			string text = data.AutoKs_PortRefresh_Seconds;
			if (text != null && text != string.Empty)
			{
				Smart.Log.Debug(TAG, "AutoKs_PortRefresh_Seconds " + text);
				int.TryParse(text, out result);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return result;
	}

	public UsbPortStatus PortAssign(int portIndex, SortedList<string, string> deviceInfo)
	{
		Smart.Log.Debug(TAG, $"Assigning port {portIndex}");
		if (!deviceInfo.ContainsKey(locationKey))
		{
			Smart.Log.Warning(TAG, "Cannot assign device port without LocationInformation");
			Smart.Log.Verbose(TAG, Smart.Convert.ToString("DeviceInfoBad", (IEnumerable<KeyValuePair<string, string>>)deviceInfo));
			return (UsbPortStatus)0;
		}
		string text = deviceInfo[locationKey];
		if (text == null)
		{
			Smart.Log.Error(TAG, "Cannot assign bad port");
			return (UsbPortStatus)0;
		}
		foreach (IDevice value in Smart.DeviceManager.Devices.Values)
		{
			value.PortIndex = 0;
		}
		PortLocations[portIndex] = text;
		SavePorts();
		Smart.Log.Debug(TAG, $"Assigned port {portIndex}");
		return (UsbPortStatus)1;
	}

	public UsbPortStatus PortQuery(int portIndex)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Smart.Log.Debug(TAG, $"Querying port {portIndex}");
		return PortStatus[portIndex];
	}

	public SortedList<string, string> PortScan()
	{
		Smart.Log.Debug(TAG, "Scanning for new device port");
		DateTime now = DateTime.Now;
		TimeSpan timeSpan = TimeSpan.FromSeconds(30.0);
		SortedList<string, SortedList<string, string>> sortedList = AllDevices();
		SortedList<string, string> sortedList2 = new SortedList<string, string>();
		while (DateTime.Now.Subtract(now).TotalMilliseconds < timeSpan.TotalMilliseconds)
		{
			foreach (SortedList<string, string> value in AllDevices().Values)
			{
				if (!value.ContainsKey(locationKey))
				{
					Smart.Log.Warning(TAG, "Could not find location info for a device during scan");
					Smart.Log.Verbose(TAG, Smart.Convert.ToString("DeviceIssue", (IEnumerable<KeyValuePair<string, string>>)value));
					continue;
				}
				string text = value[locationKey];
				bool flag = false;
				foreach (SortedList<string, string> value2 in sortedList.Values)
				{
					if (value2.ContainsKey(locationKey))
					{
						string text2 = value2[locationKey];
						if (text == text2)
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					continue;
				}
				sortedList2 = value;
				Smart.Log.Debug(TAG, $"Found new device port '{text}'");
				Smart.Log.Verbose(TAG, Smart.Convert.ToString("FoundDevice", (IEnumerable<KeyValuePair<string, string>>)value));
				break;
			}
			if (sortedList2.Count > 0)
			{
				break;
			}
			Smart.Thread.Wait(TimeSpan.FromSeconds(1.0));
		}
		if (sortedList2.Count < 1)
		{
			Smart.Log.Warning(TAG, "Could not find new device during scan");
			return new SortedList<string, string>();
		}
		return sortedList2;
	}

	public bool WaitForPort(int fixtureIndex, TimeSpan timeout)
	{
		if (!PortLocations.ContainsKey(fixtureIndex))
		{
			Smart.Log.Error(TAG, $"Cannot wait for device on unassigned port {fixtureIndex}");
			return false;
		}
		string text = PortLocations[fixtureIndex];
		Smart.Log.Debug(TAG, $"Waiting for device on port/Fixture {fixtureIndex} (location '{text}')");
		DateTime now = DateTime.Now;
		while (DateTime.Now.Subtract(now).TotalMilliseconds < timeout.TotalMilliseconds)
		{
			foreach (SortedList<string, string> value in Smart.DeviceListener.GetFastbootDevicesInfo().Values)
			{
				if (value.ContainsKey(locationKey))
				{
					string text2 = value[locationKey];
					if (text2.ToLowerInvariant().Trim() == text.ToLowerInvariant().Trim() || text2.ToLowerInvariant().Substring(0, text2.LastIndexOf('&')) == text.ToLowerInvariant().Substring(0, text.LastIndexOf('&')))
					{
						Smart.Log.Debug(TAG, string.Format("Found device {0} on fixture {1} with location {2}", value["SerialNo"], fixtureIndex, text2));
						return true;
					}
				}
			}
		}
		Smart.Log.Error(TAG, $"Timeout out waiting for port on Fixture {fixtureIndex}");
		return false;
	}

	public int FindPort(SortedList<string, string> deviceInfo)
	{
		Smart.Log.Debug(TAG, "Finding port for device");
		if (!deviceInfo.ContainsKey(locationKey))
		{
			Smart.Log.Warning(TAG, "Cannot check port assignment without LocationInformation");
			Smart.Log.Verbose(TAG, Smart.Convert.ToString("DeviceInfoBad", (IEnumerable<KeyValuePair<string, string>>)deviceInfo));
			return -2;
		}
		string text = deviceInfo[locationKey];
		foreach (int key in PortLocations.Keys)
		{
			string text2 = PortLocations[key];
			if (text == text2)
			{
				return key;
			}
		}
		return -1;
	}

	[Obsolete]
	public UsbDevice DeviceByProperty(string propertyKey, string propertyValue)
	{
		foreach (SortedList<string, string> value in AllDevices().Values)
		{
			if (value.ContainsKey(propertyKey) && value[propertyKey] == propertyValue)
			{
				return DeviceBySymbolicName(value["SymbolicName"]);
			}
		}
		Smart.Log.Error(TAG, $"Could not find device with {propertyKey} value '{propertyValue}'");
		return null;
	}

	[Obsolete]
	private UsbDevice DeviceBySymbolicName(string symbolicName)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		foreach (UsbRegistry allWinUsbDevice in UsbDevice.AllWinUsbDevices)
		{
			UsbRegistry val = allWinUsbDevice;
			if (val.SymbolicName == symbolicName)
			{
				UsbDevice device = val.Device;
				if (device == null)
				{
					Smart.Log.Error(TAG, $"No libusb device for name '{symbolicName}'");
					return null;
				}
				Smart.Log.Debug(TAG, $"Found libusb device for name '{symbolicName}'");
				return device;
			}
		}
		Smart.Log.Error(TAG, $"Could not find device with name '{symbolicName}'");
		return null;
	}

	private SortedList<string, SortedList<string, string>> AllDevices()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		SortedList<string, SortedList<string, string>> sortedList = new SortedList<string, SortedList<string, string>>();
		UsbDevice val2 = default(UsbDevice);
		foreach (UsbRegistry allWinUsbDevice in UsbDevice.AllWinUsbDevices)
		{
			UsbRegistry val = allWinUsbDevice;
			if (val.Open(ref val2))
			{
				string serialString = val2.Info.SerialString;
				Smart.Log.Info(TAG, $"Found Device SN {serialString}");
				if (!string.IsNullOrEmpty(serialString))
				{
					SortedList<string, string> sortedList2 = DeviceInfo(val);
					sortedList2["SerialNo"] = serialString;
					sortedList[val.SymbolicName] = sortedList2;
				}
			}
		}
		return sortedList;
	}

	private SortedList<string, string> DeviceInfo(UsbRegistry device)
	{
		SortedList<string, string> info = new SortedList<string, string>();
		info["SymbolicName"] = device.SymbolicName;
		info["FullName"] = device.FullName;
		info["Name"] = device.Name;
		info["Pid"] = Smart.Convert.BytesToHex(Smart.Convert.IntToBytes(device.Pid));
		info["Vid"] = Smart.Convert.BytesToHex(Smart.Convert.IntToBytes(device.Vid));
		foreach (string key in device.DeviceProperties.Keys)
		{
			info[key] = device.DeviceProperties[key].ToString();
		}
		string usbPortPhysicalLocation = new UsbPortHelper().GetUsbPortPhysicalLocation(ref info);
		info[locationKey] = usbPortPhysicalLocation;
		return info;
	}
}
