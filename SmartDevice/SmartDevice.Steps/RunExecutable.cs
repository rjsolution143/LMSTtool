using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ISmart;

namespace SmartDevice.Steps;

public class RunExecutable : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0d13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cea: Unknown result type (might be due to invalid IL or missing references)
		string text = ((dynamic)base.Info.Args).EXE;
		Result result = (Result)8;
		string text2 = string.Empty;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		string text3 = string.Empty;
		if (((dynamic)base.Info.Args).Command != null)
		{
			text3 = ((dynamic)base.Info.Args).Command;
			if (((dynamic)base.Info.Args).Format != null)
			{
				List<string> list = new List<string>();
				foreach (object item2 in ((dynamic)base.Info.Args).Format)
				{
					string text4 = (string)(dynamic)item2;
					string item = text4;
					if (text4.StartsWith("$"))
					{
						string key2 = text4.Substring(1);
						item = base.Cache[key2];
					}
					list.Add(item);
				}
				text3 = string.Format(text3, list.ToArray());
			}
		}
		string text5 = "ExeProcess";
		if (((dynamic)base.Info.Args).Name != null)
		{
			text5 = ((dynamic)base.Info.Args).Name;
		}
		string currentDirectory = Environment.CurrentDirectory;
		string directoryName = Path.GetDirectoryName(text);
		bool createNoWindow = true;
		if (((dynamic)base.Info.Args).ConsoleOff != null)
		{
			createNoWindow = (bool)((dynamic)base.Info.Args).ConsoleOff;
		}
		bool flag = false;
		if (((dynamic)base.Info.Args).WaitForExit != null)
		{
			flag = (bool)((dynamic)base.Info.Args).WaitForExit;
		}
		bool flag2 = false;
		if (((dynamic)base.Info.Args).LogExeOutput != null)
		{
			flag2 = (bool)((dynamic)base.Info.Args).LogExeOutput;
		}
		if (Directory.Exists(directoryName))
		{
			Directory.SetCurrentDirectory(directoryName);
		}
		text = Smart.Convert.QuoteFilePathName(text);
		if (text.ToUpper().Contains("ODMSocketServer".ToUpper()))
		{
			string messageForDynamicData = string.Empty;
			KillProcessWhoOcupyODMSocketServerPort(out messageForDynamicData);
		}
		try
		{
			Process process = new Process();
			process.StartInfo.FileName = text;
			process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
			process.StartInfo.Arguments = text3;
			process.StartInfo.ErrorDialog = false;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = createNoWindow;
			if (flag2)
			{
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
				{
					if (!string.IsNullOrEmpty(e.Data))
					{
						Smart.Log.Debug(TAG, e.Data);
					}
				};
				process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e)
				{
					if (!string.IsNullOrEmpty(e.Data))
					{
						Smart.Log.Debug(TAG, e.Data);
					}
				};
			}
			if (process.Start())
			{
				if (!base.Cache.ContainsKey(text5))
				{
					Smart.Log.Info(TAG, $"{text} started");
					base.Cache[text5] = process;
					if (flag2)
					{
						Smart.Log.Info(TAG, $"Start recording the log of {text}");
						process.BeginOutputReadLine();
						process.BeginErrorReadLine();
					}
					if (flag)
					{
						process.WaitForExit();
					}
					process = null;
				}
				else
				{
					Smart.Log.Debug(TAG, $"Process name: {text5} is already running");
				}
			}
			else
			{
				text2 = $"Failed to start {text}";
				result = (Result)1;
				Smart.Log.Error(TAG, text2);
			}
		}
		catch (Exception ex)
		{
			text2 = $"Exception - {ex.Message}";
			result = (Result)1;
			Smart.Log.Error(TAG, text2);
			Smart.Log.Debug(TAG, ex.StackTrace);
		}
		finally
		{
			Directory.SetCurrentDirectory(currentDirectory);
		}
		LogResult(result, text2);
	}

	private bool KillProcessWhoOcupyODMSocketServerPort(out string messageForDynamicData)
	{
		bool result = false;
		messageForDynamicData = string.Empty;
		try
		{
			Process process = new Process();
			process.StartInfo.FileName = "cmd.exe";
			process.StartInfo.Arguments = "/c netstat.exe -aonp TCP|findstr 5000";
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.UseShellExecute = false;
			process.EnableRaisingEvents = true;
			process.StartInfo.CreateNoWindow = true;
			List<string> output = new List<string>();
			List<string> error = new List<string>();
			process.OutputDataReceived += delegate(object _sender, DataReceivedEventArgs _e)
			{
				Redirected(output, _sender, _e);
			};
			process.ErrorDataReceived += delegate(object _sender, DataReceivedEventArgs _e)
			{
				Redirected(error, _sender, _e);
			};
			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			if (!process.WaitForExit(10000))
			{
				messageForDynamicData = $"Process {process.StartInfo.FileName} {process.StartInfo.Arguments} exit fail.";
				result = false;
			}
			else
			{
				foreach (string item in output)
				{
					Smart.Log.Info(TAG, $"Check {item}.");
					if (!item.Contains("127.0.0.1:5000 "))
					{
						continue;
					}
					string text = string.Empty;
					string[] array = item.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (array.Length == 5)
					{
						text = array[4];
					}
					if (!(text != string.Empty))
					{
						continue;
					}
					Process processById = Process.GetProcessById(Convert.ToInt32(text));
					if (processById != null)
					{
						string processName = processById.ProcessName;
						if (!processName.Contains("ODMSocketServer"))
						{
							messageForDynamicData = $"Found process {processName} use 5000 port, kill it...";
							Smart.Log.Info(TAG, messageForDynamicData);
							processById.Kill();
							Thread.Sleep(1000);
						}
					}
				}
				result = true;
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Info(TAG, ex.Message + Environment.NewLine + ex.StackTrace);
		}
		return result;
	}

	private void Redirected(List<string> dataList, object sender, DataReceivedEventArgs e)
	{
		if (e.Data != null)
		{
			Smart.Log.Info(TAG, "Base Redirected Shell Resp: " + e.Data);
			string item = e.Data.Trim();
			dataList.Add(item);
		}
	}
}
