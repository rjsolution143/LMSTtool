using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;

namespace SmartDevice;

public class UsbPortHelper
{
	public enum USB_PROTOCOL
	{
		unknown = -1,
		usb11,
		usb20,
		usb30
	}

	private readonly Dictionary<string, string> cachedhubname = new Dictionary<string, string>();

	public string GetUsbPortPhysicalLocation(string dbccName)
	{
		_ = MethodBase.GetCurrentMethod().Name;
		string pnpDeviceIdBasedOnDbccName = GetPnpDeviceIdBasedOnDbccName(dbccName);
		return GetUsbPortPhysicalLocation(pnpDeviceIdBasedOnDbccName, dbccName);
	}

	public string GetUsbPortPhysicalLocation(ref SortedList<string, string> info)
	{
		_ = MethodBase.GetCurrentMethod().Name;
		string text = info["DeviceID"];
		text = text.Replace("\\", "\\\\");
		string dbccname = info["SymbolicName"];
		string text2 = DetermineHub(text);
		string hubPortId = GetHubPortId(text2);
		int portNumberOnHub = DeterminePortNumberOnHub(dbccname, text2);
		return ComposeUsbPortPhysicalLocation(hubPortId, portNumberOnHub, text);
	}

	public string GetUsbPortPhysicalLocation(string pnpDeviceId, string pnpDbccName)
	{
		_ = MethodBase.GetCurrentMethod().Name;
		pnpDeviceId = pnpDeviceId.Replace("\\", "\\\\");
		string text = DetermineHub(pnpDeviceId);
		string hubPortId = GetHubPortId(text);
		int portNumberOnHub = DeterminePortNumberOnHub(pnpDbccName, text);
		return ComposeUsbPortPhysicalLocation(hubPortId, portNumberOnHub, pnpDeviceId);
	}

	public string GetPnpDeviceIdBasedOnDbccName(string dbccName)
	{
		_ = MethodBase.GetCurrentMethod().Name;
		int num = dbccName.IndexOf("USB", StringComparison.OrdinalIgnoreCase);
		int num2 = dbccName.IndexOf("{", StringComparison.OrdinalIgnoreCase);
		if (num == -1 || num2 == -1)
		{
			if (dbccName.ToLower().Contains("wpdbusenumroot"))
			{
				dbccName = dbccName.Replace("##", "#");
				num = dbccName.ToLower().IndexOf("usbstor", StringComparison.OrdinalIgnoreCase);
				num2 = dbccName.IndexOf("{", StringComparison.OrdinalIgnoreCase);
			}
			else if (dbccName.ToLower().Contains("usbstor"))
			{
				num = dbccName.ToLower().IndexOf("usbstor", StringComparison.OrdinalIgnoreCase);
				num2 = dbccName.IndexOf("{", StringComparison.OrdinalIgnoreCase);
			}
			if (num == -1 || num2 == -1)
			{
				return null;
			}
		}
		(new byte[1])[0] = 1;
		return dbccName.Substring(num, num2 - num - 1).Replace('#', '\\');
	}

	private string ComposeUsbPortPhysicalLocation(string HubPortId, int PortNumberOnHub, string PNPDeviceID)
	{
		_ = MethodBase.GetCurrentMethod().Name;
		if (Environment.OSVersion.Version.Major >= 6 && !string.IsNullOrEmpty(HubPortId) && PortNumberOnHub != -1)
		{
			return HubPortId + "&" + PortNumberOnHub;
		}
		if (PNPDeviceID == null)
		{
			return null;
		}
		int num = PNPDeviceID.IndexOf('\\', 0);
		int num2 = PNPDeviceID.LastIndexOf('\\');
		string text = PNPDeviceID.Substring(num + 1, num2 - num - 1);
		string text2 = PNPDeviceID.Substring(num2 + 1);
		char[] separator = new char[1] { '&' };
		string[] array = text.Split(separator);
		if (array.Length == 3)
		{
			text = array[0] + "&" + array[1];
			array = text2.Split(separator);
			if (array.Length >= 3)
			{
				text2 = array[0] + "&" + array[1] + "&" + array[2];
			}
			text2 = PnPHwPortIdentifier.GetParentPrefixIdOfComposite(text, text2);
		}
		return text2;
	}

	private int DeterminePortNumberOnHub(string dbccname, string pnphubname)
	{
		int result = -1;
		if (Environment.OSVersion.Version.Major >= 6 && DetermineUsbProtocol(pnphubname) == USB_PROTOCOL.usb30)
		{
			_ = string.Empty;
			string[] array = dbccname.Split(new char[1] { '#' });
			if (array.Count() > 2)
			{
				_ = array[2];
				string text = RegistryHelper.ReadRegistryItem("SYSTEM\\CurrentControlSet\\Enum\\USB\\" + array[1] + "\\" + array[2], "LocationInformation");
				if (!string.IsNullOrEmpty(text) && text.StartsWith("Port_#"))
				{
					string[] array2 = text.Split(new char[1] { '.' });
					if (array2.Count() > 0)
					{
						if (int.TryParse(array2[0].Replace("Port_#", ""), out result))
						{
							return result;
						}
						result = -1;
					}
				}
			}
		}
		string text2 = PnPHwPortIdentifier.IdentifyPortInterfaceEnumeratedOn(dbccname);
		if (string.IsNullOrEmpty(text2))
		{
			return -1;
		}
		string[] array3 = text2.Split(new char[1] { '&' });
		if (array3.Length != 0 && !int.TryParse(array3[^1], out result))
		{
			result = -1;
		}
		return result;
	}

