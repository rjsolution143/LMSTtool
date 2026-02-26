using System;
using System.Runtime.InteropServices;

namespace AuthenticodeExaminer.Interop;

internal static class Wintrust
{
	[DllImport("wintrust.dll")]
	[return: MarshalAs(UnmanagedType.I4)]
	public unsafe static extern int WinVerifyTrustEx([In][MarshalAs(UnmanagedType.SysInt)] IntPtr hwnd, [In][MarshalAs(UnmanagedType.LPStruct)] Guid pgActionID, [In] WINTRUST_DATA* pWVTData);
}
