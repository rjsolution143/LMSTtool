using System;
using System.Collections.Generic;

namespace ISmart;

public interface IResultSubLogger : IDisposable
{
	string Name { get; }

	bool IsOpen { get; }

	UseCase UseCase { get; set; }

	void AddInfo(string name, string value);

	void AddResult(string name, SortedList<string, dynamic> details);
}
