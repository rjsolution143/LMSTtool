using System;
using System.Collections.Generic;
using System.Management;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class UnisocEnterClibrationMode : UnisocProgramming
{
	private string TAG => GetType().FullName;

	[Obsolete]
	private void Timer_Tick(object sender, EventArgs e)
	{
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = string.Join(" ", error) + " " + string.Join(" ", output);
		Smart.Log.Debug("Timer_Tick", "response:" + text);
		if (text.Contains("<EnterMode> pass"))
		{
			device.Prompt.CloseMessageBox();
			return;
		}
		Smart.Log.Debug("Timer_Tick", "CloseMessageBox...");
		device.Prompt.CloseMessageBox();
		if (((dynamic)base.Info.Args).PromptText != null)
		{
			string promptText = ((dynamic)base.Info.Args).PromptText;
			Task.Run(delegate
			{
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				promptText = Smart.Locale.Xlate(promptText);
				string text2 = Smart.Locale.Xlate(base.Info.Name);
				Smart.Log.Debug(TAG, "Opening new MessageBox");
				device.Prompt.MessageBox(text2, promptText, (MessageBoxButtons)0, (MessageBoxIcon)64);
			});
		}
	}

	protected int GetAndSaveComportToLocalFileByPidVid(string deviceVidPid)
	{
		Smart.Log.Info(MethodBase.GetCurrentMethod().Name, "enter...");
		int result = 0;
		string empty = string.Empty;
		empty = GetDeviceComportNumberByPidVid(deviceVidPid);
		if (empty == string.Empty)
		{
			string text = "Not found Comport";
			Smart.Log.Error(TAG, text);
			dynamic_data = dynamic_data + "_" + text;
			result = -1;
		}
		else if ((!empty.Equals(base.Cache["kId"])))
		{
			string text2 = string.Format("Comport in Shell {0} vs actual comport {1}.", base.Cache["kId"], empty);
			Smart.Log.Error(TAG, text2);
			dynamic_data = dynamic_data + "_" + text2;
			base.Cache["kId"] = empty;
			SavePortToLocalFile(base.Cache["pidvid"], base.Cache["kId"], ComportCfgFile);
			result = 0;
		}
		Smart.Log.Info(MethodBase.GetCurrentMethod().Name, "exit...");
		return result;
	}

	private string GetDeviceComportNumberByPidVid(string deviceVidPid)
	{
		//IL_0a4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7a: Unknown result type (might be due to invalid IL or missing references)
		string name = MethodBase.GetCurrentMethod().Name;
		Smart.Log.Info(name, "enter...");
		string text = deviceVidPid.Substring(deviceVidPid.LastIndexOf('\\') + 1).Trim();
		Smart.Log.Info(name, $"id To Search ComPort {text}");
		if (!text.Contains("&"))
		{
			text = string.Empty;
		}
		if (((dynamic)base.Info.Args).IdToSearchComPort != null && ((dynamic)base.Info.Args).IdToSearchComPort != string.Empty)
		{
			text = ((dynamic)base.Info.Args).IdToSearchComPort;
		}
		if (((dynamic)base.Info.Args).OnlyUseComPortNameToSearchDevice != null && (bool)((dynamic)base.Info.Args).OnlyUseComPortNameToSearchDevice)
		{
			text = string.Empty;
		}
		Smart.Log.Info(name, $"Final id To Search ComPort {text}");
		string text2 = "SPRD";
		if (((dynamic)base.Info.Args).ExpectedPartialComportName != null && ((dynamic)base.Info.Args).ExpectedPartialComportName != string.Empty)
		{
			text2 = ((dynamic)base.Info.Args).ExpectedPartialComportName;
		}
		double num = 18.0;
		if (((dynamic)base.Info.Args).TimeoutSecToSearchComPort != null)
		{
			num = (double)((dynamic)base.Info.Args).TimeoutSecToSearchComPort;
		}
		DateTime now = DateTime.Now;
		double num2 = num * 1000.0;
		string text3 = string.Empty;
		while (DateTime.Now.Subtract(now).TotalMilliseconds < num2 && text3 == string.Empty)
		{
			string empty = string.Empty;
			empty = ((!(text == string.Empty)) ? $"SELECT * FROM Win32_PnPEntity where DeviceID like'%{text}%' and Name like '%{text2}%'" : $"SELECT * FROM Win32_PnPEntity where Name like '%{text2}%'");
			Smart.Log.Verbose(name, empty);
			ManagementObjectSearcher val = new ManagementObjectSearcher(empty);
			Smart.Log.Verbose(name, "query done");
			ManagementObjectEnumerator enumerator = val.Get().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string text4 = ((ManagementBaseObject)(ManagementObject)enumerator.Current).Properties["Name"].Value.ToString();
					Smart.Log.Verbose(name, "Port Name:" + text4);
					if (text4.ToUpper().Contains("COM") && text4.ToUpper().Contains("DIAG"))
					{
						text3 = text4.Substring(text4.IndexOf("(COM") + 4).Trim().Trim(new char[1] { ')' });
						Smart.Log.Verbose(name, "Port number:" + text3);
						if (Convert.ToInt32(text3) > 0)
						{
							break;
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator)?.Dispose();
			}
			Thread.Sleep(1000);
		}
		Smart.Log.Verbose(name, $"Found {text2} = {text3}");
		return text3;
	}

	public bool DeviceInCalMode(string deviceVidPid)
	{
		if (string.IsNullOrEmpty(GetDeviceComportNumberByPidVid(deviceVidPid).Trim()))
		{
			return false;
		}
		return true;
	}

	protected bool FastbootPowerOff(IDevice device)
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
		bool flag2 = true;
		if (((dynamic)base.Info.Args).SetBackFlag != null)
		{
			flag2 = (bool)((dynamic)base.Info.Args).SetBackFlag;
		}
		if (flag2)
		{
			empty = "oem set_backflag";
			if (((dynamic)base.Info.Args).BackFlagCommand != null && ((dynamic)base.Info.Args).BackFlagCommand != string.Empty)
			{
				empty = ((dynamic)base.Info.Args).BackFlagCommand;
			}
			FastbootSendCmd(device, empty, timeoutSec, strToIndicateCmdPass);
		}
		empty = "oem poweroff";
		if (((dynamic)base.Info.Args).PowerOffCommand != null && ((dynamic)base.Info.Args).PowerOffCommand != string.Empty)
		{
			empty = ((dynamic)base.Info.Args).PowerOffCommand;
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

	protected int ExtractComPortFromShellResponse(List<string> dataList)
	{
		int result = 0;
		string pattern = "COM\\d*";
		if (((dynamic)base.Info.Args).PatternToLocatePortNumber != null)
		{
			pattern = ((dynamic)base.Info.Args).PatternToLocatePortNumber;
		}
		for (int num = dataList.Count - 1; num >= 0; num--)
		{
			string input = dataList[num];
			Regex regex = new Regex(pattern);
			if (regex.IsMatch(input))
			{
				string value = regex.Match(input).Groups[0].Value;
				Smart.Log.Verbose(TAG, "comport extract from shell response is " + value);
				base.Cache["kId"] = value.Substring(3);
				break;
			}
		}
		return result;
	}

	[Obsolete]
	protected void GetAndSaveComport(IDevice device)
	{
		Smart.Log.Info(MethodBase.GetCurrentMethod().Name, "enter...");
		string text = "SPRD U2S Diag";
		if (((dynamic)base.Info.Args).ComportName != null && ((dynamic)base.Info.Args).ComportName != string.Empty)
		{
			text = ((dynamic)base.Info.Args).ComportName;
		}
		int num = 10;
		if (((dynamic)base.Info.Args).WaitingSecToFindComPort != null)
		{
			num = ((dynamic)base.Info.Args).WaitingSecToFindComPort;
		}
		string text2 = Smart.Rsd.ComPortId(text, num);
		Smart.Log.Debug(TAG, $"Found {text} = {text2}");
		if (text2 == string.Empty)
		{
			string text3 = "Not found Com port " + text + " in PC Device Manager.";
			Smart.Log.Error(TAG, text3);
			throw new Exception(text3);
		}
		if ((!text2.Equals(base.Cache["kId"])))
		{
			string text4 = string.Format("Shell cmd passed in Com port {0} doesn't match with actual com port {1}. Update to actual Com port.", base.Cache["kId"], text2);
			Smart.Log.Error(TAG, text4);
			base.Cache["kId"] = text2;
			SavePortToLocalFile(base.Cache["pidvid"], text2, ComportCfgFile);
			throw new Exception(text4);
		}
	}
}
