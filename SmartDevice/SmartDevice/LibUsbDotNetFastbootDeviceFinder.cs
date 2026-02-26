using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using ISmart;
using LibUsbDotNet;
using LibUsbDotNet.Info;
using LibUsbDotNet.Main;

namespace SmartDevice;

public class LibUsbDotNetFastbootDeviceFinder : ILibUsbDotNetDeviceFinder, IDeviceFinder
{
	private const string Adb_Interface_Calss_Protocol = "FF-42-01";

	private const string Fastboot_Interface_Calss_Protocol = "FF-42-03";

	private object deviceListUpdateLock = new object();

	private object winUsbQueryLock = new object();

	private bool UseDeviceNotifyEvent = true;

	private int mMaxExit;

	private int mMaxWait;

	private SortedList<string, SortedList<string, string>> fastbootDevicesInfo = new SortedList<string, SortedList<string, string>>();

	private string locationKey = "USBPortPhysicalLocation";

	public string Name => "LibUsbDotNetFastbootDeviceFinder";

	public DeviceMode Mode => (DeviceMode)4;

	private string TAG => GetType().FullName;

	public bool UseDeviceListener { get; set; }

	public SortedList<string, SortedList<string, string>> FastbootDevicesInfo
	{
		get
		{
			lock (deviceListUpdateLock)
			{
				return new SortedList<string, SortedList<string, string>>(fastbootDevicesInfo);
			}
		}
	}

	public LibUsbDotNetFastbootDeviceFinder()
	{
		UseDeviceListener = false;
	}

	List<string> IDeviceFinder.Refresh()
	{
		return FindDevices();
	}

	public List<string> FindDevices()
	{
		if (UseDeviceNotifyEvent)
		{
			if (UseDeviceListener)
			{
				return Smart.DeviceListener.GetFastbootDevicesInfo().Keys.ToList();
			}
			return new List<string>();
		}
		return FindDevices(useLibUsbDotNetDll: true);
	}

	public string Shell(IDevice device, string command, int timeout)
	{
		throw new NotImplementedException();
	}

	public List<string> Shell(string deviceId, string command, int timeout, string exe, out int exitCode, int waitForResponseTimeout = 6000, bool usedExeDir = false)
	{
		lock (winUsbQueryLock)
		{
			return Shell(deviceId, command, timeout, exe, logStatus: true, out exitCode, waitForResponseTimeout, realtimePrioritied: false, usedExeDir);
		}
	}

