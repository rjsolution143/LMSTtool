using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using ISmart;

namespace SmartDevice.Steps;

public class UnisocProgramming : UnisocBaseTest
{
	public string seqName = string.Empty;

	public string GPSSignFile = "C:\\prod\\codeplugs\\downloadedfiles\\GPSSign";

	public string CNCEHashFile = "C:\\prod\\codeplugs\\downloadedfiles\\CNCEHash";

	public string strToIndicateTestPass = "!!!!! All Finished, pass !!!!!";

	public string dynamic_data = string.Empty;

	public string prompt_for_retry = string.Empty;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0942: Unknown result type (might be due to invalid IL or missing references)
		//IL_0944: Invalid comparison between Unknown and I4
		//IL_0bcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdf: Invalid comparison between Unknown and I4
		//IL_0b96: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0be3: Invalid comparison between Unknown and I4
		//IL_0c3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c32: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Result result = (Result)8;
		val.Prompt.CloseMessageBox();
		string exe = ((dynamic)base.Info.Args).EXE;
		if (exe.StartsWith("$"))
		{
			string key = exe.Substring(1);
			exe = base.Cache[key];
		}
		Smart.Log.Verbose(TAG, $"Exe to be used: {exe}");
		string? directoryName = Path.GetDirectoryName(exe);
		string path = Path.Combine(directoryName, "App");
		if (!exe.EndsWith("FrameworkDemo.exe"))
		{
			exe = Path.Combine(path, "FrameworkDemo.exe");
		}
		Smart.Log.Verbose(TAG, $"Final exe to be used: {exe}");
		string seqFileFolder = Path.Combine(directoryName, "Project");
		string command = BuildCommand(seqFileFolder);
		Smart.Log.Verbose(TAG, $"Shell command to be used: {exe} {command}");
		KillExistingExe(exe);
		int shellTimeoutPerRun = 120;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			shellTimeoutPerRun = Convert.ToInt32(((dynamic)base.Info.Args).Timeout);
		}
		if (((dynamic)base.Info.Args).StrToIndicateTestPass != null)
		{
			strToIndicateTestPass = ((dynamic)base.Info.Args).StrToIndicateTestPass.ToString();
		}
		List<string> list = new List<string>();
		Smart.Thread.RunAndWait<int>((Func<int>)(() => ExecuteShell(exe, command, shellTimeoutPerRun)), true);
		list.AddRange(error);
		list.AddRange(output);
		if (string.Join(" ", list.ToArray()).Contains(strToIndicateTestPass))
		{
			result = (Result)8;
		}
		else
		{
			result = (Result)1;
			Smart.Log.Info(TAG, "result is " + ((object)(Result)(ref result)).ToString() + ", try again...");
			int num = 10;
			if (((dynamic)base.Info.Args).WaitBetweenAttemptSec != null)
			{
				num = Convert.ToInt32(((dynamic)base.Info.Args).WaitBetweenAttemptSec);
			}
			Thread.Sleep(num * 1000);
		}
		Smart.Log.Info(TAG, "Cmd line tool execute result:" + ((object)(Result)(ref result)).ToString());
		if ((int)result == 8 && ((dynamic)base.Info.Args).Verify != null && (bool)((dynamic)base.Info.Args).Verify)
		{
			result = VerifyPhoneReturnedValue(list);
			Smart.Log.Info(TAG, "Test result:" + ((object)(Result)(ref result)).ToString());
		}
		val.Prompt.CloseMessageBox();
		ActualTestResult = result;
		VerifyOnly(ref result);
		if (((int)result == 1 || (int)result == 4) && list.Count > 0)
		{
			string resp = ((list.Count > 0) ? string.Join("", list.ToArray()) : "No response from the device");
			string description = "Shell command failed";
			resp = GetDynamicDataFromResp(val, TAG, command, resp);
			LogResult(result, description, resp);
		}
		else
		{
			LogResult(result);
		}
		if (!CheckRetest(result))
		{
			Smart.Log.Info(TAG, "Last try done, clean up hash file");
			CleanUpHashFile(deleteCNCEHashFile: true, deleteGPSHashFile: true);
		}
	}

	private string GetDynamicDataFromResp(IDevice device, string testName, string fullShellCmd, string resp)
	{
		Smart.Log.Verbose(TAG, $"Get Dynamic_Data From Resp for {testName}.");
		if (testName.Contains("EnterClibrationMode"))
		{
			string value = "connect server success";
			if (resp.Contains(value))
			{
				Smart.Log.Verbose(TAG, $"Found COM port and connec to COM port OK,and socket server start success, but fail on GPS auth process");
				string text = "client send the message:";
				int num = resp.IndexOf(text) + text.Length;
				string value2 = "client received the message:";
				int num2 = resp.IndexOf(value2);
				string text2 = resp.Substring(num, num2 - num).Trim();
				Smart.Log.Verbose(TAG, $"Odm Tool Send Out Data For Sign:{text2}.");
				string text3 = base.Cache["ReceivedDataToBeSignedFromPhone"];
				string text4 = base.Cache["GpsReturnedAuthData"];
				Smart.Log.Verbose(TAG, $"Lmst Received Odm Data To Be Signed:{text3}.");
				Smart.Log.Verbose(TAG, $"Returned Gps Data:{text4}.");
				if (text2.Trim().Length > 5)
				{
					if (text3.Trim().Length <= 5)
					{
						Smart.Log.Verbose(TAG, $"Found COM port and connec to COM port OK,and socket server start success, \r\n                                                                and odm tool has send out data for sign, but LMST socket server not received data, \r\n                                                                indicate LMST socket server port occupied.");
						dynamic_data += "_odm tool has send data for sign but LMST not received data,socket might be occupied";
					}
					else if (text4.Trim().Length <= 0)
					{
						Smart.Log.Verbose(TAG, $"Found COM port and connec to COM port OK,and socket server start success, \r\n                                                                and odm tool has send out data for sign, and LMST socket server has received data for sign, \r\n                                                                but not receive gsp returned data.");
						dynamic_data += "_Data send to GPS but failed to get returned auth data.Might be server/token issue.";
					}
					else
					{
						Smart.Log.Verbose(TAG, $"Found COM port and connec to COM port OK,and socket server start success, \r\n                                                                and odm tool has send out data for sign, and LMST socket server has received data for sign, \r\n                                                                and has received gsp returned data. But still enter mode fail.");
						dynamic_data += "_GPS auth pass, but device failed to enter mode.";
					}
				}
				else
				{
					Smart.Log.Verbose(TAG, $"Found COM port and connec to COM port OK,and socket server start success, \r\n                                                                but odm tool not send out data for sign, phone issue.");
					dynamic_data += "_odm tool not send data for sign,phone issue";
				}
			}
			else if (resp.Contains("Start to <EnterMode"))
			{
				string text5 = $"Stuck EnterMode due to Com port incorrect or not found.";
				Smart.Log.Verbose(TAG, text5);
				dynamic_data = dynamic_data + "_" + text5;
			}
			else if (resp.Contains("Pre-Run"))
			{
				string text6 = $"Has Pre-Run but not run to Start EnterMode";
				Smart.Log.Verbose(TAG, text6);
				dynamic_data = dynamic_data + "_" + text6;
			}
			else
			{
				string text7 = $"No Pre-Run,ODM Tool not started";
				Smart.Log.Verbose(TAG, text7);
				dynamic_data = dynamic_data + "_" + text7;
			}
		}
		else
		{
			dynamic_data = dynamic_data + "_" + resp.Substring(resp.Trim().Length - 70);
		}
		dynamic_data = dynamic_data.TrimStart(new char[1] { '_' }).Replace(" ", "");
		Smart.Log.Verbose(TAG, $"Dynamic_Data is {dynamic_data}.");
		base.Cache["ReceivedDataToBeSignedFromPhone"] = "";
		base.Cache["GpsReturnedAuthData"] = "";
		return dynamic_data;
	}

	public bool FastbootSendCmd(IDevice device, string command, int timeoutSec, string strToIndicateCmdPass)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		Smart.Log.Info(MethodBase.GetCurrentMethod().Name, "enter...");
		Smart.Log.Info(MethodBase.GetCurrentMethod().Name, "Run cmd:" + command);
		string filePathName = Smart.Rsd.GetFilePathName("fastbootExe", base.Recipe.Info.UseCase, device);
		int num = -1;
		List<string> list = Smart.MotoAndroid.Shell(device.ID, command, timeoutSec * 1000, filePathName, ref num, 6000, false);
		string text = string.Join(" ", list.ToArray());
		Smart.Log.Info(TAG, "totalResult:" + text);
		if (text.ToUpper().Contains(strToIndicateCmdPass.ToUpper()) || num == 0)
		{
			Smart.Log.Info(TAG, "fastboot cmd pass");
			return true;
		}
		Smart.Log.Error(TAG, "fastboot cmd fail");
		return false;
	}

	public bool DeviceInFastbootMode(IDevice device)
	{
		Smart.Log.Info(MethodBase.GetCurrentMethod().Name, "enter...");
		string command = "getvar imei";
		int timeoutSec = 8;
		if (((dynamic)base.Info.Args).DetectFbModeTimeoutSec != null && ((dynamic)base.Info.Args).DetectFbModeTimeoutSec != string.Empty)
		{
			timeoutSec = ((dynamic)base.Info.Args).DetectFbModeTimeoutSec;
		}
		string strToIndicateCmdPass = "imei:";
		return FastbootSendCmd(device, command, timeoutSec, strToIndicateCmdPass);
	}

	public bool DeviceInAdbMode(IDevice device)
	{
		Smart.Log.Info(MethodBase.GetCurrentMethod().Name, "enter...");
		int num = 8;
		if (((dynamic)base.Info.Args).DetectAdbModeTimeoutSec != null)
		{
			num = ((dynamic)base.Info.Args).DetectAdbModeTimeoutSec;
		}
		DateTime now = DateTime.Now;
		bool flag = false;
		while (!flag)
		{
			try
			{
				Smart.ADB.FindDevices();
				Smart.ADB.Shell(device.ID, "pwd", 10000);
			}
			catch (Exception ex)
			{
				if (DateTime.Now.Subtract(now).TotalSeconds < (double)num)
				{
					Smart.Thread.Wait(TimeSpan.FromSeconds(5.0));
					continue;
				}
				Smart.Log.Error(TAG, "ADB connection timed out");
				Smart.Log.Error(TAG, ex.ToString());
				LogResult((Result)4, "ADB connection timed out");
				return false;
			}
			flag = true;
		}
		if (flag)
		{
			return true;
		}
		return false;
	}

	public override void Redirected(List<string> dataList, object sender, DataReceivedEventArgs e)
	{
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (e.Data == null)
		{
			return;
		}
		string text = e.Data.Trim();
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		dataList.Add(text);
		text = HideLogInShellResponse(text);
		Smart.Log.Verbose(TAG, text);
		try
		{
			if (text.Contains("Input User Data IMEI1"))
			{
				string text2 = string.Empty;
				if (base.Cache.Keys.Contains("SerialNumberOut"))
				{
					text2 = base.Cache["SerialNumberOut"];
				}
				if (((dynamic)base.Info.Args).SerialNumber != null && ((dynamic)base.Info.Args).SerialNumber != string.Empty)
				{
					text2 = ((dynamic)base.Info.Args).SerialNumber.ToString();
					if (text2.StartsWith("$"))
					{
						text2 = text2.Substring(1);
					}
					text2 = base.Cache[text2];
				}
				if (string.IsNullOrEmpty(text2))
				{
					text2 = device.SerialNumber;
				}
				Smart.Log.Debug(TAG, "Data to Input for cmd line tool: " + text2);
				mProcess.StandardInput.WriteLine(text2);
			}
			else if (text.Contains("Input User Data IMEI2"))
			{
				string text3 = string.Empty;
				if (base.Cache.Keys.Contains("SerialNumberOutDual"))
				{
					text3 = base.Cache["SerialNumberOutDual"];
				}
				if (((dynamic)base.Info.Args).SerialNumberDual != null && ((dynamic)base.Info.Args).SerialNumberDual != string.Empty)
				{
					text3 = ((dynamic)base.Info.Args).SerialNumberDual.ToString();
					if (text3.StartsWith("$"))
					{
						text3 = text3.Substring(1);
					}
					text3 = base.Cache[text3];
				}
				if (string.IsNullOrEmpty(text3))
				{
					text3 = device.SerialNumber2;
				}
				Smart.Log.Debug(TAG, "Data to Input for cmd line tool: " + text3);
				mProcess.StandardInput.WriteLine(text3);
			}
			else if (text.Contains("Input User Data IMEI"))
			{
				bool flag = false;
				if (((dynamic)base.Info.Args).UseODMSocketServerAuth != null)
				{
					flag = (bool)((dynamic)base.Info.Args).UseODMSocketServerAuth;
				}
				if (flag)
				{
					Smart.Log.Info(TAG, "Use ODM Socket Server Auth...");
					if (((dynamic)base.Info.Args).InputCmd != null)
					{
						string text4 = ((dynamic)base.Info.Args).InputCmd.ToString();
						Smart.Log.Debug(TAG, "Original cmd for Input User Data IMEI:" + text4);
						string[] array = text4.Split(new char[1] { ',' });
						for (int i = 0; i < array.Length; i++)
						{
							string text5 = array[i];
							if (text5.StartsWith("$"))
							{
								string key = text5.Substring(1);
								array[i] = base.Cache[key];
							}
						}
						text4 = string.Join(",", array);
						Smart.Log.Debug(TAG, "Data to Input for cmd line tool: " + text4);
						mProcess.StandardInput.WriteLine(text4);
						return;
					}
					string text6 = ((dynamic)base.Info.Args).KeyType;
					if (text6.StartsWith("$"))
					{
						string key2 = text6.Substring(1);
						text6 = base.Cache[key2];
					}
					string text7 = string.Empty;
					if (base.Cache.Keys.Contains("SerialNumberOut"))
					{
						text7 = base.Cache["SerialNumberOut"];
					}
					if (((dynamic)base.Info.Args).SerialNumber != null && ((dynamic)base.Info.Args).SerialNumber != string.Empty)
					{
						text7 = ((dynamic)base.Info.Args).SerialNumber.ToString();
						if (text7.StartsWith("$"))
						{
							text7 = text7.Substring(1);
						}
						text7 = base.Cache[text7];
					}
					if (string.IsNullOrEmpty(text7))
					{
						text7 = device.SerialNumber;
					}
					if (((dynamic)base.Info.Args).Dual != null && (bool)((dynamic)base.Info.Args).Dual)
					{
						string text8 = string.Empty;
						if (base.Cache.Keys.Contains("SerialNumberOutDual"))
						{
							text8 = base.Cache["SerialNumberOutDual"];
						}
						if (((dynamic)base.Info.Args).SerialNumberDual != null && ((dynamic)base.Info.Args).SerialNumberDual != string.Empty)
						{
							text8 = ((dynamic)base.Info.Args).SerialNumberDual.ToString();
							if (text8.StartsWith("$"))
							{
								text8 = text8.Substring(1);
							}
							text8 = base.Cache[text8];
						}
						if (string.IsNullOrEmpty(text8))
						{
							text8 = device.SerialNumber2;
						}
						text7 = text7 + "," + text8;
					}
					string text9 = text6 + "," + text7;
					Smart.Log.Debug(TAG, "Data to Input for cmd line tool: " + text9);
					mProcess.StandardInput.WriteLine(text9);
					return;
				}
				string text10 = string.Empty;
				if (base.Cache.Keys.Contains("SerialNumberOut"))
				{
					text10 = base.Cache["SerialNumberOut"];
					Smart.Log.Debug(TAG, "use SerialNumberOut: " + text10);
				}
				if (((dynamic)base.Info.Args).SerialNumber != null && ((dynamic)base.Info.Args).SerialNumber != string.Empty)
				{
					text10 = ((dynamic)base.Info.Args).SerialNumber.ToString();
					if (text10.StartsWith("$"))
					{
						text10 = text10.Substring(1);
					}
					text10 = base.Cache[text10];
					Smart.Log.Debug(TAG, "use Info.Args.SerialNumber: " + text10);
				}
				if (string.IsNullOrEmpty(text10))
				{
					text10 = device.SerialNumber;
				}
				if (((dynamic)base.Info.Args).Dual != null && (bool)((dynamic)base.Info.Args).Dual)
				{
					string text11 = string.Empty;
					if (base.Cache.Keys.Contains("SerialNumberOutDual"))
					{
						text11 = base.Cache["SerialNumberOutDual"];
					}
					if (((dynamic)base.Info.Args).SerialNumberDual != null && ((dynamic)base.Info.Args).SerialNumberDual != string.Empty)
					{
						text11 = ((dynamic)base.Info.Args).SerialNumberDual.ToString();
						if (text11.StartsWith("$"))
						{
							text11 = text11.Substring(1);
						}
						text11 = base.Cache[text11];
					}
					if (string.IsNullOrEmpty(text11))
					{
						text11 = device.SerialNumber2;
					}
					text10 = text10 + "," + text11;
				}
				Smart.Log.Debug(TAG, "Data to Input for cmd line tool: " + text10);
				mProcess.StandardInput.WriteLine(text10);
			}
			else if (text.Contains("Secure IMEI") || text.Contains("Secure SIMLOCK") || text.Contains("Secure SIMUNLOCK") || text.Contains("Factory Flag"))
			{
				bool flag2 = false;
				if (((dynamic)base.Info.Args).UseODMSocketServerAuth != null)
				{
					flag2 = (bool)((dynamic)base.Info.Args).UseODMSocketServerAuth;
				}
				if (flag2)
				{
					Smart.Log.Info(TAG, "Use ODM Socket Server Auth...");
					if (text.Contains("Secure IMEI"))
					{
						Smart.Log.Info(TAG, "Do nothing here, imei has been input when find \"Input User Data IMEI\"...");
					}
					else
					{
						if (!text.Contains("Secure SIMUNLOCK") && !text.Contains("Secure SIMLOCK"))
						{
							return;
						}
						if (((dynamic)base.Info.Args).InputCmd != null)
						{
							string text12 = ((dynamic)base.Info.Args).InputCmd.ToString();
							Smart.Log.Debug(TAG, "Original cmd for Secure SIMUNLOCK/Secure SIMLOCK:" + text12);
							string[] array2 = text12.Split(new char[1] { ',' });
							for (int j = 0; j < array2.Length; j++)
							{
								string text13 = array2[j];
								if (text13.StartsWith("$"))
								{
									string key3 = text13.Substring(1);
									array2[j] = base.Cache[key3];
								}
							}
							text12 = string.Join(",", array2);
							Smart.Log.Debug(TAG, "Data to Input for cmd line tool: " + text12);
							mProcess.StandardInput.WriteLine(text12);
						}
						else
						{
							string text14 = ((dynamic)base.Info.Args).KeyType;
							if (text14.StartsWith("$"))
							{
								string key4 = text14.Substring(1);
								text14 = base.Cache[key4];
							}
							Smart.Log.Debug(TAG, "Data to Input for cmd line tool: " + text14);
							mProcess.StandardInput.WriteLine(text14);
						}
					}
					return;
				}
				if (File.Exists(GPSSignFile))
				{
					Smart.Log.Verbose(TAG, "GPSSignFile already exist, skip Auth");
					return;
				}
				Smart.Thread.Run((ThreadStart)delegate
				{
					Smart.Log.Verbose(TAG, "Use legacy Server Auth...");
					string text25 = null;
					int num = 0;
					do
					{
						if (File.Exists(CNCEHashFile))
						{
							Smart.Log.Verbose(TAG, "CNCE hash file exist:" + CNCEHashFile);
							Smart.Log.Verbose(TAG, "Start to read CNCE Hash Files...");
							try
							{
								using StreamReader streamReader = new StreamReader(CNCEHashFile);
								text25 = streamReader.ReadToEnd();
								Smart.Log.Verbose(TAG, "CNCEHashFilestring:" + text25);
							}
							catch (Exception ex2)
							{
								Smart.Log.Error(TAG, ex2.Message);
							}
						}
						else
						{
							Smart.Log.Verbose(TAG, "CNCE hash file not exist:" + CNCEHashFile);
							num++;
							Thread.Sleep(200);
						}
					}
					while (num < 300 && string.IsNullOrEmpty(text25));
					if (!File.Exists(CNCEHashFile))
					{
						string text26 = $"CNCEHashFile {CNCEHashFile} not found...";
						Smart.Log.Error(TAG, text26);
					}
					else
					{
						Smart.Log.Verbose(TAG, "Found CNCE hash file");
						string text27 = string.Empty;
						if (base.Cache.Keys.Contains("SerialNumberOut"))
						{
							text27 = base.Cache["SerialNumberOut"];
						}
						if (((dynamic)base.Info.Args).SerialNumber != null && ((dynamic)base.Info.Args).SerialNumber != string.Empty)
						{
							text27 = ((dynamic)base.Info.Args).SerialNumber.ToString();
							if (text27.StartsWith("$"))
							{
								text27 = text27.Substring(1);
							}
							text27 = base.Cache[text27];
						}
						if (string.IsNullOrEmpty(text27))
						{
							text27 = device.SerialNumber;
						}
						string rsdLogId = device.Log.RsdLogId;
						string text28 = ((dynamic)base.Info.Args).ProdID;
						if (text28.StartsWith("$"))
						{
							string key5 = text28.Substring(1);
							text28 = base.Cache[key5];
						}
						string text29 = ((dynamic)base.Info.Args).KeyType;
						if (text29.StartsWith("$"))
						{
							string key6 = text29.Substring(1);
							text29 = base.Cache[key6];
						}
						string text30 = "1";
						if (((dynamic)base.Info.Args).KeyName != null && ((dynamic)base.Info.Args).KeyName != string.Empty)
						{
							text30 = ((dynamic)base.Info.Args).KeyName;
							if (text30.StartsWith("$"))
							{
								string key7 = text30.Substring(1);
								text30 = base.Cache[key7];
							}
						}
						string text31 = "0x00";
						if (((dynamic)base.Info.Args).ClientReqType != null)
						{
							text31 = ((dynamic)base.Info.Args).ClientReqType;
						}
						Smart.Log.Verbose(TAG, "CNCEHashFilestring length:" + text25.Length);
						Smart.Log.Verbose(TAG, "Start to do DataSignODM:" + text25);
						string text32 = string.Empty;
						bool flag3 = true;
						for (int l = 0; l < 3; l++)
						{
							try
							{
								text32 = Smart.Web.DataSignODM(text27, rsdLogId, text31, text28, text29, text30, text25);
							}
							catch (Exception ex3)
							{
								Smart.Log.Error(TAG, ex3.Message + Environment.NewLine + ex3.StackTrace);
								flag3 = false;
							}
							if (flag3)
							{
								break;
							}
						}
						string text33 = text32;
						text33 = text33.ToLower();
						using (StreamWriter streamWriter = File.CreateText(GPSSignFile))
						{
							try
							{
								streamWriter.Write(text33);
							}
							finally
							{
								streamWriter.Close();
							}
						}
						Smart.Log.Info(TAG, "Write data to GPSSignFile done.");
					}
				}, true);
			}
			else if (text.Contains("Input simlock"))
			{
				if (((dynamic)base.Info.Args).InputCmd != null)
				{
					string text15 = ((dynamic)base.Info.Args).InputCmd.ToString();
					string[] array3 = text15.Split(new char[1] { ',' });
					for (int k = 0; k < array3.Length; k++)
					{
						string text16 = array3[k];
						if (!text16.StartsWith("$"))
						{
							continue;
						}
						string text17 = text16.Substring(1);
						if (text17.Equals("lock1"))
						{
							if (base.Cache.ContainsKey("lock1"))
							{
								array3[k] = base.Cache[text17];
								array3[k] = array3[k].Split(new char[1] { ';' })[0];
							}
							else
							{
								array3[k] = "00000000";
							}
						}
						else
						{
							array3[k] = base.Cache[text17];
						}
					}
					text15 = string.Join(",", array3);
					mProcess.StandardInput.WriteLine(text15);
					return;
				}
				string empty = string.Empty;
				string text18 = "00000000";
				if (base.Cache.ContainsKey("lock1"))
				{
					text18 = base.Cache["lock1"];
				}
				empty = text18;
				if (((dynamic)base.Info.Args).SecondLockCode != null)
				{
					string text19 = ((dynamic)base.Info.Args).SecondLockCode;
					Smart.Log.Info(TAG, "Need 2nd lock code:" + text19);
					if (text19.StartsWith("$"))
					{
						text19 = text19.Substring(1);
						if (base.Cache.ContainsKey(text19))
						{
							text19 = base.Cache[text19];
						}
					}
					empty = empty + "," + text19;
				}
				mProcess.StandardInput.WriteLine(empty);
			}
			else if (text.Contains("Input bin Path"))
			{
				if (!((((dynamic)base.Info.Args).InputName != null && ((dynamic)base.Info.Args).InputName != string.Empty) ? true : false))
				{
					string text20 = $"Test {seqName} is waiting for input bin file, but Info.Args.InputName not configured, so we don't know what file should be loaded and write to cmd line tool";
					Smart.Log.Debug(TAG, text20);
					throw new Exception(text20);
				}
				string text21 = ((dynamic)base.Info.Args).InputName.ToString();
				if (text21.StartsWith("$"))
				{
					text21 = base.Cache[text21.Substring(1)];
				}
				Smart.Log.Debug(TAG, "Data to Input for cmd line tool: " + text21);
				if (!File.Exists(text21))
				{
					string text22 = $"Bin file {text21} to be wrote to cmd line tool not exist.";
					Smart.Log.Debug(TAG, text22);
					throw new Exception(text22);
				}
				mProcess.StandardInput.WriteLine(text21);
				Thread.Sleep(1000);
			}
			else if (text.Contains("Unisoc FrameworkInvoker Output szData"))
			{
				string empty2 = string.Empty;
				empty2 = GetResultFromResponse("Unisoc FrameworkInvoker Output szData ", text).Trim();
				if (((dynamic)base.Info.Args).SimCodeLength != null)
				{
					empty2 = empty2[..(int)Convert.ToInt32(((dynamic)base.Info.Args).SimCodeLength)];
				}
				base.Cache["lock1"] = empty2;
			}
			else if (text.Contains("Input User Data"))
			{
				Smart.Log.Debug(TAG, $"Test {seqName} is waiting to input data...");
				if (!((((dynamic)base.Info.Args).InputName != null && ((dynamic)base.Info.Args).InputName != string.Empty) ? true : false))
				{
					string text23 = $"Test {seqName} is waiting for input some data, but Info.Args.InputName not configured, so we don't know what data should be loaded and write to cmd line tool";
					Smart.Log.Debug(TAG, text23);
					throw new Exception(text23);
				}
				string text24 = ((dynamic)base.Info.Args).InputName.ToString();
				if (text24.StartsWith("$"))
				{
					text24 = base.Cache[text24.Substring(1)];
				}
				Smart.Log.Debug(TAG, "Data to Input for cmd line tool: " + text24);
				mProcess.StandardInput.WriteLine(text24);
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Info(TAG, ex.Message + Environment.NewLine + ex.StackTrace);
		}
	}

	private string HideLogInShellResponse(string response)
	{
		string text = "";
		if (((dynamic)base.Info.Args).StringToHideLog != null)
		{
			text = ((dynamic)base.Info.Args).StringToHideLog.ToString();
		}
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			if (base.Cache.ContainsKey(key))
			{
				text = base.Cache[key];
			}
		}
		char c = ';';
		if (((dynamic)base.Info.Args).Spliter != null)
		{
			c = ((dynamic)base.Info.Args).Spliter.ToChar();
		}
		if (text.Length > 0 && text.Split(new char[1] { c }).ToList().Any((string data) => response.Contains(data)))
		{
			return "******************";
		}
		return response;
	}

	public void KillExistingExe(string exe)
	{
		Thread.Sleep(3000);
		for (int i = 0; i < 3; i++)
		{
			try
			{
				Process[] array = null;
				array = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(exe));
				if (array != null)
				{
					Process[] array2 = array;
					foreach (Process process in array2)
					{
						Smart.Log.Info(TAG, "kill " + process.ProcessName);
						process.Kill();
					}
				}
				break;
			}
			catch (Exception ex)
			{
				Smart.Log.Info(TAG, $"kill process error:{ex.Message}");
				Thread.Sleep(2000);
			}
		}
	}

	private string BuildCommand(string seqFileFolder)
	{
		string text = string.Empty;
		if (((dynamic)base.Info.Args).Command != null)
		{
			text = ((dynamic)base.Info.Args).Command.ToString();
		}
		if (((dynamic)base.Info.Args).Format != null)
		{
			List<string> list = new List<string>();
			foreach (object item in ((dynamic)base.Info.Args).Format)
			{
				string text2 = (string)(dynamic)item;
				string text3 = text2;
				if (text2.StartsWith("$"))
				{
					string text4 = text2.Substring(1);
					text3 = base.Cache[text4];
					text4.ToLower();
				}
				if (text2.EndsWith(".seq", StringComparison.OrdinalIgnoreCase))
				{
					seqName = text2;
					text3 = text2;
					if (!Path.IsPathRooted(seqName))
					{
						text3 = Path.Combine(seqFileFolder, seqName);
						if (!File.Exists(text3))
						{
							string text5 = $"Seq file not exist {text3}.";
							Smart.Log.Error(TAG, text5);
							throw new Exception(text5);
						}
					}
				}
				list.Add(text3);
			}
			text = string.Format(text, list.ToArray());
		}
		Smart.Log.Verbose(TAG, $"Shell command to be used: {text}");
		return text;
	}

	private void CleanUpHashFile(bool deleteCNCEHashFile, bool deleteGPSHashFile)
	{
		Smart.Log.Verbose(TAG, $"Clean up local hash files...");
		if (deleteGPSHashFile)
		{
			if (((dynamic)base.Info.Args).GPSSignFile != null && ((dynamic)base.Info.Args).GPSSignFile != string.Empty)
			{
				GPSSignFile = ((dynamic)base.Info.Args).GPSSignFile.ToString();
				if (GPSSignFile.StartsWith("$"))
				{
					GPSSignFile = base.Cache[GPSSignFile.Substring(1)];
				}
			}
			if (File.Exists(GPSSignFile))
			{
				try
				{
					Smart.Log.Debug(TAG, "Delete GPSSignFile: " + GPSSignFile);
					File.Delete(GPSSignFile);
				}
				catch (Exception ex)
				{
					Smart.Log.Error(TAG, ex.Message + Environment.NewLine + ex.StackTrace);
				}
			}
			else if (!Directory.Exists(Path.GetDirectoryName(GPSSignFile)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(GPSSignFile));
			}
		}
		if (!deleteCNCEHashFile)
		{
			return;
		}
		if (((dynamic)base.Info.Args).CNCHashFile != null && ((dynamic)base.Info.Args).CNCHashFile != string.Empty)
		{
			CNCEHashFile = ((dynamic)base.Info.Args).CNCHashFile.ToString();
			if (CNCEHashFile.StartsWith("$"))
			{
				CNCEHashFile = base.Cache[CNCEHashFile.Substring(1)];
			}
		}
		if (File.Exists(CNCEHashFile))
		{
			try
			{
				Smart.Log.Debug(TAG, "Delete CNCHashFile: " + CNCEHashFile);
				File.Delete(CNCEHashFile);
				return;
			}
			catch (Exception ex2)
			{
				Smart.Log.Error(TAG, ex2.Message + Environment.NewLine + ex2.StackTrace);
				return;
			}
		}
		if (!Directory.Exists(Path.GetDirectoryName(CNCEHashFile)))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(CNCEHashFile));
		}
	}

	private Result VerifyPhoneReturnedValue(List<string> responses)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		Result val = (Result)1;
		IDevice val2 = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		try
		{
			string text = string.Empty;
			if (((dynamic)base.Info.Args).Value != null)
			{
				text = ((dynamic)base.Info.Args).Value.ToString();
			}
			Smart.Log.Info(TAG, $"Expected value {text}");
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = base.Cache[key];
			}
			else if (text.ToLowerInvariant() == "trackid".ToLowerInvariant())
			{
				text = val2.ID;
			}
			else if (text.ToLowerInvariant() == "serialnumber".ToLowerInvariant())
			{
				text = val2.SerialNumber;
			}
			else if (text.ToLowerInvariant() == "serialnumber2".ToLowerInvariant())
			{
				text = val2.SerialNumber2;
			}
			Smart.Log.Info(TAG, $"Final expected value {text}");
			string deviceReturnedValue = GetDeviceReturnedValue(responses);
			Smart.Log.Info(TAG, $"Value From Phone {deviceReturnedValue}");
			if (deviceReturnedValue != text)
			{
				Smart.Log.Error(TAG, $"Value read from phone {deviceReturnedValue} does not math with expected value {text}");
				return (Result)1;
			}
			return (Result)8;
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, ex.Message + Environment.NewLine + ex.StackTrace);
			return (Result)1;
		}
	}

	private string GetDeviceReturnedValue(List<string> responses)
	{
		string text = string.Empty;
		List<string> list = new List<string>();
		foreach (string response in responses)
		{
			if (response.Contains("Unisoc FrameworkInvoker Output") && !response.Contains("Init OK"))
			{
				list.Add(response);
			}
		}
		foreach (string item in list)
		{
			Smart.Log.Verbose(TAG, "Filtered response:" + item);
		}
		string text2 = string.Empty;
		if (((dynamic)base.Info.Args).MsgToExtractVal != null)
		{
			text2 = (string)((dynamic)base.Info.Args).MsgToExtractVal;
		}
		foreach (string item2 in list)
		{
			Smart.Log.Verbose(TAG, "Check line:" + item2);
			if (!string.IsNullOrEmpty(text2))
			{
				if (item2.Contains(text2))
				{
					text = GetResultFromResponse(text2, item2);
					break;
				}
			}
			else if (item2.Contains("Output SN1"))
			{
				text = GetResultFromResponse("Unisoc FrameworkInvoker Output SN1 ", item2);
			}
			else if (item2.Contains("Output SN2"))
			{
				text = GetResultFromResponse("Unisoc FrameworkInvoker Output SN2 ", item2);
			}
			else if (item2.Contains("Output IMEI1"))
			{
				text = GetResultFromResponse("Unisoc FrameworkInvoker Output IMEI1 ", item2);
			}
			else if (item2.Contains("Output IMEI2"))
			{
				text = GetResultFromResponse("Unisoc FrameworkInvoker Output IMEI2 ", item2);
			}
			else if (item2.Contains("Output WIFI"))
			{
				text = GetResultFromResponse("Unisoc FrameworkInvoker Output WIFI ", item2);
			}
			else if (item2.Contains("Output BT"))
			{
				text = GetResultFromResponse("Unisoc FrameworkInvoker Output BT ", item2);
			}
			else if (item2.Contains("Output Battery Level"))
			{
				text = GetResultFromResponse("Unisoc FrameworkInvoker Output Battery Level ", item2);
			}
			else if (item2.Contains("Output Battery Voltage"))
			{
				text = GetResultFromResponse("Unisoc FrameworkInvoker Output Battery Voltage ", item2);
			}
		}
		Smart.Log.Info(TAG, "val_from_phone: " + text);
		return text.Trim();
	}

	private string GetResultFromResponse(string strIn, string response)
	{
		string result = string.Empty;
		if (response.Contains(strIn))
		{
			result = response.Trim().Substring(response.IndexOf(strIn)).Replace(strIn, "");
		}
		return result;
	}
}
