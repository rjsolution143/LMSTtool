using System.Collections.Generic;
using ISmart;

namespace SmartRsd;

public class DetectionKeyComparer : IEqualityComparer<DetectionKey>
{
	public bool Equals(DetectionKey a, DetectionKey b)
	{
		if (a == null && b == null)
		{
			return true;
		}
		if (a == null || b == null)
		{
			return false;
		}
		if (!a.Matches(b))
		{
			return a.Equals(b);
		}
		return true;
	}

	public int GetHashCode(DetectionKey key)
	{
		return ((object)key).GetHashCode();
	}
}
