using System.IO;
using ISmart;

namespace SmartDevice.Steps;

public class CountryCodeFileSetup : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0632: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		string text = ((dynamic)base.Info.Args).EXE;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		string text2 = ((dynamic)base.Info.Args).File;
		if (text2.StartsWith("$"))
		{
			string key2 = text2.Substring(1);
			text2 = base.Cache[key2];
		}
		string directoryName = Path.GetDirectoryName(text);
		string directoryName2 = Path.GetDirectoryName(text2);
		string empty = string.Empty;
		string[] files = Directory.GetFiles(directoryName2, "AP*");
		if (files.Length != 0)
		{
			empty = files[0];
			Smart.Log.Debug(TAG, $"APDB file: {empty}");
			string fileName = Path.GetFileName(empty);
			string path = Path.Combine(directoryName, "APDB");
			path = Path.Combine(path, fileName);
			File.Copy(empty, path, overwrite: true);
		}
		else
		{
			result = (Result)1;
			Smart.Log.Error(TAG, $"APDB file doies not exist in {directoryName2}");
		}
		string empty2 = string.Empty;
		files = Directory.GetFiles(directoryName2, "*_lwg*");
		if (files.Length != 0)
		{
			empty2 = files[0];
			Smart.Log.Debug(TAG, $"BPDB file: {empty2}");
			string fileName2 = Path.GetFileName(empty2);
			string path2 = Path.Combine(directoryName, "MDDB");
			path2 = Path.Combine(path2, fileName2);
			File.Copy(empty2, path2, overwrite: true);
		}
		else
		{
			result = (Result)1;
			Smart.Log.Error(TAG, $"BPDB file doies not exist in {directoryName2}");
		}
		string text3 = ((dynamic)base.Info.Args).RoCarrier;
		if (text3.StartsWith("$"))
		{
			string key3 = text3.Substring(1);
			text3 = base.Cache[key3];
		}
		string text4 = ((dynamic)base.Info.Args).DefaultLanguage;
		if (text4.StartsWith("$"))
		{
			string key4 = text4.Substring(1);
			text4 = base.Cache[key4];
		}
		string text5 = ((dynamic)base.Info.Args).CountryCode;
		if (text5.StartsWith("$"))
		{
			string key5 = text5.Substring(1);
			text5 = base.Cache[key5];
		}
		string contents = $"ro.carrier={text3}\r\ndefaultlanguage={text4}\r\ncountrycode={text5}";
		File.WriteAllText(Path.Combine(Path.Combine(directoryName, "Data"), "Datacode.txt"), contents);
		LogResult(result);
	}
}
