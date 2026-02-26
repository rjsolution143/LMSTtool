using System.IO;
using ISmart;

namespace SmartDevice.Steps;

public class ODMKeyRequest : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0962: Unknown result type (might be due to invalid IL or missing references)
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
		string text = ((dynamic)base.Info.Args).CertModel;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		string text2 = ((dynamic)base.Info.Args).CertType;
		string text3 = "0x00";
		if (((dynamic)base.Info.Args).ClientReqType != null)
		{
			text3 = ((dynamic)base.Info.Args).ClientReqType;
		}
		Smart.Log.Info(TAG, string.Format("Requesting ODM {1} for {0}", text2, empty2));
		string text4 = Smart.Web.KeyDispatchODM(empty2, rsdLogId, text3, text, text2);
		string text5 = "$ODMKey";
		if (((dynamic)base.Info.Args).ODMKey != null)
		{
			text5 = ((dynamic)base.Info.Args).ODMKey;
		}
		string text6 = text4;
		if (text5.StartsWith("$"))
		{
			string key2 = text5.Substring(1);
			base.Cache[key2] = text6;
		}
		byte[] bytes = Smart.Convert.HexToBytes(text6);
		string filePathName = Smart.Rsd.GetFilePathName("tempDir", base.Recipe.Info.UseCase, val);
		string text7 = $"ODM{text2}File";
		string text9;
		if (text2 == "DRM")
		{
			string text8 = ".zip";
			if (((dynamic)base.Info.Args).KeyFormat != null && ((dynamic)base.Info.Args).KeyFormat != string.Empty)
			{
				text8 = ((dynamic)base.Info.Args).KeyFormat.ToString();
			}
			text9 = Path.Combine(filePathName, text2 + "_" + val.ID + text8);
		}
		else
		{
			text9 = Path.Combine(filePathName, text2 + "_" + val.ID + ".bin");
		}
		File.WriteAllBytes(text9, bytes);
		string text10 = "$" + text7;
		if (((dynamic)base.Info.Args).ODMFile != null)
		{
			text10 = ((dynamic)base.Info.Args).ODMFile;
		}
		if (text10.StartsWith("$"))
		{
			string key3 = text10.Substring(1);
			base.Cache[key3] = text9;
		}
		LogPass();
	}
}
