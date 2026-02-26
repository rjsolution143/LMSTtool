using System;

namespace AuthenticodeExaminer;

internal static class HexHelpers
{
	private static ReadOnlySpan<byte> LookupTable => "0123456789ABCDEF"u8;

	public static bool TryHexEncodeBigEndian(ReadOnlySpan<byte> data, Span<char> buffer)
	{
		if (data.Length == 0)
		{
			return true;
		}
		int num = data.Length * 2;
		if (buffer.Length < num)
		{
			return false;
		}
		int num2 = 0;
		int num3 = data.Length * 2 - 2;
		while (num2 < data.Length)
		{
			byte b = data[num2];
			buffer[num3] = (char)LookupTable[(b & 0xF0) >> 4];
			buffer[num3 + 1] = (char)LookupTable[b & 0xF];
			num2++;
			num3 -= 2;
		}
		return true;
	}

	public static string HexEncodeBigEndian(ReadOnlySpan<byte> data)
	{
		if (data.Length == 0)
		{
			return string.Empty;
		}
		int num = data.Length * 2;
		Span<char> span = ((num >= 256) ? ((Span<char>)new char[num]) : stackalloc char[256]);
		Span<char> buffer = span;
		if (!TryHexEncodeBigEndian(data, buffer))
		{
			throw new InvalidOperationException("Incorrectly sized buffer.");
		}
		return buffer.Slice(0, num).ToString();
	}
}
