using System.IO;
using ISmart;

namespace SmartDevice.Steps;

public class ODMDataSignRequest : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
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
		string text = ((dynamic)base.Info.Args).ProdID;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		string text2 = ((dynamic)base.Info.Args).KeyType;
		if (text2.StartsWith("$"))
		{
			string key2 = text2.Substring(1);
			text2 = base.Cache[key2];
		}
		string text3 = "1";
		if (((dynamic)base.Info.Args).KeyName != null && ((dynamic)base.Info.Args).KeyName != string.Empty)
		{
			text3 = ((dynamic)base.Info.Args).KeyName;
			if (text3.StartsWith("$"))
			{
				string key3 = text3.Substring(1);
				text3 = base.Cache[key3];
			}
		}
		string text4 = "0x00";
		if (((dynamic)base.Info.Args).ClientReqType != null)
		{
			text4 = ((dynamic)base.Info.Args).ClientReqType;
		}
		string text5 = "$ODMData";
		if (((dynamic)base.Info.Args).ODMData != null)
		{
			text5 = ((dynamic)base.Info.Args).ODMData;
		}
		string text6 = text5;
		if (text5.StartsWith("$"))
		{
			string key4 = text5.Substring(1);
			text6 = base.Cache[key4];
		}
		Smart.Log.Info(TAG, $"Requesting ODM data signing for {empty2}");
		string text7 = Smart.Web.DataSignODM(empty2, rsdLogId, text4, text, text2, text3, text6);
		string text8 = "$ODMSigned";
		if (((dynamic)base.Info.Args).ODMSigned != null)
		{
			text8 = ((dynamic)base.Info.Args).ODMSigned;
		}
		string text9 = text7;
		if (text8.StartsWith("$"))
		{
			string key5 = text8.Substring(1);
			base.Cache[key5] = text9;
		}
		if (((dynamic)base.Info.Args).SaveSignedDataToFile != null && (bool)((dynamic)base.Info.Args).SaveSignedDataToFile)
		{
			string text10 = ((dynamic)base.Info.Args).FileName.ToString();
			string empty3 = string.Empty;
			empty3 = ((!Path.IsPathRooted(text10)) ? Smart.File.TempFolder() : Path.GetDirectoryName(text10));
			if (!Directory.Exists(empty3))
			{
				Directory.CreateDirectory(empty3);
			}
			string text11 = Path.Combine(empty3, text10);
			if (((dynamic)base.Info.Args).WriteSignedDataInByteToFile != null && (bool)((dynamic)base.Info.Args).WriteSignedDataInByteToFile)
			{
				byte[] bytes = Smart.Convert.HexToBytes(text9);
				File.WriteAllBytes(text11, bytes);
			}
			else
			{
				File.WriteAllText(text11, text9);
			}
			string key6 = text8;
			if (text8.StartsWith("$"))
			{
				key6 = text8.Substring(1);
			}
			base.Cache[key6] = text11;
		}
		LogPass();
	}
}
