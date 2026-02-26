using System;
using System.Text.RegularExpressions;
using ISmart;

namespace SmartDevice.Steps;

public class FindDeviceIP : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice val = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = val.IP;
		if (val.IP == null || val.IP == string.Empty)
		{
			string arg = ((dynamic)base.Info.Args).IPType;
			string text2 = $"ifconfig {arg}";
			string empty = string.Empty;
			try
			{
				empty = Smart.ADB.Shell(val.ID, text2, 10000);
			}
			catch (Exception ex)
			{
				Smart.Log.Error(TAG, "Error sending ADB shell command: " + ex.Message);
				Smart.Log.Error(TAG, ex.ToString());
				LogResult((Result)4, "Could not send ADB shell command", ex.Message);
				return;
			}
			Smart.Log.Debug(TAG, $"Device IP response: {empty}");
			string pattern = $"{arg}: ip (?<ip>\\d+\\.\\d+\\.\\d+\\.\\d+) .*";
			text = Regex.Match(empty, pattern, RegexOptions.Multiline).Groups["ip"].Value;
			if (text == string.Empty)
			{
				pattern = string.Format("inet addr:(?<ip>\\d+\\.\\d+\\.\\d+\\.\\d+) ", arg);
				text = Regex.Match(empty, pattern).Groups["ip"].Value;
			}
			if (text == null || text.Trim() == string.Empty)
			{
				LogResult((Result)4, "Could not find device IP address");
				return;
			}
		}
		else
		{
			Smart.Log.Debug(TAG, $"Device IP already detected as {text}");
		}
		val.IP = text;
		base.Cache["deviceIP"] = text;
		LogPass();
	}
}
