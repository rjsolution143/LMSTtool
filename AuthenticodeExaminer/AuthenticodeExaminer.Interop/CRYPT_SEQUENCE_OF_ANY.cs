namespace AuthenticodeExaminer.Interop;

internal struct CRYPT_SEQUENCE_OF_ANY
{
	public uint cValue;

	public unsafe CRYPTOAPI_BLOB* rgValue;
}
