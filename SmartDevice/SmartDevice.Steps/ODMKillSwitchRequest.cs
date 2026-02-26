using System.IO;
using ISmart;

namespace SmartDevice.Steps;

public class ODMKillSwitchRequest : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_09dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d87: Unknown result type (might be due to invalid IL or missing references)
		string empty = string.Empty;
		string empty2 = string.Empty;
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (((dynamic)base.Info.Args).InputName != null && ((dynamic)base.Info.Args).InputName != string.Empty)
		{
			empty = ((dynamic)base.Info.Args).InputName.ToString();
			empty2 = base.Cache[empty];
		}
		else
		{
			empty = "DeviceSerialNumber";
			empty2 = val.SerialNumber;
		}
		string rsdLogId = val.Log.RsdLogId;
		string text = ((dynamic)base.Info.Args).ProdName;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		string text2 = ((dynamic)base.Info.Args).BuildType;
		if (text2.StartsWith("$"))
		{
			string key2 = text2.Substring(1);
			text2 = base.Cache[key2];
		}
		string text3 = "0x00";
		if (((dynamic)base.Info.Args).ClientReqType != null)
		{
			text3 = ((dynamic)base.Info.Args).ClientReqType;
		}
		string text4 = "$CPUID";
		if (((dynamic)base.Info.Args).CPUID != null)
		{
			text4 = ((dynamic)base.Info.Args).CPUID;
		}
		string text5 = text4;
		if (text4.StartsWith("$"))
		{
			string key3 = text4.Substring(1);
			text5 = base.Cache[key3];
		}
		Smart.Log.Info(TAG, $"Requesting ODM KillSwitch unlock for {empty2}");
		KillSwitchData val2 = Smart.Web.KillSwitchODM(empty2, rsdLogId, text3, text, text5, text2);
		string text6 = "$ODMKSData";
		if (((dynamic)base.Info.Args).ODMKSData != null)
		{
			text6 = ((dynamic)base.Info.Args).ODMKSData;
		}
		string data = ((KillSwitchData)(ref val2)).Data;
		if (text6.StartsWith("$"))
		{
			string key4 = text6.Substring(1);
			base.Cache[key4] = data;
		}
		string text7 = "$ODMKSPassword";
		if (((dynamic)base.Info.Args).ODMKSPassword != null)
		{
			text7 = ((dynamic)base.Info.Args).ODMKSPassword;
		}
		string password = ((KillSwitchData)(ref val2)).Password;
		if (text7.StartsWith("$"))
		{
			string key5 = text7.Substring(1);
			base.Cache[key5] = password;
		}
		byte[] bytes = Smart.Convert.HexToBytes(data);
		string text8 = Path.Combine(Smart.Rsd.GetFilePathName("tempDir", base.Recipe.Info.UseCase, val), "signature_" + password);
		File.WriteAllBytes(text8, bytes);
		string text9 = "$ODMKSFile";
		if (((dynamic)base.Info.Args).ODMKSFile != null)
		{
			text9 = ((dynamic)base.Info.Args).ODMKSFile;
		}
		if (text9.StartsWith("$"))
		{
			string key6 = text9.Substring(1);
			base.Cache[key6] = text8;
		}
		LogPass();
	}
}
