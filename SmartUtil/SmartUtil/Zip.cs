using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ISmart;
using Ionic.Zip;
using Ionic.Zlib;

namespace SmartUtil;

public class Zip : IZip
{
	private string TAG => GetType().FullName;

	public void Compress(string zipFile, List<string> filesToCompress)
	{
		Compress(zipFile, filesToCompress, flat: false);
	}

	public void Compress(string zipFile, List<string> filesToCompress, bool flat)
	{
		Compress(zipFile, filesToCompress, flat: false, null);
	}

	public void Compress(string zipFile, List<string> filesToCompress, bool flat, string password)
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		if (filesToCompress.Count < 1)
		{
			throw new NotSupportedException("No files to compress");
		}
		string text = null;
		foreach (string item in filesToCompress)
		{
			DirectoryInfo directoryInfo = new FileInfo(item).Directory;
			if (text == null)
			{
				text = directoryInfo.FullName;
				continue;
			}
			while (directoryInfo != null)
			{
				if (text.StartsWith(directoryInfo.FullName))
				{
					text = directoryInfo.FullName;
					break;
				}
				directoryInfo = directoryInfo.Parent;
			}
			if (directoryInfo != null)
			{
				continue;
			}
			text = string.Empty;
			break;
		}
		if (text.Length > 0)
		{
			string text2 = text;
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			if (!text2.EndsWith(directorySeparatorChar.ToString()))
			{
				string text3 = text;
				directorySeparatorChar = Path.DirectorySeparatorChar;
				text = text3 + directorySeparatorChar;
			}
		}
		ZipFile val = new ZipFile(zipFile);
		try
		{
			if (password != null)
			{
				val.Password = password;
			}
			foreach (string item2 in filesToCompress)
			{
				FileInfo fileInfo = new FileInfo(item2);
				string fullName = fileInfo.FullName;
				string text4 = "";
				if (fullName.StartsWith(text) && fullName.Length > text.Length)
				{
					text4 = fullName.Substring(text.Length, fullName.Length - text.Length - fileInfo.Name.Length);
					fullName = fullName.Substring(text.Length);
				}
				else
				{
					Smart.Log.Warning(TAG, $"File path '{fullName}' does not match common path '{text}'");
				}
				if (flat)
				{
					val.AddFile(fileInfo.FullName, "");
				}
				else
				{
					val.AddFile(fileInfo.FullName, text4);
				}
			}
			val.Save();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void GZipExtract(string gzipFile, string destinationPath)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		string text = "././@LongLink";
		int num = 512;
		byte[] array = new byte[num];
		int num2 = 100;
		byte[] array2 = new byte[num2];
		int num3 = 12;
		byte[] array3 = new byte[num3];
		int srcOffset = 124;
		Smart.Log.Debug(TAG, "GZip unzipping file " + gzipFile);
		using Stream stream = Smart.File.ReadStream(gzipFile);
		GZipStream val = new GZipStream(stream, (CompressionMode)1);
		try
		{
			string text2 = null;
			while (((Stream)(object)val).Read(array, 0, num) >= 1)
			{
				Buffer.BlockCopy(array, 0, array2, 0, num2);
				string text3 = Encoding.UTF8.GetString(array2);
				if (text2 != null)
				{
					text3 = text2;
					text2 = null;
				}
				text3 = text3.TrimEnd(new char[1]);
				if (text3 == string.Empty)
				{
					continue;
				}
				bool flag = false;
				if (text3 == text)
				{
					flag = true;
				}
				string text4 = Smart.File.PathJoin(destinationPath, text3);
				Buffer.BlockCopy(array, srcOffset, array3, 0, num3);
				ulong num4 = System.Convert.ToUInt64(Encoding.UTF8.GetString(array3).TrimEnd(new char[1]), 8);
				if (num4 < 1 && (text3.EndsWith("/") || text3.EndsWith("\\")))
				{
					Directory.CreateDirectory(text4);
					continue;
				}
				int num5 = (int)(num4 % (ulong)num);
				if (num5 > 0)
				{
					num4 += (ulong)(num - num5);
				}
				int num6 = (int)(num4 / (ulong)num);
				byte[] buffer = new byte[num];
				if (!flag)
				{
					using (Stream stream2 = Smart.File.WriteStream(text4))
					{
						for (int i = 0; i < num6; i++)
						{
							int count = num;
							if (i == num6 - 1 && num5 > 0)
							{
								count = num5;
							}
							((Stream)(object)val).Read(buffer, 0, num);
							stream2.Write(buffer, 0, count);
						}
					}
					continue;
				}
				using Stream stream3 = new MemoryStream();
				for (int j = 0; j < num6; j++)
				{
					int count2 = num;
					if (j == num6 - 1 && num5 > 0)
					{
						count2 = num5;
					}
					((Stream)(object)val).Read(buffer, 0, num);
					stream3.Write(buffer, 0, count2);
				}
				stream3.Seek(0L, SeekOrigin.Begin);
				using StreamReader streamReader = new StreamReader(stream3);
				text2 = streamReader.ReadToEnd();
				text2 = text2.TrimEnd(new char[1]);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void Extract(string zipFile, string destinationPath)
	{
		Extract(zipFile, destinationPath, string.Empty);
	}

	public void Extract(string zipFile, string destinationPath, string password)
	{
		ZipFile val = ZipFile.Read(zipFile);
		try
		{
			foreach (ZipEntry item in val)
			{
				Smart.Log.Debug(TAG, "Extracting file " + item.FileName);
				if (password != null && password != string.Empty)
				{
					item.Password = password;
				}
				item.Extract(destinationPath, (ExtractExistingFileAction)1);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public ITempZip ExtractTemp(string zipFile, string password)
	{
		return (ITempZip)(object)new TempZip(zipFile, password);
	}

	public ITempZip ExtractTemp(string zipFile)
	{
		return ExtractTemp(zipFile, string.Empty);
	}

	public void GZipDecompress(string gzFile, string destinationPath)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		string text = Smart.File.PathJoin(destinationPath, Path.GetFileNameWithoutExtension(gzFile));
		Smart.Log.Debug(TAG, "Decompressing file " + text);
		using Stream stream = Smart.File.ReadStream(gzFile);
		GZipStream val = new GZipStream(stream, (CompressionMode)1);
		try
		{
			using Stream stream2 = Smart.File.WriteStream(text);
			Smart.File.CopyStream((Stream)(object)val, stream2);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public byte[] Compress(byte[] bytes)
	{
		return ZlibStream.CompressBuffer(bytes);
	}

	public byte[] Extract(byte[] bytes)
	{
		return ZlibStream.UncompressBuffer(bytes);
	}
}
