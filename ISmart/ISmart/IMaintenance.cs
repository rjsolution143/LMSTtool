using System.Collections.Generic;

namespace ISmart;

public interface IMaintenance
{
	SortedList<string, SortedList<string, object>> EventData { get; }

	void ReportEvent(string eventType);

	void Check();

	void CleanPort();
}
