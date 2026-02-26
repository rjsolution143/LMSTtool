using System.Collections.Generic;

namespace ISmart;

public interface ISession
{
	bool IsSaved(SessionType type);

	void Delete(SessionType type);

	void Save(SessionType type, SortedList<string, string> data);

	SortedList<string, string> Load(SessionType type);
}
