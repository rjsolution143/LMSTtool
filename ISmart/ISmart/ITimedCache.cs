using System;
using System.Collections;
using System.Collections.Generic;

namespace ISmart;

public interface ITimedCache : IDictionary<string, string>, ICollection<KeyValuePair<string, string>>, IEnumerable<KeyValuePair<string, string>>, IEnumerable
{
	void Add(string key, string value, TimeSpan expiration);
}
