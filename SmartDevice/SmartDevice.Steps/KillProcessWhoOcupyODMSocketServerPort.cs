using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using ISmart;

namespace SmartDevice.Steps;

[Obsolete]
public class KillProcessWhoOcupyODMSocketServerPort : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Result val = (Result)1;
		string messageForDynamicData = string.Empty;
		val = ((!Kill(out messageForDynamicData)) ? ((Result)1) : ((Result)8));
		LogResult(val, TAG, messageForDynamicData);
	}

	public virtual void Redirected(List<string> dataList, object sender, DataReceivedEventArgs e)
	{
		if (e.Data != null)
		{
			Smart.Log.Info(TAG, "Base Redirected Shell Resp: " + e.Data);
			string item = e.Data.Trim();
			dataList.Add(item);
		}
	}

	public bool Kill(out string messageForDynamicData)
	{
		bool flag = false;
		messageForDynamicData = string.Empty;
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
			return false;
		}
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
		return true;
	}
}
