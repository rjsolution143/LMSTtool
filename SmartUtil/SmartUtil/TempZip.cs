using System;
using System.Collections;
using System.Collections.Generic;
using ISmart;

namespace SmartUtil;

public class TempZip : ITempZip, IList<string>, ICollection<string>, IEnumerable<string>, IEnumerable, IDisposable
{
	private string TAG => GetType().FullName;

	public List<string> FileList { get; set; }

	public string TempFolder { get; set; }

	public string this[int index]
	{
		get
		{
			return FileList[index];
		}
		set
		{
			FileList[index] = value;
		}
	}

	public int Count => FileList.Count;

	public bool IsReadOnly => true;

	public TempZip(string zipFile, string password)
	{
		TempFolder = Smart.File.TempFolder();
		Smart.Zip.Extract(zipFile, TempFolder, password);
		List<string> fileList = Smart.File.FindFiles("*.*", TempFolder, true);
		FileList = fileList;
	}

	public int IndexOf(string item)
	{
		return FileList.IndexOf(item);
	}

	public void Insert(int index, string item)
	{
		FileList.Insert(index, item);
	}

	public void RemoveAt(int index)
	{
		FileList.RemoveAt(index);
	}

	public void Add(string item)
	{
		FileList.Add(item);
	}

	public void Clear()
	{
		FileList.Clear();
	}

	public bool Contains(string item)
	{
		return FileList.Contains(item);
	}

	public void CopyTo(string[] array, int arrayIndex)
	{
		FileList.CopyTo(array, arrayIndex);
	}

	public bool Remove(string item)
	{
		return FileList.Remove(item);
	}

	public IEnumerator<string> GetEnumerator()
	{
		return FileList.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return FileList.GetEnumerator();
	}

	public void Dispose()
	{
		Smart.File.Remove(TempFolder);
	}
}
