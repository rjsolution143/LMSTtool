using System;
using System.IO;
using System.Text;
using ISmart;

namespace SmartDevice.Steps;

public class PrintLabelSaveDataLocally : BaseStep
{
	private static readonly object sGenericLock = new object();

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		_ = string.Empty;
		_ = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = string.Empty;
		if (((dynamic)base.Info.Args).DataToSave != null)
		{
			text = ((dynamic)base.Info.Args).DataToSave.ToString();
		}
		string text2 = string.Empty;
		string[] array = text.Split(new char[1] { ',' });
		foreach (string text3 in array)
		{
			string text4 = text3;
			if (text3.Contains("LoginAccount"))
			{
				Login login = Smart.Rsd.Login;
				text4 = ((Login)(ref login)).UserName;
			}
			else if (text3.Contains("datetime_"))
			{
				string text5 = text3.Substring(text3.IndexOf("_") + 1);
				text4 = DateTime.Now.ToString(text5);
			}
			else if (text3.StartsWith("$"))
			{
				text4 = ((!base.Cache.Keys.Contains(text3.Substring(1))) ? "" : ((string)base.Cache[text3.Substring(1)]));
			}
			text2 = text2 + "," + text4;
		}
		text2 = text2.Substring(1);
		Smart.Log.Verbose(TAG, "Data to be saved:" + text2);
		string text6 = Smart.File.CommonStorageDir;
		if (((dynamic)base.Info.Args).LogFileFolder != null)
		{
			text6 = ((dynamic)base.Info.Args).LogFileFolder;
			if (text6.StartsWith("$"))
			{
				text6 = base.Cache[text6.Substring(1)];
			}
		}
		string text7 = "GiftBoxPrint_Record";
		if (((dynamic)base.Info.Args).LogFilePrefix != null)
		{
			text7 = ((dynamic)base.Info.Args).LogFilePrefix;
		}
		string text8 = text7 + "_" + DateTime.Now.ToString("yyyyMM") + ".csv";
		string path = text6 + "\\" + text8;
		lock (sGenericLock)
		{
			bool flag = true;
			if (File.Exists(path))
			{
				flag = false;
			}
			using StreamWriter streamWriter = new StreamWriter(path, append: true, Encoding.UTF8);
			if (flag)
			{
				string value = "Model,IMEI,IMEI2,IMEID,Old IMEI,PN,Account,Date Time";
				if (((dynamic)base.Info.Args).Header != null)
				{
					value = ((dynamic)base.Info.Args).Header;
				}
				streamWriter.WriteLine(value);
			}
			streamWriter.WriteLine(text2);
		}
		LogResult((Result)8);
	}
}
