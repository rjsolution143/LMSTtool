namespace AuthenticodeExaminer.Interop;

internal struct CRYPT_ALGORITHM_IDENTIFIER
{
	public string pszObjId;

	public CRYPTOAPI_BLOB Parameters;
}
