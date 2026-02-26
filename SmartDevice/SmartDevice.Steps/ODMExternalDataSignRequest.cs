using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class ODMExternalDataSignRequest : ExternalRequestStep
{
	private SortedList<string, string> parameters = new SortedList<string, string>();

	private string TAG => GetType().FullName;

	public override void Run()
	{
		base.Run();
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
			text = base.Cache[text.Substring(1)];
		}
		string text2 = ((dynamic)base.Info.Args).KeyType;
		if (text2.StartsWith("$"))
		{
			text2 = base.Cache[text2.Substring(1)];
		}
		string text3 = ((dynamic)base.Info.Args).KeyName;
		if (text3.StartsWith("$"))
		{
			text3 = base.Cache[text3.Substring(1)];
		}
		string value = "0x00";
		if (((dynamic)base.Info.Args).ClientReqType != null)
		{
			value = ((dynamic)base.Info.Args).ClientReqType;
		}
		parameters["serialNumber"] = empty2;
		parameters["logId"] = rsdLogId;
		parameters["prodId"] = text;
		parameters["keyType"] = text2;
		parameters["keyName"] = text3;
		parameters["clientReqType"] = value;
		Smart.Log.Debug(TAG, "Setting handler for ODM External Data Sign Request");
		base.Cache["ReceivedDataToBeSignedFromPhone"] = "";
		base.Cache["GpsReturnedAuthData"] = "";
		lock (ExternalRequestStep.requestLock)
		{
			ExternalRequestStep.handlers.Add(HandleSignRequest);
		}
		LogPass();
	}

	protected string HandleSignRequest(string odmDataRequest)
	{
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Smart.Log.Debug(TAG, "odmDataRequest: " + odmDataRequest);
			base.Cache["ReceivedDataToBeSignedFromPhone"] = odmDataRequest;
			string text = Smart.Convert.ToString("Parameters", (IEnumerable<KeyValuePair<string, string>>)parameters.ToList());
			Smart.Log.Debug(TAG, text);
			string text2 = parameters["serialNumber"];
			string text3 = parameters["logId"];
			string text4 = parameters["prodId"];
			string text5 = parameters["keyType"];
			string text6 = parameters["keyName"];
			string text7 = parameters["clientReqType"];
			int num = odmDataRequest.IndexOf(':');
			if (num < 1)
			{
				throw new FormatException($"ODM request cannot be recognized: {odmDataRequest}");
			}
			if (odmDataRequest.Substring(0, num).ToLowerInvariant().ToString() != "data")
			{
				throw new NotSupportedException($"Unsupported request: {odmDataRequest}");
			}
			string text8 = odmDataRequest.Substring(num + 1);
			num = text8.IndexOf(':');
			if (num < 1)
			{
				Smart.Log.Debug(TAG, $"No key type specified in odmData, use recipe.keyType {text5}");
			}
			else
			{
				text5 = text8.Substring(0, num);
				text8 = text8.Substring(num + 1);
				Smart.Log.Debug(TAG, $"Use keyType: {text5} specified in odmData");
			}
			Smart.Log.Verbose(TAG, $"External Request for ODM data signing: {text8}");
			Smart.Log.Info(TAG, $"Requesting (External) ODM data signing for {text2}");
			string text9 = Smart.Web.DataSignODM(text2, text3, text7, text4, text5, text6, text8);
			Smart.Log.Verbose(TAG, $"Received (External) ODM data response: {text9}");
			base.Cache["GpsReturnedAuthData"] = text9;
			return ExternalRequestStep.SendResponse(ExternalResponseType.Data, text9);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, $"Error during HandleSignRequest: {ex.Message}");
			Smart.Log.Verbose(TAG, ex.ToString());
			if (ex.Message.ToLower().Contains("publicip Not allowed".ToLower()))
			{
				object obj = (object)(IDevice)((dynamic)base.Recipe.Info.Args).Device;
				((IDevice)obj).Prompt.CloseMessageBox();
				string text10 = "Please add your network public ip to your USB etoken on RSD!";
				text10 = Smart.Locale.Xlate(text10);
				string text11 = Smart.Locale.Xlate(base.Info.Name);
				((IDevice)obj).Prompt.MessageBox(text11, text10, (MessageBoxButtons)0, (MessageBoxIcon)16);
			}
			return ExternalRequestStep.SendResponse(ExternalResponseType.Error, ex.Message);
		}
	}
}
