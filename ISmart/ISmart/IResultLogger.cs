using System;
using System.Collections.Generic;

namespace ISmart;

public interface IResultLogger : IDisposable
{
	List<Tuple<string, SortedList<string, dynamic>>> Results { get; }

	SortedList<string, string> Info { get; }

	double Progress { get; set; }

	UseCase UseCase { get; set; }

	Result OverallResult { get; set; }

	string CurrentStep { get; set; }

	bool UserQuit { get; set; }

	bool AbortedStep { get; set; }

	string ResultID { get; set; }

	List<IResultSubLogger> SubLogs { get; }

	string RsdLogId { get; }

	List<string> PassedSteps { get; }

	DateTime StartTime { get; }

	DateTime EndTime { get; }

	object Lock { get; }

	void AddInfo(string name, string value);

	void RemoveInfo(string name);

	void CopyInfo(SortedList<string, string> info);

	void AddResult(string name, SortedList<string, dynamic> details);

	void AddResult(string name, string step, Result result, string description = "", string friendlyName = "", string dynamicMessage = "", double upperLimit = double.MinValue, double lowerLimit = double.MinValue, double value = double.MinValue, SortedList<string, dynamic> details = null);
}
