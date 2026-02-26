using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ISmart;

namespace SmartDevice.Steps;

public class UnzipFile : UnisocBaseTest
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string[] source = ((string)((dynamic)base.Info.Args).FilePath).Split(new char[1] { ',' });
		string[] source2 = ((string)((dynamic)base.Info.Args).ZippedFileName).Split(new char[1] { ',' });
		string[] array = ((string)((dynamic)base.Info.Args).CacheName).Split(new char[1] { ',' });
		string[] array2 = ((string)((dynamic)base.Info.Args).LookupSpecifiedFileSaveToCache).Split(new char[1] { ',' });
		Smart.Log.Verbose(TAG, $"Start to build file list...");
		List<string> list = new List<string>();
		list = BuildFileList(source.ToList(), source2.ToList(), device);
		for (int i = 0; i < list.Count; i++)
		{
			string text = list[i];
			string text2 = Path.GetDirectoryName(text) + "\\" + Path.GetFileNameWithoutExtension(text);
			Smart.Log.Verbose(TAG, $"Try to unzip {text} to {text2}");
			string extension = Path.GetExtension(text);
			if (extension.EndsWith(".gz", StringComparison.OrdinalIgnoreCase))
			{
				Smart.Zip.GZipExtract(text, text2);
			}
			else
			{
				if (!extension.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
				{
					string text3 = $"Not support to unzip {extension}";
					Smart.Log.Error(TAG, text3);
					throw new Exception(text3);
				}
				Smart.Zip.Extract(text, text2);
			}
			if (array2[i].Trim().Length > 0)
			{
				string text4 = LookUpSpecifiedFilePath(text2, array2[i]);
				if (!File.Exists(text4))
				{
					string text5 = $"File {array2[i]} not found under unzip location {text2}";
					Smart.Log.Error(TAG, text5);
					throw new Exception(text5);
				}
				string text6 = $"Found required file {text4} under unzip location {text2}, save the full path to cache";
				Smart.Log.Verbose(TAG, text6);
				base.Cache[array[i]] = text4;
			}
			else
			{
				base.Cache[array[i]] = text2;
			}
		}
		SetPreCondition(((object)(Result)(ref result)).ToString());
		VerifyOnly(ref result);
		LogResult(result);
	}

	private string LookUpSpecifiedFilePath(string destPath, string searchPattern)
	{
		return SearchFileRecursive(destPath, searchPattern);
	}

	private string SearchFileRecursive(string rootPath, string fileName)
	{
		if (!Directory.Exists(rootPath))
		{
			throw new DirectoryNotFoundException($"The path {rootPath} is not a valid directory.");
		}
		string text = SearchFile(rootPath, fileName);
		if (text == null)
		{
			DirectoryInfo[] directories = new DirectoryInfo(rootPath).GetDirectories();
			foreach (DirectoryInfo directoryInfo in directories)
			{
				text = SearchFileRecursive(directoryInfo.FullName, fileName);
				if (text != null)
				{
					break;
				}
			}
		}
		return text;
	}

	private static string SearchFile(string path, string fileName)
	{
		string[] files = Directory.GetFiles(path, fileName);
		if (files.Length == 0)
		{
			return null;
		}
		return files[0];
	}
}
