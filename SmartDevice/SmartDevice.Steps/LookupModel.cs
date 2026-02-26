using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class LookupModel : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (val.ModelId != null && val.ModelId.Trim() != string.Empty && !val.ModelId.ToLower().Contains("unknown"))
		{
			base.Log.AddInfo("ModelLookup", "NotNeeded");
			Smart.Log.Debug(TAG, $"Model {val.ModelId} already set for SN {val.SerialNumber}");
			LogResult((Result)7);
			return;
		}
		Dictionary<string, string> allModelCarriers = Smart.Fsb.GetAllModelCarriers();
		List<string> list = new List<string>(allModelCarriers.Keys);
		list.Sort();
		string empty = string.Empty;
		string empty2 = string.Empty;
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
		string text = "customer_model_no";
		List<string> list2 = new List<string>();
		if (base.Cache.ContainsKey(empty + "Details") && base.Cache[empty + "Details"].ContainsKey(text))
		{
			SortedList<string, string> sortedList = base.Cache[empty + "Details"];
			string text2 = sortedList[text].Trim();
			string text3 = "iBase_Successful";
			if (sortedList.ContainsKey("cit"))
			{
				string text4 = sortedList["cit"].Trim();
				if ((val.ID == string.Empty || val.ID == "UNKNOWN") && text4 != string.Empty)
				{
					val.ID = text4;
				}
			}
			string text5 = default(string);
			bool flag = default(bool);
			string modelCarrierFromSaleModel = Smart.Fsb.GetModelCarrierFromSaleModel(text2, val, ref text5, ref text3, ref flag);
			if (text5 != string.Empty)
			{
				val.Log.AddInfo("Svnkit", text5);
			}
			val.Log.AddInfo("DetectionStatus", text3);
			if (modelCarrierFromSaleModel != string.Empty)
			{
				list = new List<string> { modelCarrierFromSaleModel };
				val.Log.AddInfo("Detection", flag ? "iBase Supp" : "iBase");
			}
			else
			{
				string text6 = Smart.Fsb.GetModelNameFromSaleModel(text2).ToLowerInvariant();
				if (text6 == string.Empty)
				{
					Smart.Log.Error(TAG, $"Sales model {text2} not found in local file");
					base.Log.AddInfo("ModelLookup", "NotFound");
				}
				else
				{
					foreach (string item in list)
					{
						if (item.ToLowerInvariant().Contains(text6))
						{
							list2.Add(item);
						}
					}
					if (list2.Count > 0)
					{
						list = list2;
						Smart.Log.Debug(TAG, $"{list2.Count} models found for sales model {text2} result {text6}");
						base.Log.AddInfo("ModelLookup", "Success");
					}
					else
					{
						base.Log.AddInfo("ModelLookup", "NoMatches");
						Smart.Log.Error(TAG, $"No models found for sales model {text2} result {text6}");
					}
				}
			}
		}
		else
		{
			Smart.Log.Error(TAG, $"Could not find any sales model for SN {empty2}");
			base.Log.AddInfo("ModelLookup", "NotPossible");
		}
		if (list.Count < 1)
		{
			Smart.Log.Error(TAG, $"No models to choose from for SN: {empty2}");
			LogResult((Result)1, "No models to choose from");
			return;
		}
		string modelCarrier = list[0];
		if (list.Count > 1)
		{
			modelCarrier = SelectModelCarrier(list);
		}
		string deviceModelId = GetDeviceModelId(modelCarrier, allModelCarriers);
		Smart.Log.Debug(TAG, $"User selected {deviceModelId} for model");
		val.ModelId = deviceModelId;
		LogPass();
	}

	private string SelectModelCarrier(List<string> choices)
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		object obj = (object)(IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = Smart.Locale.Xlate("Model Selection");
		string text2 = Smart.Locale.Xlate("Please type expected model/carrier(e.g. XT1234-1-DS/Retail_Brazil_5G_NR) to filter the list");
		Smart.Log.Verbose(TAG, $"Total choices: {choices.Count}");
		string empty = string.Empty;
		DialogResult val = ((IDevice)obj).Prompt.SearchSelect(text, text2, choices, ref empty);
		if (!Smart.Convert.ToBool(val))
		{
			throw new OperationCanceledException("User canceled model/carrier selection");
		}
		return empty;
	}

	private string GetDeviceModelId(string modelCarrier, Dictionary<string, string> modelCarrierToFlexModel)
	{
		string result = modelCarrier.Replace("/", "|");
		if (modelCarrierToFlexModel.TryGetValue(modelCarrier, out var value))
		{
			string[] array = modelCarrier.Split(new char[1] { '/' });
			result = $"{array[0]}/{value}|{array[1]}";
		}
		return result;
	}
}
