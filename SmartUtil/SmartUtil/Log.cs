using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ISmart;

namespace SmartUtil;

public class Log : ILog, IDisposable
{
	private string logPath = string.Empty;

	private string logDebugPath = string.Empty;

	private string errorLines = string.Empty;

	private string secretCode = "ZGVCdWc6MjBAKQ==";

	private System.Threading.Thread logThread;

	private List<string> debugQueue = new List<string>();

	private List<string> logQueue = new List<string>();

	private object logLock = new object();

	private static string[] hiddenCodeList = new string[16]
	{
		"Unlock Key", "UnlockCode", "SIM lock", "Simlock", "Unlock Code", "SubLockCode", "Set lock", "NWSCP", "UNLOCK_CODE", "Output szData",
		"hck:", "hck1:", "salt:", "sck:", "UnlockKey", "usKeySet"
	};

	protected List<string> HiddenCodes = new List<string>(hiddenCodeList);

	private bool disposedValue;

	private string TAG => GetType().FullName;

	public object ReportLock { get; private set; } = new object();


	public bool HideInfo { get; private set; }

	public Log()
	{
		HideInfo = true;
		string commonStorageDir = Smart.File.CommonStorageDir;
		if (!Directory.Exists(commonStorageDir))
		{
			try
			{
				Directory.CreateDirectory(commonStorageDir);
			}
			catch (Exception)
			{
			}
		}
		if (System.IO.File.Exists(Smart.File.PathJoin(commonStorageDir, "debug.mode")))
		{
			HideInfo = false;
		}
		string text = Smart.File.LogName;
		if (text == null || text.Trim() == string.Empty)
		{
			text = "templog";
		}
		logPath = Smart.File.PathJoin(commonStorageDir, text + ".log");
		if (System.IO.File.Exists(logPath))
		{
			System.IO.File.Delete(logPath);
		}
		logDebugPath = Smart.File.PathJoin(commonStorageDir, text + "-secure.log");
		if (System.IO.File.Exists(logDebugPath) && !Smart.App.CrashRecovery)
		{
			System.IO.File.Delete(logDebugPath);
		}
		logThread = Smart.Thread.RunThread((ThreadStart)LogLoop);
		Smart.App.ScheduleShutdownTask("ZZZ Close debug log", (Action)Dispose);
	}

	public void Assert(string tag, bool value, string message)
	{
		if (!value)
		{
			((ILog)this).Log((LogLevel)1, tag, message);
		}
	}

	public void Critical(string tag, string message)
	{
		((ILog)this).Log((LogLevel)(-1), tag, message);
	}

	public void Debug(string tag, string message)
	{
		((ILog)this).Log((LogLevel)3, tag, message);
	}

	public void Error(string tag, string message)
	{
		((ILog)this).Log((LogLevel)0, tag, message);
	}

	public void Info(string tag, string message)
	{
		((ILog)this).Log((LogLevel)2, tag, message);
	}

	public void Verbose(string tag, string message)
	{
		((ILog)this).Log((LogLevel)4, tag, message);
	}

	public void Warning(string tag, string message)
	{
		((ILog)this).Log((LogLevel)1, tag, message);
	}

