using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class UnisocBaseTest : BaseStep
{
	public Process mProcess;

	public List<string> output = new List<string>();

	public List<string> error = new List<string>();

	public Result ActualTestResult = (Result)1;

	public string ComportCfgFile = Path.Combine(Smart.File.CommonStorageDir, "ComportAssignment.txt");

	public static readonly object obj = new object();

	private string TAG => GetType().FullName;

	public override void Run()
	{
	}

	public int ExecuteShell(string exe, string command, int timeoutSecToWaitShellExit)
	{
		output.Clear();
		error.Clear();
		TimeSpan timeSpan = TimeSpan.FromSeconds(timeoutSecToWaitShellExit);
		string currentDirectory = Environment.CurrentDirectory;
		string directoryName = Path.GetDirectoryName(exe);
		exe = Smart.Convert.QuoteFilePathName(exe);
		mProcess = new Process();
		mProcess.StartInfo.FileName = exe;
		mProcess.StartInfo.WorkingDirectory = directoryName;
		mProcess.StartInfo.Arguments = command;
		mProcess.StartInfo.RedirectStandardInput = true;
		mProcess.StartInfo.RedirectStandardOutput = true;
		mProcess.StartInfo.RedirectStandardError = true;
		mProcess.StartInfo.UseShellExecute = false;
		mProcess.EnableRaisingEvents = true;
		bool createNoWindow = true;
		if (((dynamic)base.Info.Args).HideWindow != null)
		{
			createNoWindow = (bool)((dynamic)base.Info.Args).HideWindow;
		}
		mProcess.StartInfo.CreateNoWindow = createNoWindow;
		mProcess.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
		{
			Redirected(output, sender, e);
		};
		mProcess.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e)
		{
			Redirected(error, sender, e);
		};
		Smart.Log.Verbose(TAG, $"Starting shell command: {exe} {command}");
		mProcess.Start();
		mProcess.BeginOutputReadLine();
		mProcess.BeginErrorReadLine();
		if (!mProcess.WaitForExit((int)timeSpan.TotalMilliseconds))
		{
			Smart.Log.Debug(TAG, "Process not exit in specified time. Force kill it...");
			try
			{
				mProcess.Kill();
			}
			catch (Exception ex)
			{
				Smart.Log.Debug(TAG, "Fail to shut down the shell command process - errorMsg " + ex.Message);
			}
		}
		if (mProcess.ExitCode != 0)
		{
			Smart.Log.Error(TAG, $"Error, Shell process exited with code {mProcess.ExitCode}");
		}
		Directory.SetCurrentDirectory(currentDirectory);
		Smart.Log.Verbose(TAG, $"Finished shell command: {exe} {command} with ExitCode = {mProcess.ExitCode} ");
		return mProcess.ExitCode;
	}

	public virtual void Redirected(List<string> dataList, object sender, DataReceivedEventArgs e)
	{
		_ = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (e.Data != null)
		{
			Smart.Log.Debug(TAG, "Base Redirected Shell Resp: " + e.Data);
			string item = e.Data.Trim();
			dataList.Add(item);
		}
	}

	public void SavePortToLocalFile(string pidvid, string comportNum, string comportCfgFile)
	{
		Smart.Log.Info(MethodBase.GetCurrentMethod().Name, "enter...");
		try
		{
			lock (obj)
			{
				if (!File.Exists(comportCfgFile))
				{
					File.Create(comportCfgFile).Close();
				}
				Dictionary<string, string> dictionary = (from line in File.ReadAllLines(comportCfgFile).ToList()
					select line.Split(new char[1] { ',' })).ToDictionary((string[] splits) => splits[0], (string[] splits) => splits[1]);
				if (dictionary.ContainsKey(pidvid))
				{
					dictionary[pidvid] = comportNum;
				}
				else
				{
					dictionary.Add(pidvid, comportNum);
				}
				File.WriteAllLines(comportCfgFile, dictionary.Select((KeyValuePair<string, string> x) => x.Key + "," + x.Value));
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, ex.Message + ex.StackTrace);
			throw ex;
		}
	}

	protected List<string> BuildFileList(List<string> _sourcePaths, List<string> _sourceFiles, IDevice device)
	{
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Expected O, but got Unknown
		Smart.Log.Verbose(TAG, $"Start to build file list...");
		List<string> list = new List<string>();
		for (int i = 0; i < _sourcePaths.Count; i++)
		{
			string text = _sourcePaths[i];
			Smart.Log.Verbose(TAG, string.Format("Configured path:" + text));
			if (text.Contains("$"))
			{
				string[] array = text.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
				string text2 = string.Empty;
				string[] array2 = array;
				foreach (string text3 in array2)
				{
					string text4 = text3;
					if (text3.Contains("$"))
					{
						text4 = base.Cache[text3.Substring(1)];
					}
					if (File.Exists(text4))
					{
						text4 = Path.GetDirectoryName(text4);
					}
					text2 = text2 + "\\" + text4;
				}
				text = text2.Trim(new char[1] { '\\' });
			}
			if (text.Contains("LMSTCommonStorageDir"))
			{
				text = text.Replace("LMSTCommonStorageDir", Smart.File.CommonStorageDir);
			}
			else if (text.Contains("LMSTInstallationDir"))
			{
				text = text.Replace("LMSTInstallationDir", Application.StartupPath);
			}
			if (text.Contains("*") || text.Contains("?"))
			{
				int num = text.LastIndexOf(Path.DirectorySeparatorChar);
				string text5 = text.Substring(num + 1);
				string text6 = text.Substring(0, num + 1);
				if (text6.Contains("*") || text6.Contains("?"))
				{
					string text7 = "Only support last dir name use search pattern";
					Smart.Log.Error(TAG, text7);
					throw new Exception(text7);
				}
				if (text5.Contains("*") || text5.Contains("?"))
				{
					if (!Directory.Exists(text6))
					{
						string text8 = "Parent directory not exist:" + text6;
						Smart.Log.Error(TAG, text8);
						throw new Exception(text8);
					}
					string[] directories = Directory.GetDirectories(text6, text5, SearchOption.TopDirectoryOnly);
					Smart.Log.Error(TAG, $"Found {directories.Length} dir with search pattern {text5}");
					if (directories.Length > 1)
					{
						string text9 = $"Found multiple dirs {directories.Length} with search pattern {text5} under {text6}";
						Smart.Log.Error(TAG, text9);
						if (_sourceFiles == null)
						{
							return directories.ToList();
						}
						Smart.Log.Info(TAG, "Only pick up first found folder");
						text = directories[0];
					}
					else
					{
						if (directories.Length < 1)
						{
							string text10 = $"Not found dir with search pattern {text5}";
							Smart.Log.Error(TAG, text10);
							if (_sourceFiles == null)
							{
								return new List<string>();
							}
							throw new Exception(text10);
						}
						text = directories[0];
					}
				}
				Smart.Log.Verbose(TAG, string.Format("Temp3 Path to be used:" + text));
			}
			if (text.Contains("ccWriteExe"))
			{
				int num2 = text.LastIndexOf(Path.DirectorySeparatorChar);
				string text11 = text.Substring(num2 + 1);
				text.Substring(0, num2 + 1);
				Smart.Log.Verbose(TAG, string.Format("To get the actual folder name from matrix for " + text11));
				string empty = string.Empty;
				string filePathName = Smart.Rsd.GetFilePathName(text11, base.Recipe.Info.UseCase, device, new Progress(base.ProgressUpdate), ref empty);
				Smart.Log.Verbose(TAG, string.Format("The actual folder name get from matrix is " + filePathName));
				text = Path.GetDirectoryName(filePathName);
				Smart.Log.Verbose(TAG, string.Format("The final folder name to be used is " + text));
			}
			if (File.Exists(text))
			{
				text = Path.GetDirectoryName(text);
			}
			Smart.Log.Verbose(TAG, string.Format("Final path to be used:" + text));
			List<string> list2 = new List<string>();
			if (_sourceFiles != null && _sourceFiles.Count > 0)
			{
				if (_sourceFiles[i].Contains("#"))
				{
					list2.AddRange(_sourceFiles[i].Split(new char[1] { '#' }).ToList());
				}
				else
				{
					list2.Add(_sourceFiles[i]);
				}
			}
			Smart.Log.Verbose(TAG, "Initial fileNames");
			foreach (string item in list2)
			{
				Smart.Log.Verbose(TAG, item);
			}
			if (list2.Count > 0)
			{
				foreach (string item2 in list2)
				{
					string text12 = item2;
					if (text12.StartsWith("$"))
					{
						text12 = base.Cache[text12.Substring(1)];
					}
					else
					{
						if (text12.Contains("{$sku}") || text12.Contains("{sku}"))
						{
							string logInfoValue = device.GetLogInfoValue(DetectionKey.NameToDisplayStringLookup["sku"]);
							text12 = text12.Replace("{$sku}", logInfoValue).Replace("{sku}", logInfoValue);
						}
						if (text12.Contains("{trackid}"))
						{
							string iD = device.ID;
							text12 = text12.Replace("{trackid}", iD);
						}
						if (text12.Contains("{datetime}"))
						{
							string newValue = DateTime.Now.ToString("yyyyMMddHHmmss");
							text12 = text12.Replace("{datetime}", newValue);
						}
					}
					if (File.Exists(text12))
					{
						text12 = Path.GetFileName(text12);
					}
					Smart.Log.Verbose(TAG, "FileName to be used " + text12);
					List<string> list3 = new List<string>();
					if (text12.Contains("*") || text12.Contains("?"))
					{
						list3 = Directory.GetFiles(text, text12, SearchOption.TopDirectoryOnly).ToList();
						if (list3.Count > 1)
						{
							string text13 = $"{list3.Count} files with name {text12} found in path {text}";
							Smart.Log.Error(TAG, text13);
						}
						else if (list3.Count < 1)
						{
							string text14 = $"No file {text12} found in path {text}";
							Smart.Log.Error(TAG, text14);
						}
					}
					else
					{
						list3.Add(Path.Combine(text, text12));
					}
					foreach (string item3 in list3)
					{
						list.Add(item3);
					}
				}
			}
			else
			{
				list.Add(text);
			}
		}
		return list;
	}
}