	private List<string> Shell(string deviceId, string command, int timeout, string exe, bool logStatus, out int exitCode, int waitForResponseTimeout, bool realtimePrioritied, bool usedExeDir = false)
	{
		List<string> list = new List<string>();
		List<string> output = new List<string>();
		List<string> error = new List<string>();
		bool flag = exe.EndsWith("fastboot.exe") || exe.EndsWith("adb.exe");
		string workingDirectory = (usedExeDir ? Path.GetDirectoryName(exe) : Environment.CurrentDirectory);
		exe = Smart.Convert.QuoteFilePathName(exe);
		string arguments = command;
		if (!string.IsNullOrEmpty(deviceId) && deviceId.ToUpper() != "UNKNOWN" && flag)
		{
			arguments = "-s " + deviceId + " " + command;
		}
		try
		{
			Process process = new Process();
			process.StartInfo.FileName = exe;
			process.StartInfo.Arguments = arguments;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.CreateNoWindow = true;
			process.EnableRaisingEvents = true;
			process.StartInfo.WorkingDirectory = workingDirectory;
			process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
			{
				Redirected(output, sender, e);
			};
			process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e)
			{
				Redirected(error, sender, e);
			};
			process.Start();
			if (realtimePrioritied)
			{
				process.PriorityClass = ProcessPriorityClass.RealTime;
				process.PriorityBoostEnabled = true;
			}
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			if (logStatus)
			{
				Smart.Log.Verbose(TAG, $"Starting: '{process.StartInfo.FileName} {process.StartInfo.Arguments}'");
			}
			DateTime now = DateTime.Now;
			bool num = process.WaitForExit(timeout);
			if (logStatus)
			{
				int num2 = (int)(DateTime.Now - now).TotalMilliseconds;
				if (num2 > mMaxExit)
				{
					mMaxExit = num2;
				}
				Smart.Log.Verbose(TAG, $"Set timeout = {timeout}ms, process exited after {num2}ms, maxExit: {mMaxExit}");
			}
			list = WaitForCompleteResponse(output, error, waitForResponseTimeout, logStatus);
			if (!num)
			{
				try
				{
					process.Kill();
					if (logStatus)
					{
						Smart.Log.Verbose(TAG, $"'{process.StartInfo.FileName} {process.StartInfo.Arguments}' is timed out");
					}
					list.Add("Error: timed out");
				}
				catch (Exception ex)
				{
					Smart.Log.Debug(TAG, $"{ex.Message} - Ignored ");
				}
			}
			exitCode = process.ExitCode;
			if (logStatus)
			{
				Smart.Log.Verbose(TAG, $"ExitCode: {exitCode} cmd: {command}");
				Smart.Log.Verbose(TAG, string.Format("Resp stdOut: {0} stdError: {1}", string.Join("\r\n", output.ToArray()), string.Join("\r\n", error.ToArray())));
			}
		}
		catch (Exception ex2)
		{
			Smart.Log.Error(TAG, "Exeption - errorMsg: " + ex2.Message);
			exitCode = 1;
		}
		return list;
	}

	public bool Check(string id)
	{
		return FindDevices().Contains(id);
	}

	private List<string> WaitForCompleteResponse(List<string> output, List<string> error, int waitForRespTimeout, bool logStatus)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		List<string> list = new List<string>();
		list.AddRange(output);
		list.AddRange(error);
		while (num < waitForRespTimeout)
		{
			if (list.Count > 0)
			{
				if (list.Count == num2)
				{
					num3++;
					if (num3 == 2)
					{
						break;
					}
				}
				else
				{
					num3 = 0;
					num2 = list.Count;
				}
			}
			Thread.Sleep(50);
			num += 50;
			list.Clear();
			list.AddRange(output);
			list.AddRange(error);
		}
		if (logStatus && num > mMaxWait)
		{
			mMaxWait = num;
		}
		return list;
	}

	private void Redirected(List<string> dataList, object sender, DataReceivedEventArgs e)
	{
		if (e.Data != null)
		{
			dataList.Add(e.Data);
		}
	}

	private List<string> FindDevices(bool useLibUsbDotNetDll)
	{
		return AllFastbootDevices();
	}

	public List<string> AllFastbootDevices()
	{
		SortedList<string, SortedList<string, string>> sortedList = new SortedList<string, SortedList<string, string>>();
		SortedList<string, SortedList<string, string>> adbDevices = new SortedList<string, SortedList<string, string>>();
		lock (winUsbQueryLock)
		{
			AllDevices(sortedList, adbDevices);
		}
		fastbootDevicesInfo = sortedList;
		return sortedList.Keys.ToList();
	}

	public List<string> AllAdbDevices()
	{
		SortedList<string, SortedList<string, string>> fastbootDevices = new SortedList<string, SortedList<string, string>>();
		SortedList<string, SortedList<string, string>> sortedList = new SortedList<string, SortedList<string, string>>();
		AllDevices(fastbootDevices, sortedList);
		return sortedList.Keys.ToList();
	}

	private void AllDevices(SortedList<string, SortedList<string, string>> fastbootDevices, SortedList<string, SortedList<string, string>> adbDevices)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected I4, but got Unknown
		UsbRegDeviceList allWinUsbDevices = UsbDevice.AllWinUsbDevices;
		Smart.Log.Info(TAG, "AllWinUsbDevices count " + allWinUsbDevices.Count);
		UsbDevice val2 = default(UsbDevice);
		foreach (UsbRegistry item in allWinUsbDevices)
		{
			UsbRegistry val = item;
			Smart.Log.Info(TAG, string.Format("Found Device:\r\n{0} {1} {2} {3}", val.DeviceProperties["FriendlyName"], val.DeviceProperties["LocationInformation"], val.DeviceProperties["SymbolicName"], val.DeviceProperties["DeviceID"]));
			if (val.Open(ref val2))
			{
				string serialString = val2.Info.SerialString;
				Smart.Log.Info(TAG, $"Found Device SN {serialString}");
				if (string.IsNullOrEmpty(serialString))
				{
					continue;
				}
				foreach (UsbConfigInfo config in val2.Configs)
				{
					foreach (UsbInterfaceInfo interfaceInfo in config.InterfaceInfoList)
					{
						string text = ((int)interfaceInfo.Descriptor.Class).ToString("X") + "-" + Smart.Convert.BytesToHex(new byte[1] { interfaceInfo.Descriptor.SubClass }) + "-" + Smart.Convert.BytesToHex(new byte[1] { interfaceInfo.Descriptor.Protocol });
						if (text.ToUpper() == "FF-42-03".ToUpper())
						{
							SortedList<string, string> sortedList = DeviceInfo(val);
							sortedList["SerialNo"] = serialString;
							Smart.Log.Info(TAG, $"Device {serialString} is fastboot device at location {sortedList[locationKey]}");
							fastbootDevices[serialString] = sortedList;
							break;
						}
						if (text.ToUpper() == "FF-42-01".ToUpper())
						{
							SortedList<string, string> sortedList2 = DeviceInfo(val);
							sortedList2["SerialNo"] = serialString;
							Smart.Log.Info(TAG, $"Device {serialString} is Adb device at location {sortedList2[locationKey]}");
							adbDevices[serialString] = sortedList2;
							break;
						}
						Smart.Log.Info(TAG, $"Device {serialString} interface type is un-identified");
					}
				}
				val2.Close();
			}
			else
			{
				Smart.Log.Info(TAG, $"Open {val.FullName}:{val.SymbolicName} fail");
			}
		}
		UsbDevice.Exit();
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