	void ILog.Log(LogLevel level, string tag, string message)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Invalid comparison between Unknown and I4
		string text = DateTime.Now.ToString("yyyyMMdd:HHmmss:ffff");
		string text2 = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
		string format = "{0}~{1}-{2}[{3}]** {4} **";
		if (HideInfo)
		{
			message = CheckMessage(tag, message);
		}
		string[] args = new string[5]
		{
			text,
			text2,
			tag,
			((object)(LogLevel)(ref level)).ToString(),
			message
		};
		string item = string.Format(format, args);
		lock (logLock)
		{
			debugQueue.Add(item);
			if ((int)level < 3)
			{
				logQueue.Add(item);
			}
		}
	}

	private string CheckMessage(string tag, string message)
	{
		if (!tag.ToLowerInvariant().Contains("shell") && !tag.ToLowerInvariant().Contains("smartweb") && !tag.ToLowerInvariant().Contains("subsidy") && !tag.ToLowerInvariant().Contains("programming"))
		{
			return message;
		}
		string text = message.ToLowerInvariant();
		foreach (string hiddenCode in HiddenCodes)
		{
			if (text.Contains(hiddenCode.ToLowerInvariant()))
			{
				return Regex.Replace(message, "[\\d]", "*");
			}
		}
		return message;
	}

	private void LogLoop()
	{
		while (true)
		{
			ProcessLog();
			System.Threading.Thread.Sleep(5000);
		}
	}

	private void ProcessLog()
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		lock (logLock)
		{
			if (logQueue.Count < 1 && debugQueue.Count < 1)
			{
				return;
			}
			list.AddRange(logQueue);
			logQueue.Clear();
			list2.AddRange(debugQueue);
			debugQueue.Clear();
		}
		List<string> list3 = new List<string>();
		foreach (string item2 in list2)
		{
			string item = Smart.Security.EncryptString(item2);
			list3.Add(item);
		}
		list2 = list3;
		try
		{
			if (list.Count > 0 || list2.Count > 0)
			{
				System.IO.File.AppendAllLines(logPath, list, Encoding.UTF8);
				System.IO.File.AppendAllLines(logDebugPath, list2, Encoding.UTF8);
			}
		}
		catch (IOException)
		{
		}
	}

	public string GenerateErrorReport(string folder, string tag)
	{
		Smart.Log.Debug(TAG, "Waiting to generate error report...");
		Smart.Thread.Wait(TimeSpan.FromSeconds(5.0));
		lock (ReportLock)
		{
			Smart.Log.Debug(TAG, "Generating error report");
			string result = GenerateErrorReportPrivate(folder, tag);
			Smart.Log.Debug(TAG, "Generated error report");
			return result;
		}
	}

	private string GenerateErrorReportPrivate(string folder, string tag)
	{
		ProcessLog();
		DirectoryInfo directoryInfo = new DirectoryInfo(Smart.File.CommonStorageDir);
		List<string> list = new List<string>();
		Image obj = Smart.Graphics.ScreenShot();
		string text = Smart.File.PathJoin(directoryInfo.FullName, "screenshot.png");
		list.Add(text);
		obj.Save(text);
		List<string> list2 = new List<string>();
		try
		{
			List<Tuple<string, string, int>> list3 = new List<Tuple<string, string, int>>();
			string item = Smart.Rsd.GetDefaultCitPrintFilePath();
			IThreadLocked val = Smart.Rsd.LocalOptions();
			try
			{
				dynamic data = val.Data;
				string text2 = data.SaveCitPath;
				if (text2 != null && text2 != string.Empty)
				{
					item = text2;
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			list3.Add(new Tuple<string, string, int>(item, "*.pdf", 1));
			foreach (Tuple<string, string, int> item5 in list3)
			{
				string item2 = item5.Item1;
				string item3 = item5.Item2;
				int item4 = item5.Item3;
				TimeSpan timeSpan = TimeSpan.FromMinutes(2.0);
				DirectoryInfo directoryInfo2 = new DirectoryInfo(item2);
				if (!directoryInfo2.Exists)
				{
					continue;
				}
				FileInfo[] files = directoryInfo2.GetFiles(item3, SearchOption.TopDirectoryOnly);
				if (files == null || files.Length < 1)
				{
					continue;
				}
				List<FileInfo> list4 = new List<FileInfo>(files);
				list4.Sort((FileInfo leftFile, FileInfo rightFile) => leftFile.LastWriteTimeUtc.CompareTo(rightFile.LastWriteTimeUtc));
				list4.Reverse();
				if (list4.Count > item4)
				{
					list4 = list4.GetRange(0, item4);
				}
				foreach (FileInfo item6 in list4)
				{
					DateTime lastWriteTimeUtc = item6.LastWriteTimeUtc;
					if (!(lastWriteTimeUtc < DateTime.UtcNow) || !(DateTime.UtcNow.Subtract(lastWriteTimeUtc).TotalSeconds > timeSpan.TotalSeconds))
					{
						list2.Add(item6.FullName);
					}
				}
			}
		}
		catch (Exception)
		{
		}
		FileInfo[] files2 = directoryInfo.GetFiles("*.log", SearchOption.TopDirectoryOnly);
		foreach (FileInfo fileInfo in files2)
		{
			if (!fileInfo.Name.ToLowerInvariant().Contains("sd-encrypted") && !list.Contains(fileInfo.FullName))
			{
				list.Add(fileInfo.FullName);
			}
		}
		List<string> list5 = new List<string>();
		string text3 = "UNKNOWN";
		lock (logLock)
		{
			string text4 = logDebugPath.Replace("-secure.log", "-debug.log");
			string text5 = text4 + ".zip";
			string text6 = string.Empty;
			string text7 = string.Empty;
			if (list.Contains(logDebugPath))
			{
				if (System.IO.File.Exists(text4))
				{
					System.IO.File.Delete(text4);
				}
				using (FileStream stream = System.IO.File.OpenWrite(text4))
				{
					using TextWriter textWriter = new StreamWriter(stream);
					string text8 = string.Empty;
					foreach (string item7 in System.IO.File.ReadLines(logDebugPath))
					{
						string text9 = Smart.Security.DecryptString(item7);
						textWriter.WriteLine(text9);
						if (text9.ToLowerInvariant().Contains("[verbose]**"))
						{
							continue;
						}
						Match match = Regex.Match(text9, "\\*\\* Running (?<name>.*) use case \\*\\*");
						if (match.Success)
						{
							text7 = match.Groups["name"].Value;
							text6 = text6 + text9 + Environment.NewLine;
						}
						if (text7 != string.Empty)
						{
							if (text9.Contains("** Running step "))
							{
								text8 = string.Empty;
								text8 = text8 + text9 + Environment.NewLine;
							}
							else if (text9.Contains("result Failed **") || text9.Contains("result AuditFailed **") || text9.Contains("result Aborted **"))
							{
								text8 = text8 + text9 + Environment.NewLine;
								text6 += text8;
								text8 = string.Empty;
							}
							else
							{
								text8 = text8 + text9 + Environment.NewLine;
							}
						}
						if (text7 != string.Empty)
						{
							match = Regex.Match(text9, "\\*\\* (?<name>.*) result (?<result>.*) \\*\\*");
							if (match.Success && match.Groups["name"].Value == text7)
							{
								text6 = text6 + text9 + Environment.NewLine;
								text7 = string.Empty;
							}
						}
					}
				}
				list.Remove(logDebugPath);
				if (System.IO.File.Exists(text5))
				{
					System.IO.File.Delete(text5);
				}
				byte[] bytes = System.Convert.FromBase64String(secretCode);
				string @string = Encoding.UTF8.GetString(bytes);
				List<string> list6 = new List<string>();
				list6.Add(text4);
				Smart.Zip.Compress(text5, list6, true, @string);
				if (System.IO.File.Exists(text4))
				{
					System.IO.File.Delete(text4);
				}
				if (!list.Contains(text5))
				{
					list.Add(text5);
				}
				if (list.Contains(text4))
				{
					list.Remove(text4);
				}
				if (text6.Length < 1)
				{
					text6 = "No errors";
				}
				string text10 = logDebugPath.Replace("-secure.log", "-errorlines.log");
				System.IO.File.WriteAllText(text10, text6);
				if (!list.Contains(text10))
				{
					list.Add(text10);
				}
			}
			try
			{
				foreach (string item8 in list2)
				{
					FileInfo fileInfo2 = new FileInfo(item8);
					if (fileInfo2.Exists)
					{
						string name = fileInfo2.Name;
						string text11 = Smart.File.PathJoin(directoryInfo.FullName, name);
						fileInfo2.CopyTo(text11, overwrite: true);
						list5.Add(text11);
						list.Add(text11);
					}
				}
			}
			catch (Exception)
			{
			}
			string text12 = tag + "_ErrorReport_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".zip";
			text3 = Smart.File.PathJoin(folder, text12);
			Smart.Zip.Compress(text3, list, true);
			list5.Add(text5);
		}
		foreach (string item9 in list)
		{
			try
			{
				if (System.IO.File.Exists(item9))
				{
					DateTime lastWriteTime = Directory.GetLastWriteTime(item9);
					if (DateTime.Now.Subtract(lastWriteTime).TotalHours > 2.0 || Regex.IsMatch(Path.GetFileName(item9), "bugreport.*.log", RegexOptions.IgnoreCase) || Regex.IsMatch(Path.GetFileName(item9), "logcat.*.log", RegexOptions.IgnoreCase) || Regex.IsMatch(Path.GetFileName(item9), "img.*.log", RegexOptions.IgnoreCase))
					{
						list5.Add(item9);
					}
				}
			}
			catch (Exception)
			{
			}
		}
		foreach (string item10 in list5)
		{
			try
			{
				if (System.IO.File.Exists(item10))
				{
					System.IO.File.Delete(item10);
				}
			}
			catch (Exception)
			{
			}
		}
		return text3;
	}

	public void Dispose()
	{
		if (!disposedValue)
		{
			if (logThread != null && logThread.IsAlive)
			{
				logThread.Abort();
			}
			disposedValue = true;
		}
	}
}
