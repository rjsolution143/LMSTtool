using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ISmart;

namespace SmartDevice.Steps;

public class CopyFiles : UnisocBaseTest
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		//IL_073c: Unknown result type (might be due to invalid IL or missing references)
		//IL_072d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		Result val = (Result)8;
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		val = ((((dynamic)base.Info.Args).DeleteFilesOnly != null && (bool)((dynamic)base.Info.Args).DeleteFilesOnly) ? DeleteFiles(device) : ((((dynamic)base.Info.Args).DeleteFoldersOnly != null && (bool)((dynamic)base.Info.Args).DeleteFoldersOnly) ? DeleteFolders(device) : ((!((((dynamic)base.Info.Args).CopyFolder != null && (bool)((dynamic)base.Info.Args).CopyFolder) ? true : false)) ? CopySpecifiedFiles(device) : CopyWholeFolder(device))));
		VerifyOnly(ref val);
		LogResult(val);
	}

	private Result CopySpecifiedFiles(IDevice device)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d4b: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		Smart.Log.Verbose(TAG, $"Build source and dest file list...");
		List<string> list = ((string)((dynamic)base.Info.Args).SourceFilePathList).Split(new char[1] { ',' }).ToList();
		List<string> list2 = ((string)((dynamic)base.Info.Args).SourceFileNameList).Split(new char[1] { ',' }).ToList();
		List<string> list3 = ((string)((dynamic)base.Info.Args).DestPathList).Split(new char[1] { ',' }).ToList();
		string text = ((dynamic)base.Info.Args).DestFileNameList;
		List<string> list4 = (from s in text.Split(new char[1] { ',' })
			where !string.IsNullOrEmpty(s)
			select s).ToList();
		List<string> list5 = new List<string>();
		List<string> list6 = new List<string>();
		for (int i = 0; i < list.Count; i++)
		{
			List<string> list7 = BuildFileList(new List<string> { list[i] }, new List<string> { list2[i] }, device);
			list5.AddRange(list7);
			Smart.Log.Verbose(TAG, "Print tempFileList");
			foreach (string item in list7)
			{
				Smart.Log.Verbose(TAG, item);
			}
			if (string.IsNullOrEmpty(text.Trim()))
			{
				Smart.Log.Verbose(TAG, "not specify dest file names, use source file name to build dest file list");
				string text2 = string.Empty;
				foreach (string item2 in list7)
				{
					text2 = text2 + "#" + Path.GetFileName(item2);
				}
				text2 = text2.Trim(new char[1] { '#' });
				list7 = BuildFileList(new List<string> { list3[i] }, new List<string> { text2 }, device);
				list6.AddRange(list7);
			}
			else
			{
				Smart.Log.Verbose(TAG, "use specify dest file names to build dest file list");
				list7 = BuildFileList(new List<string> { list3[i] }, new List<string> { list4[i] }, device);
				list6.AddRange(list7);
			}
		}
		Smart.Log.Verbose(TAG, "Print sourceFileList");
		foreach (string item3 in list5)
		{
			Smart.Log.Verbose(TAG, item3);
		}
		Smart.Log.Verbose(TAG, "Print destFileList");
		foreach (string item4 in list6)
		{
			Smart.Log.Verbose(TAG, item4);
		}
		if (list5.Count != list6.Count)
		{
			Smart.Log.Error(TAG, "sourceFileList.Count != destFileList.Count");
		}
		List<string> list8 = new List<string>();
		if (((dynamic)base.Info.Args).SkipCopyIfSrcFilesNotExist != null)
		{
			list8 = ((string)((dynamic)base.Info.Args).SkipCopyIfSrcFilesNotExist).Split(new char[1] { ',' }).ToList();
		}
		Smart.Log.Verbose(TAG, "_skipCopyIfScrNotExist initial count " + list8.Count);
		if (list8.Count < list5.Count)
		{
			list8.AddRange(Enumerable.Repeat("false", list5.Count - list8.Count));
		}
		Smart.Log.Verbose(TAG, "Print _skipCopyIfScrNotExist");
		foreach (string item5 in list8)
		{
			Smart.Log.Verbose(TAG, item5);
		}
		List<string> list9 = new List<string>();
		if (((dynamic)base.Info.Args).DeleteSourceAfterCopy != null)
		{
			list9 = ((string)((dynamic)base.Info.Args).DeleteSourceAfterCopy).Split(new char[1] { ',' }).ToList();
		}
		Smart.Log.Verbose(TAG, "_deleteSourceAfterCopy initial count " + list9.Count);
		if (list9.Count < list5.Count)
		{
			list9.AddRange(Enumerable.Repeat("false", list5.Count - list9.Count));
		}
		Smart.Log.Verbose(TAG, "Print _deleteSourceAfterCopy");
		foreach (string item6 in list9)
		{
			Smart.Log.Verbose(TAG, item6);
		}
		List<string> list10 = new List<string>();
		if (((dynamic)base.Info.Args).SkipCopyIfFilesAreSame != null)
		{
			list10 = ((string)((dynamic)base.Info.Args).SkipCopyIfFilesAreSame).Split(new char[1] { ',' }).ToList();
		}
		Smart.Log.Verbose(TAG, "_skipCopyIfFilesAreSame initial count " + list10.Count);
		if (list10.Count < list5.Count)
		{
			list10.AddRange(Enumerable.Repeat("true", list5.Count - list10.Count));
		}
		Smart.Log.Verbose(TAG, "Print _skipCopyIfFilesAreSame");
		foreach (string item7 in list10)
		{
			Smart.Log.Verbose(TAG, item7);
		}
		Smart.Log.Verbose(TAG, $"Start to copy file...");
		for (int j = 0; j < list5.Count; j++)
		{
			Smart.Log.Verbose(TAG, $"Try to Copy {list5[j]} to {list6[j]}");
			if (list9[j].Equals("true", StringComparison.OrdinalIgnoreCase))
			{
				list8[j] = "true";
				Smart.Log.Verbose(TAG, $"Source file might be already delete last time, so _skipCopyIfScrNotExist must be set to true...");
			}
			if (!File.Exists(list5[j]))
			{
				if (list8[j].Equals("true", StringComparison.OrdinalIgnoreCase))
				{
					Smart.Log.Verbose(TAG, $"Src File {list5[j]} not exist, skip copy...");
					continue;
				}
				result = (Result)1;
				string text3 = $"Src File {list5[j]} not exist, can't copy...";
				Smart.Log.Error(TAG, text3);
				break;
			}
			string directoryName = Path.GetDirectoryName(list6[j]);
			Directory.CreateDirectory(directoryName);
			if (Directory.Exists(list6[j]))
			{
				list6[j] = Path.Combine(directoryName, Path.GetFileName(list5[j]));
			}
			if (File.Exists(list6[j]) && FileCompare(list5[j], list6[j]))
			{
				Smart.Log.Verbose(TAG, $"File content are same, Skip Copy {list5[j]} to {list6[j]}");
				if (list9[j].Equals("true", StringComparison.OrdinalIgnoreCase))
				{
					Smart.Log.Verbose(TAG, $"Delete {list5[j]}");
					File.Delete(list5[j]);
				}
				continue;
			}
			Smart.Log.Verbose(TAG, $"Copying {list5[j]} to {list6[j]}");
			try
			{
				File.Copy(list5[j], list6[j], overwrite: true);
				if (list9[j].Equals("true", StringComparison.OrdinalIgnoreCase))
				{
					Smart.Log.Verbose(TAG, $"Copy pass, delete {list5[j]}");
					File.Delete(list5[j]);
				}
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				Smart.Log.Error(TAG, message + Environment.NewLine + ex.StackTrace);
				bool flag = true;
				if (((dynamic)base.Info.Args).ThrowCopyException != null)
				{
					flag = (bool)((dynamic)base.Info.Args).ThrowCopyException;
				}
				if (flag)
				{
					result = (Result)1;
					LogResult(result, message, message);
				}
			}
		}
		return result;
	}

	private Result DeleteFolders(IDevice device)
	{
		List<string> sourcePaths = ((string)((dynamic)base.Info.Args).SourceFilePathList).Split(new char[1] { ',' }).ToList();
		bool flag = true;
		if (((dynamic)base.Info.Args).IgnoreFolderDeleteError != null)
		{
			flag = (bool)((dynamic)base.Info.Args).IgnoreFolderDeleteError;
		}
		List<string> list = BuildFileList(sourcePaths, null, device);
		Smart.Log.Verbose(TAG, "Print dirList");
		foreach (string item in list)
		{
			Smart.Log.Verbose(TAG, item);
		}
		foreach (string item2 in list)
		{
			Smart.Log.Verbose(TAG, "Path:" + item2);
			List<string> list2 = new List<string>();
			if (item2.Contains("*") || item2.Contains("?"))
			{
				string text = item2.Substring(0, item2.LastIndexOf("\\")).Trim(new char[1] { '\\' });
				string text2 = item2.Substring(item2.LastIndexOf("\\")).Trim(new char[1] { '\\' });
				Smart.Log.Verbose(TAG, $"Search folders under {text} with pattern {text2}.");
				list2 = Directory.GetDirectories(text, text2).ToList();
			}
			else
			{
				list2.Add(item2);
			}
			foreach (string item3 in list2)
			{
				if (Directory.Exists(item3))
				{
					try
					{
						Smart.Log.Verbose(TAG, "Delete dir " + item3);
						Directory.Delete(item3, recursive: true);
					}
					catch (Exception ex)
					{
						Smart.Log.Error(TAG, ex.Message);
						if (!flag)
						{
							throw ex;
						}
					}
				}
				else
				{
					Smart.Log.Warning(TAG, $"Folder {item3} not exist,can't delete");
				}
			}
		}
		return (Result)8;
	}

	private Result DeleteFiles(IDevice device)
	{
		List<string> sourcePaths = ((string)((dynamic)base.Info.Args).SourceFilePathList).Split(new char[1] { ',' }).ToList();
		List<string> sourceFiles = ((string)((dynamic)base.Info.Args).SourceFileNameList).Split(new char[1] { ',' }).ToList();
		bool flag = true;
		if (((dynamic)base.Info.Args).IgnoreFileDeleteError != null)
		{
			flag = (bool)((dynamic)base.Info.Args).IgnoreFileDeleteError;
		}
		List<string> list = BuildFileList(sourcePaths, sourceFiles, device);
		Smart.Log.Verbose(TAG, "Print sourceFileList");
		foreach (string item in list)
		{
			Smart.Log.Verbose(TAG, item);
		}
		foreach (string item2 in list)
		{
			if (File.Exists(item2))
			{
				try
				{
					Smart.Log.Verbose(TAG, "Delete file " + item2);
					File.Delete(item2);
				}
				catch (Exception ex)
				{
					Smart.Log.Error(TAG, ex.Message);
					if (!flag)
					{
						throw ex;
					}
				}
			}
			else
			{
				Smart.Log.Warning(TAG, $"File {item2} not exist,can't delete");
			}
		}
		return (Result)8;
	}

	private Result CopyWholeFolder(IDevice device)
	{
		Smart.Log.Verbose(TAG, $"Build source folder list...");
		List<string> sourcePaths = ((string)((dynamic)base.Info.Args).SourceFilePathList).Split(new char[1] { ',' }).ToList();
		List<string> list = BuildFileList(sourcePaths, null, device);
		Smart.Log.Verbose(TAG, "Print sourceFolderList");
		foreach (string item in list)
		{
			Smart.Log.Verbose(TAG, item);
		}
		Smart.Log.Verbose(TAG, $"Build destination folder list...");
		List<string> sourcePaths2 = ((string)((dynamic)base.Info.Args).DestPathList).Split(new char[1] { ',' }).ToList();
		List<string> list2 = BuildFileList(sourcePaths2, null, device);
		Smart.Log.Verbose(TAG, "Print destFolderList");
		foreach (string item2 in list2)
		{
			Smart.Log.Verbose(TAG, item2);
		}
		Smart.Log.Verbose(TAG, $"start copy folder ...");
		for (int i = 0; i < list.Count; i++)
		{
			string text = list[i];
			string text2 = list2[i];
			Smart.Log.Verbose(TAG, $"Copy {text} to {text2}");
			CopyDirectory(text, text2);
		}
		Smart.Log.Verbose(TAG, $"Copy folder done...");
		return (Result)8;
	}

	private void CopyDirectory(string sourceFolder, string destinationFolder)
	{
		if (!Directory.Exists(destinationFolder))
		{
			Directory.CreateDirectory(destinationFolder);
		}
		string[] files = Directory.GetFiles(sourceFolder, "*.*", SearchOption.AllDirectories);
		foreach (string text in files)
		{
			string path = text.Substring(sourceFolder.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			string text2 = Path.Combine(destinationFolder, path);
			try
			{
				Smart.Log.Verbose(TAG, $"Copy {text} to {text2}");
				string directoryName = Path.GetDirectoryName(text2);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				File.Copy(text, text2, overwrite: true);
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, ex.Message);
			}
		}
	}

	private bool FileCompare(string file1, string file2)
	{
		string name = MethodBase.GetCurrentMethod().Name;
		Smart.Log.Verbose(name, "enter...");
		FileStream fileStream = new FileStream(file1, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		FileStream fileStream2 = new FileStream(file2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		if (fileStream.Length != fileStream2.Length)
		{
			fileStream.Close();
			fileStream2.Close();
			return false;
		}
		int num;
		int num2;
		do
		{
			num = fileStream.ReadByte();
			num2 = fileStream2.ReadByte();
		}
		while (num == num2 && num != -1);
		fileStream.Close();
		fileStream2.Close();
		Smart.Log.Verbose(name, "Exit...");
		return num - num2 == 0;
	}
}
