using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace UnisocCfcTest;

public class UnisocCallFrameworkInvoker : UnisocFrameWorkInvokerBase
{
	private new string loggroup = MethodBase.GetCurrentMethod().DeclaringType.Name;

	public string _testname;

	public int _portNumber;

	public testcommandclass _testcommandsharp;

	public ParametricDataStruct _parametricdata;

	public string project { get; set; }

	private new string LogGroup => loggroup;

	public string SeqFile_Simba { get; set; }

	public override int Execute(string testname, int portNumber, testcommandclass testcommandsharp, ref ParametricDataStruct parametricdata)
	{
		string name = MethodBase.GetCurrentMethod().Name;
		LogMessage(LogGroup, name, "enter...", TraceEventType.Information, AddTimeToLogMessage);
		int num = 0;
		inputs = new List<string>();
		_ = string.Empty;
		try
		{
			inputs.Add("-k");
			inputs.Add(portNumber.ToString());
			inputs.Add("-seq");
			if (!File.Exists(SeqFile_Simba))
			{
				num = -7739169;
			}
			else
			{
				inputs.Add(SeqFile_Simba);
				num = base.Execute(testname, portNumber, testcommandsharp, ref parametricdata);
			}
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
