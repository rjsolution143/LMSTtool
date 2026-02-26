namespace AuthenticodeExaminer.Interop;

internal struct CMSG_SIGNER_INFO
{
	public uint dwVersion;

	public CRYPTOAPI_BLOB Issuer;

	public CRYPTOAPI_BLOB SerialNumber;

	public CRYPT_ALGORITHM_IDENTIFIER HashAlgorithm;

	public CRYPT_ALGORITHM_IDENTIFIER HashEncryptionAlgorithm;

	public CRYPTOAPI_BLOB EncryptedHash;

	public CRYPT_ATTRIBUTES AuthAttrs;

	public CRYPT_ATTRIBUTES UnauthAttrs;
}
