using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class FindMtkMetaComIds : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_102c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1033: Unknown result type (might be due to invalid IL or missing references)
		//IL_1035: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1650: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_17da: Invalid comparison between Unknown and I4
		//IL_17c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1797: Unknown result type (might be due to invalid IL or missing references)
		//IL_17e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_17dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_17de: Invalid comparison between Unknown and I4
		//IL_17ee: Unknown result type (might be due to invalid IL or missing references)
		Result val = (Result)1;
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).EXE;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			_ = (int)((dynamic)base.Info.Args).Timeout;
		}
		string key2 = "pId";
		if (((dynamic)base.Info.Args).PreloaderId != null)
		{
			key2 = ((dynamic)base.Info.Args).PreloaderId;
		}
		string text2 = "kId";
		if (((dynamic)base.Info.Args).KernelId != null)
		{
			text2 = ((dynamic)base.Info.Args).KernelId;
		}
		string text3 = "MediaTek PreLoader USB";
		if (((dynamic)base.Info.Args).PreloaderPortName != null)
		{
			text3 = ((dynamic)base.Info.Args).PreloaderPortName;
		}
		string kernelPortName = "MediaTek USB VCOM";
		if (((dynamic)base.Info.Args).KernelPortName != null)
		{
			kernelPortName = ((dynamic)base.Info.Args).kernelPortName;
		}
		string project = "malta";
		if (((dynamic)base.Info.Args).Project != null)
		{
			project = ((dynamic)base.Info.Args).Project;
		}
		int seachPortTimeout = 15;
		if (((dynamic)base.Info.Args).TimeoutSecToSeachPreloaderPort != null)
		{
			seachPortTimeout = ((dynamic)base.Info.Args).TimeoutSecToSeachPreloaderPort;
		}
		int waitSecForToolExit = 30;
		if (((dynamic)base.Info.Args).TimeoutSecToWaitForToolExit != null)
		{
			waitSecForToolExit = ((dynamic)base.Info.Args).TimeoutSecToWaitForToolExit;
		}
		int timeoutSecToSeachKernalPort = 20;
		if (((dynamic)base.Info.Args).TimeoutSecToSeachKernalPort != null)
		{
			timeoutSecToSeachKernalPort = ((dynamic)base.Info.Args).TimeoutSecToSeachKernalPort;
		}
		string text4 = string.Empty;
		string empty = string.Empty;
		string currentDirectory = Environment.CurrentDirectory;
		Directory.SetCurrentDirectory(Path.GetDirectoryName(text));
		text = Smart.Convert.QuoteFilePathName(text);
		List<string> list = new List<string>();
		list = Smart.Rsd.ComPortIdList(text3);
		if (list.Count > 1)
		{
			string text5 = "Multiple preloader port found, please only connect one device";
			Smart.Log.Error(TAG, text5);
			device.Prompt.MessageBox("Error", text5, (MessageBoxButtons)0, (MessageBoxIcon)16);
			val = (Result)1;
			LogResult(val, text5);
			return;
		}
		if (list.Count < 1)
		{
			string text6 = "No preloader port found";
			Smart.Log.Verbose(TAG, text6);
		}
		else
		{
			text4 = list[0];
		}
		if (text4 == string.Empty)
		{
			for (int i = 0; i < 3; i++)
			{
				bool flag = false;
				if (((dynamic)base.Info.Args).NeedPowerOffPhone != null && (bool)((dynamic)base.Info.Args).NeedPowerOffPhone)
				{
					if (DeviceInFastbootMode(device))
					{
						flag = Smart.Thread.RunAndWait<bool>((Func<bool>)(() => FastbootPowerOff(device)), true);
					}
					else if (DeviceInAdbMode(device))
					{
						flag = Smart.Thread.RunAndWait<bool>((Func<bool>)(() => AdbPowerOff(device)), true);
					}
					else
					{
						Smart.Log.Debug(TAG, "Device not in fastboot mode or adb mode");
					}
				}
				if (!flag)
				{
					device.Prompt.CloseMessageBox();
					if (((dynamic)base.Info.Args).PromptTextToConnectPhone != null || ((dynamic)base.Info.Args).PromptText != null)
					{
						Task.Run(delegate
						{
							//IL_023c: Unknown result type (might be due to invalid IL or missing references)
							//IL_0241: Unknown result type (might be due to invalid IL or missing references)
							Smart.Log.Verbose(TAG, "Prompting user to reconnect in fastboot/adb/poweroff mode");
							string text12 = ((((dynamic)base.Info.Args).PromptTextToConnectPhone == null) ? ((dynamic)base.Info.Args).PromptText : ((dynamic)base.Info.Args).PromptTextToConnectPhone);
							text12 = Smart.Locale.Xlate(text12);
							string text13 = Smart.Locale.Xlate(base.Info.Name);
							DialogResult val3 = device.Prompt.MessageBox(text13, text12, (MessageBoxButtons)0, (MessageBoxIcon)64);
							Smart.Log.Debug(TAG, "User click " + ((object)(DialogResult)(ref val3)).ToString());
						});
						Thread.Sleep(100);
					}
				}
				text4 = SearchComPort(text3, list, seachPortTimeout);
				device.Prompt.CloseMessageBox();
				if (!(text4 == string.Empty))
				{
					base.Cache[key2] = text4;
					break;
				}
			}
		}
		device.Prompt.CloseMessageBox();
		if (text4 == string.Empty)
		{
			string text7 = $"Preloader COM port of {text3} is not found";
			Smart.Log.Error(TAG, text7);
			val = (Result)1;
		}
		else
		{
			Smart.Log.Error(TAG, $"Found preloader port {text4}, start to switch to meta mode");
			List<string> existingKernalPortList = new List<string>();
			val = SiwtchToMetaMode(device, text4, project, text, waitSecForToolExit);
			empty = Smart.Thread.RunAndWait<string>((Func<string>)(() => SearchComPort(kernelPortName, existingKernalPortList, timeoutSecToSeachKernalPort)), true);
			device.Prompt.CloseMessageBox();
			if (empty == string.Empty)
			{
				string text8 = $"Kernal COM port of \"{kernelPortName}\" is not found";
				Smart.Log.Error(TAG, text8);
				Task.Run(delegate
				{
					//IL_0234: Unknown result type (might be due to invalid IL or missing references)
					//IL_0239: Unknown result type (might be due to invalid IL or missing references)
					Smart.Log.Verbose(TAG, "Prompting user to connect phone again due to meta com port is not found");
					string text10 = "Please remove USB cable and connect USB cable again";
					if (((dynamic)base.Info.Args).PromptTextToConnectPhoneForMetaPortDetection != null)
					{
						text10 = ((dynamic)base.Info.Args).PromptTextToConnectPhoneForMetaPortDetection.ToString();
					}
					text10 = Smart.Locale.Xlate(text10);
					string text11 = Smart.Locale.Xlate(base.Info.Name);
					DialogResult val2 = device.Prompt.MessageBox(text11, text10, (MessageBoxButtons)0, (MessageBoxIcon)64);
					Smart.Log.Debug(TAG, "User click " + ((object)(DialogResult)(ref val2)).ToString());
				});
				Thread.Sleep(100);
				empty = Smart.Thread.RunAndWait<string>((Func<string>)(() => SearchComPort(kernelPortName, existingKernalPortList, timeoutSecToSeachKernalPort)), true);
			}
			device.Prompt.CloseMessageBox();
			if (empty == string.Empty)
			{
				string text9 = $"Kernal COM port of {kernelPortName} is not found after retry";
				Smart.Log.Error(TAG, text9);
				val = (Result)1;
			}
			else
			{
				Smart.Log.Debug(TAG, $"Found {text2} = {empty}");
				base.Cache[text2] = empty;
				val = (Result)8;
			}
		}
		Directory.SetCurrentDirectory(currentDirectory);
		VerifyOnly(ref val);
		if ((int)val == 1 || (int)val == 4)
		{
			LogResult(val, "Kernal Port not found");
		}
		else
		{
			LogResult(val);
		}
	}

	private string SearchComPort(string expectedComPortName, List<string> existingComPortList, int seachPortTimeout)
	{
		string text = string.Empty;
		DateTime now = DateTime.Now;
		double num = seachPortTimeout * 1000;
		while (DateTime.Now.Subtract(now).TotalMilliseconds < num)
		{
			List<string> list = Smart.Rsd.ComPortIdList(expectedComPortName);
			Smart.Log.Debug(TAG, string.Format("Found {0} COM Port:{1}", expectedComPortName, string.Join(",", list)));
			List<string> list2 = list.Except(existingComPortList).ToList();
			if (list2.Count == 1)
			{
				text = list2[0];
				Smart.Log.Debug(TAG, "Found one new Com Port:" + text);
				break;
			}
			if (list2.Count > 1)
			{
				Smart.Log.Error(TAG, string.Format("Found more than one new COM Port:{0}, need to add 'StepLockName' to all power off related steps in recipe", string.Join(",", list2)));
			}
			else
			{
				Smart.Log.Error(TAG, "Not found new COM Port");
			}
			Thread.Sleep(500);
		}
		return text;
	}

	private Result SiwtchToMetaMode(IDevice device, string pId, string project, string exe, int waitSecForToolExit)
	{
		_ = string.Empty;
		Process process = new Process();
		process.StartInfo.FileName = exe;
		process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.UseShellExecute = false;
		process.EnableRaisingEvents = true;
		process.StartInfo.CreateNoWindow = true;
		List<string> output = new List<string>();
		List<string> error = new List<string>();
		process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
		{
			Redirected(output, sender, e);
		};
		process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e)
		{
			Redirected(error, sender, e);
		};
		string command = $"-p {pId} -odm1 ontim -project {project} -m user -t verify_track_id {device.ID} -skip";
		command = BuildCommand(command);
		process.StartInfo.Arguments = command;
		process.Start();
		process.PriorityClass = ProcessPriorityClass.RealTime;
		process.PriorityBoostEnabled = true;
		process.BeginOutputReadLine();
		process.BeginErrorReadLine();
		Smart.Log.Debug(TAG, $"Starting command: {exe} {command}");
		if (!process.WaitForExit(waitSecForToolExit * 1000))
		{
			try
			{
				process.Kill();
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, "Timeout to wait for tool exit, kill process error:" + ex.Message);
			}
		}
		Smart.Log.Verbose(TAG, "Tool exited wit code " + process.ExitCode);
		List<string> list = new List<string>();
		list.AddRange(error);
		list.AddRange(output);
		if (((dynamic)base.Info.Args).StrToIndicateTestPass != null)
		{
			string text = ((dynamic)base.Info.Args).StrToIndicateTestPass.ToString();
			string text2 = string.Join("", list.ToArray());
			Smart.Log.Info(TAG, $"Check response {text2} contain {text}");
			if (!text2.Contains(text))
			{
				return (Result)1;
			}
			return (Result)8;
		}
		if (process.ExitCode != 0)
		{
			return (Result)1;
		}
		return (Result)8;
	}

	private string SearchPreloaderComport(IDevice device, string preloaderPortName, string preLoaderId, int timeoutSecToSeachComport)
	{
		_ = string.Empty;
		string text = Smart.Rsd.ComPortId(preloaderPortName, timeoutSecToSeachComport);
		if (text == string.Empty)
		{
			string text2 = $"Preloader COM port {preloaderPortName} is not found in {timeoutSecToSeachComport} seconds";
			Smart.Log.Error(TAG, text2);
		}
		return text;
	}

	private string BuildCommand(string command)
	{
		if (((dynamic)base.Info.Args).Command != null)
		{
			command = ((dynamic)base.Info.Args).Command.ToString();
		}
		if (((dynamic)base.Info.Args).Format != null)
		{
			List<string> list = new List<string>();
			foreach (object item2 in ((dynamic)base.Info.Args).Format)
			{
				string text = (string)(dynamic)item2;
				string item = text;
				if (text.StartsWith("$"))
				{
					string text2 = text.Substring(1);
					item = base.Cache[text2];
					text2.ToLower();
				}
				list.Add(item);
			}
			command = string.Format(command, list.ToArray());
		}
		Smart.Log.Verbose(TAG, $"Shell command to be used: {command}");
		return command;
	}

	private bool DeviceInAdbMode(IDevice device)
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
				Smart.Log.Verbose(TAG, "Sending cmd to confirm adb device exist");
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

	private bool DeviceInMetaMode(string kernelPortName, int timeoutSec)
	{
		string text = Smart.Rsd.ComPortId(kernelPortName, timeoutSec);
		Smart.Log.Debug("DeviceInMetaMode", "Found kId is " + text);
		if (text == string.Empty)
		{
			return false;
		}
		return true;
	}

	private bool DeviceInFastbootMode(IDevice device)
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

	private bool AdbPowerOff(IDevice device)
	{
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		Smart.Log.Info(MethodBase.GetCurrentMethod().Name, "enter...");
		bool flag = true;
		string text = "reboot -p";
		if (((dynamic)base.Info.Args).AdbPowerOffCommand != null && ((dynamic)base.Info.Args).AdbPowerOffCommand != string.Empty)
		{
			text = ((dynamic)base.Info.Args).AdbPowerOffCommand;
		}
		int num = 10;
		if (((dynamic)base.Info.Args).PowerOffTimeoutSec != null && ((dynamic)base.Info.Args).PowerOffTimeoutSec != string.Empty)
		{
			num = ((dynamic)base.Info.Args).PowerOffTimeoutSec;
		}
		num *= 1000;
		string text2 = "Done";
		string empty = string.Empty;
		try
		{
			string filePathName = Smart.Rsd.GetFilePathName("adbExe", base.Recipe.Info.UseCase, device);
			int num2 = -1;
			List<string> list = Smart.MotoAndroid.Shell(device.ID, text, num, filePathName, ref num2, 6000, false);
			empty = string.Join("\r\n", list.ToArray());
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error sending ADB Command: " + ex.Message);
			Smart.Log.Error(TAG, ex.ToString());
			return false;
		}
		if (empty.ToUpper().Contains(text2.ToUpper()))
		{
			Smart.Log.Info(TAG, "adb power off pass");
			return true;
		}
		Smart.Log.Info(TAG, "adb power off fail");
		return false;
	}

	private bool FastbootPowerOff(IDevice device)
	{
		Smart.Log.Info(MethodBase.GetCurrentMethod().Name, "enter...");
		bool flag = true;
		int timeoutSec = 8;
		if (((dynamic)base.Info.Args).FBCmdTimeoutSec != null && ((dynamic)base.Info.Args).FBCmdTimeoutSec != string.Empty)
		{
			timeoutSec = (int)((dynamic)base.Info.Args).FBCmdTimeoutSec;
		}
		string strToIndicateCmdPass = "OKAY";
		string empty = string.Empty;
		empty = "oem poweroff";
		if (((dynamic)base.Info.Args).FastbootPowerOffCommand != null && ((dynamic)base.Info.Args).FastbootPowerOffCommand != string.Empty)
		{
			empty = ((dynamic)base.Info.Args).FastbootPowerOffCommand;
		}
		if (FastbootSendCmd(device, empty, timeoutSec, strToIndicateCmdPass))
		{
			flag = true;
			Smart.Log.Verbose(TAG, "fastboot power off pass");
		}
		else
		{
			flag = false;
			Smart.Log.Verbose(TAG, "fastboot power off fail");
		}
		Smart.Log.Info(MethodBase.GetCurrentMethod().Name, "exit...");
		return flag;
	}

	private bool FastbootSendCmd(IDevice device, string command, int timeoutSec, string strToIndicateCmdPass)
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

	private void Redirected(List<string> dataList, object sender, DataReceivedEventArgs e)
	{
		if (e.Data != null && e.Data.Trim().Length > 0)
		{
			Smart.Log.Debug(TAG, "Shell Resp: " + e.Data);
		}
	}
}
