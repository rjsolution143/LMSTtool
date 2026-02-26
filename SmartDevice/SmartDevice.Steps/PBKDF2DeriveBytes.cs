using System;
using System.Security.Cryptography;

namespace SmartDevice.Steps;

public class PBKDF2DeriveBytes : DeriveBytes, IDisposable
{
	private readonly IPseudoRandomFunction prf;

	private readonly byte[] salt;

	private readonly long iterationCount;

	private readonly byte[] saltAndBlockNumber;

	private byte[] buffer;

	private int bufferIndex;

	private int nextBlock;

	public PBKDF2DeriveBytes(IPseudoRandomFunction prf, byte[] salt, long iterationCount)
	{
		if (prf == null)
		{
			throw new ArgumentNullException("prf");
		}
		if (salt == null)
		{
			throw new ArgumentNullException("salt");
		}
		this.prf = prf;
		this.salt = salt;
		this.iterationCount = iterationCount;
		saltAndBlockNumber = new byte[salt.Length + 4];
		Buffer.BlockCopy(salt, 0, saltAndBlockNumber, 0, salt.Length);
		Reset();
	}

	public override byte[] GetBytes(int keyLength)
	{
		byte[] array = new byte[keyLength];
		int num = 0;
		if (buffer != null && bufferIndex > 0)
		{
			int num2 = Math.Min(prf.HashSize - bufferIndex, keyLength);
			if (num2 > 0)
			{
				Buffer.BlockCopy(buffer, bufferIndex, array, 0, num2);
				bufferIndex += num2;
				num += num2;
			}
		}
		if (num < keyLength)
		{
			ComputeBlocks(array, num);
			if (bufferIndex == prf.HashSize)
			{
				bufferIndex = 0;
			}
		}
		return array;
	}

	public sealed override void Reset()
	{
		buffer = null;
		bufferIndex = 0;
		nextBlock = 1;
	}

	private void ComputeBlocks(byte[] result, int resultIndex)
	{
		int num = nextBlock;
		while (resultIndex < result.Length)
		{
			F(num);
			int num2 = Math.Min(prf.HashSize, result.Length - resultIndex);
			Buffer.BlockCopy(buffer, 0, result, resultIndex, num2);
			bufferIndex = num2;
			resultIndex += num2;
			num++;
		}
		nextBlock = num;
	}

	private void F(int currentBlock)
	{
		Buffer.BlockCopy(BlockNumberToBytes(currentBlock), 0, saltAndBlockNumber, salt.Length, 4);
		buffer = prf.Transform(saltAndBlockNumber);
		byte[] array = buffer;
		for (long num = 2L; num <= iterationCount; num++)
		{
			array = prf.Transform(array);
			for (int i = 0; i < buffer.Length; i++)
			{
				buffer[i] ^= array[i];
			}
		}
	}

	private static byte[] BlockNumberToBytes(int blockNumber)
	{
		byte[] bytes = BitConverter.GetBytes(blockNumber);
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse((Array)bytes);
		}
		return bytes;
	}

	public new virtual void Dispose()
	{
	}
}
