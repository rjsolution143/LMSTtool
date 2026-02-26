using System;
using System.IO;
using System.Threading;
using ISmart;

namespace SmartDevice.Steps;

public class PushFile : BaseStep
{
	private string devicePath;

	private long deviceFileSize;

	private IDevice device;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		string text = ((dynamic)base.Info.Args).LocalPath;
		if (text.StartsWith("$"))
		{
			string key = text.Substring(1);
			text = base.Cache[key];
		}
		FileInfo fileInfo = new FileInfo(text);
		deviceFileSize = fileInfo.Length;
		devicePath = ((dynamic)base.Info.Args).DevicePath;
		Thread thread = new Thread(ReadSize);
		thread.IsBackground = true;
		thread.Start();
		Smart.Log.Debug(TAG, $"DeviceId: {device.ID} Local Path: {text} Device Path: {devicePath}");
		Smart.ADB.PushFile(device.ID, text, devicePath);
		thread.Join();
		LogPass();
	}

	public void ReadSize(object o)
	{
		string text = "stat -c %s " + devicePath;
		try
		{
			long num;
			do
			{
				Smart.Thread.Wait(TimeSpan.FromMilliseconds(5000.0));
				num = long.Parse(Smart.ADB.Shell(device.ID, text, 10000));
				int num2 = (int)(num * 100 / deviceFileSize);
				Smart.Log.Debug(TAG, $"DeviceId: {device.ID} Pushed: {num2}%");
				ProgressUpdate(num2);
			}
			while (num != deviceFileSize);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error reading push file progress " + ex.Message);
			Smart.Log.Error(TAG, ex.ToString());
		}
	}
}
