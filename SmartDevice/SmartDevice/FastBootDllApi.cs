using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice;

public class FastBootDllApi : Fastboot
{
	public Dictionary<string, string> GetAllFastbootDevices()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		string name = MethodBase.GetCurrentMethod().DeclaringType.Name;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		DummyWindow dummyWindow = new DummyWindow();
		CreateParams val = new CreateParams();
		val.Caption = Thread.CurrentThread.ManagedThreadId.ToString();
		((NativeWindow)dummyWindow).CreateHandle(val);
		dummyWindow.Logger = Smart.Log;
		StringBuilder stringBuilder = new StringBuilder(2048);
		_ = IntPtr.Zero;
		IntPtr expextedMotoFastbootPortVidPidList = Marshal.StringToHGlobalAnsi(string.Empty);
		int num = 0;
		num = Fastboot.FastbootGetAllDevices(expextedMotoFastbootPortVidPidList, getAdbDevices: false, getFastbootDevices: true, stringBuilder, platformType, ((NativeWindow)dummyWindow).Handle);
		((NativeWindow)dummyWindow).DestroyHandle();
		if (num != 0)
		{
			string text = $"{name} returned error code {num}";
			Smart.Log.Error(name, text);
		}
		else
		{
			string text2 = stringBuilder.ToString();
			Smart.Log.Info(name, "Response:" + text2);
			string[] array = text2.Split(new string[2] { "_SN:", "_PNPDBCCNAME:" }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i += 2)
			{
				dictionary.Add(array[i], array[i + 1]);
			}
		}
		return dictionary;
	}

	public Dictionary<string, string> GetAllAdbDevices()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		string name = MethodBase.GetCurrentMethod().DeclaringType.Name;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		DummyWindow dummyWindow = new DummyWindow();
		CreateParams val = new CreateParams();
		val.Caption = Thread.CurrentThread.ManagedThreadId.ToString();
		((NativeWindow)dummyWindow).CreateHandle(val);
		dummyWindow.Logger = Smart.Log;
		StringBuilder stringBuilder = new StringBuilder(256);
		_ = IntPtr.Zero;
		IntPtr expextedMotoFastbootPortVidPidList = Marshal.StringToHGlobalAnsi(string.Empty);
		int num = 0;
		num = Fastboot.FastbootGetAllDevices(expextedMotoFastbootPortVidPidList, getAdbDevices: true, getFastbootDevices: false, stringBuilder, platformType, ((NativeWindow)dummyWindow).Handle);
		((NativeWindow)dummyWindow).DestroyHandle();
		if (num != 0)
		{
			string text = $"{name} returned error code {num}";
			Smart.Log.Error(name, text);
		}
		else
		{
			string text2 = stringBuilder.ToString();
			Smart.Log.Info(name, "Response:" + text2);
			string[] array = text2.Split(new string[2] { "_SN:", "_PNPDBCCNAME:" }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i += 2)
			{
				dictionary.Add(array[i], array[i + 1]);
			}
		}
		return dictionary;
	}

	public List<string> FastbootShell(IDevice device, string command)
	{
		string name = MethodBase.GetCurrentMethod().DeclaringType.Name;
		Smart.Log.Error(name, "command:" + command);
		List<string> list = new List<string>();
		if (command.TrimStart(Array.Empty<char>()).StartsWith("oem"))
		{
			list = FastbootShell_OemTest(device, command);
		}
		else if (command.TrimStart(Array.Empty<char>()).StartsWith("getvar"))
		{
			list = FastbootShell_GetVarTest(device, command);
		}
		else if (command.TrimStart(Array.Empty<char>()).StartsWith("reboot"))
		{
			command.Contains("bootloader");
		}
		Smart.Log.Error(name, "response:" + list);
		return list;
	}

	public List<string> FastbootShell_OemTest(IDevice device, string oemCommand)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		string name = MethodBase.GetCurrentMethod().DeclaringType.Name;
		_MyDevicePnpDbccName = device.PnpDbccName;
		_myDevice = base.IsMyFastbootDevice;
		_ = IntPtr.Zero;
		DummyWindow dummyWindow = new DummyWindow();
		CreateParams val = new CreateParams();
		val.Caption = Thread.CurrentThread.ManagedThreadId.ToString();
		((NativeWindow)dummyWindow).CreateHandle(val);
		dummyWindow.Logger = Smart.Log;
		IntPtr command = Marshal.StringToHGlobalAnsi(oemCommand);
		StringBuilder stringBuilder = new StringBuilder(2048);
		int num = 0;
		num = Fastboot.FastbootOEMWithAllResponse(command, stringBuilder, platformType, ((NativeWindow)dummyWindow).Handle, _myDevice);
		((NativeWindow)dummyWindow).DestroyHandle();
		if (num != 0)
		{
			string text = $"{name} returned error code {num}";
			Smart.Log.Error(name, text);
		}
		else
		{
			string text2 = stringBuilder.ToString();
			Smart.Log.Info(name, "Oem Response:" + text2);
		}
		return new List<string>();
	}

	public List<string> FastbootShell_GetVarTest(IDevice device, string getVarCommand)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		string name = MethodBase.GetCurrentMethod().DeclaringType.Name;
		_MyDevicePnpDbccName = device.PnpDbccName;
		_myDevice = base.IsMyFastbootDevice;
		_ = IntPtr.Zero;
		DummyWindow dummyWindow = new DummyWindow();
		CreateParams val = new CreateParams();
		val.Caption = Thread.CurrentThread.ManagedThreadId.ToString();
		((NativeWindow)dummyWindow).CreateHandle(val);
		dummyWindow.Logger = Smart.Log;
		IntPtr variable = Marshal.StringToHGlobalAnsi(getVarCommand);
		StringBuilder stringBuilder = new StringBuilder(2048);
		int num = 0;
		num = Fastboot.FastbootGetVar(variable, stringBuilder, platformType, ((NativeWindow)dummyWindow).Handle, _myDevice);
		((NativeWindow)dummyWindow).DestroyHandle();
		if (num != 0)
		{
			string text = $"{name} returned error code {num}";
			Smart.Log.Error(name, text);
		}
		else
		{
			string text2 = stringBuilder.ToString();
			Smart.Log.Info(name, "Getvar Response:" + text2);
		}
		return new List<string>();
	}
}
