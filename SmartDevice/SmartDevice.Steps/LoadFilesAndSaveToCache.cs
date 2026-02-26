using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ISmart;

namespace SmartDevice.Steps;

public class LoadFilesAndSaveToCache : UnisocBaseTest
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0949: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd7: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string[] array = ((string)((dynamic)base.Info.Args).CacheNames).Split(new char[1] { ',' });
		bool flag = false;
		if (((dynamic)base.Info.Args).ManuallyLoadFile != null)
		{
			flag = (bool)((dynamic)base.Info.Args).ManuallyLoadFile;
		}
		if (flag)
		{
			bool flag2 = false;
			if (((dynamic)base.Info.Args).SkipManuallyLoadIfFileAlreadyInCache != null)
			{
				flag2 = (bool)((dynamic)base.Info.Args).SkipManuallyLoadIfFileAlreadyInCache;
			}
			if (flag2 && base.Cache.ContainsKey(array[0]))
			{
				string text = string.Format("Cache already contain {0} with value {1}, skip manually load", array[0], base.Cache[array[0]]);
				Smart.Log.Warning(TAG, text);
				LogResult((Result)8);
				return;
			}
			string text2 = "Please select File";
			if (((dynamic)base.Info.Args).PromptTitle != null)
			{
				text2 = ((dynamic)base.Info.Args).PromptTitle.ToString();
			}
			string text3 = "All Files|*.*";
			if (((dynamic)base.Info.Args).FileFilter != null)
			{
				text3 = ((dynamic)base.Info.Args).FileFilter.ToString();
			}
			string text4 = val.Prompt.SlectFile(text2, Environment.GetFolderPath(Environment.SpecialFolder.Desktop), text3);
			if (File.Exists(text4))
			{
				base.Cache[array[0]] = text4;
				result = (Result)8;
				if (((dynamic)base.Info.Args).SetFileNameAsPreCondVal != null && (bool)((dynamic)base.Info.Args).SetFileNameAsPreCondVal)
				{
					SetPreCondition(text4.ToString());
				}
				else
				{
					SetPreCondition(((object)(Result)(ref result)).ToString());
				}
			}
			else
			{
				string text5 = $"User select file {text4} not exist";
				Smart.Log.Error(TAG, text5);
				result = (Result)1;
				SetPreCondition(((object)(Result)(ref result)).ToString());
				VerifyOnly(ref result);
			}
			LogResult(result);
			return;
		}
		List<string> sourcePaths = ((string)((dynamic)base.Info.Args).FilePathList).Split(new char[1] { ',' }).ToList();
		List<string> sourceFiles = ((string)((dynamic)base.Info.Args).FileNameList).Split(new char[1] { ',' }).ToList();
		List<string> list = BuildFileList(sourcePaths, sourceFiles, val);
		if (list.Count != array.Length)
		{
			string text6 = $"Total {list.Count} files found, not equal to cacheName count {array.Length}";
			Smart.Log.Verbose(TAG, text6);
			throw new Exception(text6);
		}
		for (int i = 0; i < list.Count; i++)
		{
			string text7 = list[i];
			string text8 = array[i];
			Smart.Log.Verbose(TAG, $"Save value {text7} to Cache {text8}");
			base.Cache[text8] = text7;
			if (!File.Exists(text7))
			{
				string text9 = "File to be loaded " + text7 + " does not exist!";
				Smart.Log.Error(TAG, text9);
				bool flag3 = true;
				if (((dynamic)base.Info.Args).ThrowExceptionIfFileNotExist != null)
				{
					flag3 = (bool)((dynamic)base.Info.Args).ThrowExceptionIfFileNotExist;
				}
				if (flag3)
				{
					result = (Result)1;
					LogResult(result, text9, text9);
					throw new Exception(text9);
				}
			}
		}
		SetPreCondition(((object)(Result)(ref result)).ToString());
		VerifyOnly(ref result);
		LogResult(result);
	}
}
