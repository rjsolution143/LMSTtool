using System;
using ISmart;

namespace SmartDevice.Steps;

public class TestCommandFSAC : TestCommandStep
{
	public enum FSAAction
	{
		Open = 0,
		Read = 1,
		Write = 2,
		Close = 4,
		Delete = 5,
		SearchDelete = 13,
		None = 65535
	}

	public enum FSAProcessor
	{
		BP,
		AP
	}

	private string TAG => GetType().FullName;

	public FSAAction Action { get; set; } = FSAAction.None;


	public string WriteData { get; set; } = string.Empty;


	public int NumberOfBytes { get; set; }

	public string FileName { get; set; } = string.Empty;


	public Result TestResult { get; protected set; } = (Result)8;


	public bool ResultLogged { get; set; } = true;


	public override void Run()
	{
		//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ff: Invalid comparison between Unknown and I4
		//IL_0c92: Unknown result type (might be due to invalid IL or missing references)
		string text = "004A";
		if (((dynamic)base.Info.Args).OpCode != null)
		{
			text = ((dynamic)base.Info.Args).OpCode;
		}
		FSAAction fSAAction = ((!ResultLogged) ? Action : ((FSAAction)Enum.Parse(typeof(FSAAction), (string)((dynamic)base.Info.Args).Action, ignoreCase: true)));
		FileName = ((((dynamic)base.Info.Args).FileName != null) ? ((dynamic)base.Info.Args).FileName : string.Empty);
		FSAProcessor processor = ((!((((dynamic)base.Info.Args).Processor != null) ? true : false)) ? FSAProcessor.AP : ((FSAProcessor)Enum.Parse(typeof(FSAProcessor), (string)((dynamic)base.Info.Args).Processor, ignoreCase: true)));
		int num = ((!ResultLogged) ? NumberOfBytes : ((int)((((dynamic)base.Info.Args).NumberOfBytes != null) ? ((dynamic)base.Info.Args).NumberOfBytes : ((object)0))));
		string text2 = BuildData(writeData: (!ResultLogged) ? WriteData : ((string)((((dynamic)base.Info.Args).WriteData != null) ? ((dynamic)base.Info.Args).WriteData : string.Empty)), action: fSAAction, processor: processor, fileName: FileName, numberOfBytes: num);
		ITestCommandResponse val = base.tcmd.SendCommand(text, text2);
		TestResult = (Result)(val.Failed ? 1 : 8);
		if ((int)TestResult == 8)
		{
			switch (fSAAction)
			{
			case FSAAction.Read:
				if (val.DataHex.Length / 2 < num)
				{
					Smart.Log.Error(TAG, $"Expected {num} bytes, but only {val.DataHex.Length / 2} bytes returned");
					TestResult = (Result)1;
					break;
				}
				base.Cache.Add("FSACReadHexResponse", val.DataHex);
				Smart.Log.Debug(TAG, "FSACReadHexResponse: " + val.DataHex);
				if (((dynamic)base.Info.Args).SetPreCond != null && (bool)((dynamic)base.Info.Args).SetPreCond)
				{
					SetPreCondition(val.DataHex);
				}
				break;
			case FSAAction.Open:
				if (!string.IsNullOrEmpty(val.DataHex.Trim()))
				{
					Smart.Log.Error(TAG, $"Open file fail");
					TestResult = (Result)1;
				}
				break;
			case FSAAction.Write:
				if (!string.IsNullOrEmpty(val.DataHex.Trim()))
				{
					Smart.Log.Error(TAG, $"Write file fail");
					TestResult = (Result)1;
				}
				break;
			case FSAAction.Close:
				if (!string.IsNullOrEmpty(val.DataHex.Trim()))
				{
					Smart.Log.Error(TAG, $"Close file fail");
					TestResult = (Result)1;
				}
				break;
			}
		}
		LogResult(TestResult, ResultLogged);
	}

	private string BuildData(FSAAction action, FSAProcessor processor, string fileName, int numberOfBytes, string writeData)
	{
		string result = string.Empty;
		string text = Smart.Convert.AsciiStringToHexString(fileName);
		string text2 = "00000000";
		switch (action)
		{
		case FSAAction.Open:
		{
			string[] array = new string[5];
			int num = (int)processor;
			array[0] = num.ToString("X4");
			num = (int)action;
			array[1] = num.ToString("X4");
			array[2] = text2;
			array[3] = text;
			array[4] = "0000";
			result = string.Concat(array);
			break;
		}
		case FSAAction.Read:
		{
			int num = (int)processor;
			string text8 = num.ToString("X4");
			num = (int)action;
			result = text8 + num.ToString("X4") + numberOfBytes.ToString("X8");
			break;
		}
		case FSAAction.Write:
		{
			int num = (int)processor;
			string text7 = num.ToString("X4");
			num = (int)action;
			result = text7 + num.ToString("X4") + numberOfBytes.ToString("X8") + writeData;
			break;
		}
		case FSAAction.Close:
		{
			int num = (int)processor;
			string text6 = num.ToString("X4");
			num = (int)action;
			result = text6 + num.ToString("X4");
			break;
		}
		case FSAAction.Delete:
		{
			int num = (int)processor;
			string text5 = num.ToString("X4");
			num = (int)action;
			result = text5 + num.ToString("X4") + text + "0000";
			break;
		}
		case FSAAction.SearchDelete:
		{
			string text3 = "0002" + Smart.Convert.AsciiStringToUnicodeHexString(fileName, (ByteOrder)1);
			int num = (int)processor;
			string text4 = num.ToString("X4");
			num = (int)action;
			result = text4 + num.ToString("X4") + text3 + "0000";
			break;
		}
		default:
			Smart.Log.Error(TAG, "Unknown FSAAction " + action);
			break;
		}
		return result;
	}
}
