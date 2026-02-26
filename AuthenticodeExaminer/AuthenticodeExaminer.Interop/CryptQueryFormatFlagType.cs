using System;

namespace AuthenticodeExaminer.Interop;

[Flags]
internal enum CryptQueryFormatFlagType : uint
{
	CERT_QUERY_FORMAT_FLAG_BINARY = 2u,
	CERT_QUERY_FORMAT_FLAG_BASE64_ENCODED = 4u,
	CERT_QUERY_FORMAT_FLAG_ASN_ASCII_HEX_ENCODED = 8u,
	CERT_QUERY_FORMAT_FLAG_ALL = 0xEu
}
