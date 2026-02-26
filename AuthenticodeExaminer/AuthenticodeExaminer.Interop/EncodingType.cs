using System;

namespace AuthenticodeExaminer.Interop;

[Flags]
internal enum EncodingType : uint
{
	PKCS_7_ASN_ENCODING = 0x10000u,
	X509_ASN_ENCODING = 1u
}
