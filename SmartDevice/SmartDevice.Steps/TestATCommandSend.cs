using System;
using ISmart;

namespace SmartDevice.Steps;

public class TestATCommandSend : TestCommandStep
{
	private string mOpCode = string.Empty;

	private string mHexData = string.Empty;

	private string mRespString;

	private bool mLogResult = true;

	private Result mResult = (Result)8;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(mOpCode))
		{
			mOpCode = ((dynamic)base.Info.Args).OpCode;
		}
		if (string.IsNullOrEmpty(mHexData))
		{
			if (((dynamic)base.Info.Args).AtCommandHex != null)
			{
				mHexData = ((dynamic)base.Info.Args).AtCommandHex;
			}
			else
			{
				if (!((((dynamic)base.Info.Args).AtCommand != null) ? true : false))
				{
					throw new Exception("No AtCommand string and AtCommandHex are specifified in recipe");
				}
				string text = ((dynamic)base.Info.Args).AtCommand;
				mHexData = Smart.Convert.AtToTCMD(text);
			}
		}
		ITestCommandResponse val = base.tcmd.SendCommand(mOpCode, mHexData);
		mRespString = Smart.Convert.BytesToAscii(val.Data);
		Smart.Log.Debug(TAG, $"AT command returned response {mRespString}");
		if (mRespString.IndexOf("OK") < 0)
		{
			mResult = (Result)1;
		}
		if (mLogResult)
		{
			LogResult(mResult);
		}
	}

	protected Result SendWithHexData(string opCode, string atCmdHex, out string response)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		mLogResult = false;
		mOpCode = opCode;
		mHexData = atCmdHex;
		Run();
		response = mRespString;
		return mResult;
	}

	protected Result SendWitAtCommand(string opCode, string atCommand, out string response)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		mLogResult = false;
		mOpCode = opCode;
		mHexData = (string.IsNullOrEmpty(atCommand) ? atCommand : Smart.Convert.AtToTCMD(atCommand));
		Run();
		response = mRespString;
		return mResult;
	}
}
