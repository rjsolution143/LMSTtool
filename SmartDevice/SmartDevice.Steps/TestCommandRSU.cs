using System;
using ISmart;
using SmartDevice.Cfc;

namespace SmartDevice.Steps;

public class TestCommandRSU : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		object obj = (object)(IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string serialNumber = ((IDevice)obj).SerialNumber;
		string iD = ((IDevice)obj).ID;
		string text = ((dynamic)base.Info.Args).DeviceModel;
		if (text != null && text != string.Empty && text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		string text2 = ((dynamic)base.Info.Args).Operator;
		if (text2 != null && text2 != string.Empty && text2.StartsWith("$"))
		{
			string key2 = text2.Substring(1);
			text2 = base.Cache[key2];
		}
		string text3 = ((dynamic)base.Info.Args).SocModel;
		if (text3 != null && text3 != string.Empty && text3.StartsWith("$"))
		{
			string key3 = text3.Substring(1);
			text3 = base.Cache[key3];
		}
		string text4 = ((dynamic)base.Info.Args).Sip;
		if (text4 != null && text4 != string.Empty && text4.StartsWith("$"))
		{
			string key4 = text4.Substring(1);
			text4 = base.Cache[key4];
		}
		Tuple<string, string> tuple = new ProgramRSUKey().Execute(text, text2, text3, text4, TestCommand);
		string item = tuple.Item1;
		string item2 = tuple.Item2;
		Smart.Web.Rsu(serialNumber, text3, item2, item, text4, text, text2, iD);
		LogPass();
	}

	protected string TestCommand(string opCode, string data)
	{
		return base.tcmd.SendCommand(opCode, data).DataHex;
	}
}
