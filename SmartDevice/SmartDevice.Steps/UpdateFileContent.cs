using System.Collections.Generic;
using System.IO;
using System.Linq;
using ISmart;

namespace SmartDevice.Steps;

public class UpdateFileContent : UnisocBaseTest
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		List<string> list = new List<string>();
		string[] source = ((string)((dynamic)base.Info.Args).FilePathList).Split(new char[1] { ',' });
		string[] source2 = ((string)((dynamic)base.Info.Args).FileNameList).Split(new char[1] { ',' });
		Smart.Log.Verbose(TAG, $"Start to build file list to be updated...");
		list = BuildFileList(source.ToList(), source2.ToList(), device);
		string[] array = ((string)((dynamic)base.Info.Args).PlaceHolderInFile).Split(new char[1] { ',' });
		string[] array2 = ((string)((dynamic)base.Info.Args).NewValueToReplaceHolder).Split(new char[1] { ',' });
		for (int i = 0; i < list.Count; i++)
		{
			string text = list[i];
			string text2 = File.ReadAllText(text);
			Smart.Log.Verbose(TAG, string.Format("Update file:" + text));
			string[] array3 = array[i].Split(new char[1] { '#' });
			string[] array4 = array2[i].Split(new char[1] { '#' });
			for (int j = 0; j < array3.Length; j++)
			{
				string text3 = array3[j];
				string text4 = array4[j];
				if (text4.StartsWith("$"))
				{
					text4 = base.Cache[text4.Substring(1)];
				}
				Smart.Log.Verbose(TAG, $"Replace {text3} with {text4}");
				text2 = text2.Replace(text3, text4);
			}
			File.WriteAllText(text, text2);
		}
		VerifyOnly(ref result);
		LogResult(result);
	}
}
