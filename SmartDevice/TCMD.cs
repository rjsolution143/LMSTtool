using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using ISmart;

namespace SmartDevice;

public class TCMD : IDeviceFinder
{
	public static string FoundSubnet = string.Empty;

	private static bool checkedFile = false;

	private static bool searchedSubnet = false;

	private const int port = 11000;

	private static int[] offsets = new int[12]
	{
		2, 10, 18, 26, 34, 42, 50, 58, 64, 72,
		80, 88
	};

	private static int[] commonOffsets = new int[33]
	{
		2, 10, 18, 5, 6, 9, 14, 13, 17, 22,
		26, 34, 42, 50, 58, 64, 3, 4, 7, 8,
		11, 12, 15, 16, 19, 20, 21, 23, 24, 25,
		72, 80, 88
	};

	private const string commonSubnet = "192.168.137";

	private static int highestIp = -1;

	private static bool scanning = false;

	private static object deviceLock = new object();

	private static SortedList<int, string> savedDevices = new SortedList<int, string>();

	private string TAG => GetType().FullName;

	public string Name => "TCMD";

	public DeviceMode Mode => (DeviceMode)8;

	public List<string> FindNetworkingDrivers(string nicDescription = "Motorola USB Networking Driver")
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		List<string> list = new List<string>();
		ManagementClass val = new ManagementClass("Win32_NetworkAdapterConfiguration");
		try
		{
			_ = ((ManagementBaseObject)val).Properties;
			ManagementObjectCollection instances = val.GetInstances();
			try
			{
				ManagementObjectEnumerator enumerator = instances.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ManagementObject val2 = (ManagementObject)enumerator.Current;
						bool flag = (bool)((ManagementBaseObject)val2)["IPEnabled"];
						if (((string)((ManagementBaseObject)val2)["Description"]).Contains(nicDescription) && flag)
						{
							string text = ((Array)((ManagementBaseObject)val2).Properties["IPAddress"].Value).GetValue(0).ToString();
							Smart.Log.Verbose(TAG, "Motorola network ipaddress:" + text);
							if (IPAddress.TryParse(text, out IPAddress address) && address.AddressFamily == AddressFamily.InterNetwork)
							{
								int num = text.LastIndexOf('.') + 1;
								string text2 = text.Substring(0, num);
								string text3 = (int.Parse(text.Substring(num)) + 1).ToString();
								text = text2 + text3;
								list.Add(text);
							}
						}
					}
				}
				finally
				{
					((IDisposable)enumerator)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)instances)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		if (list.Count > 0)
		{
			Smart.Log.Debug(TAG, $"Found {list.Count} Network Driver interfaces");
		}
		else
		{
			Smart.Log.Debug(TAG, "No Network Driver interfaces found");
		}
		return list;
	}

	public List<string> FindDevices()
	{
		if (FoundSubnet == string.Empty && !checkedFile)
		{
			checkedFile = true;
			Smart.Log.Debug(TAG, "Checking subnet file");
			IThreadLocked val = Smart.Rsd.LocalOptions();
			try
			{
				dynamic data = val.Data;
				string text = data.TcmdSubnet;
				if (text != null && text != string.Empty)
				{
					Smart.Log.Debug(TAG, "Using saved TCMD subnet at " + text);
					string[] array = text.Split(new char[1] { '.' });
					if (array.Length != 4)
					{
						throw new NotSupportedException("Invalid subnet: " + text);
					}
					FoundSubnet = string.Format("{0}.{1}.{2}.", array);
					int.Parse(array[3]);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		if (FoundSubnet == string.Empty)
		{
			FoundSubnet = SubnetSearch();
			if (FoundSubnet == string.Empty)
			{
				return new List<string>();
			}
			Smart.Log.Debug(TAG, "TCMD subnet found at " + FoundSubnet);
			string[] array2 = FoundSubnet.Split(new char[1] { '.' });
			if (array2.Length != 4)
			{
				throw new NotSupportedException("Invalid subnet: " + FoundSubnet);
			}
			IThreadLocked val2 = Smart.Rsd.LocalOptions();
			try
			{
				dynamic data2 = val2.Data;
				data2.TcmdSubnet = FoundSubnet;
				val2.Data = (object)data2;
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			FoundSubnet = string.Format("{0}.{1}.{2}.", array2);
			int.Parse(array2[3]);
		}
		lock (deviceLock)
		{
			if (scanning)
			{
				return new List<string>(savedDevices.Values);
			}
			try
			{
				scanning = true;
				return DeviceScan();
			}
			finally
			{
				scanning = false;
			}
		}
	}

	private bool IsIpExists(string checkIp)
	{
		return (from addr in (from ni in NetworkInterface.GetAllNetworkInterfaces()
				where ni.OperationalStatus == OperationalStatus.Up
				select ni).SelectMany((NetworkInterface ni) => ni.GetIPProperties().UnicastAddresses)
			where addr.Address.AddressFamily == AddressFamily.InterNetwork
			select addr).Any((UnicastIPAddressInformation addr) => addr.Address.ToString() == checkIp);
	}

	private string GetTempIp(string originalIp)
	{
		string[] array = originalIp.Split(new char[1] { '.' });
		if (array.Length != 4)
		{
			return null;
		}
		if (int.TryParse(array[3], out var result) && result >= 1 && result <= 255)
		{
			array[3] = (result - 1).ToString();
			return string.Join(".", array);
		}
		return null;
	}

	public string QuickScan(string id, TimeSpan timeout, bool filterNetwork, bool noFallback, bool skipTrackIdCheck)
	{
		List<string> list = new List<string>();
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		int[] array = commonOffsets;
		foreach (int num in array)
		{
			string item = "192.168.137." + num;
			list.Add(item);
		}
		List<string> list2 = list;
		bool flag = true;
		if (filterNetwork)
		{
			List<string> list3 = FindNetworkingDrivers();
			if (list3.Count > 0 || noFallback)
			{
				list2 = list3;
				flag = false;
			}
			if (list3.Count == 0)
			{
				text = "Motorola Network port not found";
			}
		}
		Smart.Log.Debug(TAG, $"Quick scan for TCMD with TRACK ID {id}...");
		TimeSpan timeSpan = TimeSpan.FromSeconds(1.0);
		DateTime now = DateTime.Now;
		int num2 = 0;
		DateTime now2 = DateTime.Now;
		while (DateTime.Now.Subtract(now).TotalSeconds <= timeout.TotalSeconds)
		{
			text = string.Empty;
			text2 = string.Empty;
			if (num2 >= list2.Count)
			{
				num2 = 0;
				TimeSpan ts = DateTime.Now.Subtract(now2);
				if (ts.TotalMilliseconds < timeSpan.TotalMilliseconds)
				{
					TimeSpan timeSpan2 = timeSpan.Subtract(ts);
					Smart.Log.Verbose(TAG, $"Waiting {timeSpan2.TotalMilliseconds} ms");
					Smart.Thread.Wait(timeSpan2);
				}
				list2 = list;
				flag = true;
				if (filterNetwork)
				{
					List<string> list4 = FindNetworkingDrivers();
					if (list4.Count > 0 || noFallback)
					{
						list2 = list4;
						flag = false;
					}
					if (list4.Count == 0)
					{
						text = "Motorola Network port not found";
					}
				}
				now2 = DateTime.Now;
				if (!string.IsNullOrEmpty(text3) && list2.Contains(text3))
				{
					list2.Remove(text3);
				}
				if (list2.Count == 0)
				{
					text = "Motorola Network port not found";
				}
				continue;
			}
			string text4 = list2[num2];
			num2++;
			Smart.Log.Debug(TAG, $"Checking IP {text4}...");
			if (!flag)
			{
				text2 = text4;
			}
			string tempIp = GetTempIp(text4);
			if (flag)
			{
				bool flag2 = IsIpExists(tempIp);
				Smart.Log.Debug(TAG, string.Format("Checking IP = {0}, device IP {1} {2}", text4, tempIp, flag2 ? "exist" : "not exist"));
				if (!flag2)
				{
					continue;
				}
			}
			using (TcpClient tcpClient = new TcpClient())
			{
				tcpClient.SendTimeout = 1000;
				tcpClient.ReceiveTimeout = 1000;
				try
				{
					if (tcpClient.ConnectAsync(text4, 11000).Wait(1000))
					{
						text2 = text4;
						Smart.Log.Debug(TAG, string.Format("Open port at IP {0} succeed", text4 + ":" + 11000));
						goto IL_0350;
					}
					text = ((!flag) ? string.Format("Open port {0} failed", text4 + ":" + 11000) : string.Format("Motorola Network port not found use Default IP {0}", text4 + ":" + 11000));
					Smart.Log.Debug(TAG, string.Format("Open port at IP {0} failed", text4 + ":" + 11000));
				}
				catch (Exception ex)
				{
					text += ex.Message;
				}
			}
			continue;
			IL_0350:
			try
			{
				Smart.Log.Debug(TAG, $"Checking power up at IP {text4}...");
				if (!PowerUpCheck(text4))
				{
					Smart.Log.Debug(TAG, $"IP {text4} is not powered up");
					text = "Device not powered up";
				}
				else
				{
					Smart.Log.Debug(TAG, $"IP {text4} connected successfully");
				}
			}
			catch (Exception ex2)
			{
				text = ex2.Message;
				continue;
			}
			try
			{
				Smart.Log.Debug(TAG, $"Finding TRACK ID for IP {text4}...");
				string text5 = ReadId(text4);
				if (text5.Trim().ToLowerInvariant() == id.Trim().ToLowerInvariant())
				{
					Smart.Log.Debug(TAG, $"Found device {text5} at IP {text4}...");
					return text4;
				}
				Smart.Log.Debug(TAG, $"IP {text4} has TRACK ID '{text5}', does not match expected TRACK ID '{id}',will skip scan this IP next time");
				text3 = text4;
				if (!string.IsNullOrEmpty(text3) && list2.Contains(text3))
				{
					list2.Remove(text3);
				}
				text = "TrackID not matched";
				if (skipTrackIdCheck)
				{
					Smart.Log.Debug(TAG, $"Skip TRACK ID check...");
					return text4;
				}
			}
			catch (Exception ex3)
			{
				text = ex3.Message;
			}
		}
		throw new TimeoutException(text2 + "-" + text);
	}

	public string FindDevice(string id)
	{
		lock (deviceLock)
		{
			if (savedDevices.ContainsValue(id))
			{
				int num = savedDevices.Keys[savedDevices.IndexOfValue(id)];
				string text = ReadId(num);
				if (!(id != text))
				{
					return FoundSubnet + num;
				}
				Smart.Log.Debug(TAG, $"Track ID {id} changed to {text} for host {num}");
				savedDevices.Remove(num);
			}
		}
		throw new KeyNotFoundException("Could not find device with ID " + id);
	}

	List<string> IDeviceFinder.Refresh()
	{
		return new List<string>();
	}

	public bool Check(string id)
	{
		return FindDevices().Contains(id);
	}

	private string SubnetSearch()
	{
		if (!searchedSubnet)
		{
			Smart.Log.Debug(TAG, "Searching for TCMD subnet");
			searchedSubnet = true;
		}
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		List<string> list = new List<string>();
		NetworkInterface[] array = allNetworkInterfaces;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (UnicastIPAddressInformation unicastAddress in array[i].GetIPProperties().UnicastAddresses)
			{
				if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
				{
					string text = unicastAddress.Address.ToString();
					if (text.StartsWith("10.") || text.StartsWith("172.") || text.StartsWith("192.168."))
					{
						list.Add(text);
					}
				}
			}
		}
		foreach (int item in Enumerable.Range(0, 4))
		{
			foreach (string item2 in list)
			{
				string[] array2 = item2.Split(new char[1] { '.' });
				if (array2.Length != 4)
				{
					continue;
				}
				string text2 = string.Format("{0}.{1}.{2}.", array2);
				int num = int.Parse(array2[3]);
				string text3 = text2 + offsets[item];
				Smart.Log.Debug(TAG, $"Trying to connect to IP {text3}");
				using TcpClient tcpClient = new TcpClient();
				try
				{
					if (!tcpClient.ConnectAsync(text3, 11000).Wait(1000))
					{
						throw new TimeoutException("Connection to port checker timed out");
					}
					return text2 + num;
				}
				catch (Exception)
				{
				}
			}
		}
		return string.Empty;
	}

	private List<string> DeviceScan()
	{
		int num = 2;
		int[] array;
		if (highestIp > 0)
		{
			array = offsets;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] < highestIp)
				{
					num++;
				}
			}
		}
		List<int> list = new List<int>();
		int num2 = 0;
		array = offsets;
		for (int i = 0; i < array.Length; i++)
		{
			int num3 = array[i];
			if (num2 > num)
			{
				break;
			}
			num2++;
			string host = FoundSubnet + num3;
			using TcpClient tcpClient = new TcpClient();
			try
			{
				if (tcpClient.ConnectAsync(host, 11000).Wait(1000))
				{
					if (num3 > highestIp)
					{
						highestIp = num3;
					}
					list.Add(num3);
				}
			}
			catch (Exception)
			{
			}
		}
		foreach (int item in new List<int>(savedDevices.Keys))
		{
			if (!list.Contains(item))
			{
				Smart.Log.Debug(TAG, $"Removing saved host {item}");
				savedDevices.Remove(item);
			}
		}
		foreach (int item2 in list)
		{
			if (savedDevices.ContainsKey(item2))
			{
				continue;
			}
			try
			{
				if (!PowerUpCheck(item2))
				{
					Smart.Log.Debug(TAG, $"Waiting for host {item2} to power up");
					continue;
				}
				string text = ReadId(item2);
				if (text != string.Empty)
				{
					savedDevices[item2] = text;
					Smart.Log.Debug(TAG, $"Saved host {item2} as ID {text}");
				}
			}
			catch (Exception ex2)
			{
				Smart.Log.Error(TAG, "Read ID failed: " + ex2.Message);
				Smart.Log.Debug(TAG, ex2.ToString());
			}
		}
		return new List<string>(savedDevices.Values);
	}

	private bool PowerUpCheck(int host)
	{
		string ip = FoundSubnet + host;
		return PowerUpCheck(ip);
	}

	private bool PowerUpCheck(string ip)
	{
		string text = "009A";
		string text2 = "00";
		ITestCommandClient val = Smart.NewTestCommandClient();
		ITestCommandClient val2 = val;
		try
		{
			((INetworkClient)val).SetEndPoint(ip, 11000);
			((INetworkClient)val).Connect();
			Smart.Thread.Wait(TimeSpan.FromSeconds(5.0), (Checker<bool>)((INetworkClient)val).IsConnected);
			if (!((INetworkClient)val).IsConnected())
			{
				throw new TimeoutException("Connection to client timed out");
			}
			ITestCommandResponse val3 = val.SendCommand(text, text2);
			return val3.DataHex == "01" || val3.DataHex == "0001";
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private string ReadId(int host)
	{
		string ip = FoundSubnet + host;
		return ReadId(ip);
	}

	private string ReadId(string ip)
	{
		string text = "0020";
		string text2 = "2726000100000080";
		ITestCommandClient val = Smart.NewTestCommandClient();
		ITestCommandClient val2 = val;
		try
		{
			((INetworkClient)val).SetEndPoint(ip, 11000);
			((INetworkClient)val).Connect();
			Smart.Thread.Wait(TimeSpan.FromSeconds(5.0), (Checker<bool>)((INetworkClient)val).IsConnected);
			if (!((INetworkClient)val).IsConnected())
			{
				throw new TimeoutException("Connection to client timed out");
			}
			byte[] data = val.SendCommand(text, text2).Data;
			string text3 = Smart.Convert.BytesToAscii(data).Substring(1).Replace('\0', ' ');
			int startIndex = 21;
			string text4 = text3.Substring(startIndex, 10).Trim();
			if (text4 == string.Empty)
			{
				text = "0108";
				text2 = "000001";
				data = val.SendCommand(text, text2).Data;
				text4 = Smart.Convert.BytesToAscii(data).Substring(2);
			}
			Smart.Log.Debug(TAG, $"Found track ID {text4} for ip {ip}");
			return text4;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}
}
