using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace UnisocCfcTest;

public class UnisocFrameWorkInvokerBase : ConsoleToolBase
{
	private new string loggroup = MethodBase.GetCurrentMethod().DeclaringType.Name;

	public bool enable_log { get; set; }

	private new string LogGroup => loggroup;

	public override int Execute(string testname, int portNumber, testcommandclass testcommandsharp, ref ParametricDataStruct parametricdata)
	{
		string name = MethodBase.GetCurrentMethod().Name;
		LogMessage(LogGroup, name, "enter...", TraceEventType.Information, AddTimeToLogMessage);
		int num = 0;
		enable_log = true;
		if (enable_log)
		{
			enable_full_log = true;
		}
		else
		{
			enable_full_log = false;
		}
		try
		{
			outputs = new List<string>();
			start_timeout = 10000;
			Smart.Log.Info(loggroup, Path.Combine(tool_directory, tool_name));
			num = base.Execute(testname, portNumber, testcommandsharp, ref parametricdata);
		}
		catch (Exception ex)
		{
			string progressText = "error message " + ex.Message + "; " + ex.StackTrace;
			LogMessage(LogGroup, name, progressText, TraceEventType.Error, AddTimeToLogMessage);
			num = -7739169;
		}
		LogMessage(LogGroup, name, "exit...", TraceEventType.Information, AddTimeToLogMessage);
		return num;
	}
}
