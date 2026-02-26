using System;

namespace AuthenticodeExaminer.Interop;

internal struct CRYPTOAPI_BLOB
{
	public uint cbData;

	public IntPtr pbData;

	public unsafe ReadOnlySpan<byte> AsSpan()
	{
		return new ReadOnlySpan<byte>(pbData.ToPointer(), checked((int)cbData));
	}
}