	private string GetHubPortId(string hubName)
	{
		if (string.IsNullOrEmpty(hubName))
		{
			return null;
		}
		if (DetermineUsbProtocol(hubName) == USB_PROTOCOL.usb30)
		{
			if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor > 1)
			{
				return hubName.Substring(17);
			}
			return hubName.Substring(19);
		}
		if (DetermineUsbProtocol(hubName) == USB_PROTOCOL.usb20)
		{
			return hubName.Substring(17);
		}
		if (DetermineUsbProtocol(hubName) == USB_PROTOCOL.usb11)
		{
			return hubName.Substring(15);
		}
		return string.Empty;
	}

	public USB_PROTOCOL DetermineUsbProtocol(string hubname)
	{
		if (string.IsNullOrEmpty(hubname))
		{
			return USB_PROTOCOL.unknown;
		}
		if (hubname.StartsWith("iusb3\\\\root_hub30\\\\") || hubname.StartsWith("usb\\\\root_hub30\\\\") || hubname.StartsWith("nusb3\\\\root_hub30\\\\"))
		{
			return USB_PROTOCOL.usb30;
		}
		if (hubname.StartsWith("usb\\\\root_hub20\\\\"))
		{
			return USB_PROTOCOL.usb20;
		}
		if (hubname.StartsWith("usb\\\\root_hub\\\\"))
		{
			return USB_PROTOCOL.usb11;
		}
		return USB_PROTOCOL.unknown;
	}

	private string DetermineHub(string pnpid)
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Expected O, but got Unknown
		_ = MethodBase.GetCurrentMethod().Name;
		if (string.IsNullOrEmpty(pnpid))
		{
			return string.Empty;
		}
		string key = pnpid.ToLower();
		if (cachedhubname.ContainsKey(key))
		{
			return cachedhubname[key];
		}
		string text = string.Empty;
		string hostName = Dns.GetHostName();
		string text2 = Environment.GetEnvironmentVariable("COMPUTERNAME");
		string text3 = Environment.MachineName;
		if (string.IsNullOrEmpty(text2))
		{
			text2 = hostName;
		}
		if (string.IsNullOrEmpty(text3))
		{
			text3 = hostName;
		}
		string text4 = "\\\\" + hostName + "\\root\\cimv2:Win32_PnPEntity.DeviceID=\"";
		string text5 = "\\\\" + text2 + "\\root\\cimv2:Win32_PnPEntity.DeviceID=\"";
		string text6 = "\\\\" + text3 + "\\root\\cimv2:Win32_PnPEntity.DeviceID=\"";
		string value = text4 + pnpid + "\"";
		string value2 = text5 + pnpid + "\"";
		string value3 = text6 + pnpid + "\"";
		string text7 = "select * from Win32_USBControllerDevice";
		ManagementObjectEnumerator enumerator = new ManagementObjectSearcher("root\\cimv2", text7).Get().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ManagementObject val = (ManagementObject)enumerator.Current;
				if (!((ManagementBaseObject)val)["dependent"].ToString().Equals(value, StringComparison.OrdinalIgnoreCase) && !((ManagementBaseObject)val)["dependent"].ToString().Equals(value2, StringComparison.OrdinalIgnoreCase) && !((ManagementBaseObject)val)["dependent"].ToString().Equals(value3, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}
				string value4 = ((ManagementBaseObject)val)["antecedent"].ToString();
				text7 = "select * from Win32_USBControllerDevice";
				ManagementObjectEnumerator enumerator2 = new ManagementObjectSearcher("root\\cimv2", text7).Get().GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						ManagementObject val2 = (ManagementObject)enumerator2.Current;
						if (!((ManagementBaseObject)val2)["antecedent"].ToString().Equals(value4, StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}
						string text8 = ((ManagementBaseObject)val2)["dependent"].ToString().ToLower();
						if (!text8.Contains("\\root_hub"))
						{
							continue;
						}
						char[] separator = new char[2] { '=', '"' };
						string[] array = text8.Split(separator, StringSplitOptions.RemoveEmptyEntries);
						if (array.Length > 1)
						{
							text = array[1];
							if (cachedhubname.ContainsKey(key))
							{
								cachedhubname[key] = text;
							}
							else
							{
								cachedhubname.Add(key, text);
							}
							break;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator2)?.Dispose();
				}
			}
			return text;
		}
		finally
		{
			((IDisposable)enumerator)?.Dispose();
		}
	}
}
