using System.Collections.Generic;
using System.IO;
using ISmart;

namespace SmartDevice.Steps;

public class PullFile : BaseStep
{
	private string devicePath;

	private IDevice device;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_082b: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).LocalPath;
		if (string.IsNullOrEmpty(text))
		{
			text = Smart.File.TempFolder();
			base.Cache.Add("TempFolder", text);
		}
		else if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		int num = 20;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
		}
		devicePath = ((dynamic)base.Info.Args).DevicePath;
		Smart.Log.Debug(TAG, $"DeviceId: {device.ID} Local Path: {text} Device Path: {devicePath}");
		string filePathName = Smart.Rsd.GetFilePathName("adbExe", base.Recipe.Info.UseCase, device);
		string text2 = $"pull \"{devicePath}\" \"{text}\"";
		int num2 = default(int);
		List<string> list = Smart.MotoAndroid.Shell(device.ID, text2, num * 1000, filePathName, ref num2, 6000, false);
		if (string.Join("\r\n", list.ToArray()).ToLower().Contains("error") || num2 != 0)
		{
			result = (Result)1;
		}
		string text3 = "*.csv";
		if (((dynamic)base.Info.Args).SearchPattern != null)
		{
			text3 = ((dynamic)base.Info.Args).SearchPattern;
		}
		List<string> list2 = Smart.File.FindFiles(text3, text, true);
		if (list2.Count == 0)
		{
			Smart.Log.Error(TAG, "No results files found");
			result = (Result)1;
		}
		else
		{
			Smart.Log.Debug(TAG, $"{list2.Count} {text3} file(s) found");
		}
		bool flag = false;
		if (((dynamic)base.Info.Args).ClearLocal != null)
		{
			flag = ((dynamic)base.Info.Args).ClearLocal;
		}
		if (flag)
		{
			Directory.Delete(text, recursive: true);
		}
		LogResult(result);
	}
}
