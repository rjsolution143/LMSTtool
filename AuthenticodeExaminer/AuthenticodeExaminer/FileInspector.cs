using System.Collections.Generic;

namespace AuthenticodeExaminer;

public class FileInspector
{
	private readonly string _filePath;

	public FileInspector(string filePath)
	{
		_filePath = filePath;
	}

	public SignatureCheckResult Validate(RevocationChecking revocationChecking = RevocationChecking.Offline)
	{
		return (SignatureCheckResult)FileSignatureVerifier.IsFileSignatureValid(_filePath, revocationChecking);
	}

	public IEnumerable<AuthenticodeSignature> GetSignatures()
	{
		IEnumerable<ICmsSignature> enumerable = SignatureTreeInspector.Extract(_filePath).VisitAll(SignatureKind.AnySignature, deep: true);
		foreach (ICmsSignature item in enumerable)
		{
			yield return new AuthenticodeSignature(item);
		}
	}
}
