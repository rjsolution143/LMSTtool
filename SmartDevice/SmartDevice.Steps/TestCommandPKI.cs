using ISmart;
using SmartDevice.Cfc;

namespace SmartDevice.Steps;

public class TestCommandPKI : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string rsdLogId = val.Log.RsdLogId;
		string text = ((dynamic)base.Info.Args).PKIKey;
		string originalImei = val.SerialNumber;
		if (val.Log.Info.ContainsKey("OriginalImei"))
		{
			originalImei = val.Log.Info["OriginalImei"];
		}
		if (text != null && text != string.Empty && text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		string text2 = ((dynamic)base.Info.Args).ProductFamily;
		if (text2 != null && text2 != string.Empty && text2.StartsWith("$"))
		{
			string key2 = text2.Substring(1);
			text2 = base.Cache[key2];
		}
		if (ProgramPKI(text, text2, originalImei, rsdLogId))
		{
			LogPass();
		}
		else
		{
			LogResult((Result)8, "PKI Key Type already programmed");
		}
		Result val2 = (Result)8;
		SetPreCondition(((object)(Result)(ref val2)).ToString());
	}

	protected bool ProgramPKI(string keyType, string productFamily, string originalImei, string logId)
	{
		Smart.Log.Debug(TAG, $"Programming PKI Key type {keyType}");
		bool flag = false;
		if (keyType.Length < 5)
		{
			Smart.Log.Debug(TAG, "Programming standard PKI key");
			return new ProgramPKI().Execute(keyType, TestCommand, production: true, originalImei, logId);
		}
		Smart.Log.Debug(TAG, "Programming four-byte PKI key");
		Smart.Log.Debug(TAG, $"Using product family {productFamily}");
		return new ProgramPKIFourByteType().Execute(keyType, TestCommand, production: true, productFamily, originalImei, logId);
	}

	protected string TestCommand(string opCode, string data)
	{
		return base.tcmd.SendCommand(opCode, data).DataHex;
	}
}
