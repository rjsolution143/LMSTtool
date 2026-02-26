using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SmartDevice;

public class Fastboot
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate bool IsMyDevice(IntPtr p);

	public const string defaultXMLfilename = "flashfile.xml";

	protected string _MyDevicePnpDbccName = string.Empty;

	public int platformType = 1;

	protected IsMyDevice _myDevice;

	public bool IsMyFastbootDevice(IntPtr p)
	{
		if (IntPtr.Zero == p)
		{
			return false;
		}
		string value = Marshal.PtrToStringAnsi(p);
		if (string.IsNullOrEmpty(value))
		{
			return false;
		}
		return _MyDevicePnpDbccName.Equals(value);
	}

	[DllImport("fastboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern int FlashXML(IntPtr fileName, uint MaxDownloadSize, int eFBClass, IntPtr windowHandle, IsMyDevice pfn);

	[DllImport("fastboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern int FlashSingleFile(IntPtr fileName, IntPtr PartitionName, IntPtr MD5, uint MaxDownloadSize, int eFBClass, IntPtr windowHandle, IsMyDevice pfn);

	[DllImport("fastboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern int Boot(IntPtr fileName, IntPtr PartitionName, IntPtr MD5, int eFBClass, IntPtr windowHandle, IsMyDevice pfn);

	[DllImport("fastboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern int FastbootReboot(int eFBClass, IntPtr windowHandle, IsMyDevice pfn);

	[DllImport("fastboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern int FastbootRebootNoWait(int eFBClass, IntPtr windowHandle, IsMyDevice pfn);

	[DllImport("fastboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern int FastbootRebootBootloader(int eFBClass, IntPtr windowHandle, IsMyDevice pfn);

	[DllImport("fastboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern int FastbootRebootBootloaderNoWait(int eFBClass, IntPtr windowHandle, IsMyDevice pfn);

	[DllImport("fastboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern int FastbootOEM(IntPtr command, [Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder oemvalue, int eFBClass, IntPtr windowHandle, IsMyDevice pfn);

	[DllImport("fastboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern int FastbootOEMWithAllResponse(IntPtr command, [Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder oemvalue, int eFBClass, IntPtr windowHandle, IsMyDevice pfn);

	[DllImport("fastboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern int FastbootGetVar(IntPtr variable, [Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder varvalue, int eFBClass, IntPtr windowHandle, IsMyDevice pfn);

	[DllImport("fastboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern int FastbootErasePartition(IntPtr partition, int eFBClass, IntPtr windowHandle, IsMyDevice pfn);

	[DllImport("fastboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern int LoadXML(IntPtr fileName, int eFBClass, IntPtr windowHandle);

	[DllImport("fastboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern int UnloadXML(IntPtr fileName, int eFBClass, IntPtr windowHandle);

	[DllImport("fastboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern int UnloadFile(IntPtr fileName, int eFBClass, IntPtr windowHandle);

	[DllImport("fastboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern int FBContinue([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder varvalue, int eFBClass, IntPtr windowHandle, IsMyDevice pfn);

	[DllImport("fastboot.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public static extern int FastbootGetAllDevices(IntPtr expextedMotoFastbootPortVidPidList, bool getAdbDevices, bool getFastbootDevices, [Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder response, int eFBClass, IntPtr windowHandle);
}
