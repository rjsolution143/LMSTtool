using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ISmart;
using Microsoft.Win32;

namespace SmartUtil;

public class File : IFile
{
	private string MOTOROLA_DIR = "Motorola";

	private string commonStorageDir;

	private string TAG => GetType().FullName;

	public string StorageDirName { get; set; }

	public string LogName { get; set; }

	public string CommonStorageDir
	{
		get
		{
			if (commonStorageDir == null)
			{
				string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
				commonStorageDir = Path.Combine(folderPath, MOTOROLA_DIR);
				if (StorageDirName != null && StorageDirName != string.Empty)
				{
					commonStorageDir = Path.Combine(commonStorageDir, StorageDirName);
				}
			}
			return commonStorageDir;
		}
	}

	public Stream ReadStream(string filePath)
	{
		return new FileStream(filePath, FileMode.Open, FileAccess.Read);
	}

	public Stream WriteStream(string filePath)
	{
		return WriteStream(filePath, FileMode.Create);
	}

	public Stream WriteStream(string filePath, FileMode mode)
	{
		EnsureExists(filePath);
		return new FileStream(filePath, mode, FileAccess.Write);
	}

	public void CopyStream(Stream input, Stream output)
	{
		byte[] array = new byte[4096];
		int count;
		while ((count = input.Read(array, 0, array.Length)) > 0)
		{
			output.Write(array, 0, count);
		}
	}

	public void CopyStream(Stream input, Stream output, Func<byte[], byte[]> modifier)
	{
		byte[] array = new byte[4096];
		int num;
		while ((num = input.Read(array, 0, array.Length)) > 0)
		{
			byte[] array2 = array;
			if (modifier != null)
			{
				if (array2.Length != num)
				{
					array2 = new byte[num];
					Array.Copy(array, array2, num);
				}
				array2 = modifier(array2);
			}
			output.Write(array2, 0, num);
		}
	}

