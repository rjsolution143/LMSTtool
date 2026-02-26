using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice;

public class Recipe : IRecipe
{
	private string TAG => GetType().FullName;

	public IRecipeInfo Info { get; private set; }

	public IResultLogger Log { get; set; }

	public List<IStep> Steps { get; private set; }

	public SortedList<string, dynamic> Cache { get; private set; }

	public void Load(IRecipeInfo info)
	{
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c8: Invalid comparison between Unknown and I4
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Invalid comparison between Unknown and I4
		Info = info;
		dynamic val = ((dynamic)info.Args).Options;
		Steps = new List<IStep>();
		double num = 0.0;
		double num2 = 100.0 / (double)info.Steps.Count;
		foreach (IStepInfo step in info.Steps)
		{
			double num3 = 1.0;
			if (((dynamic)step.Args)["ProgressFactor"] != null)
			{
				num3 = ((dynamic)step.Args)["ProgressFactor"];
			}
			num += num3 * num2;
		}
		double num4 = 0.0;
		foreach (IStepInfo step2 in info.Steps)
		{
			Smart.Log.Verbose(TAG, $"Adding step {step2.Step}");
			IStep val2;
			try
			{
				val2 = Smart.NewStep(step2.Step);
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, $"Error creationg step {step2.Step}");
				Smart.Log.Verbose(TAG, ex.ToString());
				Smart.PrintBase();
				val2 = Smart.NewStep("NotSupported");
			}
			((dynamic)step2.Args).ProgressStart = num4;
			double num5 = 1.0;
			if (((dynamic)step2.Args)["ProgressFactor"] != null)
			{
				num5 = ((dynamic)step2.Args)["ProgressFactor"];
			}
			double num6 = num5 * num2 / num;
			double num7 = num4 + num6 * 100.0;
			((dynamic)step2.Args).ProgressEnd = num7;
			num4 = num7;
			if (((int)info.UseCase == 131 || (int)info.UseCase == 168) && val["CITRetest"] != null)
			{
				((dynamic)step2.Args).Retest = val["CITRetest"];
			}
			val2.Load((IRecipe)(object)this, step2);
			Steps.Add(val2);
		}
		((dynamic)Steps.Last().Info.Args).ProgressEnd = 100.0;
		Log = Smart.NewResultLogger();
		Cache = new SortedList<string, object>();
	}

	public void Run()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a94: Unknown result type (might be due to invalid IL or missing references)
		//IL_2fd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_380b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3811: Invalid comparison between Unknown and I4
		//IL_38b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_38be: Invalid comparison between Unknown and I4
		//IL_381c: Unknown result type (might be due to invalid IL or missing references)
		//IL_3826: Invalid comparison between Unknown and I4
		//IL_3912: Unknown result type (might be due to invalid IL or missing references)
		//IL_391c: Invalid comparison between Unknown and I4
		//IL_38c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_38d0: Invalid comparison between Unknown and I4
		//IL_382e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3838: Invalid comparison between Unknown and I4
		//IL_3840: Unknown result type (might be due to invalid IL or missing references)
		//IL_384a: Invalid comparison between Unknown and I4
		//IL_1c23: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_30fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_3182: Unknown result type (might be due to invalid IL or missing references)
		//IL_3127: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a94: Unknown result type (might be due to invalid IL or missing references)
		//IL_3567: Unknown result type (might be due to invalid IL or missing references)
		//IL_356d: Invalid comparison between Unknown and I4
		//IL_39b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_39c0: Invalid comparison between Unknown and I4
		//IL_3ace: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ad3: Unknown result type (might be due to invalid IL or missing references)
		//IL_39cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_39d5: Invalid comparison between Unknown and I4
		//IL_3b0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b18: Invalid comparison between Unknown and I4
		//IL_39dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_39e7: Invalid comparison between Unknown and I4
		//IL_3b42: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b47: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b20: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b2a: Invalid comparison between Unknown and I4
		//IL_3afd: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b03: Invalid comparison between Unknown and I4
		//IL_39ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_39f9: Invalid comparison between Unknown and I4
		//IL_0ff0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff6: Invalid comparison between Unknown and I4
		//IL_3a01: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a0b: Invalid comparison between Unknown and I4
		//IL_3a13: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a1d: Invalid comparison between Unknown and I4
		//IL_3bb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_3bbf: Invalid comparison between Unknown and I4
		//IL_3a25: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a2f: Invalid comparison between Unknown and I4
		//IL_3a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a41: Invalid comparison between Unknown and I4
		//IL_3cb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_37df: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f5: Invalid comparison between Unknown and I4
		//IL_29ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_11fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1203: Invalid comparison between Unknown and I4
		//IL_3d8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d93: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d95: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d98: Invalid comparison between Unknown and I4
		//IL_120b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d9d: Invalid comparison between Unknown and I4
		//IL_0a16: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a20: Invalid comparison between Unknown and I4
		//IL_3d9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3da2: Invalid comparison between Unknown and I4
		//IL_0a28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a32: Invalid comparison between Unknown and I4
		//IL_1a81: Unknown result type (might be due to invalid IL or missing references)
		//IL_1447: Unknown result type (might be due to invalid IL or missing references)
		//IL_144d: Invalid comparison between Unknown and I4
		//IL_3da4: Unknown result type (might be due to invalid IL or missing references)
		//IL_3da7: Invalid comparison between Unknown and I4
		//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6d: Invalid comparison between Unknown and I4
		//IL_0a3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a44: Invalid comparison between Unknown and I4
		//IL_1a85: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1455: Unknown result type (might be due to invalid IL or missing references)
		//IL_145b: Invalid comparison between Unknown and I4
		//IL_3da9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7f: Invalid comparison between Unknown and I4
		//IL_0a4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a56: Invalid comparison between Unknown and I4
		//IL_1c5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c5f: Invalid comparison between Unknown and I4
		//IL_1463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a87: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a91: Invalid comparison between Unknown and I4
		//IL_1c61: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c64: Invalid comparison between Unknown and I4
		//IL_0a99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa3: Invalid comparison between Unknown and I4
		//IL_1dc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dce: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dd1: Invalid comparison between Unknown and I4
		//IL_0aab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab5: Invalid comparison between Unknown and I4
		Smart.Log.Debug(TAG, $"Running Recipe {Info.Name}");
		StationDescriptor stationDescriptor = Smart.Rsd.GetStationDescriptor();
		string shopId = ((StationDescriptor)(ref stationDescriptor)).ShopId;
		Cache["MASC"] = shopId;
		IDevice val = (IDevice)((dynamic)Info.Args).Device;
		if (val != null)
		{
			val.Locked = true;
			Smart.Log.Debug(TAG, $"Device LOCKED: {val.ID}");
			Smart.Log.Debug(TAG, ((object)val).ToString());
			val.MultiUp = false;
			if (val.UserSerialNumber != null && val.UserSerialNumber != string.Empty)
			{
				Cache["SerialNumberIn"] = val.UserSerialNumber;
				Smart.Log.Debug(TAG, $"Using User Entered SerialNumberIn: {val.UserSerialNumber}");
			}
		}
		bool flag = true;
		bool flag2 = false;
		bool flag3 = false;
		if (((dynamic)Info.Args).Options.CITStopOnFail != null)
		{
			flag2 = ((dynamic)Info.Args).Options.CITStopOnFail;
		}
		bool flag4 = false;
		bool flag5 = false;
		if (((dynamic)Info.Args).Options.AutoReport != null)
		{
			flag4 = ((dynamic)Info.Args).Options.AutoReport;
		}
		if (((dynamic)Info.Args).Options.AutoFailureReport != null)
		{
			flag5 = ((dynamic)Info.Args).Options.AutoFailureReport;
		}
		string autoLogDir = string.Empty;
		if (((dynamic)Info.Args).Options.SavePath != null)
		{
			autoLogDir = ((dynamic)Info.Args).Options.SavePath;
		}
		bool flag6 = (int)Info.UseCase == 131 || (int)Info.UseCase == 168 || (int)Info.UseCase == 205 || (int)Info.UseCase == 210;
		bool flag7 = (int)Info.UseCase == 166 || (int)Info.UseCase == 902 || (int)Info.UseCase == 900 || (int)Info.UseCase == 901 || (int)Info.UseCase == 903;
		if ((flag6 && !flag2) || flag7)
		{
			flag = false;
			if (flag6 && ((dynamic)Info.Args).Options.CITStopOnAbort != null)
			{
				flag3 = ((dynamic)Info.Args).Options.CITStopOnAbort;
			}
		}
		if (val != null && val.ManualDevice)
		{
			Log.AddInfo("ManualSerialNumber", val.SerialNumber);
			Log.AddInfo("ManualSerialNumber2", val.SerialNumber2);
		}
		else if (val != null)
		{
			Log.AddInfo("PhysicalSnStart", val.SerialNumber);
			Log.AddInfo("PhysicalSnStart2", val.SerialNumber2);
			if (val.UserSerialNumber != null && val.UserSerialNumber != string.Empty && val.UserSerialNumber != "UNKNOWN")
			{
				Log.AddInfo("ManualSerialNumber", val.UserSerialNumber);
			}
		}
		Log.AddInfo("Retried", "0");
		try
		{
			foreach (IStep step in Steps)
			{
				bool flag8 = false;
				Log.CurrentStep = step.Info.Name;
				bool flag9 = true;
				if (((dynamic)step.Info.Args)["AllowSkip"] != null)
				{
					flag9 = ((dynamic)step.Info.Args).AllowSkip;
				}
				bool userQuit = Log.UserQuit;
				bool flag10 = (int)Log.OverallResult == 1 && flag;
				bool flag11 = false;
				if (((dynamic)step.Info.Args)["Requires"] != null)
				{
					string[] array = ((string)((dynamic)step.Info.Args).Requires).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
					List<string> passedSteps = Log.PassedSteps;
					string[] array2 = array;
					foreach (string text in array2)
					{
						if (!passedSteps.Contains(text.ToLowerInvariant().Trim()) && (!(text.ToLowerInvariant() == "overallresult") || ((int)Log.OverallResult != 8 && (int)Log.OverallResult != 7 && (int)Log.OverallResult != 0)))
						{
							Smart.Log.Debug(TAG, $"Step {step.Info.Name} canceled due to required pass {text} missing");
							flag11 = true;
						}
					}
				}
				if (((dynamic)step.Info.Args)["RequireFail"] != null)
				{
					string text2 = ((dynamic)step.Info.Args).RequireFail;
					string[] array3 = text2.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
					List<string> passedSteps2 = Log.PassedSteps;
					bool flag12 = false;
					string[] array2 = array3;
					foreach (string text3 in array2)
					{
						if (passedSteps2.Contains(text3.ToLowerInvariant().Trim()))
						{
							flag12 = true;
							break;
						}
						if (text3.ToLowerInvariant() == "overallresult" && ((int)Log.OverallResult == 8 || (int)Log.OverallResult == 7 || (int)Log.OverallResult == 0))
						{
							flag12 = true;
							break;
						}
					}
					if (flag12)
					{
						Smart.Log.Debug(TAG, $"Step {step.Info.Name} skipped due to required fail {text2} passing");
						Log.AddResult(step.Info.Name, step.Info.Step, (Result)7, "", "", "", double.MinValue, double.MinValue, double.MinValue, (SortedList<string, object>)null);
						continue;
					}
				}
				if (((dynamic)step.Info.Args)["RequiredOption"] != null)
				{
					string text4 = ((dynamic)step.Info.Args).RequiredOption;
					bool flag13 = true;
					if ((((dynamic)Info.Args).Options[text4] != null) && (bool)((dynamic)Info.Args).Options[text4])
					{
						flag13 = false;
					}
					if (flag13)
					{
						Smart.Log.Debug(TAG, $"Step {step.Info.Name} skipped due to required option {text4} missing");
						Log.AddResult(step.Info.Name, step.Info.Step, (Result)7, "", "", "", double.MinValue, double.MinValue, double.MinValue, (SortedList<string, object>)null);
						continue;
					}
				}
				if (flag9 && (userQuit || flag10))
				{
					flag11 = true;
				}
				if (flag11)
				{
					Smart.Log.Verbose(TAG, $"Canceled step {step.Info.Name} ({step.Info.Step})");
					Log.AddResult(step.Info.Name, step.Info.Step, (Result)2, "", "", "", double.MinValue, double.MinValue, double.MinValue, (SortedList<string, object>)null);
					Log.Progress = ((dynamic)step.Info.Args).ProgressEnd;
					continue;
				}
				Result val2 = (Result)0;
				try
				{
					val2 = step.Audit();
				}
				catch (Exception ex)
				{
					Result val3 = (Result)4;
					if (((dynamic)step.Info.Args).IgnoreFail == true)
					{
						Smart.Log.Debug(TAG, $"Ignoring test status {((object)(Result)(ref val3)).ToString()}, skipping result for {step.Info.Name}");
						val3 = (Result)7;
					}
					string text5 = $"Error during audit '{step.Info.Name}': {ex.Message}";
					Smart.Log.Error(TAG, text5);
					Smart.Log.Verbose(TAG, ex.ToString());
					Log.AddResult(step.Info.Name, step.Info.Step, val3, ex.Message, "", "", double.MinValue, double.MinValue, double.MinValue, (SortedList<string, object>)null);
					continue;
				}
				if ((int)val2 == 3 || (int)val2 == 1)
				{
					Smart.Log.Error(TAG, $"Audit failed for step {step.Info.Name} ({step.Info.Step})");
					Smart.UserAccounts.CurrentUser.PassStreak = 0;
					IUserAccount currentUser = Smart.UserAccounts.CurrentUser;
					currentUser.SessionPoints -= 100;
					if (Smart.UserAccounts.CurrentUser.QualityPoints >= -22)
					{
						IUserAccount currentUser2 = Smart.UserAccounts.CurrentUser;
						currentUser2.QualityPoints -= 1;
						Smart.Log.Debug(TAG, $"User quality points decreased to {Smart.UserAccounts.CurrentUser.QualityPoints}");
						Smart.UserAccounts.SaveQuality();
					}
					Smart.Log.Debug(TAG, "User session points decreased by 100 due to failed audit");
					Log.AddResult($"{step.Info.Name}-Audit", step.Info.Step, (Result)3, "", "", "", double.MinValue, double.MinValue, double.MinValue, (SortedList<string, object>)null);
					string text6 = Smart.Locale.Xlate("Audit Failed");
					string text7 = Smart.Locale.Xlate("WARNING: You have failed a random operator audit. This failure has been logged and will be reviewed. Testing will now resume. Please follow all testing instructions carefully.");
					val.Prompt.MessageBox(text6, text7, (MessageBoxButtons)0, (MessageBoxIcon)48);
				}
				else if ((int)val2 == 8)
				{
					Smart.Log.Debug(TAG, $"Audit passed for step {step.Info.Name} ({step.Info.Step})");
					IUserAccount currentUser3 = Smart.UserAccounts.CurrentUser;
					currentUser3.PassStreak += 1;
					if (Smart.UserAccounts.CurrentUser.PassStreak > 9)
					{
						Smart.UserAccounts.CurrentUser.PassStreak = 0;
						IUserAccount currentUser4 = Smart.UserAccounts.CurrentUser;
						currentUser4.SessionPoints += 300;
						Smart.Log.Debug(TAG, "User session points increased by 300 due to 10 passed audits");
					}
					if (Smart.UserAccounts.CurrentUser.QualityPoints <= 22)
					{
						IUserAccount currentUser5 = Smart.UserAccounts.CurrentUser;
						currentUser5.QualityPoints += 1;
						Smart.Log.Debug(TAG, $"User quality points increased to {Smart.UserAccounts.CurrentUser.QualityPoints}");
						Smart.UserAccounts.SaveQuality();
					}
					IUserAccount currentUser6 = Smart.UserAccounts.CurrentUser;
					currentUser6.SessionPoints += 100;
					Smart.Log.Debug(TAG, "User session points increased by 100 due to passed audit");
					Log.AddResult($"{step.Info.Name}-Audit", step.Info.Step, (Result)8, "", "", "", double.MinValue, double.MinValue, double.MinValue, (SortedList<string, object>)null);
				}
				if (val != null && !val.MultiUp && Smart.DeviceManager.Devices.Count > 1)
				{
					foreach (IDevice value in Smart.DeviceManager.Devices.Values)
					{
						if (!(value.ID == val.ID) && !value.ManualDevice && value.Locked)
						{
							val.MultiUp = true;
							break;
						}
					}
				}
				try
				{
					object obj = new object();
					string text8 = "Default";
					if (((dynamic)step.Info.Args)["StepLockName"] != null)
					{
						text8 = ((dynamic)step.Info.Args)["StepLockName"];
						string key = text8 + "Lock";
						if (!Smart.Thread.LockCache.ContainsKey(key))
						{
							Smart.Thread.LockCache[key] = obj;
						}
						else
						{
							obj = Smart.Thread.LockCache[key];
						}
					}
					DateTime now = DateTime.Now;
					lock (obj)
					{
						TimeSpan timeSpan = DateTime.Now.Subtract(now);
						if (timeSpan.TotalMilliseconds > 100.0)
						{
							string arg = Smart.Convert.TimeSpanToDisplay(timeSpan);
							Smart.Log.Debug(TAG, $"Sync lock waited {arg} for {text8} lock");
						}
						if (!step.VerifyPreContionMet())
						{
							continue;
						}
						Smart.Log.Debug(TAG, $"======================================== Setting up step {step.Info.Name} ({step.Info.Step}) ========================================");
						flag8 = true;
						step.Setup();
						bool flag14 = false;
						do
						{
							if (flag14)
							{
								int num = int.Parse(Log.Info["Retried"]) + 1;
								Log.AddInfo("Retried", num.ToString());
							}
							flag14 = true;
							Smart.Log.Info(TAG, $"Running step {step.Info.Name} ({step.Info.Step})");
							if (val != null)
							{
								string arg2 = "LOCKED";
								if (!val.Locked)
								{
									arg2 = "UNLOCKED";
								}
								Smart.Log.Verbose(TAG, $"Device {val.ID} ({val.Unique}) is {arg2}");
							}
							try
							{
								step.Run();
							}
							catch (Exception ex2)
							{
								Smart.Log.Error(TAG, ex2.Message + Environment.NewLine + ex2.StackTrace);
								if (!step.CheckRetest((Result)4))
								{
									throw;
								}
								string text9 = $"Error attempting step '{step.Info.Name}': {ex2.Message}";
								Smart.Log.Error(TAG, text9);
							}
							List<string> passedSteps3 = Log.PassedSteps;
							string name = step.Info.Name;
							bool flag15 = !passedSteps3.Contains(name.ToLowerInvariant().Trim());
							if (((dynamic)step.Info.Args).RetryLoops != null)
							{
								((dynamic)step.Info.Args).RetryLoops -= 1;
								if (flag8 && flag15 && ((dynamic)step.Info.Args)["PopupFailureForRetry"] != null && ((dynamic)step.Info.Args).RetryLoops > 0)
								{
									string text10 = ((dynamic)step.Info.Args).PopupFailureForRetry;
									string text11 = Smart.Locale.Xlate(text10);
									string text12 = Smart.Locale.Xlate(name);
									Smart.User.MessageBox(text12, text11, (MessageBoxButtons)0, (MessageBoxIcon)64);
								}
								if (flag8 && ((dynamic)step.Info.Args).RetryDelayMilliseconds != null && ((dynamic)step.Info.Args).RetryLoops > 0)
								{
									int num2 = ((dynamic)step.Info.Args).RetryDelayMilliseconds;
									Smart.Log.Debug(TAG, $"Delay {num2} milliseconds before next retry");
									Thread.Sleep(num2);
								}
							}
							if (flag15 && ((dynamic)step.Info.Args).Retesting == true)
							{
								Smart.Log.Debug(TAG, $"Step '{name}' failed with retest set, restarting");
								step.Restart();
							}
						}
						while (((dynamic)step.Info.Args).Retesting == true);
					}
				}
				catch (Exception ex3)
				{
					Result val4 = (Result)4;
					if (((dynamic)step.Info.Args).IgnoreFail == true)
					{
						Smart.Log.Debug(TAG, $"Ignoring test status {((object)(Result)(ref val4)).ToString()}, skipping result for {step.Info.Name}");
						val4 = (Result)7;
					}
					if (ex3.InnerException != null)
					{
						Log.AddResult(step.Info.Name, step.Info.Step, val4, ex3.Message, string.Empty, ex3.InnerException.Message, double.MinValue, double.MinValue, double.MinValue, (SortedList<string, object>)null);
					}
					else
					{
						Log.AddResult(step.Info.Name, step.Info.Step, val4, ex3.Message, "", "", double.MinValue, double.MinValue, double.MinValue, (SortedList<string, object>)null);
					}
					string text13 = $"Error during step '{step.Info.Name}': {ex3.Message}";
					Smart.Log.Error(TAG, text13);
					Smart.Log.Verbose(TAG, ex3.ToString());
				}
				finally
				{
					Log.Progress = ((dynamic)step.Info.Args).ProgressEnd;
					Smart.Log.Verbose(TAG, "======================================== Progress End: " + Log.Progress + " ========================================");
					if (flag8)
					{
						try
						{
							step.TearDown();
						}
						catch (Exception ex4)
						{
							string text14 = $"Error during tear down in step '{step.Info.Name}': {ex4.Message}";
							Smart.Log.Error(TAG, text14);
							Smart.Log.Verbose(TAG, ex4.ToString());
						}
					}
					bool flag16 = false;
					if (!flag && ((dynamic)step.Info.Args)["ForceStopOnFail"] != null)
					{
						string item = step.Info.Name.ToLowerInvariant().Trim();
						if (!Log.PassedSteps.Contains(item))
						{
							flag16 = ((dynamic)step.Info.Args).ForceStopOnFail;
						}
					}
					if (!flag && (int)Log.OverallResult == 1 && flag16)
					{
						flag = true;
					}
					if (!flag && Log.AbortedStep && flag3)
					{
						flag = true;
					}
					if (flag8 && ((dynamic)step.Info.Args)["PopupFailure"] != null)
					{
						List<string> passedSteps4 = Log.PassedSteps;
						string name2 = step.Info.Name;
						if (!passedSteps4.Contains(name2.ToLowerInvariant().Trim()))
						{
							string text15 = ((dynamic)step.Info.Args).PopupFailure;
							string text16 = Smart.Locale.Xlate(text15);
							string text17 = Smart.Locale.Xlate(name2);
							Smart.User.MessageBox(text17, text16, (MessageBoxButtons)0, (MessageBoxIcon)16);
						}
					}
				}
			}
		}
		finally
		{
			if ((int)Log.OverallResult == 8 && ((int)Info.UseCase == 131 || (int)Info.UseCase == 168 || (int)Info.UseCase == 140))
			{
				string empty = string.Empty;
				if (val != null && !val.InvalidSerialNumber)
				{
					empty = val.SerialNumber;
					string text18 = Smart.Convert.GenerateCode(empty, true);
					if (!string.IsNullOrWhiteSpace(text18))
					{
						Smart.Log.Verbose(TAG, $"Generated report code for {val.SerialNumber}: {text18}");
						Log.AddInfo("Report Code", text18);
					}
				}
			}
			if ((int)Log.OverallResult == 8 && (int)Info.UseCase == 134)
			{
				Log.AddInfo("DeviceRead", "Success");
				if (!string.IsNullOrEmpty(val.RecordId))
				{
					Log.AddInfo("RecordId", val.RecordId);
				}
			}
			else if ((int)Info.UseCase == 134)
			{
				Log.AddInfo("DeviceRead", "Failed");
			}
			if (val != null)
			{
				Log.AddInfo("PhysicalSnEnd", val.SerialNumber);
				Log.AddInfo("PhysicalSnEnd2", val.SerialNumber2);
			}
			if (val != null)
			{
				val.Locked = false;
				Smart.Log.Debug(TAG, $"Device FREED: {val.ID}");
				Smart.Log.Debug(TAG, ((object)val).ToString());
				if (val.ManualDevice && (int)Info.UseCase != 141 && (int)Info.UseCase != 950 && (int)Info.UseCase != 167 && (int)Info.UseCase != 134 && (int)Info.UseCase != 951 && (int)Info.UseCase != 952 && (int)Info.UseCase != 192 && (int)Info.UseCase != 164)
				{
					Smart.DeviceManager.RemoveManualDevice();
				}
			}
			((IDisposable)Log).Dispose();
			Smart.Log.Verbose(TAG, ((object)Log).ToString());
			Smart.Log.Info(TAG, string.Format("Finished Recipe {0} with result {1} for device {2}", Info.Name, Log.OverallResult, (val == null) ? "UNKNOWN_DEVICE" : val.ID));
			ILog log = Smart.Log;
			string tAG = TAG;
			UseCase useCase = Info.UseCase;
			log.Info(tAG, $"Check need to generate error report for {((object)(UseCase)(ref useCase)).ToString()}...");
			if (flag4 || (flag5 && (int)Log.OverallResult != 8))
			{
				if ((int)Info.UseCase == 141 || (int)Info.UseCase == 134)
				{
					ILog log2 = Smart.Log;
					string tAG2 = TAG;
					useCase = Info.UseCase;
					log2.Error(tAG2, "Skip auto generate error report for " + ((object)(UseCase)(ref useCase)).ToString());
				}
				else if (!Smart.File.Exists(autoLogDir))
				{
					Smart.Log.Error(TAG, $"Could not file auto report log directory: {autoLogDir}");
				}
				else
				{
					try
					{
						string text19 = "Pass";
						if ((int)Log.OverallResult != 8)
						{
							text19 = "Fail";
						}
						string text20 = Smart.App.Name;
						if (text20.ToLowerInvariant() == "LM Smart Tool".ToLowerInvariant())
						{
							text20 = "LMST";
						}
						string tag = text20;
						tag = tag + "_" + text19;
						if (val.SerialNumber != null && val.SerialNumber != string.Empty && val.SerialNumber != "UNKNOWN")
						{
							tag = tag + "_" + val.SerialNumber;
						}
						else if (val.ID != null && val.ID != string.Empty && val.ID != "UNKNOWN")
						{
							tag = tag + "_" + val.ID;
						}
						useCase = Info.UseCase;
						string text21 = ((object)(UseCase)(ref useCase)).ToString();
						if (text21.ToLowerInvariant().StartsWith((text20 + "_").ToLowerInvariant()))
						{
							text21 = text21.Substring(text20.Length + 1);
						}
						tag = tag + "_" + text21;
						string text22 = string.Empty;
						foreach (Tuple<string, SortedList<string, object>> result in Log.Results)
						{
							string item2 = result.Item1;
							SortedList<string, object> item3 = result.Item2;
							Result val5 = (Result)(dynamic)item3["result"];
							if ((int)val5 != 8 && (int)val5 != 7 && (int)val5 != 6 && (int)val5 != 3 && (int)val5 != 0)
							{
								text22 = item2;
								text22 = new Regex("[^a-zA-Z0-9]").Replace(text22, "");
								break;
							}
						}
						if (text22 != string.Empty)
						{
							tag = tag + "_" + text22;
						}
						Smart.Log.Info(TAG, "Recipe done to generate error report...");
						ThreadStart threadStart = delegate
						{
							Smart.Log.GenerateErrorReport(autoLogDir, tag);
						};
						Smart.Thread.Run(threadStart);
					}
					catch (Exception ex5)
					{
						Smart.Log.Error(TAG, "Error generating error report: " + ex5.Message);
						Smart.Log.Error(TAG, ex5.ToString());
					}
				}
			}
		}
	}
}
