using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class ShellOdm1 : BaseStep
{
	private ShellResponse mShellResponse;

	private bool mError;

	private bool mTerminate;

	private bool mCompleted;

	private Process mProcess;

	private bool mNoMsgReceived = true;

	private bool mFirstConnecting = true;

	private bool mVerbose;

	private List<List<string>> mLogMsgToPrompts;

	private List<string> mLogMsgToClosePrompts;

	private List<List<string>> mLogMsgToCommands;

	private List<List<string>> mLogMsgToExtractValues;

	private MessageBoxIcon mIconType = (MessageBoxIcon)64;

	private SortedList<string, string> mInput;

	private SortedList<string, string> mOutput;

	private string[] mOutputLength;

	private string mPassFlag;

	private string mNotPassFlag;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_1edd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_20c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_20c4: Invalid comparison between Unknown and I4
		//IL_20aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_20a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_20c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_20c8: Invalid comparison between Unknown and I4
		//IL_21bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_21a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_21ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee6: Unknown result type (might be due to invalid IL or missing references)
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Result result = (Result)8;
		string filePath = string.Empty;
		if (((dynamic)base.Info.Args).ODM1Recipe != null)
		{
			filePath = ((dynamic)base.Info.Args).ODM1Recipe;
		}
		List<SortedList<string, string>> list = ReadCsvFile(filePath);
		if (((dynamic)base.Info.Args).Verbose != null)
		{
			mVerbose = (bool)((dynamic)base.Info.Args).Verbose;
		}
		bool useShellExecute = false;
		if (((dynamic)base.Info.Args).UseShellExecute != null)
		{
			useShellExecute = (bool)((dynamic)base.Info.Args).UseShellExecute;
		}
		foreach (SortedList<string, string> item6 in list)
		{
			mInput = new SortedList<string, string>();
			mOutput = new SortedList<string, string>();
			mOutputLength = new string[0];
			string value11;
			string value10;
			string value9;
			string value8;
			string value7;
			string value6;
			string value5;
			string value4;
			string value3;
			string value2;
			string value;
			string value12 = (value11 = (value10 = (value9 = (value8 = (value7 = (value6 = (value5 = (value4 = (value3 = (value2 = (value = null)))))))))));
			int num = 0;
			string[] array3;
			string[] array2;
			string[] array;
			string[] array4 = (array3 = (array2 = (array = null)));
			item6.TryGetValue("StepName", out value12);
			item6.TryGetValue("ExePath", out value11);
			item6.TryGetValue("Cmd", out value10);
			item6.TryGetValue("InputKey", out value9);
			item6.TryGetValue("InputValue", out value8);
			item6.TryGetValue("PassFlag", out value7);
			item6.TryGetValue("OutputKey", out value6);
			item6.TryGetValue("OutputLength", out value5);
			item6.TryGetValue("OutputCacheName", out value4);
			item6.TryGetValue("Completed", out value3);
			item6.TryGetValue("Timeout", out value2);
			item6.TryGetValue("RetryLoops", out value);
			ShellResponse.ExeStringToShellCmdType.Add(Path.GetFileName(value11), ShellResponse.ShellCmdType.GENERICTOOL);
			ShellResponse.GenericResponseToStatus.Clear();
			if (value9.Contains(";"))
			{
				array4 = value9.Split(new char[1] { ';' });
			}
			else if (!string.IsNullOrEmpty(value9))
			{
				array4 = new string[1] { value9 };
			}
			if (value8.Contains(";"))
			{
				array3 = value8.Split(new char[1] { ';' });
			}
			else if (!string.IsNullOrEmpty(value8))
			{
				array3 = new string[1] { value8 };
			}
			if (value6.Contains(";"))
			{
				array2 = value6.Split(new char[1] { ';' });
			}
			else if (!string.IsNullOrEmpty(value6))
			{
				array2 = new string[1] { value6 };
			}
			if (value4.Contains(";"))
			{
				array = value4.Split(new char[1] { ';' });
			}
			else if (!string.IsNullOrEmpty(value4))
			{
				array = new string[1] { value4 };
			}
			if (value5.Contains(";"))
			{
				mOutputLength = value5.Split(new char[1] { ';' });
			}
			else if (!string.IsNullOrEmpty(value5))
			{
				mOutputLength = new string[1] { value5 };
			}
			num = (string.IsNullOrEmpty(value) ? 1 : int.Parse(value));
			if (array4 != null)
			{
				for (int i = 0; i < array4.Length; i++)
				{
					if (array3[i].Contains("(!)"))
					{
						array3[i] = array3[i].Replace("(!)", ",");
					}
					mInput.Add(array4[i], array3[i]);
					ShellResponse.GenericResponseToStatus.Add(array4[i], ShellResponse.ShellCmdStatus.Authenticating);
				}
			}
			if (array2 != null)
			{
				for (int j = 0; j < array2.Length; j++)
				{
					mOutput.Add(array2[j], array[j]);
					ShellResponse.GenericResponseToStatus.Add(array2[j], ShellResponse.ShellCmdStatus.Outputing);
				}
			}
			if (!string.IsNullOrEmpty(value3))
			{
				ShellResponse.GenericResponseToStatus.Add(value3, ShellResponse.ShellCmdStatus.Completed);
			}
			mPassFlag = value7;
			if (mPassFlag.StartsWith("$"))
			{
				string key = mPassFlag.Substring(1);
				mPassFlag = base.Cache[key];
			}
			if (mPassFlag.StartsWith("!"))
			{
				mNotPassFlag = mPassFlag.Substring(1);
			}
			Smart.Log.Debug(TAG, $"============================ Start running ODM1Recipe {value12} ============================");
			string text = value11;
			if (text.StartsWith("$"))
			{
				string key2 = text.Substring(1);
				text = base.Cache[key2];
			}
			try
			{
				mShellResponse = new ShellResponse(text);
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, $"new ShellResponse({text}) is error {ex.Message + Environment.NewLine + ex.StackTrace}");
				mShellResponse = null;
			}
			string text2 = value10;
			string text3 = text2;
			Smart.Log.Verbose(TAG, "Build Command with Format");
			if (((dynamic)base.Info.Args).Format != null)
			{
				List<string> list2 = new List<string>();
				List<string> list3 = new List<string>();
				foreach (object item7 in ((dynamic)base.Info.Args).Format)
				{
					string text4 = (string)(dynamic)item7;
					string text5 = text4;
					string item = text5;
					if (text4.StartsWith("$"))
					{
						string text6 = text4.Substring(1);
						text5 = base.Cache[text6];
						switch (text6.ToLower())
						{
						case "user":
						case "pwd":
						case "ruser":
						case "rpwd":
							item = Smart.Security.EncryptString(text5);
							break;
						default:
							item = text5;
							break;
						}
					}
					list2.Add(text5);
					list3.Add(item);
				}
				text2 = string.Format(text2, list2.ToArray());
				text3 = string.Format(text3, list3.ToArray());
			}
			Smart.Log.Verbose(TAG, "command:" + text3);
			if (((dynamic)base.Info.Args).IconType != null)
			{
				string value13 = ((dynamic)base.Info.Args).IconType;
				mIconType = (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), value13, ignoreCase: true);
			}
			Smart.Log.Verbose(TAG, "LogMsgToPrompts");
			if (((dynamic)base.Info.Args).LogMsgToPrompts != null)
			{
				mLogMsgToPrompts = new List<List<string>>();
				foreach (dynamic item8 in ((dynamic)base.Info.Args).LogMsgToPrompts)
				{
					List<string> list4 = new List<string>();
					foreach (object item9 in item8)
					{
						string item2 = (string)(dynamic)item9;
						list4.Add(item2);
					}
					mLogMsgToPrompts.Add(list4);
				}
			}
			Smart.Log.Verbose(TAG, "LogMsgToClosePrompts");
			if (((dynamic)base.Info.Args).LogMsgToClosePrompts != null)
			{
				mLogMsgToClosePrompts = new List<string>();
				foreach (object item10 in ((dynamic)base.Info.Args).LogMsgToClosePrompts)
				{
					string item3 = (string)(dynamic)item10;
					mLogMsgToClosePrompts.Add(item3);
				}
			}
			Smart.Log.Verbose(TAG, "LogMsgToActions");
			if (((dynamic)base.Info.Args).LogMsgToActions != null)
			{
				mLogMsgToCommands = new List<List<string>>();
				foreach (dynamic item11 in ((dynamic)base.Info.Args).LogMsgToActions)
				{
					List<string> list5 = new List<string>();
					foreach (object item12 in item11)
					{
						string item4 = (string)(dynamic)item12;
						list5.Add(item4);
					}
					mLogMsgToCommands.Add(list5);
				}
			}
			Smart.Log.Verbose(TAG, "LogMsgToExtractValues");
			if (((dynamic)base.Info.Args).LogMsgToExtractValues != null)
			{
				mLogMsgToExtractValues = new List<List<string>>();
				foreach (dynamic item13 in ((dynamic)base.Info.Args).LogMsgToExtractValues)
				{
					List<string> list6 = new List<string>();
					foreach (object item14 in item13)
					{
						string item5 = (string)(dynamic)item14;
						list6.Add(item5);
					}
					mLogMsgToExtractValues.Add(list6);
				}
			}
			int num2 = 3;
			if (!string.IsNullOrEmpty(value2))
			{
				num2 = int.Parse(value2);
			}
			TimeSpan timeSpan = TimeSpan.FromSeconds(num2);
			string currentDirectory = Environment.CurrentDirectory;
			List<string> list7 = new List<string>();
			string text7 = text;
			if (text.EndsWith(".bat"))
			{
				text7 = Smart.Rsd.CreateMovedFolder(text);
			}
			Smart.Log.Verbose(TAG, "start shell");
			int num3 = 0;
			do
			{
				try
				{
					string directoryName = Path.GetDirectoryName(text7);
					if (Directory.Exists(directoryName))
					{
						Directory.SetCurrentDirectory(directoryName);
					}
					text7 = Smart.Convert.QuoteFilePathName(text7);
					mProcess = new Process();
					mProcess.StartInfo.FileName = text7;
					mProcess.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
					mProcess.StartInfo.Arguments = text2;
					mProcess.StartInfo.RedirectStandardInput = true;
					mProcess.StartInfo.RedirectStandardOutput = true;
					mProcess.StartInfo.RedirectStandardError = true;
					mProcess.StartInfo.UseShellExecute = useShellExecute;
					mProcess.EnableRaisingEvents = true;
					mProcess.StartInfo.CreateNoWindow = true;
					List<string> output = new List<string>();
					List<string> error = new List<string>();
					mProcess.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
					{
						Redirected(output, sender, e);
					};
					mProcess.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e)
					{
						Redirected(error, sender, e);
					};
					Smart.Log.Verbose(TAG, $"Starting shell command: {text} {text3}");
					mProcess.Start();
					mProcess.BeginOutputReadLine();
					mProcess.BeginErrorReadLine();
					if (((dynamic)base.Info.Args).PromptText != null)
					{
						device.Prompt.CloseMessageBox();
						string promptText = ((dynamic)base.Info.Args).PromptText.ToString();
						promptText = Smart.Locale.Xlate(promptText);
						string title = Smart.Locale.Xlate(base.Info.Name);
						Task.Run(delegate
						{
							//IL_0032: Unknown result type (might be due to invalid IL or missing references)
							//IL_0037: Unknown result type (might be due to invalid IL or missing references)
							device.Prompt.MessageBox(title, promptText, (MessageBoxButtons)0, mIconType);
						});
					}
					bool flag = false;
					if (mShellResponse != null)
					{
						int num4 = (int)timeSpan.TotalMilliseconds;
						Smart.Log.Debug(TAG, $"checkInterval is {num4} ms");
						for (int k = 0; k < (int)timeSpan.TotalMilliseconds; k += num4)
						{
							if (flag)
							{
								break;
							}
							flag = mProcess.WaitForExit(num4);
							if (flag)
							{
								Thread.Sleep(num4);
							}
							if (mCompleted || mTerminate)
							{
								if (!flag)
								{
									flag = mProcess.WaitForExit(num4 * 3);
								}
								break;
							}
						}
						Smart.Log.Debug(TAG, $"mError = {mError}, mCompleted = {mCompleted}, exited = {flag}, NoMsgReceived: {mNoMsgReceived}, mTerminate: {mTerminate}");
						if (!flag)
						{
							try
							{
								mProcess.Kill();
							}
							catch (Exception ex2)
							{
								Smart.Log.Debug(TAG, "Fail to shut down the shell command process - errorMsg " + ex2.Message);
							}
						}
						if ((mError && !mCompleted) || (!flag && !mCompleted) || mNoMsgReceived)
						{
							Smart.Log.Error(TAG, $"Shell step failed Error: {mError}, Completed: {mCompleted}, exited: {flag} NoMsgReceived: {mNoMsgReceived}");
							result = (Result)1;
						}
					}
					else
					{
						flag = mProcess.WaitForExit((int)timeSpan.TotalMilliseconds);
						if (!flag)
						{
							try
							{
								mProcess.Kill();
							}
							catch (Exception ex3)
							{
								Smart.Log.Debug(TAG, "Fail to shut down the shell command process - errorMsg " + ex3.Message);
							}
						}
						Smart.Log.Debug(TAG, $"Generic Shell - ExitCode: {mProcess.ExitCode} mCompleted: {mCompleted} exited {flag} NoMsgReceived: {mNoMsgReceived}");
						if (mProcess.ExitCode != 0 && !mCompleted)
						{
							result = (Result)1;
						}
					}
					if (mShellResponse != null)
					{
						mShellResponse.CleanUp();
					}
					Smart.Log.Verbose(TAG, $"Finished shell command: {text} {text3} with ExitCode = {mProcess.ExitCode} ");
					list7.AddRange(error);
					list7.AddRange(output);
				}
				catch (Exception ex4)
				{
					Smart.Log.Error(TAG, ex4.Message + Environment.NewLine + ex4.StackTrace);
				}
				finally
				{
					Directory.SetCurrentDirectory(currentDirectory);
					if (text.EndsWith(".bat"))
					{
						Smart.Rsd.RemoveMovedFolder(text);
					}
				}
				Smart.Log.Verbose(TAG, "StrToIndicateTestPass");
				if (mPassFlag != null)
				{
					string text8 = string.Join("", list7);
					Smart.Log.Info(TAG, $"Check if responses contain '{mPassFlag}'");
					if (text8.Contains(mPassFlag))
					{
						result = (Result)8;
						num3 = num;
					}
					else
					{
						result = (Result)1;
						num3++;
					}
				}
			}
			while (num3 < num);
			VerifyOnly(ref result);
			if (((int)result == 1 || (int)result == 4) && list7.Count > 0)
			{
				string text9 = ((list7.Count > 0) ? string.Join(" ", list7.ToArray()) : "No response from the device");
				string description = "Shell command failed";
				if (text9.ToLowerInvariant().Contains("wrong password".ToLowerInvariant()))
				{
					description = "Wrong password";
				}
				else if (text9.ToLowerInvariant().Contains("publicip Not allowed".ToLowerInvariant()) || text9.ToLowerInvariant().Contains("IP address not allowed".ToLowerInvariant()))
				{
					device.Prompt.CloseMessageBox();
					string text10 = "Please add your public ip to your account on RSD!";
					text10 = Smart.Locale.Xlate(text10);
					string text11 = Smart.Locale.Xlate(base.Info.Name);
					device.Prompt.MessageBox(text11, text10, (MessageBoxButtons)0, (MessageBoxIcon)16);
				}
				LogResult(result, description, text9);
			}
			else
			{
				LogResult(result);
			}
			if (((dynamic)base.Info.Args).PromptText != null)
			{
				device.Prompt.CloseMessageBox();
			}
			ShellResponse.ExeStringToShellCmdType.Remove(Path.GetFileName(value11));
			Smart.Log.Debug(TAG, $"============================ End ODM1Recipe {value12} ============================");
		}
	}

	private void Redirected(List<string> dataList, object sender, DataReceivedEventArgs e)
	{
		try
		{
			IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
			if (e.Data == null)
			{
				return;
			}
			if (mVerbose || mShellResponse == null || (mShellResponse.ShellCmd != ShellResponse.ShellCmdType.MMMPROGTOOL && mShellResponse.ShellCmd != ShellResponse.ShellCmdType.MOBAPROGTOOL))
			{
				Smart.Log.Verbose(TAG, "Shell Resp: " + e.Data);
			}
			mNoMsgReceived = false;
			string text = e.Data.Trim();
			if (!MessageToAction(text, mLogMsgToClosePrompts, mLogMsgToPrompts, mLogMsgToCommands, mLogMsgToExtractValues))
			{
				Smart.Log.Error(TAG, "Action reports failure - ignoring it now");
			}
			if (mShellResponse != null)
			{
				string responseKey;
				ShellResponse.ShellCmdStatus shellCmdStatus = mShellResponse.ParseResponse(text, out responseKey);
				if (shellCmdStatus != 0)
				{
					if (!mVerbose && (mShellResponse.ShellCmd == ShellResponse.ShellCmdType.MMMPROGTOOL || mShellResponse.ShellCmd == ShellResponse.ShellCmdType.MOBAPROGTOOL))
					{
						Smart.Log.Debug(TAG, "Shell Resp: " + e.Data);
					}
					Smart.Log.Debug(TAG, $"status: {shellCmdStatus.ToString()} responseKey: {responseKey}");
				}
				if (mShellResponse.ShellCmd == ShellResponse.ShellCmdType.GENERICTOOL)
				{
					dataList.Add(text);
				}
				string text2 = null;
				switch (shellCmdStatus)
				{
				case ShellResponse.ShellCmdStatus.Error:
					mError = true;
					dataList.Add(text);
					if (((dynamic)base.Info.Args).ErrorPromptText != null)
					{
						Task.Run(delegate
						{
							//IL_0130: Unknown result type (might be due to invalid IL or missing references)
							//IL_0135: Unknown result type (might be due to invalid IL or missing references)
							device.Prompt.CloseMessageBox();
							string text11 = ((dynamic)base.Info.Args).ErrorPromptText.ToString();
							text11 = Smart.Locale.Xlate(text11);
							string text12 = Smart.Locale.Xlate(base.Info.Name);
							device.Prompt.MessageBox(text12, text11, (MessageBoxButtons)0, mIconType);
						});
					}
					break;
				case ShellResponse.ShellCmdStatus.Connecting:
					if (!mFirstConnecting)
					{
						break;
					}
					mFirstConnecting = false;
					Smart.Log.Debug(TAG, "Closing current MessageBox ");
					device.Prompt.CloseMessageBox();
					if (((dynamic)base.Info.Args).ExPromptText != null)
					{
						string exPromptText = ((dynamic)base.Info.Args).ExPromptText.ToString();
						Smart.Log.Debug(TAG, "ExtPromptText: " + exPromptText);
						Task.Run(delegate
						{
							//IL_0078: Unknown result type (might be due to invalid IL or missing references)
							//IL_007d: Unknown result type (might be due to invalid IL or missing references)
							exPromptText = Smart.Locale.Xlate(exPromptText);
							string text10 = Smart.Locale.Xlate(base.Info.Name);
							Smart.Log.Debug(TAG, "Opening new MessageBox");
							device.Prompt.MessageBox(text10, exPromptText, (MessageBoxButtons)0, mIconType);
						});
					}
					break;
				case ShellResponse.ShellCmdStatus.Outputing:
					switch (mShellResponse.ShellCmd)
					{
					case ShellResponse.ShellCmdType.MTPROGTOOL:
						text2 = mShellResponse.ParseChinoeOutput(text, device, base.Cache);
						break;
					case ShellResponse.ShellCmdType.MOBAPROGTOOL:
					{
						string text3 = string.Empty;
						if (((dynamic)base.Info.Args).Output != null)
						{
							text3 = ((dynamic)base.Info.Args).Output;
						}
						string[] keys = text3.Split(new char[1] { ',' });
						text2 = mShellResponse.ParseMobaOutput(text, keys);
						break;
					}
					case ShellResponse.ShellCmdType.JAVAPROGTOOL:
						text2 = mShellResponse.ParseJavaOutput(text, device);
						break;
					case ShellResponse.ShellCmdType.LMPROGTOOL:
						text2 = mShellResponse.ParseLMOutput(text, device);
						mCompleted = true;
						break;
					case ShellResponse.ShellCmdType.P410PROGTOOL:
						text2 = mShellResponse.ParseP410Output(text, device);
						break;
					case ShellResponse.ShellCmdType.ZXPROGTOOL:
					case ShellResponse.ShellCmdType.LQPROGTOOL:
						text2 = mShellResponse.ParseZxOutput(text, device);
						break;
					case ShellResponse.ShellCmdType.GENERICTOOL:
						text2 = mShellResponse.ParseGenericOutput(text, mOutput, mOutputLength, base.Cache);
						break;
					default:
						Smart.Log.Debug(TAG, $"There is no method of parsing output data for {mShellResponse.ShellCmd.ToString()} tool");
						break;
					}
					break;
				case ShellResponse.ShellCmdStatus.Authenticating:
					if (mShellResponse.ShellCmd == ShellResponse.ShellCmdType.MMMPROGTOOL)
					{
						string empty = string.Empty;
						string empty2 = string.Empty;
						if (((dynamic)base.Info.Args).InputName != null && ((dynamic)base.Info.Args).InputName != string.Empty)
						{
							empty = ((dynamic)base.Info.Args).InputName.ToString();
							empty2 = base.Cache[empty];
						}
						else
						{
							empty = "DeviceSerialNumber";
							empty2 = device.SerialNumber;
						}
						string rsdLogId = device.Log.RsdLogId;
						string text4 = ((dynamic)base.Info.Args).ProdID;
						if (text4.StartsWith("$"))
						{
							string key = text4.Substring(1);
							text4 = base.Cache[key];
						}
						string text5 = string.Empty;
						if (((dynamic)base.Info.Args).KeyType != null && ((dynamic)base.Info.Args).KeyType != string.Empty)
						{
							text5 = ((dynamic)base.Info.Args).KeyType.ToString();
							if (text5.StartsWith("$"))
							{
								string key2 = text5.Substring(1);
								text5 = base.Cache[key2];
							}
						}
						string keyName = "1";
						if (((dynamic)base.Info.Args).KeyName != null && ((dynamic)base.Info.Args).KeyName != string.Empty)
						{
							keyName = ((dynamic)base.Info.Args).KeyName;
						}
						string text6 = string.Empty;
						if (((dynamic)base.Info.Args).InputFileName != null && ((dynamic)base.Info.Args).InputFileName != string.Empty)
						{
							text6 = ((dynamic)base.Info.Args).InputFileName.ToString();
							if (text6.StartsWith("$"))
							{
								string key3 = text6.Substring(1);
								text6 = base.Cache[key3];
							}
						}
						string clientReqType = "0x00";
						if (((dynamic)base.Info.Args).ClientReqType != null)
						{
							clientReqType = ((dynamic)base.Info.Args).ClientReqType;
						}
						try
						{
							mShellResponse.WriteInput(text, empty2, rsdLogId, clientReqType, text4, text5, keyName, text6, mProcess);
						}
						catch (Exception ex)
						{
							Smart.Log.Error(TAG, "Exception errorMsg: " + ex.Message);
							Smart.Log.Debug(TAG, ex.StackTrace);
							mError = true;
						}
						if (mProcess.StartInfo.Arguments.Contains("write_simlock ") && text.Contains("UnlockCode"))
						{
							string text7 = text.Substring(responseKey.Length).Split(new char[1] { ' ' })[1];
							base.Cache["lock1"] = text7;
							Smart.Log.Verbose(TAG, "Unlock Code: " + text7);
						}
					}
					if (mShellResponse.ShellCmd == ShellResponse.ShellCmdType.GENERICTOOL)
					{
						mShellResponse.WriteInputGeneric(text, mInput, responseKey, base.Cache, mProcess);
					}
					break;
				case ShellResponse.ShellCmdStatus.Downloading:
				{
					double downloadProgressPercent = mShellResponse.GetDownloadProgressPercent(text, responseKey);
					ProgressUpdate(downloadProgressPercent);
					break;
				}
				case ShellResponse.ShellCmdStatus.Completed:
					mCompleted = true;
					break;
				case ShellResponse.ShellCmdStatus.Writing:
					if (mShellResponse.ShellCmd == ShellResponse.ShellCmdType.JAVAPROGTOOL)
					{
						text2 = mShellResponse.ParseJavaWriteValue(text, base.Cache);
					}
					else if (mShellResponse.ShellCmd == ShellResponse.ShellCmdType.P410PROGTOOL)
					{
						text2 = mShellResponse.ParseP410WriteValue(text, base.Cache);
					}
					break;
				}
				if (text2 != null)
				{
					dataList.Add(text2);
				}
				return;
			}
			Smart.Log.Debug(TAG, $"mShellResponse is null");
			dataList.Add(text);
			if (((dynamic)base.Info.Args).ExpectedString != null)
			{
				string text8 = ((dynamic)base.Info.Args).ExpectedString;
				if (text.ToLower().Contains(text8.ToLower()))
				{
					mCompleted = true;
					Smart.Log.Debug(TAG, $"response: \"{text}\" contains ExpectedString: \"{text8}\"");
				}
			}
			if (!((((dynamic)base.Info.Args).NotExpectedStrings != null) ? true : false))
			{
				return;
			}
			foreach (object item in ((dynamic)base.Info.Args).NotExpectedStrings)
			{
				string text9 = (string)(dynamic)item;
				if (text.ToLower().Contains(text9.ToLower()))
				{
					mError = true;
					Smart.Log.Debug(TAG, $"response: \"{text}\" contains NotExpectedString: \"{text9}\"");
				}
			}
		}
		catch (Exception ex2)
		{
			mTerminate = true;
			mError = true;
			Smart.Log.Debug(TAG, "Exception - errMsg: " + ex2.Message);
			Smart.Log.Debug(TAG, ex2.StackTrace);
		}
	}

	private void ValidateResponse(ref Result result, string propValue, bool containedAll = false)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected I4, but got Unknown
		if ((int)result != 1)
		{
			result = (Result)(int)VerifyPropertyValue(propValue, logOnFailed: false, "value", containedAll);
		}
	}

	private List<SortedList<string, string>> ReadCsvFile(string filePath)
	{
		List<SortedList<string, string>> list = new List<SortedList<string, string>>();
		try
		{
			string[] array = File.ReadAllLines(filePath);
			if (array.Length == 0)
			{
				return list;
			}
			string[] array2 = array[0].Split(new char[1] { ',' });
			for (int i = 1; i < array.Length; i++)
			{
				string[] array3 = array[i].Split(new char[1] { ',' });
				SortedList<string, string> sortedList = new SortedList<string, string>();
				for (int j = 0; j < Math.Min(array2.Length, array3.Length); j++)
				{
					string key = array2[j].Trim();
					string value = array3[j].Trim();
					sortedList.Add(key, value);
				}
				list.Add(sortedList);
			}
			return list;
		}
		catch (Exception ex)
		{
			Smart.Log.Debug(TAG, $"Error occured while reading csv file: {ex.Message}");
			throw new FileNotFoundException($"Error occured while reading csv file: {ex.Message}");
		}
	}
}
