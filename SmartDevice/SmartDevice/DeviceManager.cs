using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading;
using System.Windows.Forms;
using AdvancedSharpAdbClient;
using ISmart;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace SmartDevice;

public class DeviceManager : IDeviceManager
{
	private SortedList<string, IDevice> devices = new SortedList<string, IDevice>();

	private object deviceLock = new object();

	private TCMD tcmd = new TCMD();

	private int DeviceRefreshSec = 2;

	private int backgroundDisableCount;

	protected bool AdbStarted;

	protected string AdbPath = "adb.exe";

	private DateTime last = DateTime.Now.Subtract(TimeSpan.FromSeconds(10.0));

	private string TAG => GetType().FullName;

	public List<IDevice> ManualDevices { get; private set; } = new List<IDevice>();


	public bool IncludeManualDevices { get; set; }

	public bool BackgroundScan
	{
		get
		{
			lock (deviceLock)
			{
				return backgroundDisableCount < 1;
			}
		}
		set
		{
			lock (deviceLock)
			{
				if (value)
				{
					backgroundDisableCount--;
					if (backgroundDisableCount < 0)
					{
						backgroundDisableCount = 0;
					}
				}
				else
				{
					backgroundDisableCount++;
				}
				if (backgroundDisableCount > 0)
				{
					Smart.Log.Debug(TAG, $"{backgroundDisableCount} devices are disabling background scan");
				}
				else
				{
					Smart.Log.Debug(TAG, "Background scanning is enabled");
				}
			}
		}
	}

	public SortedList<string, IDevice> Devices
	{
		get
		{
			lock (deviceLock)
			{
				SortedList<string, IDevice> sortedList = new SortedList<string, IDevice>(devices);
				if (IncludeManualDevices)
				{
					foreach (IDevice manualDevice in ManualDevices)
					{
						if (manualDevice.SerialNumber != null && !(manualDevice.SerialNumber == string.Empty) && !(manualDevice.SerialNumber == "UNKNOWN"))
						{
							sortedList["MANUALXXXX"] = manualDevice;
						}
					}
				}
				return sortedList;
			}
		}
	}

	public DeviceManager()
	{
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		Smart.Net.WebHit("Start");
		AdbPath = Smart.Rsd.GetFilePathName("adbExe", (UseCase)0, (IDevice)null);
		if (Smart.Rsd.UseCaseLocked() != null && Smart.Rsd.UseCaseLocked().Contains((UseCase)211))
		{
			Smart.Log.Debug(TAG, "LMST_Auto_KillSwitch use case, skip start ADB server and change device background refresh rate to every 1 second");
			KillProces("adb.exe");
			KillProces("fastboot.exe");
		}
		else
		{
			bool flag = false;
			string arg = string.Empty;
			for (int i = 0; i < 2; i++)
			{
				try
				{
					KillProces("adb.exe");
					Thread.Sleep(3000);
				}
				catch (Exception ex)
				{
					Smart.Log.Error(TAG, "ADB process kill failed");
					Smart.Log.Error(TAG, ex.ToString());
				}
				try
				{
					Smart.Log.Verbose(TAG, "Starting ADB");
					TimeSpan timeout = TimeSpan.FromSeconds(60.0);
					Thread thread = new Thread(StartAdb);
					thread.IsBackground = true;
					thread.Start();
					if (!thread.Join(timeout))
					{
						Smart.Log.Error(TAG, "ADB start timed out");
						thread.Abort();
						AdbStarted = false;
					}
					flag = AdbStarted;
					if (flag)
					{
						Smart.Log.Verbose(TAG, "ADB server started");
						break;
					}
				}
				catch (Exception ex2)
				{
					Smart.Log.Debug(TAG, "Failed to start ADB server");
					arg = ex2.Message;
					Smart.Log.Debug(TAG, ex2.ToString());
				}
			}
			if (!flag)
			{
				string text = Smart.Locale.Xlate("Start ADBServer");
				string text2 = string.Format("{0}.\r\nError: {1}\r\n\r\n{2}", Smart.Locale.Xlate("Failed to start ADB server"), arg, Smart.Locale.Xlate("Please restart LMST"));
				Smart.NewPrompt();
				MessageBox.Show(text2, text, (MessageBoxButtons)0, (MessageBoxIcon)16);
				throw new FileLoadException("Failed to start ADB server");
			}
		}
		Refresh();
	}

