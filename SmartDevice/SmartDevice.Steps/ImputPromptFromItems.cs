using System;
using System.Collections.Generic;
using System.IO;
using ISmart;

namespace SmartDevice.Steps;

public class ImputPromptFromItems : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).Items;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		Smart.Log.Debug(TAG, "itemList: " + text);
		string[] array = text.Split(new char[1] { ',' });
		List<string> imagePaths = new List<string>();
		if (((dynamic)base.Info.Args).ColorCheck != null && ((dynamic)base.Info.Args).ColorCheck == true)
		{
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				string filePathName = Smart.Rsd.GetFilePathName(text2, base.Recipe.Info.UseCase, device);
				imagePaths.Add(filePathName);
			}
		}
		string promptText = ((dynamic)base.Info.Args).PromptText;
		promptText = Smart.Locale.Xlate(promptText) + "\r\n";
		int num = array.Length;
		for (int j = 0; j < num; j++)
		{
			promptText = promptText + "\r\n\t" + (j + 1) + " - " + array[j];
		}
		string watermark = "1 - " + num;
		promptText = promptText + "\r\n\r\n" + Smart.Locale.Xlate("Enter your choice ") + "(" + watermark + ")";
		string title = Smart.Locale.Xlate(base.Info.Name);
		string empty = string.Empty;
		empty = ((imagePaths.Count <= 0) ? Smart.Thread.RunAndWait<string>((Func<string>)(() => device.Prompt.InputBox(title, promptText, watermark)), true) : Smart.Thread.RunAndWait<string>((Func<string>)(() => device.Prompt.FrontColorInputBox(title, promptText, imagePaths, watermark)), true));
		empty = empty.Trim();
		Smart.Log.Debug(TAG, $"User inputs {empty}");
		if (empty == string.Empty)
		{
			throw new InvalidDataException("User did not enter a response");
		}
		int num2 = int.Parse(empty);
		if (num2 < 1 || num2 > num)
		{
			throw new InvalidDataException("User input is out of range");
		}
		bool flag = false;
		if (((dynamic)base.Info.Args).CacheIndex != null)
		{
			flag = (bool)((dynamic)base.Info.Args).CacheIndex;
		}
		string text3 = ((dynamic)base.Info.Args).UserInput;
		string text4 = array[num2 - 1];
		if (flag)
		{
			text4 = (num2 - 1).ToString();
		}
		Smart.Log.Info(TAG, $"Using {text4} for {text3}");
		base.Cache[text3] = text4;
		SetPreCondition(array[num2 - 1]);
		LogPass();
	}
}
