using System;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Microsoft.VisualBasic;
using Microsoft.Win32;

namespace SmartDevice;

internal sealed class RegistryHelper
{
	public static void GrantAllAccessPermission(string key)
	{
		try
		{
			NTAccount nTAccount = new SecurityIdentifier(WellKnownSidType.WorldSid, null).Translate(typeof(NTAccount)) as NTAccount;
			using RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(key, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ChangePermissions);
			RegistrySecurity accessControl = registryKey.GetAccessControl();
			RegistryAccessRule rule = new RegistryAccessRule(nTAccount.ToString(), RegistryRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow);
			accessControl.AddAccessRule(rule);
			registryKey.SetAccessControl(accessControl);
		}
		catch (SecurityException)
		{
		}
		catch (UnauthorizedAccessException)
		{
		}
	}

	public static string ReadRegistryItem(string regpath, string item)
	{
		return ReadRegistryItem(regpath, item, null);
	}

	public static string ReadRegistryItem(string regpath, string item, string defaultValue)
	{
		string result = defaultValue;
		RegistryKey localMachine = Registry.LocalMachine;
		RegistryKey registryKey = localMachine.OpenSubKey(regpath);
		if (registryKey != null)
		{
			object value = registryKey.GetValue(item, defaultValue);
			if (value != null)
			{
				RegistryValueKind valueKind = registryKey.GetValueKind(item);
				if (valueKind == RegistryValueKind.Binary)
				{
					StringBuilder stringBuilder = new StringBuilder("");
					byte[] array = (byte[])value;
					foreach (byte b in array)
					{
						stringBuilder.Append($"{b:X2}");
					}
					result = stringBuilder.ToString();
				}
				else
				{
					result = value.ToString();
				}
			}
			registryKey.Close();
		}
		localMachine.Close();
		return result;
	}

	public static string[] ReadMultiStringRegistryItem(string regpath, string item, string[] defaultValue)
	{
		string[] result = defaultValue;
		RegistryKey localMachine = Registry.LocalMachine;
		RegistryKey registryKey = localMachine.OpenSubKey(regpath);
		if (registryKey != null)
		{
			result = (string[])registryKey.GetValue(item, defaultValue);
			registryKey.Close();
		}
		localMachine.Close();
		return result;
	}

	public static void WriteMultiStringRegistryItem(string regpath, string item, string[] value)
	{
		RegistryKey localMachine = Registry.LocalMachine;
		RegistryKey registryKey = localMachine.OpenSubKey(regpath, writable: true);
		if (registryKey != null)
		{
			registryKey.SetValue(item, value, RegistryValueKind.MultiString);
			registryKey.Close();
		}
		localMachine.Close();
	}

	public static void WriteRegistryItem(string regpath, string item, object value, RegistryValueKind kind)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		RegistryKey localMachine = Registry.LocalMachine;
		try
		{
			RegistryKey registryKey = localMachine.OpenSubKey(regpath, writable: true);
			if (registryKey != null)
			{
				registryKey.SetValue(item, value, kind);
				registryKey.Close();
			}
		}
		catch (SecurityException ex)
		{
			Interaction.MsgBox((object)("Cannot update registry path '" + regpath + "'.\r\n" + ex.Message), (MsgBoxStyle)0, (object)"NexTest PNP Driver Error");
		}
		finally
		{
			localMachine.Close();
		}
	}
}
