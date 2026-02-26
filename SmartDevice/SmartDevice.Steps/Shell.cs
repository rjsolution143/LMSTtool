using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class Shell : BaseStep
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

	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bf2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f93: Unknown result type (might be due to invalid IL or missing references)
		//IL_225f: Unknown result type (might be due to invalid IL or missing references)
		//IL_24c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_24c4: Invalid comparison between Unknown and I4
		//IL_24c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_24c8: Invalid comparison between Unknown and I4
		//IL_0a78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_25bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ed3: Unknown result type (might be due to invalid IL or missing references)
		//IL_24b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_24b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_25a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_25ae: Unknown result type (might be due to invalid IL or missing references)
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Result result = (Result)8;
		if (((dynamic)base.Info.Args).Verbose != null)
		{
			mVerbose = (bool)((dynamic)base.Info.Args).Verbose;
		}
		bool useShellExecute = false;
		if (((dynamic)base.Info.Args).UseShellExecute != null)
		{
			useShellExecute = (bool)((dynamic)base.Info.Args).UseShellExecute;
		}
		string text = ((dynamic)base.Info.Args).EXE;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
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
		string text2 = ((dynamic)base.Info.Args).Command;
		string text3 = text2;
		Smart.Log.Verbose(TAG, "Build Command with Format");
		if (((dynamic)base.Info.Args).Format != null)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			foreach (object item6 in ((dynamic)base.Info.Args).Format)
			{
				string text4 = (string)(dynamic)item6;
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
				list.Add(text5);
				list2.Add(item);
			}
			text2 = string.Format(text2, list.ToArray());
			text3 = string.Format(text3, list2.ToArray());
		}
		Smart.Log.Verbose(TAG, "command:" + text3);
		if (((dynamic)base.Info.Args).IconType != null)
		{
			string value = ((dynamic)base.Info.Args).IconType;
			mIconType = (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), value, ignoreCase: true);
		}
		Smart.Log.Verbose(TAG, "LogMsgToPrompts");
		if (((dynamic)base.Info.Args).LogMsgToPrompts != null)
		{
			mLogMsgToPrompts = new List<List<string>>();
			foreach (dynamic item7 in ((dynamic)base.Info.Args).LogMsgToPrompts)
			{
				List<string> list3 = new List<string>();
				foreach (object item8 in item7)
				{
					string item2 = (string)(dynamic)item8;
					list3.Add(item2);
				}
				mLogMsgToPrompts.Add(list3);
			}
		}
		Smart.Log.Verbose(TAG, "LogMsgToClosePrompts");
		if (((dynamic)base.Info.Args).LogMsgToClosePrompts != null)
		{
			mLogMsgToClosePrompts = new List<string>();
			foreach (object item9 in ((dynamic)base.Info.Args).LogMsgToClosePrompts)
			{
				string item3 = (string)(dynamic)item9;
				mLogMsgToClosePrompts.Add(item3);
			}
		}
		Smart.Log.Verbose(TAG, "LogMsgToActions");
		if (((dynamic)base.Info.Args).LogMsgToActions != null)
		{
			mLogMsgToCommands = new List<List<string>>();
			foreach (dynamic item10 in ((dynamic)base.Info.Args).LogMsgToActions)
			{
				List<string> list4 = new List<string>();
				foreach (object item11 in item10)
				{
					string item4 = (string)(dynamic)item11;
					list4.Add(item4);
				}
				mLogMsgToCommands.Add(list4);
			}
		}
		Smart.Log.Verbose(TAG, "LogMsgToExtractValues");
		if (((dynamic)base.Info.Args).LogMsgToExtractValues != null)
		{
			mLogMsgToExtractValues = new List<List<string>>();
			foreach (dynamic item12 in ((dynamic)base.Info.Args).LogMsgToExtractValues)
			{
				List<string> list5 = new List<string>();
				foreach (object item13 in item12)
				{
					string item5 = (string)(dynamic)item13;
					list5.Add(item5);
				}
				mLogMsgToExtractValues.Add(list5);
			}
		}
		int num = 60;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(num);
		string currentDirectory = Environment.CurrentDirectory;
		List<string> list6 = new List<string>();
		string text7 = text;
		if (text.EndsWith(".bat"))
		{
			text7 = Smart.Rsd.CreateMovedFolder(text);
		}
		Smart.Log.Verbose(TAG, "start shell");
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
			InputToStdInput();
			bool flag = false;
			if (mShellResponse != null)
			{
				int num2 = Math.Min(3000, (int)timeSpan.TotalMilliseconds / 5);
				Smart.Log.Debug(TAG, $"checkInterval is {num2} ms");
				for (int i = 0; i < (int)timeSpan.TotalMilliseconds; i += num2)
				{
					if (flag)
					{
						break;
					}
					flag = mProcess.WaitForExit(num2);
					if (flag)
					{
						Thread.Sleep(num2);
					}
					if (mCompleted || mTerminate)
					{
						if (!flag)
						{
							flag = mProcess.WaitForExit(num2 * 3);
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
				if (((dynamic)base.Info.Args).UseExitCodeToDetermineResult != null && (bool)((dynamic)base.Info.Args).UseExitCodeToDetermineResult && mProcess.ExitCode != 0)
				{
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
			list6.AddRange(error);
			list6.AddRange(output);
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
		Smart.Log.Verbose(TAG, "RespIndex");
		if (((dynamic)base.Info.Args).RespIndex == null)
		{
			if (mShellResponse != null)
			{
				foreach (string item14 in list6)
				{
					ValidateResponse(ref result, item14);
				}
			}
			else
			{
				string propValue = string.Join("\r\n", list6.ToArray());
				ValidateResponse(ref result, propValue, containedAll: true);
			}
		}
		else
		{
			int num3 = ((dynamic)base.Info.Args).RespIndex;
			if (num3 < list6.Count)
			{
				ValidateResponse(ref result, list6[num3]);
			}
			else
			{
				result = (Result)1;
				Smart.Log.Error(TAG, $"Args.RespIndex: {num3} is out of range of [0..{list6.Count - 1}]");
			}
		}
		Smart.Log.Verbose(TAG, "StrToIndicateTestPass");
		if (((dynamic)base.Info.Args).StrToIndicateTestPass != null)
		{
			string text8 = ((dynamic)base.Info.Args).StrToIndicateTestPass.ToString();
			string text9 = string.Join("", list6.ToArray());
			Smart.Log.Info(TAG, $"Check response {text9} contain {text8}");
			result = ((!text9.Contains(text8)) ? ((Result)1) : ((Result)8));
		}
		VerifyOnly(ref result);
		if (((int)result == 1 || (int)result == 4) && list6.Count > 0)
		{
			string text10 = ((list6.Count > 0) ? string.Join(" ", list6.ToArray()) : "No response from the device");
			string description = "Shell command failed";
			if (text10.ToLowerInvariant().Contains("wrong password".ToLowerInvariant()))
			{
				description = "Wrong password";
			}
			else if (text10.ToLowerInvariant().Contains("publicip Not allowed".ToLowerInvariant()) || text10.ToLowerInvariant().Contains("IP address not allowed".ToLowerInvariant()))
			{
				device.Prompt.CloseMessageBox();
				string text11 = "Please add your public ip to your account on RSD!";
				text11 = Smart.Locale.Xlate(text11);
				string text12 = Smart.Locale.Xlate(base.Info.Name);
				device.Prompt.MessageBox(text12, text11, (MessageBoxButtons)0, (MessageBoxIcon)16);
			}
			LogResult(result, description, text10);
		}
		else
		{
			LogResult(result);
		}
		if (((dynamic)base.Info.Args).PromptText != null)
		{
			device.Prompt.CloseMessageBox();
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
					default:
						Smart.Log.Debug(TAG, $"There is no method of parsing output data for {mShellResponse.ShellCmd.ToString()} tool");
						break;
					}
					break;
				case ShellResponse.ShellCmdStatus.Authenticating:
				{
					if (mShellResponse.ShellCmd != ShellResponse.ShellCmdType.MMMPROGTOOL)
					{
						break;
					}
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
					break;
				}
				case ShellResponse.ShellCmdStatus.Downloading:
				{
					double downloadProgressPercent = mShellResponse.GetDownloadProgressPercent(text, responseKey);
					ProgressUpdate(downloadProgressPercent);
					break;
				}
				case ShellResponse.ShellCmdStatus.Completed:
					mCompleted = true;
					text2 = text;
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

	private void InputToStdInput()
	{
		Smart.Log.Verbose(MethodBase.GetCurrentMethod().Name, "Enter...");
		if (!((((dynamic)base.Info.Args).Input != null) ? true : false))
		{
			return;
		}
		string[] array = ((string)((dynamic)base.Info.Args).Input).Split(new char[1] { ',' });
		int millisecondsTimeout = 4000;
		if (((dynamic)base.Info.Args).InputIntervalMs != null)
		{
			millisecondsTimeout = ((dynamic)base.Info.Args).InputIntervalMs;
		}
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			if (text.StartsWith("$"))
			{
				string key = text.Substring(1);
				text = base.Cache[key];
			}
			if (text != string.Empty)
			{
				Smart.Log.Debug(TAG, $"Write \"{text}\" to stdInput");
				mProcess.StandardInput.WriteLine(text);
				mProcess.StandardInput.Flush();
				if (i < array.Length - 1)
				{
					Thread.Sleep(millisecondsTimeout);
				}
			}
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
}
