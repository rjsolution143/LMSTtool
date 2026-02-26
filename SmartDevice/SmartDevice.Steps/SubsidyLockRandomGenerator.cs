using System;
using System.Diagnostics;
using System.Security.Cryptography;

namespace SmartDevice.Steps;

public class SubsidyLockRandomGenerator
{
	private static readonly object _randomcreatelock = new object();

	private static readonly object _randomexecutedbslock = new object();

	private static readonly RandomNumberGenerator _Random = new RNGCryptoServiceProvider();

	private static SubsidyLockRandomGenerator randomgenerator = null;

	public static SubsidyLockRandomGenerator CreateSubsidyLockRandomGenerator()
	{
		if (randomgenerator == null)
		{
			lock (_randomcreatelock)
			{
				if (randomgenerator == null)
				{
					randomgenerator = new SubsidyLockRandomGenerator();
				}
			}
		}
		return randomgenerator;
	}

	public string NextKey(int keyLength, out string error)
	{
		error = string.Empty;
		string empty = string.Empty;
		try
		{
			lock (_randomcreatelock)
			{
				byte[] array = new byte[8];
				ulong num = (ulong)Math.Pow(10.0, keyLength);
				ulong num2 = ulong.MaxValue - ulong.MaxValue % num;
				ulong num3 = 0uL;
				if (keyLength > 16 || keyLength < 8)
				{
					error = "unsupported key length";
					return string.Empty;
				}
				Stopwatch stopwatch = new Stopwatch();
				do
				{
					_Random.GetBytes(array);
					num3 = BitConverter.ToUInt64(array, 0);
					if (stopwatch.ElapsedMilliseconds >= 60000)
					{
						error = "TIMEOUT_IN_KEY_GENERATION";
						return string.Empty;
					}
				}
				while (num3 >= num2);
				return (num3 % num).ToString("D" + keyLength);
			}
		}
		catch (Exception ex)
		{
			empty = string.Empty;
			error = ex.Message;
			return empty;
		}
	}
}
