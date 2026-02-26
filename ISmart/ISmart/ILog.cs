using System;

namespace ISmart;

public interface ILog : IDisposable
{
	object ReportLock { get; }

	bool HideInfo { get; }

	void Assert(string tag, bool value, string message);

	void Critical(string tag, string message);

	void Error(string tag, string message);

	void Warning(string tag, string message);

	void Info(string tag, string message);

	void Debug(string tag, string message);

	void Verbose(string tag, string message);

	void Log(LogLevel level, string tag, string message);

	string GenerateErrorReport(string folder, string tag);
}
