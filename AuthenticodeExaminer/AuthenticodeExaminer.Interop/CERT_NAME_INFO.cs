using System;

namespace AuthenticodeExaminer.Interop;

internal struct CERT_NAME_INFO
{
	public uint cRDN;

	public IntPtr rgRDN;
}
