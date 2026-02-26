using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using ISmart;
using Microsoft.VisualBasic;

namespace SmartDevice.Steps;

public class LoadValueFromCsvFile : BaseStep
{
	public Process mProcess;

	public List<string> output = new List<string>();

	public List<string> error = new List<string>();

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Expected O, but got Unknown
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Invalid comparison between Unknown and I4
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f48: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Result result = (Result)1;
		string empty = string.Empty;
		bool flag = false;
		if (((dynamic)base.Info.Args).ManuallySelectCsvFile != null)
		{
			flag = (bool)((dynamic)base.Info.Args).ManuallySelectCsvFile;
		}
		if (flag)
		{
			OpenFileDialog val2 = new OpenFileDialog();
			((FileDialog)val2).Title = "Please select CSV file";
			((FileDialog)val2).InitialDirectory = "C:\\ProgramData\\Motorola\\smarttool\\CsvFiles";
			if ((int)((CommonDialog)val2).ShowDialog() != 1)
			{
				LogResult(result);
				return;
			}
			empty = ((FileDialog)val2).FileName;
		}
		else
		{
			string text = ((dynamic)base.Info.Args).CsvFilePath;
			string text2 = ((dynamic)base.Info.Args).CsvFilePartialName;
			if (!Directory.Exists(text))
			{
				Smart.Log.Error(TAG, "CsvFilePath " + text + " not exist...");
				result = (Result)1;
				LogResult(result);
				return;
			}
			string[] files = Directory.GetFiles(text, text2);
			if (files.Length == 0)
			{
				Smart.Log.Error(TAG, $"No Csv File {text2} found under {text}.");
				result = (Result)1;
				LogResult(result);
				return;
			}
			if (files.Length > 1)
			{
				Smart.Log.Error(TAG, $"More than 1 Csv File {text2} found under {text}.");
				result = (Result)1;
				LogResult(result);
				return;
			}
			Smart.Log.Error(TAG, $"Found 1 Csv File {files[0]} under {text}.");
			empty = Path.Combine(text, files[0]);
		}
		if (!File.Exists(empty))
		{
			Smart.Log.Error(TAG, "Can Not found csv file:" + empty);
			result = (Result)1;
			LogResult(result);
			return;
		}
		ICsvFile val3 = Smart.NewCsvFile();
		val3.LoadFile(empty, ',');
		string text3 = val.ModelId;
		Smart.Log.Info(TAG, "User selected model:" + text3);
		string[] array = text3.Split(new char[1] { '-' });
		if (array.Length > 1)
		{
			text3 = array[0] + "-" + array[1];
		}
		Smart.Log.Info(TAG, "Final User selected model to be used to search in csv file:" + text3);
		string empty2 = string.Empty;
		try
		{
			string text4 = "roCarrier";
			bool flag2 = default(bool);
			empty2 = Smart.Rsd.GetValue(text4, base.Recipe.Info.UseCase, val, ref flag2, false);
			if (string.IsNullOrEmpty(empty2) || empty2.Equals("IGNORE", StringComparison.OrdinalIgnoreCase))
			{
				empty2 = string.Empty;
				text4 = "countryCode";
				empty2 = Smart.Rsd.GetValue(text4, base.Recipe.Info.UseCase, val, ref flag2, false);
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Can't find User selected carrier." + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
			empty2 = string.Empty;
		}
		Smart.Log.Info(TAG, "User selected roCarrier:" + empty2);
		List<string> list = new List<string>();
		if (string.IsNullOrEmpty(empty2))
		{
			Smart.Log.Info(TAG, "User selected carrier if empty, pop up to ask user select item.");
			List<string> list2 = new List<string>();
			foreach (List<string> row in val3.Rows)
			{
				if (row.Contains(text3))
				{
					list2.Add(string.Join(",", row));
				}
			}
			Smart.Log.Info(TAG, $"Found {list2.Count} rows for model {text3}.");
			string text5 = string.Empty;
			int count = list2.Count;
			for (int i = 0; i < count; i++)
			{
				string text6 = list2[i];
				string text7 = string.Empty;
				string[] array2 = ((dynamic)base.Info.Args).ColumnsToShowContentOnUIForUserSelection.ToString().Split(',');
				for (int j = 0; j < array2.Length; j++)
				{
					int num = char.ToUpper(array2[j].ToCharArray()[0], CultureInfo.InvariantCulture) - 65;
					string text8 = text6.Split(new char[1] { ',' })[num];
					Smart.Log.Info(TAG, "cellContent:" + text8);
					text7 = text7 + " " + text8.Trim().Trim('\r', '\n');
				}
				text5 = text5 + "\r\n\t" + (i + 1) + " - " + text7.Trim();
			}
			string text9 = "1 - " + count;
			text5 = text5 + "\r\n\r\n" + Smart.Locale.Xlate("Enter your choice ") + "(" + text9 + ")";
			string text10 = Smart.Locale.Xlate(base.Info.Name);
			string empty3 = string.Empty;
			empty3 = Interaction.InputBox(text5.Trim(), text10, "", -1, -1);
			empty3 = empty3.Trim();
			Smart.Log.Debug(TAG, $"User inputs {empty3}");
			if (empty3 == string.Empty)
			{
				throw new InvalidDataException("User did not enter a response");
			}
			int num2 = int.Parse(empty3);
			if (num2 < 1 || num2 > count)
			{
				throw new InvalidDataException("User input is out of range");
			}
			list = new List<string>(list2[num2 - 1].Split(new char[1] { ',' }));
		}
		else
		{
			Smart.Log.Info(TAG, $"Get target row based on User selected carrier {empty2}.");
			for (int k = 0; k < val3.Rows.Count; k++)
			{
				List<string> list3 = val3.Rows[k];
				if (list3.Contains(text3) && list3.Contains(empty2))
				{
					list = list3;
					break;
				}
			}
		}
		if (list.Count > 0)
		{
			Smart.Log.Info(TAG, "Found target row:" + string.Join(",", list));
			string text11 = ((dynamic)base.Info.Args).ColsToGetExpectedValue.ToString();
			string text12 = ((dynamic)base.Info.Args).CacheNameToSetExpectedValue.ToString();
			string[] array3 = text11.Split(new char[1] { ',' });
			string[] array4 = text12.Split(new char[1] { ',' });
			for (int l = 0; l < array3.Length; l++)
			{
				int index = char.ToUpper(array3[l].ToCharArray()[0], CultureInfo.InvariantCulture) - 65;
				string text13 = list[index];
				string text14 = array4[l];
				Smart.Log.Info(TAG, $"cacheName {text14} and its corresponding cellContent {text13}.");
				if (text14.ToUpper().Contains("simlock".ToUpper()))
				{
					text13 = Path.Combine(((dynamic)base.Info.Args).SimLockFilePath, text13);
				}
				else if (text14.ToUpper().Contains("elabel".ToUpper()))
				{
					text13 = Path.Combine(((dynamic)base.Info.Args).eLabelFilePath, text13);
				}
				base.Cache[text14] = text13;
				Smart.Log.Info(TAG, string.Format("Cache[{0}]={1}.", array4[l], base.Cache[array4[l]]));
			}
			result = (Result)8;
		}
		else
		{
			Smart.Log.Error(TAG, $"Not Found target row for user selected carrier {empty2} modle {text3}");
			result = (Result)1;
		}
		LogResult(result);
	}
}
