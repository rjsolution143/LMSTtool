using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Reflection;
using System.Security.AccessControl;
using System.Windows.Forms;
using ISmart;
using Microsoft.Win32;

namespace SmartDevice.Steps;

public class GetComportBasedOnVidPid : UnisocBaseTest
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Expected O, but got Unknown
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = string.Empty;
		if (((dynamic)base.Info.Args).InterfacePidVid != null)
		{
			text = ((dynamic)base.Info.Args).InterfacePidVid.ToString();
		}
		string text2 = "Motorola ADB Interface";
		if (((dynamic)base.Info.Args).InterfaceName != null)
		{
			text2 = ((dynamic)base.Info.Args).InterfaceName.ToString();
		}
		ManagementObjectSearcher val2 = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity where DeviceID Like \"USB%\"");
		List<string> list = new List<string>();
		ManagementObjectEnumerator enumerator = val2.Get().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ManagementObject val3 = (ManagementObject)enumerator.Current;
				if (((ManagementBaseObject)val3).Properties["Name"].Value.ToString().ToUpper().Contains(text2.ToUpper()))
				{
					string text3 = ((ManagementBaseObject)val3).Properties["DeviceID"].Value.ToString();
					if (text != string.Empty && text3.Contains(text))
					{
						list.Add(text3);
					}
					else
					{
						list.Add(text3);
					}
					break;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator)?.Dispose();
		}
		if (list.Count == 0)
		{
			string text4 = $"There is no device interface {text2} with {text} found on PC, please check!";
			Smart.Log.Error(TAG, text4);
			val.Prompt.CloseMessageBox();
			string text5 = Smart.Locale.Xlate(base.Info.Name);
			Smart.Log.Debug(TAG, "Opening new MessageBox");
			val.Prompt.MessageBox(text5, text4, (MessageBoxButtons)0, (MessageBoxIcon)64);
			LogResult((Result)1, TAG, text4);
			return;
		}
		if (list.Count > 1)
		{
			string text6 = $"There are more than 1 {text2} with {text} found on PC, please remove and only keep one device!";
			Smart.Log.Error(TAG, text6);
			val.Prompt.CloseMessageBox();
			string text7 = Smart.Locale.Xlate(base.Info.Name);
			Smart.Log.Debug(TAG, "Opening new MessageBox");
			val.Prompt.MessageBox(text7, text6, (MessageBoxButtons)0, (MessageBoxIcon)64);
			LogResult((Result)1, TAG, text6);
			return;
		}
		base.Cache["pidvid"] = list[0];
		Smart.Log.Info(TAG, "device vid_pid is " + base.Cache["pidvid"]);
		if (((!CheckPidVidContainBusLocationInfo(base.Cache["pidvid"]))) && ((((dynamic)base.Info.Args).SetIgnoreSerialNumberGenInReg != null && (bool)((dynamic)base.Info.Args).SetIgnoreSerialNumberGenInReg) ? true : false))
		{
			RegIgnoreSerialNumberGen(base.Cache["pidvid"]);
		}
		string empty = string.Empty;
		empty = GetInterfaceAssignedComport(list[0]);
		Smart.Log.Info(TAG, "Found com port number in local cfg file is:" + empty);
		if (empty == string.Empty)
		{
			base.Cache["kId"] = "0";
			SetPreCondition("NotFound");
		}
		else
		{
			base.Cache["kId"] = empty;
			SetPreCondition("Found");
		}
		bool flag = false;
		if (((dynamic)base.Info.Args).AlwaysSetToNotFound != null)
		{
			flag = (bool)((dynamic)base.Info.Args).AlwaysSetToNotFound;
		}
		if (flag)
		{
			SetPreCondition("NotFound");
		}
		LogResult((Result)8);
	}

	private string GetInterfaceAssignedComport(string deviceId)
	{
		string name = MethodBase.GetCurrentMethod().Name;
		Smart.Log.Info(name, "Enter...");
		Smart.Log.Info(name, $"Looking for com port number for {deviceId} from local file.");
		string result = string.Empty;
		lock (UnisocBaseTest.obj)
		{
			if (File.Exists(ComportCfgFile))
			{
				string[] array = File.ReadAllLines(ComportCfgFile);
				foreach (string text in array)
				{
					Smart.Log.Info(name, text);
					if (text.Contains(deviceId))
					{
						result = text.Split(new char[1] { ',' })[1];
						break;
					}
				}
			}
		}
		return result;
	}

	private bool CheckPidVidContainBusLocationInfo(string deviceVidPid)
	{
		Smart.Log.Info(MethodBase.GetCurrentMethod().Name, "enter...");
		Smart.Log.Info(MethodBase.GetCurrentMethod().Name, $"Check {deviceVidPid} contain bus location info");
		bool result = true;
		if (!deviceVidPid.Substring(deviceVidPid.LastIndexOf('\\') + 1).Trim().Contains("&"))
		{
			result = false;
		}
		return result;
	}

	public void RegIgnoreSerialNumberGen(string vidpidToIgnore)
	{
		Smart.Log.Info(MethodBase.GetCurrentMethod().Name, "enter...");
		if (vidpidToIgnore.StartsWith("USB\\"))
		{
			string text = vidpidToIgnore.Substring(vidpidToIgnore.IndexOf('\\') + 1);
			vidpidToIgnore = text.Substring(0, text.LastIndexOf('\\'));
		}
		Smart.Log.Verbose(TAG, "vid_pid To Ignore " + vidpidToIgnore);
		if (vidpidToIgnore.Contains("VID_") && vidpidToIgnore.Contains("&") && vidpidToIgnore.Contains("PID_"))
		{
			string[] array = vidpidToIgnore.Split('&', '_');
			vidpidToIgnore = array[1] + array[3];
		}
		Smart.Log.Verbose(TAG, "Ignore Sn gen for " + vidpidToIgnore);
		RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\usbflags", writable: true);
		registryKey.SetValue("GlobalDisableSerNumGen", new byte[1] { 1 }, RegistryValueKind.Binary);
		registryKey.SetValue("IgnoreHWSerNum" + vidpidToIgnore, new byte[1] { 1 }, RegistryValueKind.Binary);
		string[] subKeyNames = registryKey.GetSubKeyNames();
		foreach (string text2 in subKeyNames)
		{
			Smart.Log.Verbose(TAG, "vid_pid_rev_name:" + text2);
			if (text2.ToUpper().StartsWith(vidpidToIgnore.ToUpper(), StringComparison.OrdinalIgnoreCase))
			{
				registryKey.OpenSubKey(text2, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl).SetValue("IgnoreHWSerNum", new byte[1] { 1 }, RegistryValueKind.Binary);
			}
		}
	}
}
