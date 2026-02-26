using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using ConsoleToolBridge;
using SmartDevice.Steps;

namespace UnisocCfcTest;

public class ConsoleToolBase : BaseStep
{
	public bool AddTimeToLogMessage = true;

	public static bool mutexIsLocked = false;

	public static Mutex mutexSecureUnisoc = new Mutex();

	public string SeqFileDir = "C:\\prod\\bin\\simba\\Project\\";

	public string FrameWorkDir = "C:\\prod\\bin\\simba\\App";

	public List<string> inputs;

	public List<string> outputs;

	public int start_timeout;

	public string string_to_understand_that_tool_started = "FrameworkInvoker Start";

	public string string_to_understand_that_tool_is_returning_an_output = "Unisoc FrameworkInvoker Output";

	public string string_to_understand_that_tool_is_logging = "";

	public string string_to_understand_that_tool_succeeded = "!!!!! All Finished, pass !!!!!";

	public string string_to_understand_that_tool_failed = "!!!!! All Finished, fail !!!!!";

	public string tool_directory;

	public string tool_name = "FrameworkDemo.exe";

	public bool enable_full_log;

	public int PortNumFromTest = -1;

	private string tool_log_filename = string.Empty;

	private string failure_message = string.Empty;

	private object lock_object = new object();

	public ConsoleBridge my_tool = new ConsoleBridge();

	public string loggroup = MethodBase.GetCurrentMethod().DeclaringType.Name;

	private string TAG => GetType().FullName;

	public string LogGroup => loggroup;

	public event Action<Process, string> ReceiveOutputFromTool = delegate
	{
	};

	public void DisplayMsgOnUIGrid(int PortNumberScreen, string progressText)
	{
	}

	public void LogMessage(string logGroup, string methodName, string progressText, TraceEventType information, bool addTimeToLogMessage)
	{
		if (information == TraceEventType.Information)
		{
			Smart.Log.Info(logGroup, $"{methodName}:{progressText}");
		}
		else
		{
			Smart.Log.Error(logGroup, $"{methodName}:{progressText}");
		}
	}

	private void UpdateTestProgress(int portNumberScreen, string progressText)
	{
	}

	private void ReceiveLogInfoFromTool(int threadid, string message)
	{
		lock (lock_object)
		{
			if (message.Contains(string_to_understand_that_tool_is_logging))
			{
				DisplayMsgOnUIGrid(PortNumFromTest, "{" + threadid + "} " + message.Substring(string_to_understand_that_tool_is_logging.Length));
			}
			else if (message.Contains(string_to_understand_that_tool_failed))
			{
				DisplayMsgOnUIGrid(PortNumFromTest, "{" + threadid + "} " + message.Substring(string_to_understand_that_tool_failed.Length));
			}
			if (enable_full_log && !string.IsNullOrWhiteSpace(message))
			{
				Smart.Log.Info(TAG, message);
			}
			if (message.Contains(string_to_understand_that_tool_failed) && string.IsNullOrEmpty(failure_message))
			{
				failure_message = message.Substring(string_to_understand_that_tool_failed.Length);
			}
		}
	}

	public void TryDeleteFolder(string target_dir)
	{
		try
		{
			string[] files = Directory.GetFiles(target_dir);
			string[] directories = Directory.GetDirectories(target_dir);
			string[] array = files;
			foreach (string path in array)
			{
				File.SetAttributes(path, FileAttributes.Normal);
				File.Delete(path);
			}
			array = directories;
			foreach (string target_dir2 in array)
			{
				try
				{
					TryDeleteFolder(target_dir2);
				}
				catch
				{
				}
			}
			Directory.Delete(target_dir, recursive: false);
		}
		catch
		{
		}
	}

	public List<string> GetWords(string text)
	{
		Regex regex = new Regex("[a-zA-Z0-9]");
		string text2 = "";
		char[] array = text.ToCharArray();
		List<string> list = new List<string>();
		for (int i = 0; i < array.Length; i++)
		{
			char c = array[i];
			if (c > '\uffff')
			{
				continue;
			}
			if (char.IsHighSurrogate(c))
			{
				i++;
				list.Add(new string(new char[2]
				{
					c,
					array[i]
				}));
			}
			else if (regex.Match(c.ToString()).Success || c.ToString() == "/")
			{
				text2 += c;
			}
			else if (c.ToString() == " ")
			{
				if (text2.Length > 0)
				{
					list.Add(text2);
				}
				text2 = "";
			}
			else
			{
				if (text2.Length > 0)
				{
					list.Add(text2);
				}
				text2 = "";
			}
		}
		return list;
	}

	public string GetResultFromLog(string strIn, List<string> lst)
	{
		string text = string.Empty;
		foreach (string item in lst)
		{
			if (item.Contains(strIn))
			{
				text = item;
			}
		}
		return text.Replace(strIn, "");
	}

	public override void Run()
	{
	}

	public virtual int Execute(string testname, int portNumber, testcommandclass testcommandsharp, ref ParametricDataStruct parametricdata)
	{
		string name = MethodBase.GetCurrentMethod().Name;
		LogMessage(LogGroup, name, "enter...", TraceEventType.Information, AddTimeToLogMessage);
		int num = -7739182;
		try
		{
			tool_log_filename = tool_name + "_" + portNumber + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Second.ToString();
			PortNumFromTest = portNumber;
			my_tool.thread_id = PortNumFromTest;
			my_tool.outputs = outputs;
			my_tool.inputs = inputs;
			my_tool.start_timeout = start_timeout;
			my_tool.string_to_understand_that_tool_started = string_to_understand_that_tool_started;
			my_tool.string_to_understand_that_tool_is_returning_an_output = string_to_understand_that_tool_is_returning_an_output;
			if (enable_full_log)
			{
				my_tool.string_to_understand_that_tool_is_logging = "";
			}
			else
			{
				my_tool.string_to_understand_that_tool_is_logging = string_to_understand_that_tool_is_logging;
			}
			my_tool.string_to_understand_that_tool_succeeded = string_to_understand_that_tool_succeeded;
			my_tool.string_to_understand_that_tool_failed = string_to_understand_that_tool_failed;
			my_tool.tool_directory = tool_directory;
			my_tool.tool_name = tool_name;
			my_tool.TriggerLogInfo += ReceiveLogInfoFromTool;
			num = my_tool.Execute();
			if (num != 0)
			{
				string progressText = "error code " + num;
				LogMessage(LogGroup, name, progressText, TraceEventType.Error, AddTimeToLogMessage);
				return num;
			}
		}
		catch (Exception ex)
		{
			string progressText2 = "error message " + ex.Message + "; " + ex.StackTrace;
			LogMessage(LogGroup, name, progressText2, TraceEventType.Error, AddTimeToLogMessage);
			num = -7739169;
		}
		LogMessage(LogGroup, name, "exit...", TraceEventType.Information, AddTimeToLogMessage);
		return num;
	}
}
