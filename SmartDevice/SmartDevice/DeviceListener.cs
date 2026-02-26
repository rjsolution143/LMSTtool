using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ISmart;
using LibUsbDotNet;
using LibUsbDotNet.DeviceNotify;
using LibUsbDotNet.Info;
using LibUsbDotNet.WinUsb;

namespace SmartDevice;

public class DeviceListener : IDeviceListener
{
	private const string Adb_Interface_Calss_Protocol = "FF-42-01";

	private const string Fastboot_Interface_Calss_Protocol = "FF-42-03";

	private ConcurrentDictionary<string, string> SymbolicNameSerials = new ConcurrentDictionary<string, string>();

	private ConcurrentDictionary<string, string> PortLocations = new ConcurrentDictionary<string, string>();

	private string TAG => GetType().FullName;

	public IDeviceNotifier UsbDeviceNotifier { get; private set; }

	public DateTime LastArrival { get; private set; }

	public DateTime LastRemoval { get; private set; }

	public DeviceListener()
	{
		LastArrival = DateTime.Now;
		LastRemoval = DateTime.Now;
		UsbDeviceNotifier = DeviceNotifier.OpenDeviceNotifier();
	}

	public void Listen()
	{
		Smart.Log.Info(TAG, "Device Listener active");
		UsbDeviceNotifier.OnDeviceNotify += OnDeviceNotifyEvent;
	}

	public SortedList<string, SortedList<string, string>> GetFastbootDevicesInfo()
	{
		SortedList<string, SortedList<string, string>> sortedList = new SortedList<string, SortedList<string, string>>();
		try
		{
			foreach (string sn in SymbolicNameSerials.Values)
			{
				string empty = string.Empty;
				if (!PortLocations.ContainsKey(sn))
				{
					string key = SymbolicNameSerials.FirstOrDefault((KeyValuePair<string, string> x) => x.Value == sn).Key;
					string dbccName = "\\\\?\\" + key;
					empty = new UsbPortHelper().GetUsbPortPhysicalLocation(dbccName);
					Smart.Log.Info(TAG, $"Sn {sn}, PortLocation {empty}");
					PortLocations[sn] = empty;
				}
				else
				{
					empty = PortLocations[sn];
				}
				sortedList.Add(sn, new SortedList<string, string>
				{
					{ "USBPortPhysicalLocation", empty },
					{ "SerialNo", sn }
				});
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, ex.Message + Environment.NewLine + ex.StackTrace);
		}
		return sortedList;
	}

	private void OnDeviceNotifyEvent(object sender, DeviceNotifyEventArgs e)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Invalid comparison between Unknown and I4
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected I4, but got Unknown
		if ((int)e.DeviceType != 5)
		{
			return;
		}
		if ((int)e.EventType == 32768)
		{
			string fullName = e.Device.SymbolicName.FullName;
			string text = "\\\\?\\" + fullName;
			Smart.Log.Info(TAG, "Arrival Device Event with dbccName:" + text);
			WinUsbDevice val = null;
			if (WinUsbDevice.Open(text, ref val))
			{
				string serialString = ((UsbDevice)val).Info.SerialString;
				Smart.Log.Debug(TAG, $"Arrived Device SN {serialString}");
				foreach (UsbConfigInfo config in ((UsbDevice)val).Configs)
				{
					foreach (UsbInterfaceInfo interfaceInfo in config.InterfaceInfoList)
					{
						if ((((int)interfaceInfo.Descriptor.Class).ToString("X") + "-" + Smart.Convert.BytesToHex(new byte[1] { interfaceInfo.Descriptor.SubClass }) + "-" + Smart.Convert.BytesToHex(new byte[1] { interfaceInfo.Descriptor.Protocol })).ToUpper() == "FF-42-03".ToUpper())
						{
							SymbolicNameSerials.TryAdd(fullName, serialString);
							LastArrival = DateTime.Now;
							break;
						}
					}
				}
				((UsbDevice)val).Close();
			}
			else
			{
				Smart.Log.Error(TAG, $"Open Arrived device path {text} fail");
			}
			UsbDevice.Exit();
		}
		else
		{
			if ((int)e.EventType != 32772)
			{
				return;
			}
			string fullName2 = e.Device.SymbolicName.FullName;
			Smart.Log.Info(TAG, $"Removal Device Event {e.Device.SerialNumber} {e.Device.Name}");
			string value = string.Empty;
			if (SymbolicNameSerials.TryRemove(fullName2, out value))
			{
				string value2 = string.Empty;
				if (PortLocations.TryRemove(value, out value2))
				{
					Smart.Log.Debug(TAG, $"Remove device {value} from listener on port location {value2} pass");
				}
				else
				{
					Smart.Log.Debug(TAG, $"Remove device {value} from listener on port location fail");
				}
			}
			else
			{
				Smart.Log.Debug(TAG, $"Removal event for unknown device: {fullName2} fail");
			}
		}
	}
}
