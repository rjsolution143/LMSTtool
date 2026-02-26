using System.IO;
using ISmart;

namespace SmartDevice.Steps;

public class PushDirectory : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).LocalPath;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		string path = ((dynamic)base.Info.Args).DevicePath;
		string[] files = Directory.GetFiles(text, "*.zip");
		foreach (string text2 in files)
		{
			string fileName = Path.GetFileName(text2);
			Smart.ADB.PushFile(val.ID, text2, Path.Combine(path, fileName));
		}
		LogPass();
	}
}
