using System.Collections;
using System.Collections.Generic;

namespace ISmart;

public interface ICsvFile : IEnumerable<SortedList<string, string>>, IEnumerable
{
	List<string> Headers { get; set; }

	List<List<string>> Rows { get; }

	SortedList<string, string> this[int index] { get; }

	void LoadFile(string fileName, char separator = ',');

	void Load(string csvText, char separator = ',');

	void SaveFile(string fileName);

	void SaveFile(string fileName, bool writeHeaders);
}