	protected void StartAdb()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			AdbServer.Instance.StartServer(AdbPath, true);
			AdbStarted = true;
		}
		catch (Exception ex)
		{
			AdbStarted = false;
			Smart.Log.Error(TAG, "Could not start ADB: " + ex.Message);
			Smart.Log.Verbose(TAG, ex.ToString());
		}
	}

	public void Refresh()
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Invalid comparison between Unknown and I4
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Invalid comparison between Unknown and I4
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Invalid comparison between Unknown and I4
		//IL_0621: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_050a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Invalid comparison between Unknown and I4
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Invalid comparison between Unknown and I4
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Invalid comparison between Unknown and I4
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Invalid comparison between Unknown and I4
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Invalid comparison between Unknown and I4
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_081f: Unknown result type (might be due to invalid IL or missing references)
		//IL_082b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0853: Unknown result type (might be due to invalid IL or missing references)
		//IL_0858: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
		DateTime now = DateTime.Now;
		TimeSpan timeSpan = now.Subtract(last);
		if ((double)DeviceRefreshSec - timeSpan.TotalSeconds > 0.0)
		{
			Smart.Thread.Wait(TimeSpan.FromSeconds(0.1));
			return;
		}
		last = DateTime.Now;
		if (!BackgroundScan)
		{
			Smart.Log.Debug(TAG, "Background scan is turned off, skipping device check");
			return;
		}
		List<IDeviceFinder> list = new List<IDeviceFinder>();
		SortedList<string, DeviceMode> sortedList = new SortedList<string, DeviceMode>();
		if (Smart.Rsd.UseCaseLocked() != null && Smart.Rsd.UseCaseLocked().Contains((UseCase)211))
		{
			list.Add((IDeviceFinder)(object)Smart.LibUsbDotNetFastbootDeviceFinder);
		}
		else
		{
			list.Add((IDeviceFinder)(object)Smart.MotoAndroid);
			list.Add((IDeviceFinder)(object)Smart.ADB);
			list.Add((IDeviceFinder)(object)tcmd);
		}
		foreach (IDeviceFinder item in list)
		{
			try
			{
				List<string> list2 = item.Refresh();
				DeviceMode mode = item.Mode;
				foreach (string item2 in list2)
				{
					if (!sortedList.ContainsKey(item2))
					{
						sortedList[item2] = mode;
						continue;
					}
					SortedList<string, DeviceMode> sortedList2 = sortedList;
					string key = item2;
					sortedList2[key] |= mode;
				}
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, $"Error getting device list from {item.Name}: {ex.Message}");
				Smart.Log.Verbose(TAG, ex.ToString());
				return;
			}
		}
		lock (deviceLock)
		{
			List<string> list3 = new List<string>();
			foreach (string key2 in devices.Keys)
			{
				if (!sortedList.ContainsKey(key2))
				{
					IDevice val = devices[key2];
					if (val.ID != key2 && val.ID != null && val.ID != string.Empty)
					{
						Smart.Log.Debug(TAG, $"Found old device ID {key2}, updating to new ID {val.ID}");
						list3.Add(key2);
					}
				}
			}
			foreach (string item3 in list3)
			{
				IDevice val2 = devices[item3];
				string iD = val2.ID;
				if (devices.ContainsKey(iD))
				{
					Smart.Log.Warning(TAG, $"Warning, new device ID {iD} already exists, replacing existing Device object");
				}
				devices.Remove(item3);
				devices[iD] = val2;
				Smart.Log.Debug(TAG, $"Finished updating entry for {item3} to {iD}");
			}
			List<IDevice> list4 = new List<IDevice>();
			if (sortedList.Count != devices.Count)
			{
				Smart.Log.Verbose(TAG, $"Found {sortedList.Count} connected devices, checking {devices.Count} existing devices");
			}
			foreach (IDevice value in devices.Values)
			{
				Smart.Log.Verbose(TAG, $"{value.ID}: Mode--> {value.Mode} LastMode--> {value.LastMode}");
				if (!sortedList.ContainsKey(value.ID))
				{
					if (((Enum)value.LastMode).HasFlag((Enum)(object)(DeviceMode)8))
					{
						value.ReportMode((DeviceMode)16);
					}
					if (!value.Locked)
					{
						Smart.Log.Debug(TAG, $"Waiting to see if device {value.ID} ({value.Unique}) becomes locked...");
						Smart.Thread.Wait(TimeSpan.FromSeconds(5.0));
						if (!value.Locked)
						{
							list4.Add(value);
						}
					}
				}
				else
				{
					value.LastConnected = DateTime.Now;
					value.ReportMode(sortedList[value.ID]);
				}
			}
			foreach (IDevice value2 in devices.Values)
			{
				if (list4.Contains(value2) || value2.Locked || value2.Automated)
				{
					continue;
				}
				UseCase val3 = (UseCase)0;
				if (value2.Log != null)
				{
					val3 = value2.Log.UseCase;
				}
				if ((int)val3 == 141 || (int)val3 == 950 || (int)val3 == 167 || (int)val3 == 134)
				{
					DeviceMode detectMode = value2.DetectMode;
					DeviceMode mode2 = value2.Mode;
					if ((((Enum)detectMode).HasFlag((Enum)(object)(DeviceMode)4) && ((Enum)mode2).HasFlag((Enum)(object)(DeviceMode)2)) || (((Enum)detectMode).HasFlag((Enum)(object)(DeviceMode)2) && ((Enum)mode2).HasFlag((Enum)(object)(DeviceMode)4)))
					{
						Smart.Log.Debug(TAG, $"Re-detecting device {value2.ID}, switched from {detectMode} to {mode2}");
						list4.Add(value2);
						value2.Removed = true;
					}
				}
			}
			foreach (IDevice item4 in list4)
			{
				TimeSpan timeSpan2 = DateTime.Now.Subtract(item4.LastConnected);
				UseCase val4 = (UseCase)0;
				if (item4.Log != null)
				{
					val4 = item4.Log.UseCase;
				}
				Smart.Log.Debug(TAG, $"Device offline time: {timeSpan2.TotalSeconds}");
				bool flag = (int)val4 == 141 || (int)val4 == 950 || (int)val4 == 167 || (int)val4 == 134;
				if (!item4.Removed && timeSpan2.TotalSeconds < 20.0 && flag)
				{
					Smart.Log.Debug(TAG, $"Ignoring physical device removal for {item4.ID}: Seen {timeSpan2.TotalSeconds} seconds ago in state {val4}");
					continue;
				}
				if (item4.Automated)
				{
					Smart.Log.Debug(TAG, $"Ignoring Automated device removal for {item4.ID}: Seen {timeSpan2.TotalSeconds} seconds ago in state {val4}");
					continue;
				}
				devices.Remove(item4.ID);
				item4.Removed = true;
				item4.ReportMode((DeviceMode)16);
				Smart.Log.Debug(TAG, $"Physical Device REMOVED: {item4.ID} ({item4.Unique})");
				Smart.Log.Verbose(TAG, ((object)item4).ToString());
			}
			foreach (string key3 in sortedList.Keys)
			{
				if (!devices.ContainsKey(key3))
				{
					Device device = new Device();
					device.ID = key3;
					devices[key3] = (IDevice)(object)device;
					Smart.Log.Debug(TAG, $"Physical Device FOUND: {device.ID} ({device.Unique})");
				}
				IDevice val5 = devices[key3];
				DeviceMode mode3 = val5.Mode;
				val5.ReportMode(sortedList[key3]);
				if (val5.Mode != mode3)
				{
					ILog log = Smart.Log;
					string tAG = TAG;
					string? arg = ((object)(DeviceMode)(ref mode3)).ToString();
					DeviceMode mode4 = val5.Mode;
					log.Debug(tAG, $"Physical Device Mode changed: {arg} to {((object)(DeviceMode)(ref mode4)).ToString()}");
				}
			}
			List<IDevice> list5 = new List<IDevice>();
			foreach (IDevice manualDevice in ManualDevices)
			{
				if (manualDevice.Locked)
				{
					continue;
				}
				foreach (IDevice value3 in devices.Values)
				{
					if (!value3.ManualDevice && (manualDevice.SerialNumber == value3.SerialNumber || manualDevice.SerialNumber == value3.SerialNumber2))
					{
						list5.Add(manualDevice);
					}
				}
			}
			foreach (IDevice item5 in list5)
			{
				ManualDevices.Remove(item5);
			}
		}
		DateTime.Now.Subtract(now);
		last = DateTime.Now;
	}

	public void KillProces(string exe)
	{
		for (int i = 0; i < 3; i++)
		{
			try
			{
				Process[] array = null;
				array = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(exe));
				if (array != null)
				{
					Process[] array2 = array;
					foreach (Process process in array2)
					{
						Smart.Log.Info(TAG, "kill " + process.ProcessName);
						process.Kill();
					}
				}
				break;
			}
			catch (Exception ex)
			{
				Smart.Log.Info(TAG, $"kill process error:{ex.Message}");
				Thread.Sleep(2000);
			}
		}
	}

	public IDevice ManualDevice()
	{
		return ManualDevice(hidden: false);
	}

	public IDevice ManualDevice(string model)
	{
		return ManualDevice(model, hidden: false);
	}

	public IDevice ManualDevice(bool hidden)
	{
		return ManualDevice("UNKNOWN/UNKNOWN|UNKNOWN", hidden);
	}

	private IDevice ManualDevice(string model, bool hidden)
	{
		if (!IncludeManualDevices && Smart.Rsd.AllowedUseCases().Contains((UseCase)192))
		{
			IncludeManualDevices = true;
		}
		Device device = new Device();
		device.ID = "UNKNOWN";
		device.ModelId = model;
		device.SerialNumber = "UNKNOWN";
		device.ManualDevice = true;
		if (!hidden)
		{
			ManualDevices.Add((IDevice)(object)device);
		}
		return (IDevice)(object)device;
	}

	public void RemoveManualDevice()
	{
		ManualDevices.Clear();
	}

	public void MergeManualDevice(IDevice manualDevice, IDevice physicalDevice)
	{
		Smart.Log.Info(TAG, $"Merging manual device {manualDevice.ID} with physical device {physicalDevice.ID}");
		lock (deviceLock)
		{
			Smart.Log.Debug(TAG, ((object)manualDevice).ToString());
			Smart.Log.Debug(TAG, ((object)physicalDevice).ToString());
			ManualDevices.Clear();
			if (devices.ContainsKey(physicalDevice.ID))
			{
				devices.Remove(physicalDevice.ID);
			}
			manualDevice.MergeDevice(physicalDevice);
			devices[manualDevice.ID] = manualDevice;
			Smart.Log.Debug(TAG, $"Devices have been merged as device {manualDevice.ID}");
			Smart.Log.Debug(TAG, $"Manaul device list has {ManualDevices.Count} devices, physical device list has {devices.Count} devices");
			Smart.Log.Debug(TAG, ((object)manualDevice).ToString());
		}
	}

	public List<List<string>> GetVIDPID()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		List<List<string>> list = new List<List<string>>();
		List<string> list2 = new List<string> { "motorola", "spreadtrum", "mediatek" };
		ManagementObjectEnumerator enumerator = new ManagementObjectSearcher("SELECT * FROM Win32_USBControllerDevice").Get().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ManagementObject val = new ManagementObject(((ManagementBaseObject)(ManagementObject)enumerator.Current)["Dependent"].ToString());
				if (((ManagementBaseObject)val)["Manufacturer"] == null)
				{
					continue;
				}
				string text = ((ManagementBaseObject)val)["Manufacturer"].ToString().ToLower();
				foreach (string item in list2)
				{
					if (text.Contains(item))
					{
						list.Add(new List<string>
						{
							((ManagementBaseObject)val)["PNPDeviceID"].ToString().Split(new char[1] { '\\' })[1].Split(new char[1] { '&' })[0].Split(new char[1] { '_' })[1],
							((ManagementBaseObject)val)["PNPDeviceID"].ToString().Split(new char[1] { '\\' })[1].Split(new char[1] { '&' })[1].Split(new char[1] { '_' })[1]
						});
					}
				}
			}
			return list;
		}
		finally
		{
			((IDisposable)enumerator)?.Dispose();
		}
	}

	public List<SortedList<string, string>> PortInfo()
	{
		List<SortedList<string, string>> result = new List<SortedList<string, string>>();
		try
		{
			result = GetDevices();
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error while getting Port Info");
			Smart.Log.Verbose(TAG, ex.ToString());
		}
		return result;
	}

	private List<SortedList<string, string>> GetDevices()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		List<SortedList<string, string>> list = new List<SortedList<string, string>>();
		UsbDevice val2 = default(UsbDevice);
		foreach (UsbRegistry allWinUsbDevice in UsbDevice.AllWinUsbDevices)
		{
			UsbRegistry val = allWinUsbDevice;
			if (!val.Open(ref val2))
			{
				continue;
			}
			try
			{
				string serialString = val2.Info.SerialString;
				if (string.IsNullOrEmpty(serialString))
				{
					val2.Close();
					continue;
				}
				SortedList<string, string> sortedList = DeviceInfo(val);
				sortedList["SerialNo"] = serialString;
				list.Add(sortedList);
			}
			catch (Exception ex)
			{
				Smart.Log.Verbose(TAG, "Could not read USB device");
				Smart.Log.Verbose(TAG, ex.ToString());
			}
			finally
			{
				val2.Close();
			}
		}
		return list;
	}

	private SortedList<string, string> DeviceInfo(UsbRegistry device)
	{
		SortedList<string, string> sortedList = new SortedList<string, string>();
		sortedList["SymbolicName"] = device.SymbolicName;
		sortedList["FullName"] = device.FullName;
		sortedList["Name"] = device.Name;
		sortedList["Pid"] = Smart.Convert.BytesToHex(Smart.Convert.IntToBytes(device.Pid));
		sortedList["Vid"] = Smart.Convert.BytesToHex(Smart.Convert.IntToBytes(device.Vid));
		foreach (string key in device.DeviceProperties.Keys)
		{
			object[] array = new object[0];
			object obj = device.DeviceProperties[key];
			if (obj == null)
			{
				obj = string.Empty;
			}
			if (array.GetType().IsAssignableFrom(obj.GetType()))
			{
				string text = "[";
				object[] array2 = (object[])obj;
				foreach (object obj2 in array2)
				{
					text = text + obj2.ToString() + ", ";
				}
				text = text.TrimEnd(Array.Empty<char>()).TrimEnd(new char[1] { ',' }) + "]";
				obj = text;
			}
			sortedList[key] = obj.ToString();
		}
		return sortedList;
	}
}
