using System.Collections.Generic;
using ISmart;

namespace SmartRsd;

public class MatrixInfo
{
	public bool FullyParsed { get; private set; }

	public string DirectoryPath { get; private set; }

	public Dictionary<DetectionKey, List<JsonConfig>> LookupTable { get; private set; }

	public MatrixInfo(bool fullyParsed, string dir, Dictionary<DetectionKey, List<JsonConfig>> lookupTable)
	{
		FullyParsed = fullyParsed;
		DirectoryPath = dir;
		LookupTable = lookupTable;
	}
}
