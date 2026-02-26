using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class InputSelect : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).InputName.ToString();
		string promptText = ((dynamic)base.Info.Args).PromptText.ToString();
		promptText = Smart.Locale.Xlate(promptText);
		string title = Smart.Locale.Xlate(base.Info.Name);
		List<string> list = new List<string>();
		bool flag = default(bool);
		string value = Smart.Rsd.GetValue("phoneModel", base.Recipe.Info.UseCase, device, ref flag, false);
		string[] array = device.ModelId.Split(new char[1] { '|' });
		string text2 = "UNKNOWN";
		if (array.Length > 1)
		{
			text2 = array[1];
		}
		Smart.Log.Info(TAG, $"Find list of {text} on {value} {text2}");
		if (text == "pcb-part-no")
		{
			List<string> pcbaFilePathNames = Smart.Rsd.GetPcbaFilePathNames();
			for (int i = 0; i < pcbaFilePathNames.Count; i++)
			{
				string text3 = Smart.Rsd.ReadPcbaFileContent(pcbaFilePathNames[i]);
				ICsvFile obj = Smart.NewCsvFile();
				obj.Load(text3, ',');
				foreach (SortedList<string, string> item in (IEnumerable<SortedList<string, string>>)obj)
				{
					string text4 = item["RsdFamily"];
					string text5 = item["RsdCarrier"];
					if (!(text4.ToLowerInvariant() != value.ToLowerInvariant()) && !(text5.ToLowerInvariant() != text2.ToLowerInvariant()))
					{
						string text6 = item["PCBAssemblyPart"];
						Smart.Log.Debug(TAG, "Found PCBAssemblyPart: " + text6);
						if (!list.Contains(text6))
						{
							list.Add(text6);
						}
					}
				}
			}
		}
		Smart.Log.Debug(TAG, "choices.Count = " + list.Count);
		string text7 = string.Empty;
		if (list.Count > 1)
		{
			DialogResult val = device.Prompt.SearchSelect(title, promptText, list, ref text7);
			if (!Smart.Convert.ToBool(val))
			{
				throw new OperationCanceledException("User canceled Input selection");
			}
		}
		else if (list.Count == 1)
		{
			text7 = list[0];
		}
		else
		{
			string watermark = string.Empty;
			if (((dynamic)base.Info.Args).Watermark != null)
			{
				watermark = ((dynamic)base.Info.Args).Watermark.ToString();
				watermark = Smart.Locale.Xlate(watermark);
			}
			text7 = Smart.Thread.RunAndWait<string>((Func<string>)(() => device.Prompt.InputBox(title, promptText, watermark)), true);
			text7 = text7.Trim();
			if (text7 == string.Empty)
			{
				throw new InvalidDataException("User did not enter a response");
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			base.Cache[text] = text7;
			base.Log.AddInfo(text, text7);
			base.Log.AddInfo("InputName", text);
		}
		SetPreCondition(text7);
		LogPass();
	}
}
