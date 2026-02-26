using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using ConsoleToolBridge;
using ISmart;

namespace UnisocCfcTest;

public class UnisocCfcProgramming : UnisocCallFrameworkInvoker
{
	private string seqName = string.Empty;

	private string GPSSignFile = "C:\\prod\\codeplugs\\downloadedfiles\\GPSSign";

	private string CNCEHashFile = "C:\\prod\\codeplugs\\downloadedfiles\\CNCEHash";

	private int nstatus;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de5: Invalid comparison between Unknown and I4
		//IL_1070: Unknown result type (might be due to invalid IL or missing references)
		//IL_1072: Invalid comparison between Unknown and I4
		//IL_103b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1040: Unknown result type (might be due to invalid IL or missing references)
		//IL_1074: Unknown result type (might be due to invalid IL or missing references)
		//IL_1076: Invalid comparison between Unknown and I4
		//IL_10c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a41: Unknown result type (might be due to invalid IL or missing references)
		string name = MethodBase.GetCurrentMethod().Name;
		string_to_understand_that_tool_succeeded = "!!!!! All Finished, pass !!!!!";
		_ = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Result result = (Result)8;
		string text = ((dynamic)base.Info.Args).EXE;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		Smart.Log.Verbose(TAG, $"Exe to be used: {text}");
		string directoryName = Path.GetDirectoryName(text);
		FrameWorkDir = Path.Combine(directoryName, "App");
		tool_directory = FrameWorkDir;
		Smart.Log.Verbose(TAG, $"FrameWorkDir to be used: {FrameWorkDir}");
		SeqFileDir = Path.Combine(directoryName, "Project");
		string path = ((dynamic)base.Info.Args).SeqFile;
		path = Path.Combine(SeqFileDir, path);
		if (!File.Exists(path))
		{
			Smart.Log.Error(TAG, "seq file not exist:" + path);
			LogResult((Result)1);
			return;
		}
		seqName = path;
		string text2 = ((dynamic)base.Info.Args).KernalPort;
		if (text2.StartsWith("$"))
		{
			text2 = base.Cache[text2.Substring(1)];
		}
		ConsoleToolBase.mutexSecureUnisoc.WaitOne();
		ConsoleToolBase.mutexIsLocked = true;
		try
		{
			if (((dynamic)base.Info.Args).StrToIndicateTestPass != null)
			{
				string_to_understand_that_tool_succeeded = ((dynamic)base.Info.Args).StrToIndicateTestPass.ToString();
			}
			int num = 1;
			if (((dynamic)base.Info.Args).MaxAttempt != null)
			{
				num = Convert.ToInt32(((dynamic)base.Info.Args).MaxAttempt);
			}
			for (int i = 0; i < num; i++)
			{
				CleanUpHashFile(deleteCNCEHashFile: true, deleteGPSHashFile: true);
				my_tool = new ConsoleBridge();
				my_tool.TriggerActionUsingOutput += HandleSimbaOutput;
				if (((dynamic)base.Info.Args).WaitForToolExitmSec != null)
				{
					my_tool.wait_for_exit_ms = Convert.ToInt32(((dynamic)base.Info.Args).WaitForToolExitmSec);
				}
				base.SeqFile_Simba = path;
				Smart.Log.Verbose(TAG, $"SeqFile_Simba to be used: {base.SeqFile_Simba}.");
				testcommandclass testcommandsharp = new testcommandclass();
				ParametricDataStruct parametricdata = default(ParametricDataStruct);
				nstatus = Execute(name, Convert.ToInt32(text2), testcommandsharp, ref parametricdata);
				if (nstatus != 0)
				{
					result = (Result)1;
					string progressText = "Error to Execute FrameworkDemo tool!";
					LogMessage(base.LogGroup, name, progressText, TraceEventType.Error, AddTimeToLogMessage);
					int num2 = 10;
					if (((dynamic)base.Info.Args).WaitBetweenAttemptSec != null)
					{
						num2 = Convert.ToInt32(((dynamic)base.Info.Args).WaitBetweenAttemptSec);
					}
					Thread.Sleep(num2 * 1000);
					continue;
				}
				Smart.Log.Info(TAG, "Test pass...");
				result = (Result)8;
				break;
			}
		}
		catch (Exception ex)
		{
			if (ConsoleToolBase.mutexIsLocked)
			{
				ConsoleToolBase.mutexIsLocked = false;
				ConsoleToolBase.mutexSecureUnisoc.ReleaseMutex();
			}
			Smart.Log.Error(TAG, ex.Message + Environment.NewLine + ex.StackTrace);
			nstatus = -1;
		}
		finally
		{
			if (ConsoleToolBase.mutexIsLocked)
			{
				ConsoleToolBase.mutexIsLocked = false;
				ConsoleToolBase.mutexSecureUnisoc.ReleaseMutex();
			}
			KillExistingExe(Path.Combine(tool_directory, tool_name));
			LogMessage(base.LogGroup, name, "exit...", TraceEventType.Information, AddTimeToLogMessage);
		}
		Smart.Log.Info(TAG, "result is " + ((object)(Result)(ref result)).ToString() + ", delete sign file");
		foreach (string output in outputs)
		{
			_ = output;
		}
		if (nstatus != 0)
		{
			string progressText2 = "Error to Execute FrameworkDemo tool!";
			LogMessage(base.LogGroup, name, progressText2, TraceEventType.Error, AddTimeToLogMessage);
			result = (Result)1;
		}
		else
		{
			result = (Result)8;
		}
		if ((int)result == 8 && ((dynamic)base.Info.Args).Verify != null && (bool)((dynamic)base.Info.Args).Verify)
		{
			result = VerifyPhoneReturnedValue(outputs);
			Smart.Log.Info(TAG, "Test result:" + ((object)(Result)(ref result)).ToString());
		}
		VerifyOnly(ref result);
		if (((int)result == 1 || (int)result == 4) && outputs.Count > 0)
		{
			string dynamicError = ((outputs.Count > 0) ? string.Join(" ", outputs.ToArray()) : "No response from the device");
			string description = "Shell command failed";
			LogResult(result, description, dynamicError);
		}
		else
		{
			LogResult(result);
		}
	}

	private void HandleSimbaOutput(Process threadid, string message)
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (string.IsNullOrEmpty(message))
		{
			return;
		}
		string text = HideLogInShellResponse(message);
		Smart.Log.Debug(TAG, text);
		try
		{
			if (message.Contains("Input User Data IMEI1"))
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
					text2 = val.SerialNumber;
				}
				Smart.Log.Debug(TAG, "Data to Input for cmd line tool: " + text2);
				threadid.StandardInput.WriteLine(text2);
			}
			else if (message.Contains("Input User Data IMEI2"))
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
					text3 = val.SerialNumber2;
				}
				Smart.Log.Debug(TAG, "Data to Input for cmd line tool: " + text3);
				threadid.StandardInput.WriteLine(text3);
			}
			else if (message.Contains("Input User Data IMEI"))
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
						threadid.StandardInput.WriteLine(text4);
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
						text7 = val.SerialNumber;
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
							text8 = val.SerialNumber2;
						}
						text7 = text7 + "," + text8;
					}
					string text9 = text6 + "," + text7;
					Smart.Log.Debug(TAG, "Data to Input for cmd line tool: " + text9);
					threadid.StandardInput.WriteLine(text9);
					return;
				}
				string text10 = string.Empty;
				if (base.Cache.Keys.Contains("SerialNumberOut"))
				{
					text10 = base.Cache["SerialNumberOut"];
				}
				if (((dynamic)base.Info.Args).SerialNumber != null && ((dynamic)base.Info.Args).SerialNumber != string.Empty)
				{
					text10 = ((dynamic)base.Info.Args).SerialNumber.ToString();
					if (text10.StartsWith("$"))
					{
						text10 = text10.Substring(1);
					}
					text10 = base.Cache[text10];
				}
				if (string.IsNullOrEmpty(text10))
				{
					text10 = val.SerialNumber;
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
						text11 = val.SerialNumber2;
					}
					text10 = text10 + "," + text11;
				}
				Smart.Log.Debug(TAG, "Data to Input for cmd line tool: " + text10);
				threadid.StandardInput.WriteLine(text10);
			}
			else if (message.Contains("Secure IMEI") || message.Contains("Secure SIMLOCK") || message.Contains("Secure SIMUNLOCK") || message.Contains("Factory Flag"))
			{
				bool flag2 = false;
				if (((dynamic)base.Info.Args).UseODMSocketServerAuth != null)
				{
					flag2 = (bool)((dynamic)base.Info.Args).UseODMSocketServerAuth;
				}
				if (flag2)
				{
					Smart.Log.Info(TAG, "Use ODM Socket Server Auth...");
					if (message.Contains("Secure IMEI"))
					{
						Smart.Log.Info(TAG, "Do nothing here, imei has been input when find \"Input User Data IMEI\"...");
					}
					else
					{
						if (!message.Contains("Secure SIMUNLOCK") && !message.Contains("Secure SIMLOCK"))
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
							threadid.StandardInput.WriteLine(text12);
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
							threadid.StandardInput.WriteLine(text14);
						}
					}
					return;
				}
				Smart.Log.Info(TAG, "Use legacy CNCE hash file Server Auth...");
				try
				{
					_ = string.Empty;
					StreamReader streamReader = null;
					string text15 = null;
					int num = 0;
					do
					{
						if (File.Exists(CNCEHashFile))
						{
							Thread.Sleep(2000);
							streamReader = new StreamReader(CNCEHashFile);
							text15 = streamReader.ReadToEnd();
						}
						num++;
						Thread.Sleep(200);
					}
					while (num < 300 && string.IsNullOrEmpty(text15));
					streamReader.Close();
					if (!File.Exists(CNCEHashFile))
					{
						Smart.Log.Error(TAG, "not found cnce hash file: " + CNCEHashFile);
						nstatus = -7739169;
						return;
					}
					Smart.Log.Error(TAG, "found cnce hash file: " + CNCEHashFile);
					string text16 = string.Empty;
					if (base.Cache.Keys.Contains("SerialNumberOut"))
					{
						text16 = base.Cache["SerialNumberOut"];
					}
					if (((dynamic)base.Info.Args).SerialNumber != null && ((dynamic)base.Info.Args).SerialNumber != string.Empty)
					{
						text16 = ((dynamic)base.Info.Args).SerialNumber.ToString();
						if (text16.StartsWith("$"))
						{
							text16 = text16.Substring(1);
						}
						text16 = base.Cache[text16];
					}
					if (string.IsNullOrEmpty(text16))
					{
						text16 = val.SerialNumber;
					}
					string rsdLogId = val.Log.RsdLogId;
					string text17 = ((dynamic)base.Info.Args).ProdID;
					if (text17.StartsWith("$"))
					{
						string key5 = text17.Substring(1);
						text17 = base.Cache[key5];
					}
					string text18 = ((dynamic)base.Info.Args).KeyType;
					if (text18.StartsWith("$"))
					{
						string key6 = text18.Substring(1);
						text18 = base.Cache[key6];
					}
					string text19 = "1";
					if (((dynamic)base.Info.Args).KeyName != null && ((dynamic)base.Info.Args).KeyName != string.Empty)
					{
						text19 = ((dynamic)base.Info.Args).KeyName;
						if (text19.StartsWith("$"))
						{
							string key7 = text19.Substring(1);
							text19 = base.Cache[key7];
						}
					}
					string text20 = "0x00";
					if (((dynamic)base.Info.Args).ClientReqType != null)
					{
						text20 = ((dynamic)base.Info.Args).ClientReqType;
					}
					Smart.Log.Info(TAG, "Start to do DataSignODM:" + text15);
					string text21 = string.Empty;
					bool flag3 = true;
					for (int k = 0; k < 3; k++)
					{
						try
						{
							text21 = Smart.Web.DataSignODM(text16, rsdLogId, text20, text17, text18, text19, text15);
						}
						catch (Exception ex)
						{
							Smart.Log.Error(TAG, ex.Message + Environment.NewLine + ex.StackTrace);
							flag3 = false;
						}
						if (flag3)
						{
							break;
						}
					}
					string value = text21.ToLower();
					using StreamWriter streamWriter = File.CreateText(GPSSignFile);
					try
					{
						streamWriter.Write(value);
					}
					finally
					{
						streamWriter.Close();
					}
				}
				catch (Exception ex2)
				{
					LogMessage(base.LogGroup, "ODM_GPS_CONNECTION", string.Format("Failed to Connect to GPS+ODM web service:" + ex2.Message + Environment.NewLine + ex2.StackTrace), TraceEventType.Error, AddTimeToLogMessage);
					nstatus = -7739169;
				}
				Smart.Log.Info(TAG, "Write data to GPSSignFile done.");
			}
			else if (message.Contains("Input simlock"))
			{
				if (((dynamic)base.Info.Args).InputCmd != null)
				{
					string text22 = ((dynamic)base.Info.Args).InputCmd.ToString();
					string[] array3 = text22.Split(new char[1] { ',' });
					for (int l = 0; l < array3.Length; l++)
					{
						string text23 = array3[l];
						if (!text23.StartsWith("$"))
						{
							continue;
						}
						string text24 = text23.Substring(1);
						if (text24.Equals("lock1"))
						{
							if (base.Cache.ContainsKey("lock1"))
							{
								array3[l] = base.Cache[text24];
								array3[l] = array3[l].Split(new char[1] { ';' })[0];
							}
							else
							{
								array3[l] = "00000000";
							}
						}
						else
						{
							array3[l] = base.Cache[text24];
						}
					}
					text22 = string.Join(",", array3);
					threadid.StandardInput.WriteLine(text22);
					return;
				}
				string empty = string.Empty;
				string text25 = "00000000";
				if (base.Cache.ContainsKey("lock1"))
				{
					text25 = base.Cache["lock1"];
				}
				empty = text25;
				if (((dynamic)base.Info.Args).SecondLockCode != null)
				{
					string text26 = ((dynamic)base.Info.Args).SecondLockCode;
					Smart.Log.Info(TAG, "Need 2nd lock code:" + text26);
					if (text26.StartsWith("$"))
					{
						text26 = text26.Substring(1);
						if (base.Cache.ContainsKey(text26))
						{
							text26 = base.Cache[text26];
						}
					}
					empty = empty + "," + text26;
				}
				threadid.StandardInput.WriteLine(empty);
			}
			else if (message.Contains("Input bin Path"))
			{
				if (((dynamic)base.Info.Args).InputName != null && ((dynamic)base.Info.Args).InputName != string.Empty)
				{
					string text27 = ((dynamic)base.Info.Args).InputName.ToString();
					if (text27.StartsWith("$"))
					{
						text27 = base.Cache[text27.Substring(1)];
					}
					Smart.Log.Debug(TAG, "Data to Input for cmd line tool: " + text27);
					if (!File.Exists(text27))
					{
						string text28 = $"Bin file {text27} to be wrote to cmd line tool not exist.";
						Smart.Log.Debug(TAG, text28);
						throw new Exception(text28);
					}
					threadid.StandardInput.WriteLine(text27);
					Thread.Sleep(1000);
				}
				else
				{
					string text29 = $"Test {seqName} is waiting for input bin file, but Info.Args.InputName not configured, so we don't know what file should be loaded and write to cmd line tool";
					Smart.Log.Debug(TAG, text29);
					nstatus = -2;
				}
			}
			else if (message.Contains("Unisoc FrameworkInvoker Output szData"))
			{
				string empty2 = string.Empty;
				empty2 = GetResultFromResponse("Unisoc FrameworkInvoker Output szData ", message).Trim();
				if (((dynamic)base.Info.Args).SimCodeLength != null)
				{
					empty2 = empty2[..(int)Convert.ToInt32(((dynamic)base.Info.Args).SimCodeLength)];
				}
				base.Cache["lock1"] = empty2;
			}
			else
			{
				if (!message.Contains("Input User Data"))
				{
					return;
				}
				Smart.Log.Debug(TAG, $"Test {seqName} is waiting to input data...");
				if (((dynamic)base.Info.Args).InputName != null && ((dynamic)base.Info.Args).InputName != string.Empty)
				{
					string text30 = ((dynamic)base.Info.Args).InputName.ToString();
					string text31 = text30;
					if (text30.StartsWith("$"))
					{
						text31 = base.Cache[text30.Substring(1)];
					}
					Smart.Log.Debug(TAG, "Data to Input for cmd line tool: " + text31);
					threadid.StandardInput.WriteLine(text31);
				}
				else
				{
					string text32 = $"Test {seqName} is waiting for input some data, but Info.Args.InputName not configured, so we don't know what data should be loaded and write to cmd line tool";
					Smart.Log.Debug(TAG, text32);
					nstatus = -1;
				}
			}
		}
		catch (Exception ex3)
		{
			Smart.Log.Info(TAG, ex3.Message + Environment.NewLine + ex3.StackTrace);
			nstatus = -7739998;
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
		if (text.Length > 0 && response.Contains(text))
		{
			return response.Replace(text, "******************");
		}
		return response;
	}

	private void KillExistingExe(string exe)
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
