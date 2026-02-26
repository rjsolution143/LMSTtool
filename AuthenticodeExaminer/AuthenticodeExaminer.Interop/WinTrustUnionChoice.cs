namespace AuthenticodeExaminer.Interop;

internal enum WinTrustUnionChoice : uint
{
	WTD_CHOICE_FILE = 1u,
	WTD_CHOICE_CATALOG,
	WTD_CHOICE_BLOB,
	WTD_CHOICE_SIGNER,
	WTD_CHOICE_CERT
}
