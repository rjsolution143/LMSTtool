namespace AuthenticodeExaminer.Interop;

internal struct SPC_SERIALIZED_OBJECT
{
	public unsafe fixed byte ClassId[16];

	public CRYPTOAPI_BLOB SerializedData;
}