	public string ReadText(string filePath)
	{
		try
		{
			return System.IO.File.ReadAllText(filePath, Encoding.UTF8);
		}
		catch (IOException)
		{
			using Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8);
			return streamReader.ReadToEnd();
		}
	}

	public void WriteText(string filePath, string text)
	{
		EnsureExists(filePath);
		System.IO.File.WriteAllText(filePath, text, Encoding.UTF8);
	}

	public string ResourceNameToFilePath(string name, string extension)
	{
		List<string> list = new List<string>();
		list.Add(extension);
		return ResourceNameToFilePathMulti(name, list);
	}

	public string ResourceNameToFilePathMulti(string name, List<string> extensions)
	{
		string.Join(",", extensions);
		string[] array = name.Split(new char[1] { '.' });
		Smart.Log.Assert(TAG, array.Length != 0, "Name parts length should be non-zero");
		string text = Smart.File.CommonStorageDir;
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			text = Smart.File.PathJoin(text, text2);
		}
		string text3 = text;
		if (extensions != null && extensions.Count > 0)
		{
			string text4 = extensions[0];
			if (!text4.StartsWith("."))
			{
				text4 = "." + text4;
			}
			text = text3 + text4;
		}
		foreach (string extension in extensions)
		{
			string text5 = extension;
			if (text5 != string.Empty && !text5.StartsWith("."))
			{
				text5 = "." + text5;
			}
			string text6 = text3 + text5;
			if (Exists(text6))
			{
				text = text6;
				break;
			}
			FileInfo fileInfo = new FileInfo(text6);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
			DirectoryInfo directory = fileInfo.Directory;
			foreach (string item in FindFiles("*" + text5, directory.FullName, recursive: false))
			{
				FileInfo fileInfo2 = new FileInfo(item);
				if (fileInfo2.Name.ToLowerInvariant().StartsWith(fileNameWithoutExtension.ToLowerInvariant() + "_"))
				{
					text6 = fileInfo2.FullName;
					break;
				}
			}
			if (Exists(text6))
			{
				text = text6;
				break;
			}
		}
		return text;
	}

	public string Uuid()
	{
		return Guid.NewGuid().ToString("N").ToLowerInvariant();
	}

	public string FakeUid(string seed)
	{
		using MD5 mD = MD5.Create();
		byte[] b = mD.ComputeHash(Encoding.Default.GetBytes(seed));
		return new Guid(b).ToString("D").ToLowerInvariant();
	}

	public string TempFolder()
	{
		string text = string.Empty;
		do
		{
			string text2 = PathJoin(Path.GetTempPath(), Uuid());
			if (!Exists(text2))
			{
				text = text2;
			}
		}
		while (text == string.Empty);
		return text;
	}

	public string PathJoin(string path1, string path2)
	{
		return Path.Combine(path1, path2);
	}

	public bool Exists(string path)
	{
		if (!System.IO.File.Exists(path))
		{
			return Directory.Exists(path);
		}
		return true;
	}

	public long FileSize(string path)
	{
		if (!Exists(path))
		{
			return -1L;
		}
		return new FileInfo(path).Length;
	}

	public void Delete(string path)
	{
		System.IO.File.Delete(path);
		Smart.Log.Debug(TAG, $"Deleted file {path}");
	}

	public void Remove(string folderPath)
	{
		Directory.Delete(folderPath, recursive: true);
	}

	public void MirrorFiles(string sourcePath, string destinationPath)
	{
		if (!Exists(sourcePath))
		{
			throw new DirectoryNotFoundException("Could not find " + sourcePath);
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(sourcePath);
		DirectoryInfo directoryInfo2 = new DirectoryInfo(destinationPath);
		if (!directoryInfo2.Exists)
		{
			directoryInfo2.Create();
		}
		foreach (string item in FindFiles("*.*", directoryInfo.FullName, recursive: true))
		{
			FileInfo fileInfo = new FileInfo(item);
			if (!item.StartsWith(directoryInfo.FullName))
			{
				Smart.Log.Assert(TAG, false, $"File path '{item}' should start with '{directoryInfo.FullName}'");
				continue;
			}
			string text = item.Substring(directoryInfo.FullName.Length);
			Smart.Log.Verbose(TAG, "relativePath: " + text);
			string text2 = directoryInfo2.FullName + text;
			Smart.Log.Verbose(TAG, "newPath: " + text2);
			if (Exists(text2))
			{
				FileInfo fileInfo2 = new FileInfo(text2);
				if (fileInfo2.Length == fileInfo.Length && fileInfo2.LastWriteTime.Equals(fileInfo.LastWriteTime))
				{
					continue;
				}
			}
			EnsureExists(text2);
			fileInfo.CopyTo(text2, overwrite: true);
		}
	}

	public List<string> FindFiles(string searchPattern, string path, bool recursive)
	{
		List<string> list = new List<string>();
		SearchOption searchOption = SearchOption.TopDirectoryOnly;
		if (recursive)
		{
			searchOption = SearchOption.AllDirectories;
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		if (!directoryInfo.Exists)
		{
			Smart.Log.Warning(TAG, "Search folder does not exist: " + path);
			return list;
		}
		FileInfo[] files = directoryInfo.GetFiles(searchPattern, searchOption);
		foreach (FileInfo fileInfo in files)
		{
			list.Add(fileInfo.FullName);
		}
		list.Sort();
		return list;
	}

	private void EnsureExists(string filePath)
	{
		FileInfo fileInfo = new FileInfo(filePath);
		if (!fileInfo.Directory.Exists)
		{
			Directory.CreateDirectory(fileInfo.Directory.FullName);
		}
	}

	public void FileCopy(string source, string destination, bool overwrite)
	{
		System.IO.File.Copy(source, destination, overwrite);
	}

	public void ClipboardWrite(string content)
	{
		Smart.Thread.Run((ThreadStart)delegate
		{
			Clipboard.SetText(content);
		}, true);
	}

	public string ClipboardRead()
	{
		return Smart.Thread.RunAndWait<string>((Func<string>)Clipboard.GetText, true);
	}

	public string RunCommand(string command)
	{
		using Process process = Process.Start(new ProcessStartInfo("cmd.exe", "/C " + command)
		{
			CreateNoWindow = true,
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true
		});
		process.WaitForExit((int)TimeSpan.FromSeconds(5.0).TotalMilliseconds);
		string text = process.StandardOutput.ReadToEnd();
		string text2 = process.StandardError.ReadToEnd();
		if (text2.Trim() != string.Empty)
		{
			text += Environment.NewLine;
			text += text2;
		}
		return text;
	}

	public List<string> FindApps()
	{
		List<string> list = new List<string>();
		string name = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
		using RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name);
		string[] subKeyNames = registryKey.GetSubKeyNames();
		foreach (string name2 in subKeyNames)
		{
			using RegistryKey registryKey2 = registryKey.OpenSubKey(name2);
			string text = (string)registryKey2.GetValue("DisplayName");
			if (text != null && text.Trim() != string.Empty)
			{
				list.Add(text);
			}
		}
		return list;
	}
}
