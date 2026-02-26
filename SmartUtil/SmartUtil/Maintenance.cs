using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ISmart;

namespace SmartUtil;

public class Maintenance : IMaintenance
{
	private string jsonPath = Path.Combine(Smart.File.CommonStorageDir, "maintenance.json");

	private DateTime lastReport = DateTime.MinValue;

	private string TAG => GetType().FullName;

	public SortedList<string, SortedList<string, object>> EventData { get; private set; }

	public Maintenance()
	{
		Load();
	}

	private void Load()
	{
		if (!System.IO.File.Exists(jsonPath))
		{
			Save();
		}
		string text = Smart.File.ReadText(jsonPath);
		dynamic val = Smart.Json.Load(text);
		string s = val["LastReport"].ToString();
		lastReport = DateTime.Parse(s, null, DateTimeStyles.RoundtripKind);
		EventData = new SortedList<string, SortedList<string, object>>();
		if (!((val["Events"] != null) ? true : false))
		{
			return;
		}
		foreach (dynamic item in val["Events"])
		{
			string key = item.Name;
			EventData[key] = new SortedList<string, object>();
			dynamic val2 = item.Value;
			foreach (dynamic item2 in val2)
			{
				string key2 = item2.Name;
				string value = item2.Value.ToString();
				EventData[key][key2] = value;
			}
		}
	}

	private void Save()
	{
		SortedList<string, object> sortedList = new SortedList<string, object>();
		sortedList["FormatVersion"] = "1.0";
		sortedList["LastReport"] = lastReport.ToString("o");
		sortedList["Events"] = EventData;
		string text = Smart.Json.Dump((object)sortedList);
		Smart.File.WriteText(jsonPath, text);
	}

	public void Check()
	{
		if (!(DateTime.Now.Subtract(lastReport).TotalDays < 8.0))
		{
			Smart.Log.Info(TAG, string.Format("Last maintenance report was made on {0}, sending new report now", lastReport.ToString("yyyy-MM-dd HH:mm")));
			IDevice val = Smart.DeviceManager.ManualDevice(true);
			val.SerialNumber = "112233444444444";
			Smart.UseCaseRunner.Run((UseCase)176, val, false, false);
			lastReport = DateTime.Now;
			Save();
		}
	}

	public void ReportEvent(string eventName)
	{
		if (!EventData.ContainsKey(eventName))
		{
			EventData[eventName] = new SortedList<string, object>();
		}
		if (!EventData[eventName].ContainsKey("Count"))
		{
			EventData[eventName]["Count"] = 0;
		}
		int num = int.Parse(EventData[eventName]["Count"].ToString());
		EventData[eventName]["Count"] = num + 1;
		EventData[eventName]["Last"] = DateTime.Now.ToString("o");
		Save();
	}

	public void CleanPort()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			MatchCollection matchCollection = Regex.Matches(RunPowerShellScriptAsync("\n                  $hiddenDevices = Get-PnpDevice -PresentOnly:$false | Select-Object InstanceId, FriendlyName\n                  $hiddenDevices | ForEach-Object {\n                  Write-Output \"HiddenDevice: $($_.FriendlyName)\"\n                  Write-Host $_.InstanceId}").Result, "USB\\\\.*|USBSTOR\\\\.*");
			ParallelOptions parallelOptions = new ParallelOptions
			{
				MaxDegreeOfParallelism = 60
			};
			Parallel.ForEach(matchCollection.Cast<Match>(), parallelOptions, delegate(Match match)
			{
				string value = match.Value;
				Smart.Log.Info(TAG, $"Removing: {value}");
				try
				{
					string result = RunCmdAsync($"pnputil /remove-device \"{value}\"").Result;
					Smart.Log.Info(TAG, result);
				}
				catch (Exception ex2)
				{
					Smart.Log.Info(TAG, $"Error removing device {value}: {ex2.Message}");
				}
			});
			Smart.Log.Info(TAG, $"{matchCollection.Count} hidden devices removed successfully.");
			Smart.NewPrompt().MessageBox("CleanPort", $"All {matchCollection.Count} hidden devices removed successfully.", (MessageBoxButtons)0, (MessageBoxIcon)64);
		}
		catch (Exception ex)
		{
			Smart.Log.Info(TAG, "Remove hidden devices error: " + ex.GetType().FullName + ex.Message + ex.StackTrace);
		}
	}

	private static async Task<string> RunPowerShellScriptAsync(string script)
	{
		ProcessStartInfo startInfo = new ProcessStartInfo
		{
			FileName = "powershell.exe",
			Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{script}\"",
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true,
			WorkingDirectory = Environment.SystemDirectory
		};
		using Process process = new Process();
		process.StartInfo = startInfo;
		process.Start();
		using StreamReader reader = process.StandardOutput;
		char[] buffer = new char[4096];
		string output = "";
		while (true)
		{
			int num;
			int bytesRead = (num = await reader.ReadAsync(buffer, 0, buffer.Length));
			if (num <= 0)
			{
				break;
			}
			output += new string(buffer, 0, bytesRead);
		}
		await WaitForExitAsync(process);
		return output;
	}

	private static async Task<string> RunCmdAsync(string command)
	{
		ProcessStartInfo startInfo = new ProcessStartInfo
		{
			FileName = "cmd.exe",
			Arguments = $"/C {command}",
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true,
			WorkingDirectory = Environment.SystemDirectory
		};
		using Process process = new Process();
		process.StartInfo = startInfo;
		process.Start();
		using StreamReader reader = process.StandardOutput;
		char[] buffer = new char[4096];
		string output = "";
		while (true)
		{
			int num;
			int bytesRead = (num = await reader.ReadAsync(buffer, 0, buffer.Length));
			if (num <= 0)
			{
				break;
			}
			output += new string(buffer, 0, bytesRead);
		}
		await WaitForExitAsync(process);
		return output;
	}

	private static Task WaitForExitAsync(Process process)
	{
		TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
		process.EnableRaisingEvents = true;
		EventHandler exitedHandler = null;
		exitedHandler = delegate
		{
			process.Exited -= exitedHandler;
			if (!tcs.Task.IsCompleted)
			{
				tcs.SetResult(result: true);
			}
		};
		process.Exited += exitedHandler;
		if (process.HasExited && !tcs.Task.IsCompleted)
		{
			tcs.SetResult(result: true);
		}
		return tcs.Task;
	}
}
