using System;

namespace AuthenticodeExaminer;

[Flags]
public enum SignatureKind
{
	NestedSignature = 1,
	Signature = 2,
	AuthenticodeTimestamp = 4,
	Rfc3161Timestamp = 8,
	AnySignature = 3,
	AnyCounterSignature = 0xC,
	Any = 0xF
}
