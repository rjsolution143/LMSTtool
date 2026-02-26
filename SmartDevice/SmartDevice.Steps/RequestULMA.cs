using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class RequestULMA : RequestSerialNumber
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		SortedList<string, string> sortedList = FindPCBARow();
		if (sortedList != null)
		{
			string text = sortedList["UpdCustomer"];
			string text2 = sortedList["Ulma"];
			string text3 = sortedList["Gppd"];
			string text4 = sortedList["BuildType"];
			string text5 = sortedList["Protocol"];
			string text6 = sortedList["boardAssembly"];
			string iD = val.ID;
			string empty = string.Empty;
			string serialNumber = val.SerialNumber;
			SerialNumberType val2 = Smart.Convert.ToSerialNumberType(serialNumber);
			string text7 = ((object)(SerialNumberType)(ref val2)).ToString().ToUpperInvariant();
			Smart.Web.PcbaSerialNumberRequest(serialNumber, text7, text, text2, text3, text4, text5, text6, iD, empty);
			LogPass();
		}
	}
}
