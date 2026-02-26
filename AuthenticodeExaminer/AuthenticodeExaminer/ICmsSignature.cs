using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace AuthenticodeExaminer;

public interface ICmsSignature
{
	Oid? DigestAlgorithm { get; }

	Oid? HashEncryptionAlgorithm { get; }

	IReadOnlyList<CryptographicAttributeObject> UnsignedAttributes { get; }

	IReadOnlyList<CryptographicAttributeObject> SignedAttributes { get; }

	X509Certificate2? Certificate { get; }

	SignatureKind Kind { get; }

	X509Certificate2Collection AdditionalCertificates { get; }

	HashAlgorithmName DigestAlgorithmName { get; }

	byte[]? Content { get; }

	byte[] SerialNumber { get; }

	ReadOnlyMemory<byte> Signature { get; }

	IReadOnlyList<ICmsSignature> GetNestedSignatures();
}
