using System.Collections.Generic;
using System.IO;
using ISmart;

namespace SmartDevice.Steps;

public class VerifyTestResultInCsvFile : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		string text = ((dynamic)base.Info.Args).LocalPath;
		if (string.IsNullOrEmpty(text))
		{
			text = base.Cache["TempFolder"];
			flag = true;
		}
		else if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		Smart.Log.Debug(TAG, "Local path is " + text);
		List<string> list = Smart.File.FindFiles("*.csv", text, true);
		if (list.Count == 0)
		{
			Smart.Log.Error(TAG, "No results files found");
			throw new FileNotFoundException("No results file found");
		}
		Result result = (Result)8;
		Smart.Log.Debug(TAG, $"{list.Count} csv file(s) found");
		Smart.Log.Debug(TAG, $"First csv file result {list[0]}");
		ICsvFile val = Smart.NewCsvFile();
		val.LoadFile(list[0], ',');
		int index = 0;
		if (((dynamic)base.Info.Args).ResultRow != null)
		{
			index = ((dynamic)base.Info.Args).ResultRow;
		}
		int index2 = 0;
		if (((dynamic)base.Info.Args).ResultColumn != null)
		{
			index2 = ((dynamic)base.Info.Args).ResultColumn;
		}
		string text2 = "Fail";
		if (((dynamic)base.Info.Args).ResultFail != null)
		{
			text2 = ((dynamic)base.Info.Args).ResultFail;
		}
		string text3 = val.Rows[index][index2];
		Smart.Log.Debug(TAG, $"Test result read from csv file is {text3}");
		if (text3 == text2)
		{
			result = (Result)1;
			string text4 = string.Join(",", val.Headers.ToArray());
			string text5 = string.Join(",", val.Rows[index].ToArray());
			Smart.Log.Debug(TAG, text4);
			Smart.Log.Debug(TAG, text5);
			string text6 = Smart.File.ReadText(list[0]);
			Smart.Log.Verbose(TAG, text6);
		}
		if (flag)
		{
			Directory.Delete(text, recursive: true);
		}
		SetPreCondition(text3);
		LogResult(result);
	}
}
