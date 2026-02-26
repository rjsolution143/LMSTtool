using System.IO;
using System.Linq;
using AuthenticodeExaminer;

namespace SmartBase;

public class SafeCheck
{
	public static DllValidity ValidityCheckSafe(string filePath)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
		if (!File.Exists(filePath))
		{
			return DllValidity.NotFound;
		}
		if (!new string[12]
		{
			"ismart", "smartbase", "smartdevice", "smarthelper", "smartlauncher", "smartprint", "smartrsd", "smarttool", "smartutil", "smartweb",
			"smartlauncher", "smarttool"
		}.Contains(fileNameWithoutExtension.ToLowerInvariant()))
		{
			return DllValidity.NotAllowed;
		}
		FileInspector val = new FileInspector(filePath);
		if ((int)val.Validate((RevocationChecking)1) != 0)
		{
			return DllValidity.Invalid;
		}
		foreach (AuthenticodeSignature signature in val.GetSignatures())
		{
			if (!signature.SigningCertificate.Subject.ToLowerInvariant().StartsWith("cn=lenovo, o=lenovo"))
			{
				return DllValidity.Unrecognized;
			}
		}
		return DllValidity.Valid;
	}
}
