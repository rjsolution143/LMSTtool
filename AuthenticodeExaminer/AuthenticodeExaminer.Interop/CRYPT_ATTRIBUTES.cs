using System;

namespace AuthenticodeExaminer.Interop;

internal struct CRYPT_ATTRIBUTES
{
	public uint cAttr;

	public IntPtr rgAttr;
}
