using System;

namespace AuthenticodeExaminer.Interop;

[Flags]
internal enum CryptDecodeFlags : uint
{
	CRYPT_DECODE_ALLOC_FLAG = 0x8000u
}
