using System;
using System.Management;
using Microsoft.Win32;

namespace SmartDevice;

internal sealed class PnPHwPortIdentifier
{
	private static string IdentifyPortInterfaceEnumeratedOn(PropertyDataCollection properties)
	{
		_ = string.Empty;
		return IdentifyPortInterfaceEnumeratedOn(properties["DeviceID"].Value.ToString());
	}

	public static string IdentifyPortInterfaceEnumeratedOn(string val)
	{
		string empty = string.Empty;
		int num = val.IndexOf("USB", StringComparison.OrdinalIgnoreCase);
		int num2 = val.IndexOf("{", StringComparison.OrdinalIgnoreCase);
		if (num == -1 || num2 == -1)
		{
			if (val.ToLower().Contains("wpdbusenumroot"))
			{
				val = val.Replace("##", "#");
				num = val.ToLower().IndexOf("usbstor", StringComparison.OrdinalIgnoreCase);
				num2 = val.IndexOf("{", StringComparison.OrdinalIgnoreCase);
			}
			else if (val.ToLower().Contains("usbstor"))
			{
				num = val.ToLower().IndexOf("usbstor", StringComparison.OrdinalIgnoreCase);
				num2 = val.IndexOf("{", StringComparison.OrdinalIgnoreCase);
			}
			if (num == -1 || num2 == -1)
			{
				return null;
			}
		}
		string regpath = "SYSTEM\\CurrentControlSet\\Control\\UsbFlags";
		byte[] value = new byte[1] { 1 };
		string text = val.Substring(num, num2 - num - 1).Replace('#', '\\');
		int num3 = text.IndexOf('\\', 0);
		int num4 = text.LastIndexOf('\\');
		string text2 = text.Substring(num3 + 1, num4 - num3 - 1);
		string text3 = text.Substring(num4 + 1);
		string empty2 = string.Empty;
		string empty3 = string.Empty;
		string text4 = string.Empty;
		int num5 = text2.ToLower().IndexOf("vid_", 0);
		int num6 = text2.ToLower().IndexOf("pid_", 0);
		if (num5 != -1 && num6 != -1)
		{
			empty2 = text2.ToLower().Substring(4, num6 - 5);
			empty3 = text2.ToLower().Substring(num6 + 4);
			int num7 = empty3.IndexOf('&');
			if (num7 != -1)
			{
				empty3 = empty3.Substring(0, num7);
			}
			text4 = $"IgnoreHWSerNum{empty2 + empty3}";
		}
		if (text3.IndexOf("&") == -1 && !string.IsNullOrEmpty(text4))
		{
			RegistryHelper.WriteRegistryItem(regpath, text4, value, RegistryValueKind.Binary);
		}
		char[] separator = new char[1] { '&' };
		string[] array = text2.Split(separator);
		if (array.Length == 3)
		{
			text2 = array[0] + "&" + array[1];
			array = text3.Split(separator);
			if (array.Length >= 3)
			{
				text3 = array[0] + "&" + array[1] + "&" + array[2];
			}
			empty = GetParentPrefixIdOfComposite(text2, text3);
			if ((string.IsNullOrEmpty(empty) || empty.IndexOf('&') == -1) && !string.IsNullOrEmpty(text4))
			{
				RegistryHelper.WriteRegistryItem(regpath, text4, value, RegistryValueKind.Binary);
			}
		}
		else
		{
			empty = text3;
		}
		return empty;
	}

	public static string GetParentPrefixIdOfComposite(string vidpid, string parentprefixid)
	{
		string result = string.Empty;
		string name = "SYSTEM\\CurrentControlSet\\Enum\\USB\\" + vidpid;
		RegistryKey localMachine = Registry.LocalMachine;
		RegistryKey registryKey = localMachine.OpenSubKey(name);
		if (registryKey == null)
		{
			localMachine.Close();
			return string.Empty;
		}
		string[] subKeyNames = registryKey.GetSubKeyNames();
		foreach (string text in subKeyNames)
		{
			RegistryKey registryKey2 = registryKey.OpenSubKey(text);
			if (registryKey2 != null)
			{
				object value = registryKey2.GetValue("ParentIdPrefix");
				string value2 = string.Empty;
				if (value != null)
				{
					value2 = value.ToString();
				}
				if (parentprefixid.Equals(value2, StringComparison.OrdinalIgnoreCase))
				{
					result = text;
					registryKey2.Close();
					break;
				}
				registryKey2.Close();
			}
		}
		registryKey.Close();
		localMachine.Close();
		return result;
	}
}
