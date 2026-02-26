using System;
using System.Threading;
using ISmart;

namespace SmartDevice.Steps;

public class ScanTcmdPort : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_0a29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2f: Invalid comparison between Unknown and I4
		//IL_0a3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a98: Unknown result type (might be due to invalid IL or missing references)
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string dynamicError = string.Empty;
		TCMD tCMD = new TCMD();
		int num = 75;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
		}
		bool filterNetwork = true;
		if (((dynamic)base.Info.Args).FilterNetwork != null)
		{
			filterNetwork = ((dynamic)base.Info.Args).FilterNetwork;
		}
		bool noFallback = false;
		if (((dynamic)base.Info.Args).NoFallback != null)
		{
			filterNetwork = ((dynamic)base.Info.Args).NoFallback;
		}
		string empty = string.Empty;
		try
		{
			int num2 = 40;
			if (((dynamic)base.Info.Args).SleepSecBeforeScan != null)
			{
				num2 = ((dynamic)base.Info.Args).SleepSecBeforeScan;
			}
			Thread.Sleep(num2 * 1000);
			bool skipTrackIdCheck = false;
			if (((dynamic)base.Info.Args).SkipTrackIdCheck != null)
			{
				skipTrackIdCheck = Convert.ToBoolean(((dynamic)base.Info.Args).SkipTrackIdCheck.ToString());
			}
			empty = tCMD.QuickScan(val.ID, TimeSpan.FromSeconds(num), filterNetwork, noFallback, skipTrackIdCheck);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, $"Error during TCMD scan: {ex.Message}");
			Smart.Log.Verbose(TAG, ex.ToString());
			dynamicError = ex.Message;
			empty = string.Empty;
		}
		if (empty == null || empty == string.Empty)
		{
			Smart.Log.Error(TAG, $"Timed out quick scanning for device {val.ID}");
			try
			{
				Smart.Log.Debug(TAG, "Capturing local network details...");
				string text = Smart.File.RunCommand("ipconfig /all");
				Smart.Log.Debug(TAG, text);
			}
			catch (Exception)
			{
			}
			LogResult((Result)1, "Timed out quick scanning for device", dynamicError);
			return;
		}
		base.Cache["deviceIP"] = empty;
		val.IP = empty;
		if (val.UnknownMode || (int)val.LastMode == 4)
		{
			val.ReportMode((DeviceMode)8);
		}
		else if (!((Enum)val.LastMode).HasFlag((Enum)(object)(DeviceMode)8))
		{
			val.ReportMode((DeviceMode)(val.Mode | 8));
		}
		string text2 = string.Empty;
		foreach (DeviceMode value in Enum.GetValues(typeof(DeviceMode)))
		{
			DeviceMode val2 = value;
			if (((Enum)val.Mode).HasFlag((Enum)(object)val2))
			{
				text2 = text2 + ((object)(DeviceMode)(ref val2)).ToString() + "+";
			}
		}
		text2 = text2.Substring(0, text2.Length - 1);
		Smart.Log.Info(TAG, $"Device mode now is: {text2}");
		LogPass();
	}
}
