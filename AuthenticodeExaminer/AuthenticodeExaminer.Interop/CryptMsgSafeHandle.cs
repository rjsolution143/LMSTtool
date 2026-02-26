using Microsoft.Win32.SafeHandles;

namespace AuthenticodeExaminer.Interop;

internal class CryptMsgSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
{
	public static CryptMsgSafeHandle InvalidHandle { get; }

	static CryptMsgSafeHandle()
	{
		InvalidHandle = new CryptMsgSafeHandle(ownsHandle: true);
		InvalidHandle.SetHandleAsInvalid();
	}

	public CryptMsgSafeHandle(bool ownsHandle)
		: base(ownsHandle)
	{
	}

	public CryptMsgSafeHandle()
		: base(ownsHandle: true)
	{
	}

	protected override bool ReleaseHandle()
	{
		return Crypt32.CryptMsgClose(handle);
	}
}
