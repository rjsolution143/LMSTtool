using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ConsoleToolBridge;

public class ConsoleBridge
{
	private sealed class SC
	{
		public static readonly SC sc = new SC();

		public static Action<int, string> TriggerLogInfo;

		public static Action<Process, string> TriggerActionUsingOutput;

		internal void triggerLogInfo(int p0, string p1)
		{
		}

		internal void triggerActionUsingOutput(Process p0, string p1)
		{
		}
	}

	private Process toolProcess = new Process();

	public int thread_id;

	public List<string> inputs;

	public List<string> outputs;

	public int start_timeout;

	public string string_to_understand_that_tool_started;

	public string string_to_understand_that_tool_is_returning_an_output;

	public string string_to_understand_that_tool_is_logging;

	public string string_to_understand_that_tool_succeeded;

	public string string_to_understand_that_tool_failed;

	public string tool_directory;

	public string tool_name;

	private bool ToolStarted;

	private bool ToolSuccess;

	private AutoResetEvent ProcessEnd;

	private string ToolMessage;

	public int wait_for_exit_ms = 500000;

	public event Action<int, string> TriggerLogInfo;

	public event Action<Process, string> TriggerActionUsingOutput;

	private void receivedConsoleData(object sendingProcess, DataReceivedEventArgs outLine)
	{
		if (string.IsNullOrEmpty(outLine.Data))
		{
			return;
		}
		if (outLine.Data.Contains(string_to_understand_that_tool_started))
		{
			ToolStarted = true;
			this.TriggerLogInfo(thread_id, outLine.Data);
		}
		else if (outLine.Data.Contains(string_to_understand_that_tool_is_returning_an_output))
		{
			outputs.Add(outLine.Data);
			if (this.TriggerActionUsingOutput != null)
			{
				this.TriggerActionUsingOutput(toolProcess, outLine.Data);
			}
		}
		else if (outLine.Data.Contains(string_to_understand_that_tool_succeeded))
		{
			ToolSuccess = true;
			this.TriggerLogInfo(thread_id, outLine.Data);
		}
		else if (outLine.Data.Contains(string_to_understand_that_tool_failed))
		{
			this.TriggerLogInfo(thread_id, outLine.Data);
		}
		else if (outLine.Data.Contains("<CWriteSIMLOCK_TINNO> pass"))
		{
			ToolSuccess = true;
		}
		else if (outLine.Data.Contains("<CWriteIMEI_1_2_TINNO> pass"))
		{
			ToolSuccess = true;
		}
		else if (outLine.Data.Contains(string_to_understand_that_tool_is_logging))
		{
			this.TriggerLogInfo(thread_id, outLine.Data);
		}
	}

	private void ToolHasExited(object sender, EventArgs e)
	{
		ProcessEnd.Set();
	}

	public int Execute()
	{
		string text = string.Empty;
		int result = 0;
		if (!tool_directory.EndsWith("\\"))
		{
			tool_directory += "\\";
		}
		toolProcess.StartInfo.FileName = Smart.Convert.QuoteFilePathName(tool_directory + tool_name);
		toolProcess.StartInfo.WorkingDirectory = tool_directory;
		toolProcess.StartInfo.RedirectStandardOutput = true;
		toolProcess.StartInfo.RedirectStandardInput = true;
		toolProcess.StartInfo.UseShellExecute = false;
		toolProcess.StartInfo.CreateNoWindow = true;
		toolProcess.OutputDataReceived += receivedConsoleData;
		toolProcess.Exited += ToolHasExited;
		toolProcess.Disposed += ToolHasExited;
		try
		{
			foreach (string input in inputs)
			{
				text += input;
				text += " ";
			}
			text.Substring(0, text.Length - 1);
			this.TriggerLogInfo(thread_id, "Starting " + tool_name);
			toolProcess.StartInfo.Arguments = text;
			this.TriggerLogInfo(thread_id, "Args " + text);
			toolProcess.Start();
			this.TriggerLogInfo(thread_id, "Started " + tool_name);
			toolProcess.BeginOutputReadLine();
			this.TriggerLogInfo(thread_id, "BeginOutputReadLine " + tool_name);
			this.TriggerLogInfo(thread_id, "handle:" + toolProcess.Handle);
			toolProcess.WaitForExit(wait_for_exit_ms);
			ProcessEnd.Set();
			Stopwatch stopwatch = Stopwatch.StartNew();
			while (!ToolStarted && stopwatch.ElapsedMilliseconds <= start_timeout)
			{
			}
			stopwatch.Stop();
			if (!ToolStarted)
			{
				this.TriggerLogInfo(thread_id, tool_name + " FAIL to Start");
				return -1;
			}
			ProcessEnd.WaitOne();
			Thread.Sleep(5000);
			ForceExitProcess(toolProcess);
			this.TriggerLogInfo(thread_id, tool_name + " Exited");
			result = ((!ToolSuccess) ? (-1) : 0);
		}
		catch (Exception ex)
		{
			this.TriggerLogInfo(thread_id, tool_name + " CRASHED. " + ex.Message + "; " + ex.StackTrace);
			result = -1;
		}
		finally
		{
			toolProcess.OutputDataReceived -= receivedConsoleData;
			toolProcess.Exited -= ToolHasExited;
			toolProcess.Disposed -= ToolHasExited;
			toolProcess.Dispose();
		}
		return result;
	}

	private void ForceExitProcess(Process mtkFlashToolProcess)
	{
		try
		{
			if (mtkFlashToolProcess.HasExited)
			{
				return;
			}
			if (mtkFlashToolProcess.Responding)
			{
				mtkFlashToolProcess.CloseMainWindow();
				this.TriggerLogInfo(thread_id, tool_name + " closed");
				if (!mtkFlashToolProcess.HasExited)
				{
					mtkFlashToolProcess.Kill();
					this.TriggerLogInfo(thread_id, tool_name + " Killed after closed Window");
				}
			}
			else
			{
				mtkFlashToolProcess.Kill();
				this.TriggerLogInfo(thread_id, tool_name + " Killed");
			}
		}
		catch
		{
		}
	}

	public ConsoleBridge()
	{
		Action<int, string> triggerLogInfo;
		if ((triggerLogInfo = SC.TriggerLogInfo) == null)
		{
			triggerLogInfo = (SC.TriggerLogInfo = SC.sc.triggerLogInfo);
		}
		this.TriggerLogInfo = triggerLogInfo;
		Action<Process, string> triggerActionUsingOutput;
		if ((triggerActionUsingOutput = SC.TriggerActionUsingOutput) == null)
		{
			triggerActionUsingOutput = (SC.TriggerActionUsingOutput = SC.sc.triggerActionUsingOutput);
		}
		this.TriggerActionUsingOutput = triggerActionUsingOutput;
		ProcessEnd = new AutoResetEvent(initialState: false);
		ToolMessage = string.Empty;
	}
}
