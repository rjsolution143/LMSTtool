using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice.Steps;

public class DetectModelIBase : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Invalid comparison between Unknown and I4
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a08: Unknown result type (might be due to invalid IL or missing references)
		string empty = string.Empty;
		bool flag = false;
		string text = "iBase_Successful";
		Result result = (Result)8;
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (device.ModelId != null && device.ModelId.Trim() != string.Empty && !device.ModelId.ToLower().Contains("unknown"))
		{
			Smart.Log.Debug(TAG, $"Model {device.ModelId} already set for SN {device.SerialNumber}");
			base.Log.AddInfo("ModelLookup", "NotNeeded");
			LogResult((Result)7);
			return;
		}
		SortedList<string, string> sortedList = new SortedList<string, string>();
		if (base.Cache.ContainsKey("props"))
		{
			sortedList = base.Cache["props"];
		}
		try
		{
			Smart.Log.Debug(TAG, "Device SerialNumber:" + device.SerialNumber);
			if (device.SerialNumber == "000000000000000")
			{
				Smart.Log.Debug(TAG, "Device is a blank board with IMEI " + device.SerialNumber);
				result = (Result)1;
				text = "Default_IMEI_On_Device";
				return;
			}
			if (device.SerialNumber == "UNKNOWN" || device.SerialNumber == string.Empty)
			{
				if (device.ID != string.Empty && device.ID != "UNKNOWN")
				{
					Smart.Log.Debug(TAG, $"Using device trackid {device.ID} to detect ibase model");
					empty = device.ID;
					flag = true;
				}
				else
				{
					List<UseCase> list = Smart.Rsd.UseCaseLocked();
					if (list != null && list.Count == 1 && (int)list[0] == 211)
					{
						device.PortIndex = -1;
						result = (Result)1;
						text = "Invalid IMEI";
						Smart.Log.Debug(TAG, $"Device serial number {device.SerialNumber} is invalid and cannot activate the Auto Kill Switch");
						return;
					}
					string promptText = "Please Input IMEI number";
					if (((dynamic)base.Info.Args).PromptText != null)
					{
						promptText = ((dynamic)base.Info.Args).PromptText;
					}
					promptText = Smart.Locale.Xlate(promptText);
					string title = Smart.Locale.Xlate("iBase Detection");
					empty = Smart.Thread.RunAndWait<string>((Func<string>)(() => device.Prompt.InputBox(title, promptText, (string)null)), true);
					empty = empty.Trim();
					Smart.Log.Info(TAG, "User inputs serial number: " + empty);
					if (!(empty != string.Empty))
					{
						Smart.Log.Info(TAG, "User did not input an IMEI number");
						result = (Result)1;
						text = "Invalid_User_Entry_For_IMEI";
						return;
					}
					if (!Smart.Convert.IsSerialNumberValid(empty, (SerialNumberType)0))
					{
						Smart.Log.Info(TAG, $"User inputs invalid IMEI number {empty}");
						result = (Result)1;
						text = "Invalid_User_Entry_For_IMEI";
						return;
					}
					device.SerialNumber = empty;
				}
			}
			else
			{
				empty = device.SerialNumber;
			}
			Smart.Log.Debug(TAG, "Using serial number " + empty + " to detect ibase model");
			string empty2 = string.Empty;
			string empty3 = string.Empty;
			string text2 = default(string);
			string text3 = default(string);
			string text4 = default(string);
			string text5 = default(string);
			string text6 = default(string);
			string text7 = default(string);
			bool flag2 = default(bool);
			string text8 = Smart.Rsd.DetectModel(empty, flag, ref empty2, ref empty3, ref text2, ref text3, ref text4, ref text5, ref text6, ref text7, ref text, device, ref flag2);
			if (!flag)
			{
				if ((device.SerialNumber == string.Empty || device.SerialNumber == "UNKNOWN") && text2 != string.Empty)
				{
					if (device.Group == string.Empty)
					{
						Smart.Log.Debug(TAG, $"Set device.SerialNumber to iBaseImei {text2}");
						device.SerialNumber = text2;
					}
					base.Log.AddInfo("IMEI", text2);
				}
				if ((device.SerialNumber2 == string.Empty || device.SerialNumber2 == "UNKNOWN") && text5 != string.Empty)
				{
					Smart.Log.Debug(TAG, $"Set device.SerialNumber2 to iBaseImei2 {text5}");
					device.SerialNumber2 = text5;
					base.Log.AddInfo("IMEI2", text5);
				}
			}
			if (text3 != string.Empty)
			{
				device.Log.AddInfo("WarrantyCustomerModelNumber", text3);
			}
			if (text4 != string.Empty)
			{
				device.Log.AddInfo("Svnkit", text4);
			}
			string text9 = "UNKNOWN";
			if (text8 == string.Empty)
			{
				Smart.Log.Error(TAG, "Fail to detect modelId");
				if (empty2 == string.Empty && empty3 == string.Empty)
				{
					Smart.Log.Debug(TAG, "And failed to find carrier and model using sale model from iBase");
				}
				else
				{
					Smart.Log.Debug(TAG, $"But found carrier {empty2} model {empty3} from iBase");
				}
				result = (Result)1;
			}
			else
			{
				Smart.Log.Info(TAG, $"Detected modelId: {text8}");
				string key = DetectionKey.NameToPropertyLookup["sku"];
				string text10 = (sortedList.ContainsKey(key) ? sortedList[key] : string.Empty);
				string key2 = DetectionKey.NameToPropertyLookup["roCarrier"];
				string text11 = (sortedList.ContainsKey(key2) ? sortedList[key2] : string.Empty);
				DeviceAttributes deviceAttributes = Smart.Rsd.GetDeviceAttributes(ref text8, device);
				string text12 = ((deviceAttributes.Sku != string.Empty) ? deviceAttributes.Sku : text7);
				string roCarrier = deviceAttributes.RoCarrier;
				if ((text10 != string.Empty && string.Compare(text10, "unknown", ignoreCase: true) != 0 && text12 != string.Empty && string.Compare(text10, text12, ignoreCase: true) != 0) || (text11 != string.Empty && string.Compare(text11, "unknown", ignoreCase: true) != 0 && roCarrier != string.Empty && string.Compare(text11, roCarrier, ignoreCase: true) != 0))
				{
					if (text10 != string.Empty && string.Compare(text10, "unknown", ignoreCase: true) != 0 && string.Compare(text10, text12, ignoreCase: true) != 0)
					{
						text = "SKU_Not_Matched_On_Device_To_iBase";
					}
					if (text11 != string.Empty && string.Compare(text11, "unknown", ignoreCase: true) != 0 && roCarrier != string.Empty && string.Compare(text11, roCarrier, ignoreCase: true) != 0)
					{
						text = ((!(text != "SKU_Not_Matched_On_Device_To_iBase")) ? "Both_SKU_ro.carrier_Not_Matched_On_Device_To_iBase" : "ro.carrier_Not_Matched_On_Device_To_iBase");
					}
					Smart.Log.Debug(TAG, $"The read out SKU {text10}. The detected SKU {text12} using iBase");
					Smart.Log.Debug(TAG, $"The read out roCarrier {text11}. The detect roCarrier {roCarrier} using iBase");
					Smart.Log.Debug(TAG, $"The detected modelId {text8} using iBase may not correct. Falling back to in-device detection method");
					text8 = string.Empty;
					result = (Result)1;
				}
				else
				{
					if (device.Group == "LST")
					{
						string key3 = DetectionKey.NameToPropertyLookup["builddisplay"];
						string text13 = (sortedList.ContainsKey(key3) ? sortedList[key3] : string.Empty);
						text9 = ((!(text13 == string.Empty)) ? ((text13 == Smart.Rsd.GetDeviceLatestAttributes(text8).BuildDisplay) ? "YES" : "NO") : "UNKNOWN");
					}
					else
					{
						string key4 = DetectionKey.NameToPropertyLookup["fingerPrint"];
						string text14 = (sortedList.ContainsKey(key4) ? sortedList[key4] : string.Empty);
						text9 = ((!(text14 == string.Empty)) ? ((text14 == Smart.Rsd.GetDeviceLatestAttributes(text8).FingerPrint) ? "YES" : "NO") : "UNKNOWN");
					}
					text = "iBase_Successful";
					device.Log.AddInfo("Detection", flag2 ? "iBase Supp" : "iBase");
				}
			}
			base.Log.AddInfo("FirmwareLatest", text9);
			base.Log.AddInfo("Model", empty3);
			device.ModelId = text8;
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Exception - errMsg: " + ex.Message);
			Smart.Log.Debug(TAG, ex.StackTrace);
			text = "Fail_To_Get_Warranty_Record";
			result = (Result)1;
		}
		finally
		{
			Smart.Log.Debug(TAG, "detectionStatus: " + text);
			Smart.Log.Debug(TAG, "testStatus: " + result);
			device.Log.AddInfo("DetectionStatus", text);
			VerifyOnly(ref result);
			LogResult(result);
		}
	}
}
