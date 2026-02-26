namespace AuthenticodeExaminer.Interop;

internal struct CERT_RDN_ATTR
{
	public string pszObjId;

	public uint dwValueType;

	public CRYPTOAPI_BLOB Value;
}
