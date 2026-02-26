using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice;

public class UseCaseRunner : IUseCaseRunner
{
	private TimeSpan passedTimeout = TimeSpan.FromMinutes(5.0);

	private ITimedCache passedCache = Smart.NewTimedCache();

	private string TAG => GetType().FullName;

	public bool ResultsSaved(UseCase useCase, IDevice device)
	{
		string key = $"{device.SerialNumber}|{((object)(UseCase)(ref useCase)).ToString().ToLowerInvariant()}";
		return ((IDictionary<string, string>)passedCache).ContainsKey(key);
	}

	public void Run(UseCase useCase, IDevice device, bool newThread = true, bool skipPassed = false)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b46: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d82: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Invalid comparison between Unknown and I4
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Invalid comparison between Unknown and I4
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Invalid comparison between Unknown and I4
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Invalid comparison between Unknown and I4
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Invalid comparison between Unknown and I4
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Invalid comparison between Unknown and I4
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Invalid comparison between Unknown and I4
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Invalid comparison between Unknown and I4
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Invalid comparison between Unknown and I4
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Invalid comparison between Unknown and I4
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Invalid comparison between Unknown and I4
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Invalid comparison between Unknown and I4
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Invalid comparison between Unknown and I4
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Invalid comparison between Unknown and I4
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Invalid comparison between Unknown and I4
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Invalid comparison between Unknown and I4
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Invalid comparison between Unknown and I4
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Invalid comparison between Unknown and I4
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Invalid comparison between Unknown and I4
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Invalid comparison between Unknown and I4
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Invalid comparison between Unknown and I4
		//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ab: Invalid comparison between Unknown and I4
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Invalid comparison between Unknown and I4
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Invalid comparison between Unknown and I4
		//IL_08ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Invalid comparison between Unknown and I4
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Invalid comparison between Unknown and I4
		//IL_092e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0934: Invalid comparison between Unknown and I4
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Invalid comparison between Unknown and I4
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Invalid comparison between Unknown and I4
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_096a: Unknown result type (might be due to invalid IL or missing references)
		//IL_097a: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Invalid comparison between Unknown and I4
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Invalid comparison between Unknown and I4
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Invalid comparison between Unknown and I4
		//IL_0b71: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Invalid comparison between Unknown and I4
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Invalid comparison between Unknown and I4
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Invalid comparison between Unknown and I4
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Invalid comparison between Unknown and I4
		//IL_065a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Invalid comparison between Unknown and I4
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Invalid comparison between Unknown and I4
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Invalid comparison between Unknown and I4
		//IL_0cfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_067b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Invalid comparison between Unknown and I4
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Invalid comparison between Unknown and I4
		//IL_0691: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Invalid comparison between Unknown and I4
		//IL_069c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Invalid comparison between Unknown and I4
		//IL_0f07: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ad: Invalid comparison between Unknown and I4
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Invalid comparison between Unknown and I4
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Invalid comparison between Unknown and I4
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Invalid comparison between Unknown and I4
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Invalid comparison between Unknown and I4
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e4: Invalid comparison between Unknown and I4
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Invalid comparison between Unknown and I4
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fa: Invalid comparison between Unknown and I4
		//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Invalid comparison between Unknown and I4
		//IL_17fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1804: Invalid comparison between Unknown and I4
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0710: Invalid comparison between Unknown and I4
		//IL_18c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_18fd: Invalid comparison between Unknown and I4
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Invalid comparison between Unknown and I4
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Invalid comparison between Unknown and I4
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Invalid comparison between Unknown and I4
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_073c: Invalid comparison between Unknown and I4
		//IL_0741: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Invalid comparison between Unknown and I4
		//IL_074c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Invalid comparison between Unknown and I4
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Invalid comparison between Unknown and I4
		//IL_0762: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Invalid comparison between Unknown and I4
		//IL_0832: Unknown result type (might be due to invalid IL or missing references)
		//IL_0842: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_089e: Unknown result type (might be due to invalid IL or missing references)
		//IL_176d: Unknown result type (might be due to invalid IL or missing references)
		//IL_177d: Unknown result type (might be due to invalid IL or missing references)
		Smart.Log.Info(TAG, $"Running {useCase} use case");
		List<UseCase> list = Smart.Rsd.UseCaseLocked();
		if (list != null && list.Count > 0)
		{
			Smart.Log.Debug(TAG, $"Since the LockedUseCase option is set to true, all dialogs for use case {list[0]} will be disabled");
		}
		if (!Smart.Web.BrowserSessionActive && (int)useCase != 134 && (int)useCase != 141 && (int)useCase != 950 && (int)useCase != 167 && (int)useCase != 951 && (int)useCase != 952)
		{
			string format = Smart.Locale.Xlate("Cannot Start Use Case");
			string text = "Session has expired. Please login to RSD via local web browser";
			Smart.Log.Error(TAG, text);
			text = Smart.Locale.Xlate(text);
			device.Prompt.MessageBox(string.Format(format, useCase), text, (MessageBoxButtons)0, (MessageBoxIcon)16);
			return;
		}
		IRecipeInfo val = Smart.NewRecipeInfo();
		if (device == null)
		{
			if (list != null && list.Count > 0 && (int)list[0] == 211)
			{
				Smart.Log.Error(TAG, "Cannot start use case due to no device connected");
				return;
			}
			string format2 = Smart.Locale.Xlate("Cannot Start Use Case");
			string text2 = "No device connected";
			Smart.Log.Error(TAG, text2);
			text2 = Smart.Locale.Xlate(text2);
			device.Prompt.MessageBox(string.Format(format2, useCase), text2, (MessageBoxButtons)0, (MessageBoxIcon)16);
			return;
		}
		if (device.Locked)
		{
			if ((int)useCase == 141)
			{
				Smart.Log.Error(TAG, $"Attempted to run LMST_Detect on device in use {device.Unique}");
				return;
			}
			if (list != null && list.Count > 0 && (int)list[0] == 211)
			{
				Smart.Log.Error(TAG, "The device is locked when using auto killswitch use case");
				return;
			}
			string format3 = Smart.Locale.Xlate("Device In Use");
			string text3 = "Please wait for device to complete processing";
			Smart.Log.Error(TAG, text3);
			text3 = Smart.Locale.Xlate(text3);
			device.Prompt.MessageBox(string.Format(format3, useCase), text3, (MessageBoxButtons)0, (MessageBoxIcon)16);
			if ((int)useCase != 164)
			{
				return;
			}
		}
		if (device.Removed)
		{
			if (list != null && list.Count > 0 && (int)list[0] == 211)
			{
				Smart.Log.Error(TAG, "Cannot start use case due to the device is removed");
				return;
			}
			string format4 = Smart.Locale.Xlate("Cannot Start Use Case");
			string text4 = "Please disconnect and reconnect the device";
			Smart.Log.Error(TAG, text4);
			text4 = Smart.Locale.Xlate(text4);
			device.Prompt.MessageBox(string.Format(format4, useCase), text4, (MessageBoxButtons)0, (MessageBoxIcon)16);
			return;
		}
		if (device.ZeroTouchDevice)
		{
			if (list == null || list.Count <= 0 || (int)list[0] != 211)
			{
				string format5 = Smart.Locale.Xlate("Cannot Start Use Case");
				string text5 = "Cannot run use case on Zero Touch device";
				Smart.Log.Error(TAG, text5);
				text5 = Smart.Locale.Xlate(text5);
				device.Prompt.MessageBox(string.Format(format5, useCase), text5, (MessageBoxButtons)0, (MessageBoxIcon)16);
				return;
			}
			Smart.Log.Error(TAG, "Zero Touch device allow to run " + useCase);
		}
		if (device.OutOfProfile && (int)useCase != 134 && (int)useCase != 141 && (int)useCase != 163 && (int)useCase != 951 && (int)useCase != 952 && (int)useCase != 185 && (int)useCase != 164)
		{
			string format6 = Smart.Locale.Xlate("Cannot Start Use Case");
			string text6 = "Cannot run use case on device not in RSD user profile";
			Smart.Log.Error(TAG, text6);
			text6 = Smart.Locale.Xlate(text6);
			device.Prompt.MessageBox(string.Format(format6, useCase), text6, (MessageBoxButtons)0, (MessageBoxIcon)16);
			return;
		}
		if (device.ManualDevice && (device.SerialNumber == null || device.SerialNumber == string.Empty || device.SerialNumber == "UNKNOWN") && ((int)useCase == 166 || (int)useCase == 903))
		{
			string format7 = Smart.Locale.Xlate("Cannot Start Use Case");
			string text7 = "Please Enter IMEI Number";
			Smart.Log.Error(TAG, text7);
			text7 = Smart.Locale.Xlate(text7);
			device.Prompt.MessageBox(string.Format(format7, useCase), text7, (MessageBoxButtons)0, (MessageBoxIcon)16);
			return;
		}
		if ((int)useCase == 135 || (int)useCase == 195)
		{
			bool flag = false;
			foreach (IDevice value in Smart.DeviceManager.Devices.Values)
			{
				if (value.Locked)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				string format8 = Smart.Locale.Xlate("Device In Use");
				string text8 = "Please wait for all devices to complete processing";
				Smart.Log.Error(TAG, text8);
				text8 = Smart.Locale.Xlate(text8);
				device.Prompt.MessageBox(string.Format(format8, useCase), text8, (MessageBoxButtons)0, (MessageBoxIcon)16);
				return;
			}
		}
		string empty = string.Empty;
		if ((int)useCase != 134 && (int)useCase != 141 && (int)useCase != 142 && (int)useCase != 211 && (int)useCase != 147 && (int)useCase != 135 && (int)useCase != 146 && (int)useCase != 150 && (int)useCase != 162 && (int)useCase != 163 && (int)useCase != 164 && (int)useCase != 176 && (int)useCase != 180 && (int)useCase != 182 && (int)useCase != 185 && (int)useCase != 153 && (int)useCase != 144 && (int)useCase != 158 && (int)useCase != 157 && (int)useCase != 183 && (int)useCase != 195 && (int)useCase != 166 && (int)useCase != 902 && (int)useCase != 900 && (int)useCase != 901 && (int)useCase != 903 && (int)useCase != 198 && (int)useCase != 906 && (int)useCase != 907 && (int)useCase != 908 && (int)useCase != 143 && (int)useCase != 204 && (int)useCase != 205 && (int)useCase != 209 && (int)useCase != 210 && (int)useCase != 187 && (device.Log == null || !device.Log.Info.ContainsKey("DeviceRead") || device.Log.Info["DeviceRead"] != "Success"))
		{
			if (device.ModelId == string.Empty || device.ModelId == "UNKNOWN")
			{
				string format9 = Smart.Locale.Xlate("Manual Carrier/Model Selection");
				string text9 = "Device Carrier/Model was not detected. Please use the Welcome use case to manually select device Carrier/Model.";
				Smart.Log.Error(TAG, text9);
				text9 = Smart.Locale.Xlate(text9);
				device.Prompt.MessageBox(string.Format(format9, useCase), text9, (MessageBoxButtons)0, (MessageBoxIcon)16);
			}
			else
			{
				string format10 = Smart.Locale.Xlate("Cannot Start Use Case");
				string text10 = "Device info was not read correctly. Re-connect device to re-attempt.";
				Smart.Log.Error(TAG, text10);
				text10 = Smart.Locale.Xlate(text10);
				device.Prompt.MessageBox(string.Format(format10, useCase), text10, (MessageBoxButtons)0, (MessageBoxIcon)16);
			}
			return;
		}
		if ((int)useCase == 144 && !device.ManualDevice && !device.Communicating)
		{
			Smart.Log.Error(TAG, "Warning, could not communicate with connected device!");
		}
		string modelId = device.ModelId;
		string text11 = Smart.Rsd.LoadRecipe(useCase, ref modelId, device);
		device.ModelId = modelId;
		if (text11 == string.Empty)
		{
			Smart.Log.Error(TAG, "No recipe associated with connected device");
			if ((int)useCase != 134)
			{
				string format11 = Smart.Locale.Xlate("Cannot Start Use Case");
				string text12 = "No recipe associated with connected device: Please check your profile or connected device does NOT have released software.";
				text12 = Smart.Locale.Xlate(text12);
				device.Prompt.MessageBox(string.Format(format11, useCase), text12, (MessageBoxButtons)0, (MessageBoxIcon)16);
			}
			return;
		}
		if (!File.Exists(text11))
		{
			Smart.Log.Error(TAG, $"Recipe {text11} does not exist.");
			string format12 = Smart.Locale.Xlate("Cannot Start Use Case");
			string text13 = Smart.Locale.Xlate("Recipe ") + text11 + Smart.Locale.Xlate(" does not exist");
			device.Prompt.MessageBox(string.Format(format12, useCase), text13, (MessageBoxButtons)0, (MessageBoxIcon)16);
			return;
		}
		if (Path.GetFileName(text11).ToLowerInvariant().Trim()
			.StartsWith("notsupported"))
		{
			string format13 = Smart.Locale.Xlate("{0} Not Supported");
			string text14 = "Currently this Use case is NOT supported for connected model";
			Smart.Log.Error(TAG, string.Format(text14, ((object)(UseCase)(ref useCase)).ToString(), device.ModelId));
			text14 = Smart.Locale.Xlate(text14);
			device.Prompt.MessageBox(string.Format(format13, useCase), string.Format(text14, ((object)(UseCase)(ref useCase)).ToString(), device.ModelId), (MessageBoxButtons)0, (MessageBoxIcon)16);
			return;
		}
		try
		{
			Smart.Log.Verbose(TAG, $"Final recipe for use case {((object)(UseCase)(ref useCase)).ToString()} is {text11}");
			empty = Smart.Rsd.ReadRecipeContent(text11);
		}
		catch (Exception ex)
		{
			string format14 = Smart.Locale.Xlate("Cannot Start Use Case");
			string text15 = "Failed to read content of file:{0}, Exception:{1}";
			Smart.Log.Error(TAG, string.Format(text15, text11, ex.Message));
			text15 = Smart.Locale.Xlate(text15);
			device.Prompt.MessageBox(string.Format(format14, useCase), string.Format(text15, text11, ex.Message), (MessageBoxButtons)0, (MessageBoxIcon)16);
			return;
		}
		val.Load(empty, useCase, text11);
		if (((IDictionary<string, object>)(dynamic)val.Args).ContainsKey("BaseRecipe"))
		{
			string text16 = ((dynamic)val.Args).BaseRecipe;
			try
			{
				string directoryName = Path.GetDirectoryName(text11);
				List<string> list2 = Smart.File.FindFiles(text16 + "_*.json", directoryName, true);
				if (list2.Count < 1)
				{
					throw new FileNotFoundException($"Could not find recipe base file '{text16}'");
				}
				if (list2.Count > 1)
				{
					Smart.Log.Warning(TAG, $"Found {list2.Count} conflicting recipe base files for '{text16}'!");
				}
				string text17 = list2[0];
				Path.GetFileName(text17);
				string text18 = Smart.Rsd.ReadRecipeContent(text17);
				IRecipeInfo val2 = Smart.NewRecipeInfo();
				val2.Load(text18, useCase, text17);
				val = val.ExtendBase(val2);
			}
			catch (Exception ex2)
			{
				string format15 = Smart.Locale.Xlate("Cannot Start Use Case");
				string text19 = "Failed to read content of file:{0}, Exception:{1}";
				Smart.Log.Error(TAG, string.Format(text19, text16, ex2.Message));
				text19 = Smart.Locale.Xlate(text19);
				device.Prompt.MessageBox(string.Format(format15, useCase), string.Format(text19, text11, ex2.Message), (MessageBoxButtons)0, (MessageBoxIcon)16);
				return;
			}
		}
		SortedList<string, IRecipeInfo> sortedList = new SortedList<string, IRecipeInfo>();
		foreach (IStepInfo step in val.Steps)
		{
			if (!(step.Step.ToLowerInvariant().Trim() == "expandstep"))
			{
				continue;
			}
			string name = step.Name;
			string text20 = ((dynamic)step.Args).LinkRecipe;
			try
			{
				string directoryName2 = Path.GetDirectoryName(text11);
				List<string> list3 = Smart.File.FindFiles(text20 + "_*.json", directoryName2, true);
				if (list3.Count < 1)
				{
					throw new FileNotFoundException($"Could not find recipe base file '{text20}'");
				}
				if (list3.Count > 1)
				{
					Smart.Log.Warning(TAG, $"Found {list3.Count} conflicting recipe base files for '{text20}'!");
				}
				string text21 = list3[0];
				Path.GetFileName(text21);
				string text22 = Smart.Rsd.ReadRecipeContent(text21);
				IRecipeInfo val3 = Smart.NewRecipeInfo();
				val3.Load(text22, useCase, text21);
				sortedList[name] = val3;
			}
			catch (Exception ex3)
			{
				string format16 = Smart.Locale.Xlate("Cannot Start Use Case");
				string text23 = "Failed to read content of file:{0}, Exception:{1}";
				Smart.Log.Error(TAG, string.Format(text23, text20, ex3.Message));
				text23 = Smart.Locale.Xlate(text23);
				device.Prompt.MessageBox(string.Format(format16, useCase), string.Format(text23, text11, ex3.Message), (MessageBoxButtons)0, (MessageBoxIcon)16);
				return;
			}
		}
		foreach (string key2 in sortedList.Keys)
		{
			Smart.Log.Debug(TAG, "Expanding Step: " + key2);
			IRecipeInfo val4 = sortedList[key2];
			val = val.ExpandStep(key2, val4);
		}
		((dynamic)val.Args).Device = device;
		IThreadLocked val5 = Smart.Rsd.LocalOptions();
		try
		{
			dynamic data = val5.Data;
			((dynamic)val.Args).Options = data;
		}
		finally
		{
			((IDisposable)val5)?.Dispose();
		}
		if (skipPassed)
		{
			lock (passedCache)
			{
				string key = $"{device.SerialNumber}|{((object)(UseCase)(ref useCase)).ToString().ToLowerInvariant()}";
				if (((IDictionary<string, string>)passedCache).ContainsKey(key))
				{
					List<string> source = new List<string>(((IDictionary<string, string>)passedCache)[key].Split(new char[1] { '|' }));
					source = source.Distinct().ToList();
					Smart.Log.Debug(TAG, $"Skipping {source.Count} passed tests: {Smart.Convert.ToCommaSeparated((IEnumerable)source)}");
					val.Filter(source, true);
				}
				else
				{
					Smart.Log.Debug(TAG, "No passed tests found to skip");
				}
				if (val.Steps.Count < 1)
				{
					Smart.Log.Error(TAG, "All tests passed");
					return;
				}
			}
		}
		Smart.Log.Verbose(TAG, ((object)val).ToString());
		foreach (StepInfo step2 in val.Steps)
		{
			if (!(step2.Step.ToLowerInvariant() != "ValidateToken".ToLowerInvariant()))
			{
				dynamic val6 = step2.Args.PreCondTest != null || step2.Args.PreCondValue != null || step2.Args.VerifyOnly != null;
				if (!(val6 ? true : false) && !((val6 | (step2.Args.ForceStopOnFail != null)) ? true : false) && !Smart.Web.TokenConnected && !Smart.Web.ValidateToken())
				{
					string format17 = Smart.Locale.Xlate("Cannot Start Use Case");
					string text24 = "eToken is missing or invalid";
					text24 = Smart.Locale.Xlate(text24);
					device.Prompt.MessageBox(string.Format(format17, useCase), text24, (MessageBoxButtons)0, (MessageBoxIcon)16);
					return;
				}
			}
		}
		IRecipe recipe = Smart.NewRecipe();
		recipe.Load(val);
		if (device.Log != null)
		{
			recipe.Log.CopyInfo(device.Log.Info);
		}
		device.Log = recipe.Log;
		if ((int)useCase != 141)
		{
			device.Log.SubLogs.Add((IResultSubLogger)(object)new RsdLogger(device));
			device.Log.SubLogs.Add((IResultSubLogger)(object)new MqsLogger(device));
			device.Log.SubLogs.Add((IResultSubLogger)(object)new XmlLogger(device));
			device.Log.SubLogs.Add(Smart.Troubleshooting.NewTroubleshootingLogger(device));
			device.Log.SubLogs.Add((IResultSubLogger)(object)new CallbackLogger(device, SaveResult));
		}
		recipe.Log.UseCase = useCase;
		Smart.Net.WebHit(((object)(UseCase)(ref useCase)).ToString());
		ThreadStart threadStart = recipe.Run;
		if ((int)useCase == 141)
		{
			threadStart = delegate
			{
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_001c: Invalid comparison between Unknown and I4
				recipe.Run();
				if ((int)recipe.Log.OverallResult == 8)
				{
					Smart.Log.Debug(TAG, "Running device read");
					Run((UseCase)134, device, newThread: false);
				}
			};
		}
		Thread thread = Smart.Thread.RunThread(threadStart);
		if (!newThread)
		{
			thread.Join();
		}
	}

	private void SaveResult(IDevice device, string name, SortedList<string, dynamic> details)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Invalid comparison between Unknown and I4
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		string serialNumber = device.SerialNumber;
		if (serialNumber.Trim() == string.Empty || (int)(Result)details["result"] != 8)
		{
			return;
		}
		UseCase useCase = device.Log.UseCase;
		string arg = ((object)(UseCase)(ref useCase)).ToString().ToLowerInvariant();
		string text = $"{serialNumber}|{arg}";
		lock (passedCache)
		{
			string text2 = name;
			if (((IDictionary<string, string>)passedCache).ContainsKey(text))
			{
				text2 = $"{((IDictionary<string, string>)passedCache)[text]}|{name}";
				((IDictionary<string, string>)passedCache).Remove(text);
			}
			passedCache.Add(text, text2, passedTimeout);
		}
	}
}
