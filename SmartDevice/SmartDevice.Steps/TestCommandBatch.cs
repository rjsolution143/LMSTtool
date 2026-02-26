using System;
using ISmart;

namespace SmartDevice.Steps;

public class TestCommandBatch : TestCommandStep
{
	private enum BatchAction : ushort
	{
		Start,
		Stop,
		GetState,
		GetVersionInfo,
		GetFileDescriptor
	}

	private enum BatchLogAction : ushort
	{
		NoLogging = 0,
		LogResponses = 1,
		LogErrors = 2,
		LogCommands = 4
	}

	private enum BatchState : ushort
	{
		ReadyMode = 0,
		BatchActive = 1,
		BatchHalted = 2,
		InvalidState = ushort.MaxValue
	}

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0817: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_081c: Invalid comparison between Unknown and I4
		//IL_085f: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		string text = "0C2F";
		if (((dynamic)base.Info.Args).OpCode != null)
		{
			text = ((dynamic)base.Info.Args).OpCode;
		}
		string fileName = ((((dynamic)base.Info.Args).FileName != null) ? ((dynamic)base.Info.Args).FileName : string.Empty);
		BatchAction batchAction = ((((dynamic)base.Info.Args).Action != null) ? ((BatchAction)Enum.Parse(typeof(BatchAction), (string)((dynamic)base.Info.Args).Action, ignoreCase: true)) : BatchAction.Start);
		BatchLogAction logAction = ((((dynamic)base.Info.Args).Logging != null) ? ((BatchLogAction)Enum.Parse(typeof(BatchLogAction), (string)((dynamic)base.Info.Args).Logging, ignoreCase: true)) : BatchLogAction.NoLogging);
		_ = (bool)((((dynamic)base.Info.Args).SkipUnsolictedResponse != null) ? ((dynamic)base.Info.Args).SkipUnsolictedResponse : ((object)true));
		string text2 = BuildData(batchAction, logAction, 1, fileName, string.Empty);
		ITestCommandResponse val = base.tcmd.SendCommand(text, text2);
		Result val2 = (Result)(val.Failed ? 1 : 8);
		if ((int)val2 == 8 && batchAction == BatchAction.Start)
		{
			string text3 = "0000";
			if (val.DataHex != text3)
			{
				Smart.Log.Error(TAG, $"Expected return value {text3}, found value {val.DataHex}");
				val2 = (Result)1;
			}
		}
		LogResult(val2);
	}

	private string BuildData(BatchAction action, BatchLogAction logAction, int repeatCount, string fileName, string variableData)
	{
		string result = string.Empty;
		string text = Smart.Convert.AsciiStringToUnicodeHexString(fileName, (ByteOrder)1);
		switch (action)
		{
		case BatchAction.Start:
		{
			string[] array = new string[6];
			int num = (int)action;
			array[0] = num.ToString("X4");
			num = (int)logAction;
			array[1] = num.ToString("X4");
			array[2] = repeatCount.ToString("X4");
			array[3] = text;
			array[4] = "0000";
			array[5] = variableData;
			result = string.Concat(array);
			break;
		}
		case BatchAction.Stop:
		case BatchAction.GetState:
		{
			int num = (int)action;
			result = num.ToString("X4");
			break;
		}
		case BatchAction.GetVersionInfo:
			if (string.IsNullOrEmpty(fileName))
			{
				int num = (int)action;
				result = num.ToString("X4");
			}
			else
			{
				int num = (int)action;
				result = num.ToString("X4") + text + "0000";
			}
			break;
		case BatchAction.GetFileDescriptor:
		{
			int num = (int)action;
			result = num.ToString("X4") + text + "0000";
			break;
		}
		}
		return result;
	}
}
