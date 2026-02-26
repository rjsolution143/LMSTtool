using System;

namespace AuthenticodeExaminer.Interop;

internal struct CERT_RDN
{
	public uint cRDNAttr;

	public IntPtr rgRDNAttr;
}
