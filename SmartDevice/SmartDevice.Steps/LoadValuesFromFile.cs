using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ISmart;
using Microsoft.VisualBasic;

namespace SmartDevice.Steps;

public class LoadValuesFromFile : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Invalid comparison between Unknown and I4
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		Result val = (Result)8;
		IDevice val2 = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if ((bool)bool.Parse(((dynamic)base.Info.Args).LoadDataFromLabelPrintParameterFile.ToString()))
		{
			string empty = string.Empty;
			Tuple<string, int, string> tuple = Smart.PrintLabel.LoadPrintSettingsFromOptionFile(val2);
			if (!string.IsNullOrEmpty(tuple.Item3.Trim()))
			{
				val2.Prompt.MessageBox("Error", tuple.Item3, (MessageBoxButtons)0, (MessageBoxIcon)16);
				LogResult((Result)1, "Select Label File", tuple.Item3);
				return;
			}
			dynamic val3 = (base.Cache["LabelFile"] = tuple.Item1);
			empty = val3;
			base.Cache["NumberOfCopiesToPrint"] = tuple.Item2;
			Smart.Log.Debug(TAG, $"Configured label file {empty}.");
			string pN = GetPN(val2, empty);
			if (string.IsNullOrEmpty(pN))
			{
				Smart.Log.Error(TAG, $"PN is empty,can't load data from parameter file for specified PN, skip loading data");
				val = (Result)7;
				LogResult(val);
				return;
			}
			base.Cache["PN"] = pN;
			Smart.Log.Debug(TAG, "PN to be used:" + pN);
			string parameterFilePath = GetParameterFilePath(val2);
			if (string.IsNullOrEmpty(parameterFilePath) || !File.Exists(parameterFilePath))
			{
				Smart.Log.Error(TAG, $"Parameter file not configured, skip load data");
				val = (Result)7;
				LogResult(val);
				return;
			}
			base.Cache["LabelParamFile"] = parameterFilePath;
			Smart.Log.Debug(TAG, $"User configured parameter file {parameterFilePath}");
			Dictionary<string, string> dictionary = Smart.PrintLabel.ReadDataFromParameterFileForPrint(parameterFilePath)[pN];
			new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> item2 in Smart.PrintLabel.ParseAdditionalDataFromParameterFile(dictionary))
			{
				Smart.Log.Verbose(TAG, item2.Key + ":" + item2.Value);
				base.Cache[item2.Key] = item2.Value;
			}
			if (((dynamic)base.Info.Args).ParameterToCheck != null)
			{
				string text = ((dynamic)base.Info.Args).ParameterToCheck;
				string empty2 = string.Empty;
				val = Smart.PrintLabel.ValidateDataMissing((SortedList<string, object>)base.Cache, text, ref empty2);
				if ((int)val != 8)
				{
					empty2 = $"Data missing in {parameterFilePath} for PN {pN}:\r\n" + empty2;
					Smart.Log.Error(TAG, empty2);
					Interaction.MsgBox((object)empty2, (MsgBoxStyle)48, (object)null);
					LogResult(val);
					return;
				}
			}
		}
		val = (Result)8;
		VerifyOnly(ref val);
		LogPass();
	}

	private string GetParameterFilePath(IDevice device)
	{
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (((dynamic)base.Info.Args).GetParamFileFromCommonStorageDir != null)
		{
			flag = Convert.ToBoolean(((dynamic)base.Info.Args).GetParamFileFromCommonStorageDir.ToString());
		}
		string text = "ParameterFile.txt";
		if (((dynamic)base.Info.Args).ParamFileName != null)
		{
			text = ((dynamic)base.Info.Args).ParamFileName;
		}
		string labelParameterFilePath = Smart.PrintLabel.GetLabelParameterFilePath(device, flag, text, base.Recipe.Info.UseCase);
		if (string.IsNullOrEmpty(labelParameterFilePath))
		{
			Smart.Log.Error(TAG, $"Parameter file not configured on RSD");
		}
		else if (!File.Exists(labelParameterFilePath))
		{
			Smart.Log.Error(TAG, $"User configured parameter file not exist {labelParameterFilePath}");
		}
		return labelParameterFilePath;
	}

	private string GetPN(IDevice device, string labelFile)
	{
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (((dynamic)base.Info.Args).GetPNFromLabelName != null)
		{
			flag = Convert.ToBoolean(((dynamic)base.Info.Args).GetPNFromLabelName.ToString());
		}
		string pN = Smart.PrintLabel.GetPN(labelFile, device, flag, base.Recipe.Info.UseCase);
		if (string.IsNullOrEmpty(pN))
		{
			Smart.Log.Error(TAG, $"PN is empty,can't load data from parameter file for specified PN, skip loading data");
		}
		return pN;
	}
}
