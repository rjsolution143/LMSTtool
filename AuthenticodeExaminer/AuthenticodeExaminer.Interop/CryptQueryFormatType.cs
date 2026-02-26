namespace AuthenticodeExaminer.Interop;

internal enum CryptQueryFormatType : uint
{
	CERT_QUERY_FORMAT_BINARY = 1u,
	CERT_QUERY_FORMAT_BASE64_ENCODED,
	CERT_QUERY_FORMAT_ASN_ASCII_HEX_ENCODED
}
