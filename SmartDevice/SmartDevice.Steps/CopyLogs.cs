using System;
using System.Collections.Generic;
using System.IO;
using ISmart;

namespace SmartDevice.Steps;

public class CopyLogs : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		string text = Environment.CurrentDirectory;
		if (((dynamic)base.Info.Args).LogLocation != null)
		{
			text = ((dynamic)base.Info.Args).LogLocation.ToString();
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = base.Cache[key];
			}
			if (!File.GetAttributes(text).HasFlag(FileAttributes.Directory))
			{
				text = Directory.GetParent(text).FullName;
			}
		}
		if (((dynamic)base.Info.Args).LogSubDir != null)
		{
			string path = ((dynamic)base.Info.Args).LogSubDir.ToString();
			text = Path.Combine(text, path);
		}
		string commonStorageDir = Smart.File.CommonStorageDir;
		string text2 = string.Empty;
		if (((dynamic)base.Info.Args).LogName != null)
		{
			text2 = ((dynamic)base.Info.Args).LogName.ToString();
		}
		string text3 = string.Empty;
		if (((dynamic)base.Info.Args).LogExtension != null)
		{
			text3 = ((dynamic)base.Info.Args).LogExtension.ToString();
		}
		bool flag = true;
		if (((dynamic)base.Info.Args).CopyMultiple != null)
		{
			flag = ((dynamic)base.Info.Args).CopyMultiple;
		}
		bool flag2 = true;
		if (((dynamic)base.Info.Args).DeleteEmptySubDirs != null)
		{
			flag2 = ((dynamic)base.Info.Args).DeleteEmptySubDirs;
		}
		bool flag3 = false;
		if (((dynamic)base.Info.Args).LatestSubDir != null)
		{
			flag3 = ((dynamic)base.Info.Args).LatestSubDir;
		}
		bool flag4 = false;
		if (((dynamic)base.Info.Args).DeleteSource != null)
		{
			flag4 = ((dynamic)base.Info.Args).DeleteSource;
		}
		bool flag5 = true;
		if (((dynamic)base.Info.Args).DeleteTarget != null)
		{
			flag5 = ((dynamic)base.Info.Args).DeleteTarget;
		}
		if (text2 == string.Empty && text3 == string.Empty)
		{
			throw new NotSupportedException("Either log name or log extension must be specified in the recipe");
		}
		List<string> list = Smart.File.FindFiles("*.*", text, true);
		List<string> list2 = new List<string>();
		foreach (string item in list)
		{
			string fileName = Path.GetFileName(item);
			string extension = Path.GetExtension(item);
			if ((!(text2 != string.Empty) || fileName.ToLowerInvariant().Contains(text2.ToLowerInvariant())) && (!(text3 != string.Empty) || extension.ToLowerInvariant().EndsWith(text3.ToLowerInvariant())))
			{
				list2.Add(item);
			}
		}
		Smart.Log.Debug(TAG, $"Found {list2.Count} log files in {text}");
		if (flag3 && list2.Count > 1)
		{
			List<string> list3 = new List<string>();
			foreach (string item2 in list2)
			{
				string fullName = Directory.GetParent(item2).FullName;
				if (!list3.Contains(fullName))
				{
					list3.Add(fullName);
				}
			}
			string text4 = list3[0];
			DateTime dateTime = DateTime.Now.Subtract(TimeSpan.FromDays(5000.0));
			foreach (string item3 in list3)
			{
				if (Directory.Exists(item3))
				{
					DateTime lastWriteTime = File.GetLastWriteTime(item3);
					if (lastWriteTime > dateTime)
					{
						dateTime = lastWriteTime;
						text4 = item3;
					}
				}
			}
			List<string> list4 = new List<string>();
			foreach (string item4 in list2)
			{
				if (Directory.GetParent(item4).FullName == text4)
				{
					list4.Add(item4);
				}
			}
			list2 = list4;
			Smart.Log.Debug(TAG, $"Found {list2.Count} log files in latest sub-directory {text4}");
		}
		if (!flag && list2.Count > 1)
		{
			DateTime dateTime2 = DateTime.Now.Subtract(TimeSpan.FromDays(5000.0));
			string text5 = list2[0];
			foreach (string item5 in list2)
			{
				DateTime lastWriteTime2 = File.GetLastWriteTime(item5);
				if (lastWriteTime2 > dateTime2)
				{
					dateTime2 = lastWriteTime2;
					text5 = item5;
				}
			}
			string fileName2 = Path.GetFileName(text5);
			Smart.Log.Debug(TAG, $"Selecting latest log {fileName2}");
			list2.Clear();
			list2.Add(text5);
		}
		if (flag5)
		{
			List<string> list5 = Smart.File.FindFiles("*.log", commonStorageDir, false);
			List<string> list6 = new List<string>();
			foreach (string item6 in list5)
			{
				string fileName3 = Path.GetFileName(item6);
				Smart.Log.Assert(TAG, fileName3.ToLowerInvariant().EndsWith(".log"), "Log files should end in .log");
				fileName3 = fileName3.Substring(0, fileName3.Length - 4);
				string extension2 = Path.GetExtension(fileName3);
				if ((!(text2 != string.Empty) || fileName3.ToLowerInvariant().Contains(text2.ToLowerInvariant())) && (!(text3 != string.Empty) || extension2.ToLowerInvariant().EndsWith(text3.ToLowerInvariant())))
				{
					list6.Add(item6);
				}
			}
			if (list6.Count > 0)
			{
				Smart.Log.Debug(TAG, $"Deleting {list6.Count} logs from {commonStorageDir}");
				foreach (string item7 in list6)
				{
					Smart.File.Delete(item7);
				}
			}
		}
		if (list2.Count < 1)
		{
			LogResult((Result)7, "No log files to copy");
			return;
		}
		if (flag4)
		{
			Smart.Log.Debug(TAG, $"Moving {list2.Count} logs from {text}");
		}
		else
		{
			Smart.Log.Debug(TAG, $"Copying {list2.Count} logs from {text}");
		}
		foreach (string item8 in list2)
		{
			string? fileName4 = Path.GetFileName(item8);
			string text6 = Path.GetExtension(fileName4);
			string text7 = fileName4;
			if (text6.ToLowerInvariant() != ".log" || (text2 == string.Empty && text3.ToLowerInvariant().EndsWith("log")))
			{
				text7 += ".log";
				text6 += ".log";
			}
			string text8 = Smart.File.PathJoin(commonStorageDir, text7);
			if (File.Exists(text8))
			{
				Smart.Log.Debug(TAG, "Target file log file '{0)' already exists, renaming");
				int num = 2;
				string text10;
				while (true)
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(item8);
					string text9 = text6;
					fileNameWithoutExtension = fileNameWithoutExtension + "_" + num + text9;
					text10 = Smart.File.PathJoin(commonStorageDir, fileNameWithoutExtension);
					if (!File.Exists(text10))
					{
						break;
					}
					num++;
				}
				text8 = text10;
			}
			if (flag4)
			{
				File.Move(item8, text8);
				if (flag2)
				{
					string fullName2 = Directory.GetParent(item8).FullName;
					if (Directory.Exists(fullName2) && Directory.GetFiles(fullName2).Length < 1)
					{
						Directory.Delete(fullName2);
					}
				}
			}
			else
			{
				File.Copy(item8, text8);
			}
		}
		LogPass();
	}
}
