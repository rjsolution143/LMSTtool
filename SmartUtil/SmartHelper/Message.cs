using System;
using System.IO.Pipes;

namespace SmartHelper;

public struct Message
{
	private static readonly byte[] EOF = new byte[5] { 3, 3, 3, 3, 3 };

	private string TAG => GetType().FullName;

	public PipeStream Stream { get; private set; }

	public byte[] Buffer { get; private set; }

	private byte[] TextBytes { get; set; }

	public string Text => Smart.Convert.BytesToAscii(TextBytes);

	public int Position { get; private set; }

	public int Length { get; private set; }

	public bool Complete { get; private set; }

	public Message(PipeStream stream)
	{
		this = default(Message);
		Stream = stream;
		Buffer = new byte[1024];
		TextBytes = new byte[0];
		Position = 0;
		Length = 0;
	}

	public Message(PipeStream stream, string text)
		: this(stream)
	{
		TextBytes = Smart.Convert.AsciiToBytes(text);
		byte[] array = new byte[TextBytes.Length + EOF.Length];
		Array.Copy(TextBytes, array, TextBytes.Length);
		Array.Copy(EOF, 0, array, TextBytes.Length, EOF.Length);
		TextBytes = array;
	}

	public void Read(int length)
	{
		if (length == 0)
		{
			Smart.Log.Debug(TAG, "Attempted to read empty Message data");
			return;
		}
		byte[] array = new byte[TextBytes.Length + length];
		Array.Copy(TextBytes, array, TextBytes.Length);
		Array.Copy(Buffer, 0, array, TextBytes.Length, length);
		TextBytes = array;
		bool flag = true;
		for (int i = 0; i < EOF.Length; i++)
		{
			byte num = EOF[i];
			byte b = TextBytes[TextBytes.Length - EOF.Length + i];
			if (num != b)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			byte[] array2 = new byte[TextBytes.Length - EOF.Length];
			Array.Copy(TextBytes, array2, array2.Length);
			TextBytes = array2;
			Complete = true;
		}
	}

	public void Write()
	{
		Length = Buffer.Length;
		int num = TextBytes.Length - Position;
		if (num < Length)
		{
			Length = num;
		}
		Array.Copy(TextBytes, Position, Buffer, 0, Length);
		Position += Length;
		if (Position == TextBytes.Length)
		{
			Complete = true;
		}
	}

	public static string HidePassword(string content)
	{
		string text = "\"pass\": \"";
		if (!content.Contains(text))
		{
			return content;
		}
		int num = content.IndexOf(text) + text.Length;
		string text2 = content.Substring(0, num);
		return string.Concat(str1: content.Substring(content.IndexOf('"', num)), str0: text2 + "********");
	}
}
