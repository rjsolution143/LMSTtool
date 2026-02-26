using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using ISmart;
using Microsoft.Win32;

namespace SmartDevice.Steps;

public class RegistryCheck : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).RegistryKey.ToString();
		string text2 = null;
		if (((dynamic)base.Info.Args).RegistryValue != null)
		{
			text2 = ((dynamic)base.Info.Args).RegistryValue.ToString();
		}
		string text3 = null;
		if (((dynamic)base.Info.Args).Expected != null)
		{
			text3 = ((dynamic)base.Info.Args).Expected.ToString();
		}
		bool flag = false;
		if (((dynamic)base.Info.Args).Version != null)
		{
			flag = ((dynamic)base.Info.Args).Version;
		}
		RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(text);
		if (registryKey == null)
		{
			string text4 = $"Could not find specified registry key: {text}";
			Smart.Log.Error(TAG, text4);
			LogResult((Result)1, "Could not find registry key");
			PromptInstruction(device, "Could not find registry key");
			return;
		}
		object obj = null;
		if (text2 != null)
		{
			obj = registryKey.GetValue(text2);
			if (obj == null)
			{
				string text5 = $"Could not find specified registry value: {text} - {text2}";
				Smart.Log.Error(TAG, text5);
				LogResult((Result)1, "Could not find registry value");
				PromptInstruction(device, "Could not find registry value");
				return;
			}
			Smart.Log.Debug(TAG, $"Registry: {text} - {text2} has value: {obj}");
		}
		if (text3 != null)
		{
			string text6 = obj.ToString();
			if (flag)
			{
				Version version = new Version(text6);
				Version version2 = new Version(text3);
				if (version < version2)
				{
					string text7 = $"For registry version {text} - {text2} found {text6} instead of expected {text3} or greater";
					Smart.Log.Error(TAG, text7);
					LogResult((Result)1, "Registry version lower than expected");
					PromptInstruction(device, "Registry version lower than expected");
					return;
				}
				Smart.Log.Debug(TAG, $"Registry version: {text6} is equal or higher than Expected version: {text3}. The test is passed");
			}
			else
			{
				if (text6.Trim().ToLowerInvariant() != text3.Trim().ToLowerInvariant())
				{
					string text8 = $"For registry key/value {text} - {text2} found {text6} instead of expected {text3}";
					Smart.Log.Error(TAG, text8);
					LogResult((Result)1, "Registry value does not match expected value");
					PromptInstruction(device, "Registry value does not match expected value");
					return;
				}
				Smart.Log.Debug(TAG, $"Registry version: {text6} is equal to Expected version: {text3}. The test is passed");
			}
		}
		LogPass();
	}

	private void PromptInstruction(IDevice device, string errorMsg)
	{
		if (((dynamic)base.Info.Args).PromptText != null)
		{
			Task.Run(delegate
			{
				//IL_014f: Unknown result type (might be due to invalid IL or missing references)
				string text = ((dynamic)base.Info.Args).PromptText.ToString();
				text = string.Concat(Smart.Locale.Xlate("Error") + ": " + Smart.Locale.Xlate(errorMsg) + "\r\n", Smart.Locale.Xlate(text));
				string text2 = Smart.Locale.Xlate(base.Info.Name);
				device.Prompt.MessageBox(text2, text, (MessageBoxButtons)0, (MessageBoxIcon)64);
			});
		}
	}
}
