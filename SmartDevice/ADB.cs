using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using AdvancedSharpAdbClient.Logs;
using AdvancedSharpAdbClient.Models;
using AdvancedSharpAdbClient.Receivers;
using ISmart;
using LibUsbDotNet.DeviceNotify;

namespace SmartDevice;

public class ADB : IADB, IDeviceFinder
{
	private IAdbClient adb = AdbClient.Instance;

	private Dictionary<EventHandler<string>, EventHandler<DeviceNotifyEventArgs>> handlers = new Dictionary<EventHandler<string>, EventHandler<DeviceNotifyEventArgs>>();

	private SortedList<string, DeviceData> devices = new SortedList<string, DeviceData>();

	private SortedList<string, DateTime> deviceBlacklist = new SortedList<string, DateTime>();

	protected TimeSpan BlacklistLimit = TimeSpan.FromMinutes(2.0);

	private string TAG => GetType().FullName;

	public string Name => "ADB";

	public DeviceMode Mode => (DeviceMode)2;

	public event EventHandler<string> OnDeviceNotify
	{
		add
		{
		}
		remove
		{
		}
	}

	public List<string> FindDevices()
	{
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = new List<string>();
		foreach (string key in deviceBlacklist.Keys)
		{
			if (DateTime.Now.Subtract(deviceBlacklist[key]) > BlacklistLimit)
			{
				Smart.Log.Debug(TAG, $"Removing device ID from blacklist: {key}");
				list.Add(key);
			}
		}
		foreach (string item in list)
		{
			deviceBlacklist.Remove(item);
		}
		IEnumerable<DeviceData> enumerable = adb.GetDevices();
		List<string> list2 = new List<string>();
		foreach (DeviceData item2 in enumerable)
		{
			DeviceData current3 = item2;
			string serial = ((DeviceData)(ref current3)).Serial;
			if (!deviceBlacklist.ContainsKey(serial))
			{
				if (!devices.ContainsKey(serial) && !ValidAdbDevice(serial, current3))
				{
					Smart.Log.Debug(TAG, $"Adding device ID to blacklist: {serial}");
					deviceBlacklist.Add(serial, DateTime.Now);
				}
				else
				{
					list2.Add(((DeviceData)(ref current3)).Serial);
					devices[serial] = current3;
				}
			}
		}
		return list2;
	}

	private bool ValidAdbDevice(string id, DeviceData foundDevice)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		for (int i = 0; i < 3; i++)
		{
			string empty = string.Empty;
			try
			{
				ConsoleOutputReceiver val = new ConsoleOutputReceiver((ILogger<ConsoleOutputReceiver>)null);
				adb.ExecuteRemoteCommandAsync("pwd", foundDevice, (IShellOutputReceiver)(object)val, AdbClient.Encoding, new CancellationTokenSource(3000).Token).Wait(3000);
				empty = ((object)val).ToString();
			}
			catch (Exception ex)
			{
				Smart.Log.Debug(TAG, (ex.Message + Environment.NewLine + ex.InnerException == null) ? "" : (ex.InnerException.ToString() + Environment.NewLine + ex.StackTrace));
				empty = ((ex.Message + ex.InnerException == null) ? "" : ex.InnerException.ToString());
			}
			if (empty.ToLowerInvariant().Contains("unauthorized") || empty.Trim() == "/")
			{
				result = true;
				break;
			}
			Smart.Log.Debug(TAG, $"Found adb port with id:{id} but not a full function adb device");
		}
		return result;
	}

	List<string> IDeviceFinder.Refresh()
	{
		return FindDevices();
	}

	public bool Check(string id)
	{
		return FindDevices().Contains(id);
	}

	private DeviceData SelectDevice(string deviceID)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!devices.ContainsKey(deviceID))
		{
			throw new NotSupportedException("Could not find ADB connection for device: " + deviceID);
		}
		return devices[deviceID];
	}

	public string Shell(string deviceID, string command, int timeoutMs = 10000)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		ConsoleOutputReceiver val = new ConsoleOutputReceiver((ILogger<ConsoleOutputReceiver>)null);
		DeviceData val2 = SelectDevice(deviceID);
		adb.ExecuteRemoteCommandAsync(command, val2, (IShellOutputReceiver)(object)val, AdbClient.Encoding, new CancellationTokenSource(timeoutMs).Token).Wait(timeoutMs);
		return ((object)val).ToString();
	}

	public void Install(string deviceID, string apkPath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		DeviceData val = SelectDevice(deviceID);
		new PackageManager(adb, val, Array.Empty<string>()).InstallPackage(apkPath, (Action<InstallProgressEventArgs>)delegate
		{
		}, new string[1] { "-r" });
	}

	public void Uninstall(string deviceID, string apkName)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		DeviceData val = SelectDevice(deviceID);
		new PackageManager(adb, val, Array.Empty<string>()).UninstallPackage(apkName, Array.Empty<string>());
	}

	public void ForwardPort(string deviceID, int devicePort, int localPort)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		DeviceData val = SelectDevice(deviceID);
		string text = $"tcp:{localPort}";
		string text2 = $"tcp:{devicePort}";
		adb.CreateForward(val, text, text2, true);
	}

	public void RemoveForward(string deviceID, int localPort)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		DeviceData val = SelectDevice(deviceID);
		adb.RemoveForward(val, localPort);
	}

	public void PushFile(string deviceID, string localFilePath, string deviceFilePath)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		SyncService val = new SyncService(SelectDevice(deviceID));
		try
		{
			using Stream stream = File.OpenRead(localFilePath);
			val.PushAsync(stream, deviceFilePath, (UnixFileStatus)4095, (DateTimeOffset)DateTime.Now, (Action<SyncProgressChangedEventArgs>)null, default(CancellationToken));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void Reboot(string deviceID, string mode)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		DeviceData val = SelectDevice(deviceID);
		adb.Reboot(mode, val);
	}
}
