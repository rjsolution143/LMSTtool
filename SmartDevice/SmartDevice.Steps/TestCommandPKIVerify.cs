using System;
using ISmart;
using SmartDevice.Cfc;

namespace SmartDevice.Steps;

public class TestCommandPKIVerify : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		string text = ((dynamic)base.Info.Args).PKIKey;
		if (text == null || text == string.Empty)
		{
			throw new NotSupportedException("Correct keyType is required");
		}
		if (text != null && text != string.Empty && text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		if (VerifyPKI(text))
		{
			LogPass();
		}
		else
		{
			LogResult((Result)1, "PKI Key verification failed");
		}
	}

	protected bool VerifyPKI(string keyType)
	{
		Smart.Log.Debug(TAG, $"Verifying PKI Key type {keyType}");
		bool flag = keyType.Length >= 5;
		Smart.Log.Debug(TAG, string.Format("Verifying {0} PKI key", flag ? "four-byte" : "standard"));
		return new VerifyPKI().Execute(keyType, TestCommand, production: true, flag);
	}

	protected string TestCommand(string opCode, string data)
	{
		return base.tcmd.SendCommand(opCode, data).DataHex;
	}
}
