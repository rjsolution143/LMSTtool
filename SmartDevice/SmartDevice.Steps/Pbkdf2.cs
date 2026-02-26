using System;
using System.Security.Cryptography;
using System.Text;

namespace SmartDevice.Steps;

public class Pbkdf2
{
	private string password;

	private long iterations;

	private int outputBytes;

	private byte[] saltBytes;

	private IPseudoRandomFunction prf;

	private void InitArgs(string algName, string pwd, byte[] s, long iter)
	{
		password = pwd;
		saltBytes = s;
		iterations = iter;
		byte[] bytes = Encoding.UTF8.GetBytes(password);
		switch (algName)
		{
		case "MD5":
			prf = new HMACPseudoRandomFunction<HMACMD5>(bytes);
			break;
		case "SHA1":
			prf = new HMACPseudoRandomFunction<HMACSHA1>(bytes);
			break;
		case "SHA256":
			prf = new HMACPseudoRandomFunction<HMACSHA256>(bytes);
			break;
		case "SHA384":
			prf = new HMACPseudoRandomFunction<HMACSHA384>(bytes);
			break;
		case "SHA512":
			prf = new HMACPseudoRandomFunction<HMACSHA512>(bytes);
			break;
		default:
			throw new ArgumentException($"Unsupported algorithm: {algName}");
		}
		outputBytes = ((outputBytes == 0) ? prf.HashSize : outputBytes);
	}

	public string Pbkdf2HMAC(string algorithmName, string password, byte[] salt, long iter)
	{
		string empty = string.Empty;
		InitArgs(algorithmName, password, salt, iter);
		try
		{
			using PBKDF2DeriveBytes pBKDF2DeriveBytes = new PBKDF2DeriveBytes(prf, saltBytes, iterations);
			return BitConverter.ToString(pBKDF2DeriveBytes.GetBytes(outputBytes)).Replace("-", "");
		}
		finally
		{
			prf.Dispose();
		}
	}
}
