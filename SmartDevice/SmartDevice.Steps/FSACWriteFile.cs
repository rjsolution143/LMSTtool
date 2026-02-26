using System.IO;
using ISmart;

namespace SmartDevice.Steps;

public class FSACWriteFile : TestCommandFSAC
{
	private int loopCounter;

	public string SourceFileName { get; set; } = string.Empty;


	public new bool ResultLogged { get; set; } = true;


	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Invalid comparison between Unknown and I4
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Invalid comparison between Unknown and I4
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Invalid comparison between Unknown and I4
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Invalid comparison between Unknown and I4
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		string text = ((!ResultLogged) ? SourceFileName : ((string)((dynamic)base.Info.Args).SourceFileName));
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		int num = 1024;
		if (((dynamic)base.Info.Args).PacketSize != null)
		{
			num = ((dynamic)base.Info.Args).PacketSize;
		}
		Smart.Log.Debug(TAG, "sourceFile: " + text);
		if (!File.Exists(text) || new FileInfo(text).Length == 0L)
		{
			Smart.Log.Debug(TAG, $"File {text} is empty. No file write needed");
			LogResult((Result)7, ResultLogged);
			return;
		}
		base.ResultLogged = false;
		base.Action = FSAAction.Open;
		base.Run();
		if ((int)base.TestResult != 8)
		{
			Smart.Log.Error(TAG, string.Format("Failed to open file {0}", ((dynamic)base.Info.Args).FileName));
			LogResult(base.TestResult, ResultLogged);
			return;
		}
		byte[] array = Smart.Rsd.FileReadAllBytes(text);
		string text2 = Smart.Convert.AsciiBytesToHexString(array, array.Length);
		int num2 = 0;
		int num3 = 0;
		base.Action = FSAAction.Write;
		do
		{
			num3 = (base.NumberOfBytes = ((text2.Length - num2 > num * 2) ? num : ((text2.Length - num2) / 2)));
			base.WriteData = text2.Substring(num2, num3 * 2);
			base.Run();
			num2 += num3 * 2;
		}
		while (num2 < text2.Length && (int)base.TestResult == 8);
		if ((int)base.TestResult != 8)
		{
			Smart.Log.Error(TAG, string.Format("Failed to write {0} bytes to file {1}", text2.Length, ((dynamic)base.Info.Args).FileName));
			LogResult(base.TestResult, ResultLogged);
			return;
		}
		base.Action = FSAAction.Close;
		base.Run();
		if ((int)base.TestResult != 8)
		{
			Smart.Log.Error(TAG, string.Format("Failed to close file {0}", ((dynamic)base.Info.Args).FileName));
		}
		LogResult(base.TestResult, ResultLogged);
		if (((dynamic)base.Info.Args).LoopCount != null && loopCounter < (int)((dynamic)base.Info.Args).LoopCount)
		{
			((dynamic)base.Info.Args).Retesting = true;
			loopCounter++;
			Smart.Log.Error(TAG, $"Loop {loopCounter} test.");
		}
	}
}
