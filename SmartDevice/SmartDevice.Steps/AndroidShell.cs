using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class AndroidShell : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0723: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d93: Invalid comparison between Unknown and I4
		//IL_0d95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d98: Invalid comparison between Unknown and I4
		//IL_0dad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9e: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).Command;
		string empty = string.Empty;
		string text2 = text;
		if (((dynamic)base.Info.Args).Format != null)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			foreach (object item2 in ((dynamic)base.Info.Args).Format)
			{
				string text3 = (string)(dynamic)item2;
				string text4 = BaseStep.VariableSubstitution(text3);
				if (text3.Contains("<") && text3.Contains(">"))
				{
					base.Cache.Add(text3.Replace("<", "").Replace(">", ""), text4);
				}
				string item = text4;
				if (text3.StartsWith("$"))
				{
					string text5 = text3.Substring(1);
					text4 = base.Cache[text5];
					switch (text5.ToLower())
					{
					case "user":
					case "pwd":
					case "ruser":
					case "rpwd":
						item = Smart.Security.EncryptString(text4);
						break;
					default:
						item = text4;
						break;
					}
				}
				list.Add(text4);
				list2.Add(item);
			}
			text = string.Format(text, list.ToArray());
			text2 = string.Format(text2, list2.ToArray());
		}
		try
		{
			empty = Smart.ADB.Shell(val.ID, text, 10000).Trim();
			Smart.Log.Debug(TAG, $"Command '{text2}' has response {empty}");
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error sending ADB shell command: " + ex.Message);
			Smart.Log.Error(TAG, ex.ToString());
			LogResult((Result)4, "Could not send ADB shell command", ex.Message);
			Thread.Sleep(5000);
			return;
		}
		if (((dynamic)base.Info.Args).PromptText != null)
		{
			string text6 = ((dynamic)base.Info.Args).PromptText.ToString();
			text6 = Smart.Locale.Xlate(text6);
			string text7 = Smart.Locale.Xlate(base.Info.Name);
			val.Prompt.MessageBox(text7, text6, (MessageBoxButtons)0, (MessageBoxIcon)64);
		}
		if (((dynamic)base.Info.Args).RemovedText != null)
		{
			string oldValue = ((dynamic)base.Info.Args).RemovedText.ToString();
			empty = empty.Replace(oldValue, string.Empty);
		}
		string contained = ((dynamic)base.Info.Args).Expected;
		string notContained = ((dynamic)base.Info.Args).NotExpected;
		Result result = VerifyContainedPropertyValue(contained, notContained, empty);
		string key = text;
		if (((dynamic)base.Info.Args).InputName != null && ((dynamic)base.Info.Args).InputName != string.Empty)
		{
			key = ((dynamic)base.Info.Args).InputName.ToString();
		}
		base.Cache[key] = empty;
		SetPreCondition(empty);
		VerifyOnly(ref result);
		if (((int)result == 1 || (int)result == 4) && empty != null)
		{
			LogResult(result, "Android Shell command failed", empty);
		}
		else
		{
			LogResult(result);
		}
	}
}
