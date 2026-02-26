using System;
using System.Collections.Generic;
using ISmart;

namespace SmartUtil;

public class ResultLogger : IResultLogger, IDisposable
{
	private SortedList<string, string> info = new SortedList<string, string>();

	private UseCase useCase;

	private bool disposedValue;

	private string TAG => GetType().FullName;

	public List<Tuple<string, SortedList<string, dynamic>>> Results { get; private set; }

	public SortedList<string, string> Info
	{
		get
		{
			lock (Lock)
			{
				return new SortedList<string, string>(info);
			}
		}
	}

	public double Progress { get; set; }

	public string ResultID { get; set; }

	public Result OverallResult { get; set; }

	public string CurrentStep { get; set; }

	public bool UserQuit { get; set; }

	public bool AbortedStep { get; set; }

	protected int ResultCount { get; set; }

	public List<IResultSubLogger> SubLogs { get; }

	public string RsdLogId { get; private set; }

	public List<string> PassedSteps { get; private set; }

	public DateTime StartTime { get; private set; }

	public DateTime EndTime { get; private set; }

	public object Lock { get; }

	public UseCase UseCase
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return useCase;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			useCase = value;
			foreach (IResultSubLogger subLog in SubLogs)
			{
				subLog.UseCase = useCase;
			}
		}
	}

	public ResultLogger()
	{
		Lock = new object();
		Results = new List<Tuple<string, SortedList<string, object>>>();
		ResultID = "default";
		OverallResult = (Result)0;
		ResultCount = 0;
		Progress = 0.0;
		UserQuit = false;
		AbortedStep = false;
		SubLogs = new List<IResultSubLogger>();
		RsdLogId = Smart.File.Uuid().ToUpperInvariant();
		RsdLogId = Smart.Convert.HexToOddOrEven(RsdLogId, true);
		PassedSteps = new List<string>();
		StartTime = DateTime.Now;
		EndTime = DateTime.Now;
	}

	public void AddInfo(string name, string value)
	{
		lock (Lock)
		{
			info[name] = value;
			foreach (IResultSubLogger subLog in SubLogs)
			{
				if (subLog.IsOpen)
				{
					subLog.AddInfo(name, value);
				}
			}
		}
	}

	public void RemoveInfo(string name)
	{
		lock (Lock)
		{
			info.Remove(name);
		}
	}

	public void CopyInfo(SortedList<string, string> newInfo)
	{
		lock (Lock)
		{
			info = new SortedList<string, string>(newInfo);
		}
	}

	public void AddResult(string name, SortedList<string, dynamic> details)
	{
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Invalid comparison between Unknown and I4
		ResultID = Smart.File.Uuid();
		ResultCount++;
		details["name"] = name;
		details["index"] = ResultCount;
		Tuple<string, SortedList<string, object>> tuple = new Tuple<string, SortedList<string, object>>(name, details);
		lock (Lock)
		{
			if (details.ContainsKey("result") && (int)(Result)details["result"] == 8)
			{
				PassedSteps.Add(name.ToLowerInvariant().Trim());
			}
			UpdateOverallResult(tuple);
			Results.Add(tuple);
			Smart.Log.Log((LogLevel)2, TAG, string.Format("{0} result {1}", name, details["result"]));
			foreach (IResultSubLogger subLog in SubLogs)
			{
				subLog.AddResult(name, (SortedList<string, object>)details);
			}
		}
		EndTime = DateTime.Now;
	}

	public void AddResult(string name, string step, Result result, string description = "", string friendlyName = "", string dynamicMessage = "", double upperLimit = double.MinValue, double lowerLimit = double.MinValue, double value = double.MinValue, SortedList<string, dynamic> details = null)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (details == null)
		{
			details = new SortedList<string, object>();
		}
		details["name"] = name;
		details["step"] = step;
		details["result"] = result;
		details["description"] = description;
		details["dynamic"] = dynamicMessage;
		if (friendlyName != null && friendlyName.Trim() != string.Empty)
		{
			details["friendlyName"] = friendlyName;
		}
		if (upperLimit > double.MinValue)
		{
			details["upperLimit"] = upperLimit;
		}
		if (lowerLimit > double.MinValue)
		{
			details["lowerLimit"] = lowerLimit;
		}
		if (value > double.MinValue)
		{
			details["value"] = value;
		}
		AddResult(name, details);
	}

	private void UpdateOverallResult(Tuple<string, SortedList<string, dynamic>> newResult)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected I4, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Invalid comparison between Unknown and I4
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Invalid comparison between Unknown and I4
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if ((int)OverallResult == 5)
		{
			return;
		}
		Result val = (Result)newResult.Item2["result"];
		switch ((int)val)
		{
		default:
			if ((int)OverallResult == 0)
			{
				OverallResult = (Result)8;
			}
			break;
		case 6:
		case 8:
			if ((int)OverallResult == 0)
			{
				OverallResult = (Result)8;
			}
			break;
		case 2:
		case 5:
			if ((int)OverallResult == 8 || (int)OverallResult == 1 || (int)OverallResult == 0)
			{
				OverallResult = (Result)5;
			}
			UserQuit = true;
			break;
		case 4:
			AbortedStep = true;
			OverallResult = (Result)1;
			break;
		case 1:
			OverallResult = (Result)1;
			break;
		case 3:
			break;
		}
	}

	protected virtual void Dispose(bool disposing)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (disposedValue)
		{
			return;
		}
		if (disposing)
		{
			UseCase val = UseCase;
			AddResult(((object)(UseCase)(ref val)).ToString(), string.Empty, OverallResult);
			foreach (IResultSubLogger subLog in SubLogs)
			{
				((IDisposable)subLog).Dispose();
			}
		}
		disposedValue = true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	public override string ToString()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		string text = $"{UseCase} use case ";
		text = ((!(Progress >= 100.0)) ? (text + $"{Progress}% complete on step {CurrentStep}\n") : (text + $"COMPLETE with result {OverallResult}\n"));
		if (UserQuit)
		{
			text += "NOTE: User quit\n";
		}
		if (AbortedStep)
		{
			text += "NOTE: Step aborted\n";
		}
		text += $"{ResultCount} steps completed \n";
		string arg = Smart.Convert.TimeSpanToDisplay(DateTime.Now.Subtract(StartTime));
		return text + $"Total time {arg}";
	}
}
