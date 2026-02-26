using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

[Obsolete]
public class UnisocFindComPort : UnisocBaseTest
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0b: Unknown result type (might be due to invalid IL or missing references)
		Result result = (Result)1;
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		int num = 30;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
		}
		int num2 = 3;
		if (((dynamic)base.Info.Args).RetryTimes != null)
		{
			num2 = ((dynamic)base.Info.Args).RetryTimes;
		}
		string text = string.Empty;
		if (((dynamic)base.Info.Args).ComportNum != null)
		{
			text = ((dynamic)base.Info.Args).ComportNum;
		}
		string comportName = "SPRD U2S Diag";
		if (((dynamic)base.Info.Args).ComportName != null && ((dynamic)base.Info.Args).ComportName != string.Empty)
		{
			comportName = ((dynamic)base.Info.Args).ComportName;
		}
		if (text == string.Empty)
		{
			for (int i = 0; i < num2; i++)
			{
				text = Smart.Rsd.ComPortId(comportName, num);
				if (!(text == string.Empty))
				{
					break;
				}
				string text2 = $"COM port {comportName} is not found...";
				Smart.Log.Error(TAG, text2);
				if (((dynamic)base.Info.Args).PromptText != null)
				{
					Task.Run(delegate
					{
						//IL_0151: Unknown result type (might be due to invalid IL or missing references)
						device.Prompt.CloseMessageBox();
						Smart.Log.Debug(TAG, "Prompting user to disconnect/reconnect and retrying port " + comportName);
						string text5 = ((dynamic)base.Info.Args).PromptText.ToString();
						text5 = Smart.Locale.Xlate(text5);
						string text6 = Smart.Locale.Xlate(base.Info.Name);
						device.Prompt.MessageBox(text6, text5, (MessageBoxButtons)0, (MessageBoxIcon)64);
					});
					Thread.Sleep(100);
				}
			}
			Smart.Log.Info(TAG, "Found com port " + text);
		}
		else
		{
			Smart.Log.Error(TAG, "Use fixed comport number in recipe " + text);
		}
		Smart.Log.Debug(TAG, "Closing current MessageBox ");
		device.Prompt.CloseMessageBox();
		if (text == string.Empty)
		{
			string text3 = $"COM port of {comportName} is not found in {num} seconds";
			Smart.Log.Error(TAG, text3);
			LogResult((Result)1, "Kernel COM port not found");
			base.Cache["kId"] = "0";
		}
		else
		{
			Smart.Log.Debug(TAG, $"Found {comportName} = {text}");
			base.Cache["kId"] = text;
			Smart.Log.Debug(TAG, "kid:" + base.Cache["kId"]);
			string text4 = base.Cache["pidvid"];
			Smart.Log.Debug(TAG, "pidvid:" + text4);
			SavePortToLocalFile(text4, text, ComportCfgFile);
			result = (Result)8;
		}
		ActualTestResult = result;
		VerifyOnly(ref result);
		LogResult(result);
	}
}
