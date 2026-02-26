using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ISmart;

namespace SmartUtil;

public class TimedCache : ITimedCache, IDictionary<string, string>, ICollection<KeyValuePair<string, string>>, IEnumerable<KeyValuePair<string, string>>, IEnumerable
{
	private SortedList<string, string> cache = new SortedList<string, string>();

	private SortedList<string, DateTime> expirations = new SortedList<string, DateTime>();

	private string TAG => GetType().FullName;

	public string this[string key]
	{
		get
		{
			lock (this)
			{
				Refresh();
				return cache[key];
			}
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public int Count
	{
		get
		{
			lock (this)
			{
				Refresh();
				return cache.Count;
			}
		}
	}

	public bool IsReadOnly => true;

	public ICollection<string> Keys
	{
		get
		{
			lock (this)
			{
				Refresh();
				return cache.Keys;
			}
		}
	}

	public ICollection<string> Values
	{
		get
		{
			lock (this)
			{
				Refresh();
				return cache.Values;
			}
		}
	}

	private void Refresh()
	{
		List<string> list = null;
		foreach (string key in expirations.Keys)
		{
			DateTime value = expirations[key];
			if (DateTime.Now.CompareTo(value) > 0)
			{
				if (list == null)
				{
					list = new List<string>();
				}
				list.Add(key);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (string item in list)
		{
			cache.Remove(item);
			expirations.Remove(item);
		}
	}

	public void Add(KeyValuePair<string, string> item)
	{
		throw new NotImplementedException();
	}

	public void Add(string key, string value)
	{
		throw new NotImplementedException();
	}

	public void Add(string key, string value, TimeSpan expiration)
	{
		lock (this)
		{
			Refresh();
			expirations[key] = DateTime.Now.Add(expiration);
			cache[key] = value;
		}
	}

	public void Clear()
	{
		cache.Clear();
		expirations.Clear();
	}

	public bool Contains(KeyValuePair<string, string> item)
	{
		lock (this)
		{
			Refresh();
			return cache.Contains(item);
		}
	}

	public bool ContainsKey(string key)
	{
		lock (this)
		{
			Refresh();
			return cache.ContainsKey(key);
		}
	}

	public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
	{
		throw new NotImplementedException();
	}

	public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
	{
		lock (this)
		{
			Refresh();
			return cache.GetEnumerator();
		}
	}

	public bool Remove(KeyValuePair<string, string> item)
	{
		lock (this)
		{
			expirations.Remove(item.Key);
			return cache.Remove(item.Key);
		}
	}

	public bool Remove(string key)
	{
		lock (this)
		{
			expirations.Remove(key);
			return cache.Remove(key);
		}
	}

	public bool TryGetValue(string key, out string value)
	{
		lock (this)
		{
			Refresh();
			return cache.TryGetValue(key, out value);
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		lock (this)
		{
			Refresh();
			return cache.GetEnumerator();
		}
	}
}
