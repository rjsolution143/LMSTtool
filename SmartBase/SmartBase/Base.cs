using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ISmart;

namespace SmartBase;

public class Base : IBase, IDisposable
{
	protected delegate ObjectType LoadObject<ObjectType>();

	public static Base instance = new Base();

	private HashSet<Type> smarts = new HashSet<Type>();

	private SortedList<string, SortedList<string, Type>> interfaces = new SortedList<string, SortedList<string, Type>>();

	private object cacheLock = new object();

	private Dictionary<string, object> cached = new Dictionary<string, object>();

	private Dictionary<string, Type> creators = new Dictionary<string, Type>();

	private static List<string> dllVersions = new List<string>();

	private static List<string> dllValidities = new List<string>();

	private string TAG => GetType().FullName;

	public void Load()
	{
		smarts = LoadSmarts();
		interfaces = LoadDlls();
	}

	private HashSet<Type> LoadSmarts()
	{
		HashSet<Type> hashSet = new HashSet<Type>();
		Assembly assembly = typeof(IBase).Assembly;
		string filePath = Uri.UnescapeDataString(new UriBuilder(assembly.CodeBase).Path);
		if (!ValidityCheck(filePath))
		{
			throw new NotSupportedException("ISmart version not supported");
		}
		Type[] types = assembly.GetTypes();
		foreach (Type type in types)
		{
			if (type.IsInterface)
			{
				hashSet.Add(type);
			}
		}
		return hashSet;
	}

	private SortedList<string, SortedList<string, Type>> LoadDlls()
	{
		dllVersions.Clear();
		SortedList<string, SortedList<string, Type>> sortedList = new SortedList<string, SortedList<string, Type>>();
		foreach (string item2 in Directory.EnumerateFiles(".", "smart*.dll"))
		{
			if (!ValidityCheck(item2))
			{
				continue;
			}
			Assembly assembly = null;
			try
			{
				assembly = Assembly.LoadFrom(item2);
				assembly.GetTypes();
				string item = $"{assembly.FullName} version {assembly.GetName().Version.ToString()}";
				dllVersions.Add(item);
			}
			catch (Exception ex)
			{
				File.AppendAllLines(string.Format("CriticalErrorOnLaunch_{0}.txt", DateTime.Now.ToString("yyyyMM")), new string[1] { DateTime.Now.ToString("yyyyMMdd:HHmmss ") + "LoadDlls " + item2 + " error:" + ex.Message + Environment.NewLine + ex.StackTrace });
				continue;
			}
			string arg = assembly.GetName().Name.ToLowerInvariant();
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				string name = type.Name;
				if (type.IsAbstract)
				{
					continue;
				}
				Type[] array = type.GetInterfaces();
				foreach (Type type2 in array)
				{
					if (smarts.Contains(type2))
					{
						if (!sortedList.ContainsKey(type2.FullName.ToLowerInvariant()))
						{
							sortedList[type2.FullName.ToLowerInvariant()] = new SortedList<string, Type>();
						}
						string key = $"{arg}.{name}".ToLowerInvariant();
						sortedList[type2.FullName.ToLowerInvariant()][key] = type;
					}
				}
			}
		}
		return sortedList;
	}

	protected InterfaceType FindInterface<InterfaceType>(string typeName, bool create, bool skipCache, bool saveCache)
	{
		lock (cacheLock)
		{
			string text = typeof(InterfaceType).FullName.ToLowerInvariant();
			InterfaceType val = default(InterfaceType);
			bool flag = true;
			if (!skipCache && !create && cached.ContainsKey(text))
			{
				val = (InterfaceType)cached[text];
				flag = false;
			}
			if (flag)
			{
				Type type = null;
				if (!skipCache && creators.ContainsKey(text))
				{
					type = creators[text];
				}
				else
				{
					if (typeName == string.Empty)
					{
						throw new NotSupportedException($"Could not load interface: {text}");
					}
					SortedList<string, Type> sortedList = interfaces[text];
					if (typeName.StartsWith("*."))
					{
						bool flag2 = false;
						string value = typeName.Substring(1).ToLowerInvariant();
						foreach (Type value2 in sortedList.Values)
						{
							if (value2.FullName.ToLowerInvariant().EndsWith(value))
							{
								type = value2;
								flag2 = true;
							}
						}
						if (!flag2)
						{
							foreach (string key in sortedList.Keys)
							{
								if (key.EndsWith(value))
								{
									type = sortedList[key];
									flag2 = true;
								}
							}
						}
						if (!flag2)
						{
							throw new NotSupportedException($"Type pattern '{typeName}' not found in loaded types");
						}
					}
					else
					{
						if (!sortedList.ContainsKey(typeName.ToLowerInvariant()))
						{
							throw new NotSupportedException($"Could not find type {typeName} for interface: {text}");
						}
						type = sortedList[typeName.ToLowerInvariant()];
					}
				}
				val = (InterfaceType)Activator.CreateInstance(type);
				if (create && saveCache)
				{
					creators[text] = type;
				}
			}
			if (!create && saveCache)
			{
				cached[text] = val;
			}
			return val;
		}
	}

	public InterfaceType Load<InterfaceType>(string typeName)
	{
		return FindInterface<InterfaceType>(typeName, create: false, skipCache: false, saveCache: true);
	}

	public InterfaceType LoadCached<InterfaceType>()
	{
		return FindInterface<InterfaceType>(string.Empty, create: false, skipCache: false, saveCache: false);
	}

	public InterfaceType LoadNew<InterfaceType>(string typeName)
	{
		return FindInterface<InterfaceType>(typeName, create: true, skipCache: false, saveCache: false);
	}

	public bool ValidityCheck(string filePath)
	{
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
		DllValidity dllValidity = SafeCheck.ValidityCheckSafe(filePath);
		switch (dllValidity)
		{
		case DllValidity.NotFound:
			dllValidities.Add($"{fileNameWithoutExtension}: Not found at {filePath}");
			break;
		case DllValidity.NotAllowed:
			dllValidities.Add($"{fileNameWithoutExtension}: Not in allowed DLL list");
			break;
		case DllValidity.Invalid:
			dllValidities.Add($"{fileNameWithoutExtension}: Invalid signature");
			break;
		case DllValidity.Unrecognized:
			dllValidities.Add($"{fileNameWithoutExtension}: Unrecognized signature");
			break;
		case DllValidity.Valid:
			dllValidities.Add($"{fileNameWithoutExtension}: VALID");
			break;
		default:
			dllValidities.Add($"{fileNameWithoutExtension}: Unknown error");
			break;
		case DllValidity.Unknown:
			break;
		}
		return dllValidity == DllValidity.Valid;
	}

	public void PrintBase()
	{
		foreach (string dllVersion in dllVersions)
		{
			Smart.Log.Verbose(TAG, dllVersion);
		}
		foreach (string dllValidity in dllValidities)
		{
			Smart.Log.Verbose(TAG, dllValidity);
		}
		List<string> list = new List<string>();
		foreach (Type smart in smarts)
		{
			list.Add(smart.FullName);
		}
		string arg = string.Join(",", list);
		Smart.Log.Verbose(TAG, $"Smart types: {arg}");
		foreach (string key in interfaces.Keys)
		{
			list.Clear();
			foreach (Type value in interfaces[key].Values)
			{
				list.Add(value.FullName);
			}
			arg = string.Join(",", list);
			Smart.Log.Verbose(TAG, $"{key} types: {arg}");
		}
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
