using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ISmart;
using LabelManager2;
using Microsoft.VisualBasic;

namespace SmartDevice.Steps;

public class PrintLabelUseCodesoft : BaseStep
{
	private const string Lppx2ProgId = "Lppx2.Application";

	private static Application labelApp;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Invalid comparison between Unknown and I4
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_0528: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Invalid comparison between Unknown and I4
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		Result val = (Result)8;
		string empty = string.Empty;
		IDevice val2 = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string empty2 = string.Empty;
		int num = 1;
		if (base.Cache.ContainsKey("LabelFile") && base.Cache.ContainsKey("NumberOfCopiesToPrint"))
		{
			empty2 = base.Cache["LabelFile"];
			num = base.Cache["NumberOfCopiesToPrint"];
		}
		else
		{
			Tuple<string, int, string> tuple = Smart.PrintLabel.LoadPrintSettingsFromOptionFile(val2);
			if (!string.IsNullOrEmpty(tuple.Item3.Trim()))
			{
				val2.Prompt.MessageBox("Error", tuple.Item3, (MessageBoxButtons)0, (MessageBoxIcon)16);
				LogResult((Result)1, "Select Label File", tuple.Item3);
				return;
			}
			dynamic val3 = (base.Cache["LabelFile"] = tuple.Item1);
			empty2 = val3;
			val3 = (base.Cache["NumberOfCopiesToPrint"] = tuple.Item2);
			num = val3;
		}
		Smart.Log.Debug(TAG, $"Configured label file {empty2}.");
		Smart.Log.Debug(TAG, $"Configured Num. of copies {num.ToString()}.");
		if (!base.Cache.ContainsKey("LabelParamFile"))
		{
			Smart.Log.Warning(TAG, $"You may need to add 'LoadValuesFromFile' step into recipe to load additinal data from Parameter file.");
		}
		new Dictionary<string, string>();
		foreach (KeyValuePair<string, string> item2 in Smart.PrintLabel.CollectDeviceDataForPrint(val2, (SortedList<string, object>)base.Cache))
		{
			Smart.Log.Verbose(TAG, item2.Key + ":" + item2.Value);
			base.Cache[item2.Key] = item2.Value;
		}
		MapCacheValueToLabelVariableValue();
		if (((dynamic)base.Info.Args).ParameterToCheck != null)
		{
			string text = ((dynamic)base.Info.Args).ParameterToCheck;
			string empty3 = string.Empty;
			val = Smart.PrintLabel.ValidateDataMissing((SortedList<string, object>)base.Cache, text, ref empty3);
			if ((int)val != 8)
			{
				empty3 = $"Required print data missing:\r\n" + empty3;
				Smart.Log.Error(TAG, empty3);
				Interaction.MsgBox((object)empty3, (MsgBoxStyle)48, (object)null);
				LogResult(val);
				return;
			}
		}
		val = CheckValueDuplicate(val2);
		if ((int)val != 8)
		{
			LogResult(val);
			return;
		}
		empty2 = DetermineFinalLabelFileBasedOnCondition(empty2);
		Smart.Log.Debug(TAG, "Final label File to be used:" + empty2);
		if (!File.Exists(empty2))
		{
			empty = $"Label file {empty2} does not exist,please select it from Options menu!";
			Smart.Log.Debug(TAG, empty);
			MessageBox.Show(empty);
			LogResult((Result)1, "Select Label File", empty);
		}
		else
		{
			base.Cache["LabelFile"] = empty2;
			val = PrintLabel(empty2, base.Cache, num);
			LogResult(val);
		}
	}

	private void MapCacheValueToLabelVariableValue()
	{
		string name = MethodBase.GetCurrentMethod().Name;
		string text = string.Empty;
		if (((dynamic)base.Info.Args).CacheNameMapToLabelVarName != null)
		{
			text = ((dynamic)base.Info.Args).CacheNameMapToLabelVarName.ToString();
		}
		Smart.Log.Debug(TAG, "Map Cache value To Label Var value:" + text);
		if (!string.IsNullOrEmpty(text))
		{
			string[] array = text.Split(new char[1] { '#' });
			foreach (string text2 in array)
			{
				Smart.Log.Debug(name, "name:" + text2);
				if (text2.Trim().Length <= 0)
				{
					continue;
				}
				string[] array2 = text2.Split(new char[1] { ',' });
				string text3 = array2[0];
				string text4 = array2[1];
				Smart.Log.Debug(name, "variableName:" + text4);
				if (base.Cache.ContainsKey(text3))
				{
					string text5 = base.Cache[text3].ToString();
					if (base.Cache.ContainsKey(text4))
					{
						base.Cache[text4] = text5;
					}
					else
					{
						base.Cache.Add(text4, text5);
					}
					Smart.Log.Debug(name, $"Give Cach[{text3}] value {text5} to label variable {text4}");
				}
			}
		}
		Smart.Log.Debug(name, "exit...");
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
		return text;
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

	private Result PrintLabel(string labelFile, SortedList<string, dynamic> allCachedData, int numOfCopies)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		string empty = string.Empty;
		Document val = null;
		if (labelApp == null)
		{
			Smart.Log.Debug(TAG, "Create CodeSoft ApplicationClass");
			try
			{
				labelApp = (Application)new ApplicationClass();
			}
			catch (Exception ex)
			{
				empty = ex.Message + Environment.NewLine + ex.StackTrace;
				Smart.Log.Error(TAG, empty);
				result = (Result)1;
				LogResult(result, "Create CodeSoft ApplicationClass", empty);
				return result;
			}
			Smart.Log.Debug(TAG, "Create CodeSoft ApplicationClass done.");
		}
		else
		{
			Smart.Log.Debug(TAG, "CS ApplicationClass handle alraeady exist.");
		}
		if (labelApp == null)
		{
			empty = "Can't Create CodeSoft ApplicationClass!";
			Smart.Log.Error(TAG, empty);
			LogResult((Result)1, "Create CodeSoft ApplicationClass", empty);
			return result;
		}
		Smart.Log.Debug(TAG, "Open document...");
		Documents documents = ((IApplication)labelApp).Documents;
		val = documents.Open(labelFile, true);
		if (val == null)
		{
			empty = "Can't open doc " + labelFile;
			Smart.Log.Error(TAG, empty);
			result = (Result)1;
			LogResult(result, "Open Doc", empty);
			documents.CloseAll(false);
			Marshal.ReleaseComObject(documents);
			((IApplication)labelApp).Quit();
			Marshal.ReleaseComObject(labelApp);
			labelApp = null;
			return result;
		}
		Smart.Log.Debug(TAG, "Total free variables in label template:" + ((IDocument)val).Variables.FreeVariables.Count);
		Smart.Log.Debug(TAG, "Total Form variables in label template:" + ((IDocument)val).Variables.FormVariables.Count);
		Smart.PrintLabel.AssignDataToCSLabelVariable(val, (SortedList<string, object>)allCachedData);
		short num = ((IDocument)val).PrintDocument(numOfCopies);
		if (num <= 0)
		{
			num = ((IApplication)labelApp).GetLastError();
			empty = ((IApplication)labelApp).ErrorMessage(num);
			Smart.Log.Error(TAG, empty);
			MessageBox.Show(empty);
			result = (Result)1;
			LogResult(result, "PrintDocument Error", empty);
			return result;
		}
		documents.CloseAll(false);
		Marshal.ReleaseComObject(documents);
		((IApplication)labelApp).Quit();
		Marshal.ReleaseComObject(labelApp);
		labelApp = null;
		return result;
	}

	private Result CheckValueDuplicate(IDevice device)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)8;
		string empty = string.Empty;
		string text = "$imei,$imei2,$serialnumberOld";
		if (((dynamic)base.Info.Args).CheckDuplicateVal != null)
		{
			text = ((dynamic)base.Info.Args).CheckDuplicateVal.ToString();
		}
		if (text != string.Empty)
		{
			string[] array = text.Split(new char[1] { ',' });
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				string empty2 = string.Empty;
				empty2 = ((!text2.StartsWith("$")) ? text2 : ((!base.Cache.Keys.Contains(text2.Substring(1))) ? "" : ((string)base.Cache[text2.Substring(1)])));
				text = text.Replace(text2, empty2);
			}
			Smart.Log.Verbose(TAG, $"Check whether duplicate value in {text}");
			array = text.Split(new char[1] { ',' });
			if (array.Distinct().Count() != array.Length)
			{
				empty = "Duplicate values found " + text;
				Smart.Log.Error(TAG, empty);
				device.Prompt.MessageBox(TAG, empty, (MessageBoxButtons)0, (MessageBoxIcon)16);
				result = (Result)1;
				LogResult(result, "Found Duplicate Value", empty);
			}
		}
		return result;
	}

	private string DetermineFinalLabelFileBasedOnCondition(string labelFile)
	{
		bool flag = true;
		if (((dynamic)base.Info.Args).DetermineLabelBasedOnCondition != null)
		{
			flag = (bool)((dynamic)base.Info.Args).DetermineLabelBasedOnCondition;
		}
		if (!flag)
		{
			return labelFile;
		}
		string text = ".Lab";
		if (((dynamic)base.Info.Args).LabelFileExtention != null)
		{
			text = ((dynamic)base.Info.Args).LabelFileExtention.ToString();
			if (!text.StartsWith("."))
			{
				text = "." + text;
			}
		}
		string directoryName = Path.GetDirectoryName(labelFile);
		string text2 = "$sku";
		if (((dynamic)base.Info.Args).ConditionValue != null)
		{
			text2 = ((dynamic)base.Info.Args).ConditionValue;
		}
		Smart.Log.Info(TAG, $"Search label file based on {text2} in dir {directoryName}.");
		string[] array = text2.Split(new char[1] { ',' });
		for (int i = 0; i < array.Length; i++)
		{
			string text3 = array[i];
			if (text3.StartsWith("$"))
			{
				text3 = text3.Substring(1);
				if (base.Cache.Keys.Contains(text3))
				{
					array[i] = base.Cache[text3];
					continue;
				}
				array[i] = "";
				Smart.Log.Error(TAG, $"Cache not contain key {text3}.");
			}
		}
		int num = array.Length;
		int num2 = 0;
		while (true)
		{
			string text4 = string.Empty;
			for (int j = 0; j < num; j++)
			{
				text4 = text4 + "*" + array[j];
			}
			text4 = text4 + "*" + text;
			labelFile = SearchFile(directoryName, text4);
			if (!string.IsNullOrEmpty(labelFile) || num == 1)
			{
				break;
			}
			num--;
			num2++;
		}
		Smart.Log.Verbose(TAG, $"Auto picked up label file {labelFile}.");
		return labelFile;
	}

	private string SearchFile(string dir, string searchPattern)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		Smart.Log.Info(TAG, $"Search file with pattern {searchPattern} in dir {dir}.");
		string result = string.Empty;
		string[] files = Directory.GetFiles(dir, searchPattern, SearchOption.TopDirectoryOnly);
		if (files.Length > 1)
		{
			string text = $"Total {files.Length} labels contain {searchPattern} in directory {dir},please only keep one file!";
			Smart.Log.Error(TAG, text);
			Smart.User.MessageBox("Error", text, (MessageBoxButtons)0, (MessageBoxIcon)16);
		}
		else if (files.Length < 1)
		{
			string text2 = $"Can't find label with name {searchPattern} in directory {dir}";
			Smart.Log.Error(TAG, text2);
			Smart.User.MessageBox("Error", text2, (MessageBoxButtons)0, (MessageBoxIcon)16);
		}
		else
		{
			result = files[0];
		}
		return result;
	}

	private void ConnectToLppx2()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		if (labelApp != null)
		{
			return;
		}
		object obj;
		try
		{
			obj = Marshal.GetActiveObject("Lppx2.Application");
		}
		catch (Exception ex)
		{
			string text = ex.Message.ToString();
			Smart.Log.Debug(TAG, text);
			obj = null;
		}
		try
		{
			if (obj == null)
			{
				labelApp = (Application)new ApplicationClass();
			}
			else
			{
				labelApp = (Application)obj;
			}
		}
		catch (Exception ex2)
		{
			MessageBox.Show(ex2.Message.ToString());
		}
	}
}
