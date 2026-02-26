using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace AuthenticodeExaminer.Interop;

internal sealed class LocalBufferSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
{
	public static LocalBufferSafeHandle Zero
	{
		get
		{
			LocalBufferSafeHandle localBufferSafeHandle = new LocalBufferSafeHandle(ownsHandle: true);
			localBufferSafeHandle.SetHandle(IntPtr.Zero);
			return localBufferSafeHandle;
		}
	}

	[DllImport("kernel32.dll", ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.SysInt)]
	private static extern IntPtr LocalFree([In][MarshalAs(UnmanagedType.SysInt)] IntPtr hMem);

	[DllImport("kernel32.dll", ExactSpelling = true)]
	[return: MarshalAs(UnmanagedType.SysInt)]
	private static extern IntPtr LocalAlloc([In][MarshalAs(UnmanagedType.U2)] ushort uFlags, [In][MarshalAs(UnmanagedType.SysInt)] IntPtr uBytes);

	public LocalBufferSafeHandle(bool ownsHandle)
		: base(ownsHandle)
	{
	}

	public LocalBufferSafeHandle()
		: this(ownsHandle: true)
	{
	}

	public static LocalBufferSafeHandle Alloc(IntPtr size)
	{
		LocalBufferSafeHandle localBufferSafeHandle = new LocalBufferSafeHandle(ownsHandle: true);
		IntPtr intPtr = LocalAlloc(0, size);
		localBufferSafeHandle.SetHandle(intPtr);
		return localBufferSafeHandle;
	}

	public static LocalBufferSafeHandle Alloc(long size)
	{
		return Alloc(new IntPtr(size));
	}

	public static LocalBufferSafeHandle Alloc(int size)
	{
		return Alloc(new IntPtr(size));
	}

	protected override bool ReleaseHandle()
	{
		return IntPtr.Zero == LocalFree(handle);
	}
}
