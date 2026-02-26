using System;
using System.Diagnostics;
using System.IO;
using ISmart;

namespace SmartDevice.Steps;

[Obsolete]
public class KillProcess : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		_ = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		Result val = (Result)1;
		string path = "ODMSocketServer.exe";
		if (((dynamic)base.Info.Args).ProcessNameToKill != null)
		{
			path = ((dynamic)base.Info.Args).ProcessNameToKill.ToString();
		}
		Process[] array = null;
		array = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(path));
		if (array != null)
		{
			Process[] array2 = array;
			foreach (Process process in array2)
			{
				Smart.Log.Info(TAG, "kill " + process.ProcessName);
				process.Kill();
			}
		}
		val = (Result)8;
		LogResult(val);
	}
}
