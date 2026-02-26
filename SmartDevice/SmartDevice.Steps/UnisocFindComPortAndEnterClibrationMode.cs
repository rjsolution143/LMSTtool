using System;
using System.Collections.Generic;
using System.IO;
using ISmart;

namespace SmartDevice.Steps;

[Obsolete]
public class UnisocFindComPortAndEnterClibrationMode : UnisocBaseTest
{
	private string strToIndicateTestPass = "!!!!! All Finished, pass !!!!!";

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Invalid comparison between Unknown and I4
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Invalid comparison between Unknown and I4
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Invalid comparison between Unknown and I4
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		Result val = (Result)1;
		string text = string.Empty;
		_ = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		UnisocFindComPort unisocFindComPort = new UnisocFindComPort();
		unisocFindComPort.Load(base.Recipe, base.Info);
		unisocFindComPort.Run();
		val = unisocFindComPort.ActualTestResult;
		if ((int)val == 8)
		{
			Smart.Log.Info(TAG, "Find com port, start to enter calibration mode...");
			UnisocEnterClibrationMode unisocEnterClibrationMode = new UnisocEnterClibrationMode();
			unisocEnterClibrationMode.Load(base.Recipe, base.Info);
			unisocEnterClibrationMode.Run();
			val = unisocEnterClibrationMode.ActualTestResult;
			if ((int)val == 1)
			{
				text = "Enter Mode Fail...";
				Smart.Log.Error(TAG, text);
			}
		}
		else
		{
			text = "Not Find com port...";
			Smart.Log.Error(TAG, text);
		}
		ActualTestResult = val;
		VerifyOnly(ref val);
		if ((int)val != 8)
		{
			string tAG = TAG;
			LogResult(val, tAG, text);
		}
		else
		{
			LogResult(val);
		}
	}

	private Result EnterCalibrationMode(List<string> responses)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a5: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)1;
		string empty = string.Empty;
		string exe = ((dynamic)base.Info.Args).EXE;
		if (exe.StartsWith("$"))
		{
			string key = exe.Substring(1);
			exe = base.Cache[key];
		}
		Smart.Log.Verbose(TAG, $"Exe to be used: {exe}");
		string directoryName = Path.GetDirectoryName(exe);
		string path = Path.Combine(directoryName, "App");
		if (!exe.EndsWith("FrameworkDemo.exe"))
		{
			exe = Path.Combine(path, "FrameworkDemo.exe");
		}
		Smart.Log.Verbose(TAG, $"Final exe to be used: {exe}");
		string command = ((dynamic)base.Info.Args).Command;
		if (((dynamic)base.Info.Args).Format != null)
		{
			List<string> list = new List<string>();
			foreach (object item in ((dynamic)base.Info.Args).Format)
			{
				string text = (string)(dynamic)item;
				string text2 = text;
				if (text.StartsWith("$"))
				{
					string text3 = text.Substring(1);
					text2 = base.Cache[text3];
					text3.ToLower();
				}
				if (text.EndsWith(".seq", StringComparison.OrdinalIgnoreCase))
				{
					empty = text;
					text2 = text;
					if (!Path.IsPathRooted(empty))
					{
						text2 = Path.Combine(Path.Combine(directoryName, "Project"), empty);
						if (!File.Exists(text2))
						{
							string text4 = $"Seq file not exist {text2}.";
							Smart.Log.Error(TAG, text4);
							throw new Exception(text4);
						}
					}
				}
				list.Add(text2);
			}
			command = string.Format(command, list.ToArray());
		}
		Smart.Log.Verbose(TAG, $"Shell command to be used: {exe} {command}");
		int timeout = 15;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			timeout = ((dynamic)base.Info.Args).Timeout;
		}
		Smart.Thread.RunAndWait<int>((Func<int>)(() => ExecuteShell(exe, command, timeout)));
		responses.AddRange(error);
		responses.AddRange(output);
		foreach (string response in responses)
		{
			Smart.Log.Verbose(TAG, response);
		}
		result = ((!string.Join(" ", responses.ToArray()).Contains(strToIndicateTestPass)) ? ((Result)1) : ((Result)8));
		Smart.Log.Info(TAG, "Cmd line tool execute result:" + ((object)(Result)(ref result)).ToString());
		return result;
	}
}
