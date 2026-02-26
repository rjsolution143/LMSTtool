namespace AuthenticodeExaminer;

public enum SignatureCheckResult
{
	Valid = 0,
	NoSignature = -2146762496,
	BadDigest = -2146869232,
	UnknownProvider = -2146762751,
	UntrustedRoot = -2146762487,
	ExplicitDistrust = -2146762479,
	CertificateExpired = -2146762495,
	UnknownFailure = -2146762485,
	RevokedCertificate = -2146762484,
	UnknownSubject = -2146762749
}
