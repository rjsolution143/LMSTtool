using ISmart;
using SmartDevice.Cfc;

namespace SmartDevice.Steps;

public class TestCommandSIMLock : TestCommandStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string rsdLogId = val.Log.RsdLogId;
		string originalImei = val.SerialNumber;
		if (val.Log.Info.ContainsKey("OriginalImei"))
		{
			originalImei = val.Log.Info["OriginalImei"];
		}
		string snType = ((dynamic)base.Info.Args).SNType;
		string simLockDataBlockType = "0033";
		if (((dynamic)base.Info.Args).SimLockDbType != null)
		{
			simLockDataBlockType = ((dynamic)base.Info.Args).SimLockDbType;
		}
		if (!Smart.Convert.IsSerialNumberValid(val.SerialNumber, (SerialNumberType)1) && base.Cache.Keys.Contains("SerialNumberOut"))
		{
			Smart.Log.Debug(TAG, string.Format("Device serial number {0} is a not a legal IMEI ,using PCBA dispatch IMEI {1}", val.SerialNumber, base.Cache["SerialNumberOut"]));
			val.SerialNumber = base.Cache["SerialNumberOut"];
		}
		new ProgramSimLockDataBlock().Execute(val.SerialNumber, val.SerialNumber, snType, TestCommand, originalImei, rsdLogId, simLockDataBlockType);
		LogPass();
	}

	protected string TestCommand(string opCode, string data)
	{
		return base.tcmd.SendCommand(opCode, data).DataHex;
	}
}
