using System;

namespace AuthenticodeExaminer.Interop;

internal struct CRYPT_ATTRIBUTE
{
	public string pszObjId;

	public uint cValue;

	public IntPtr rgValue;
}
