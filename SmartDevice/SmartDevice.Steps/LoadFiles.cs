using System.IO;
using ISmart;

namespace SmartDevice.Steps;

public class LoadFiles : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Expected O, but got Unknown
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_096a: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (((dynamic)base.Info.Args).Files != null)
		{
			foreach (object item in ((dynamic)base.Info.Args).Files)
			{
				string text = (string)(dynamic)item;
				string empty = string.Empty;
				string filePathName = Smart.Rsd.GetFilePathName(text, base.Recipe.Info.UseCase, val, new Progress(base.ProgressUpdate), ref empty);
				Smart.Log.Debug(TAG, $"file symbol: {text} maps to path: {filePathName}");
				bool flag = text.ToLowerInvariant() == "xmlFile".ToLowerInvariant();
				bool flag2 = text.ToLowerInvariant() == "apexfile".ToLowerInvariant();
				bool flag3 = false;
				if (flag && filePathName.Contains("\\"))
				{
					string text2 = filePathName.Substring(0, filePathName.LastIndexOf("\\")) + ".zip";
					string text3 = text2 + ".dl";
					flag3 = Smart.File.Exists(text2) || Smart.File.Exists(text3);
				}
				if (flag2)
				{
					base.Cache[text] = filePathName;
					base.Cache["apexfilesha1"] = empty;
					empty = string.Empty;
					continue;
				}
				if (!Smart.Rsd.FileExists(filePathName))
				{
					Smart.Log.Error(TAG, $"Symbol: {text} => file: {filePathName} does not exist");
					if (flag3)
					{
						LogResult((Result)1, "Please wait for firmware download to finish", $"DL files found for {filePathName}");
						return;
					}
					LogResult((Result)1, "Requested file not found", $"Symbol: {text} => file: {filePathName} does not exist");
					string directoryName = Path.GetDirectoryName(filePathName);
					bool flag4 = Smart.File.Exists(directoryName);
					if (flag && flag4)
					{
						Smart.Log.Info(TAG, $"Deleting corrupt firmware directory '{directoryName}'");
						Smart.File.Remove(directoryName);
					}
					return;
				}
				if (flag && Smart.Rsd.FileSize(filePathName) < 1)
				{
					LogResult((Result)1, "Flash file is empty", $"No file content for {filePathName}");
					string directoryName2 = Path.GetDirectoryName(filePathName);
					if (Smart.File.Exists(directoryName2))
					{
						Smart.Log.Info(TAG, $"Deleting corrupt firmware directory '{directoryName2}'");
						Smart.File.Remove(directoryName2);
					}
					return;
				}
				if (empty == string.Empty)
				{
					base.Cache[text] = filePathName;
				}
				else
				{
					base.Cache[text] = empty;
				}
			}
		}
		string text4 = string.Empty;
		bool flag5 = default(bool);
		if (((dynamic)base.Info.Args).Values != null)
		{
			foreach (object item2 in ((dynamic)base.Info.Args).Values)
			{
				string text5 = (string)(dynamic)item2;
				string value = Smart.Rsd.GetValue(text5, base.Recipe.Info.UseCase, val, ref flag5, false);
				if (!flag5)
				{
					LogResult((Result)1, "Requested value not found", $"Value of {text5} does not exist");
					return;
				}
				base.Cache[text5] = value;
				text4 += $"{value},";
			}
			SetPreCondition(text4.Substring(0, text4.Length - 1));
		}
		if (((dynamic)base.Info.Args).LatestValues != null)
		{
			foreach (object item3 in ((dynamic)base.Info.Args).LatestValues)
			{
				string text6 = (string)(dynamic)item3;
				string value = Smart.Rsd.GetValue(text6, base.Recipe.Info.UseCase, val, ref flag5, true);
				if (!flag5)
				{
					LogResult((Result)1, "Requested latest value not found", $"Latest value of {text6} does not exist");
					return;
				}
				base.Cache[text6] = value;
				text4 += $"{value},";
			}
			SetPreCondition(text4.Substring(0, text4.Length - 1));
		}
		LogPass();
	}
}
