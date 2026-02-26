using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;
using ISmart;

namespace SmartUtil.TestForms;

public class GenericTest : Form
{
	private UseCase[] DetectUseCases;

	private UseCase[] ReadUseCases;

	private Timer deviceTimer;

	private int tokenCount;

	private IContainer components;

	private TextBox statusBox;

	private Button deviceButton;

	private string TAG => ((object)this).GetType().FullName;

	public GenericTest()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		UseCase[] array = new UseCase[3];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		DetectUseCases = (UseCase[])(object)array;
		UseCase[] array2 = new UseCase[4];
		RuntimeHelpers.InitializeArray(array2, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		ReadUseCases = (UseCase[])(object)array2;
		deviceTimer = new Timer();
		((Form)this)._002Ector();
		InitializeComponent();
		deviceTimer.Interval = 250;
		deviceTimer.Tick += DeviceTimer_Tick;
		deviceTimer.Start();
	}

	private void deviceButton_Click(object sender, EventArgs e)
	{
		Smart.Thread.Run((ThreadStart)DeviceCheck);
	}

	private void DeviceCheck()
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		while (Smart.DeviceManager.Devices.Count < 1)
		{
			Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
		}
		IDevice val = Smart.DeviceManager.Devices.Values[0];
		Smart.Log.Debug(TAG, $"Fixture {1} waiting for Device {val.ID} to finish detection process");
		if (val.Log == null)
		{
			Smart.Log.Debug(TAG, "Waiting for Device.Log to be created");
			while (val.Log == null)
			{
				Smart.Thread.Wait(TimeSpan.FromMilliseconds(50.0));
			}
		}
		bool flag = false;
		bool flag2 = false;
		DateTime value = DateTime.Now.Subtract(TimeSpan.FromDays(900.0));
		while (true)
		{
			bool flag3 = DetectUseCases.Contains(val.Log.UseCase);
			bool num = ReadUseCases.Contains(val.Log.UseCase);
			bool locked = val.Locked;
			double progress = val.Log.Progress;
			TimeSpan timeSpan = DateTime.Now.Subtract(value);
			if (num)
			{
				Smart.Log.Debug(TAG, "Read has been started");
				flag = false;
				flag2 = false;
				break;
			}
			if (!flag && !flag3)
			{
				flag = true;
				Smart.Log.Debug(TAG, "Waiting for Detect to start");
				continue;
			}
			if (flag)
			{
				if (!flag3)
				{
					Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
					continue;
				}
				Smart.Log.Debug(TAG, "Detect has started");
				flag = false;
			}
			if (!flag2 && !locked)
			{
				if (timeSpan.TotalDays > 500.0)
				{
					Smart.Log.Debug(TAG, "Waiting for Detect to Lock device");
					value = DateTime.Now;
					continue;
				}
				if (timeSpan.TotalSeconds > 10.0)
				{
					flag = false;
					flag2 = false;
					Smart.Log.Debug(TAG, "Device Detect never became Locked");
					break;
				}
				Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
			}
			else if (!flag2)
			{
				flag2 = true;
				Smart.Log.Debug(TAG, "Waiting for Detect to finish");
			}
			else if (flag2 && !locked)
			{
				Smart.Log.Debug(TAG, $"Detect has completed, progress reached {progress}%");
				flag = false;
				flag2 = false;
				break;
			}
		}
		bool flag4 = false;
		flag2 = false;
		DateTime now = DateTime.Now;
		while (true)
		{
			bool flag5 = ReadUseCases.Contains(val.Log.UseCase);
			bool locked2 = val.Locked;
			double progress2 = val.Log.Progress;
			TimeSpan timeSpan2 = DateTime.Now.Subtract(now);
			if (!flag4 && !flag5)
			{
				flag4 = true;
				Smart.Log.Debug(TAG, "Waiting for Read to start");
				continue;
			}
			if (flag4)
			{
				if (timeSpan2.TotalSeconds > 30.0)
				{
					Smart.Log.Debug(TAG, "Device Read never started (maybe Detect failed?)");
					flag4 = false;
					break;
				}
				if (!flag5)
				{
					Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
					continue;
				}
				Smart.Log.Debug(TAG, "Read has started");
				if (!locked2)
				{
					Smart.Log.Debug(TAG, "Waiting for Detect to Lock device");
				}
				flag4 = false;
			}
			if (!flag2 && !locked2)
			{
				if (timeSpan2.TotalSeconds > 30.0)
				{
					flag4 = false;
					flag2 = false;
					Smart.Log.Debug(TAG, "Device Read never became Locked");
					break;
				}
				Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
			}
			else if (!flag2)
			{
				flag2 = true;
				Smart.Log.Debug(TAG, "Waiting for Read to finish");
			}
			else if (flag2 && !locked2)
			{
				Smart.Log.Debug(TAG, $"Read has completed, progress reached {progress2}%");
				flag = false;
				flag2 = false;
				break;
			}
		}
		Smart.Log.Debug(TAG, $"Fixture {1} Device {val.ID} has finished detection process");
		while (val.Locked)
		{
			Smart.Log.Warning(TAG, "Device is still locked, waiting...");
			Smart.Thread.Wait(TimeSpan.FromSeconds(2.0));
		}
	}

	public void StatusUpdate(string value)
	{
		if (((Control)this).InvokeRequired)
		{
			((Control)this).Invoke((Delegate)new Action<string>(StatusUpdate), new object[1] { value });
		}
		else
		{
			TextBox obj = statusBox;
			((Control)obj).Text = ((Control)obj).Text + value;
		}
	}

	private void TokenLoop()
	{
		int num = tokenCount;
		tokenCount++;
		StatusUpdate(num + " init" + Environment.NewLine);
		TimeSpan timeSpan = TimeSpan.FromMinutes(5.0);
		TimeSpan timeSpan2 = TimeSpan.FromSeconds(12.0);
		DateTime now = DateTime.Now;
		while (DateTime.Now.Subtract(now).TotalMilliseconds < timeSpan.TotalMilliseconds)
		{
			if (Smart.Web.ValidateToken())
			{
				StatusUpdate(num + " token success..." + Environment.NewLine);
			}
			else
			{
				StatusUpdate(num + " TOKEN Failed!" + Environment.NewLine);
			}
			Smart.Thread.Wait(timeSpan2);
		}
	}

	private void DeviceTimer_Tick(object sender, EventArgs e)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		SortedList<string, IDevice> devices = Smart.DeviceManager.Devices;
		string text = "No Devices";
		if (devices.Count > 0)
		{
			IDevice val = devices.Values[0];
			text = "Device " + val.ID;
			IResultLogger log = val.Log;
			string text2 = "Locked";
			if (!val.Locked)
			{
				text2 = "Unlocked";
			}
			text = text + Environment.NewLine + text2;
			if (log != null)
			{
				text = text + Environment.NewLine + log.UseCase;
				text = text + Environment.NewLine + log.Progress;
			}
		}
		if (((Control)statusBox).Text != text)
		{
			((Control)statusBox).Text = text;
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		((Form)this).Dispose(disposing);
	}

	private void InitializeComponent()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		statusBox = new TextBox();
		deviceButton = new Button();
		((Control)this).SuspendLayout();
		((Control)statusBox).Anchor = (AnchorStyles)15;
		((Control)statusBox).Location = new Point(12, 129);
		((TextBoxBase)statusBox).Multiline = true;
		((Control)statusBox).Name = "statusBox";
		statusBox.ScrollBars = (ScrollBars)2;
		((Control)statusBox).Size = new Size(434, 193);
		((Control)statusBox).TabIndex = 0;
		((TextBoxBase)statusBox).WordWrap = false;
		((Control)deviceButton).Location = new Point(12, 21);
		((Control)deviceButton).Name = "deviceButton";
		((Control)deviceButton).Size = new Size(75, 23);
		((Control)deviceButton).TabIndex = 1;
		((Control)deviceButton).Text = "Test";
		((ButtonBase)deviceButton).UseVisualStyleBackColor = true;
		((Control)deviceButton).Click += deviceButton_Click;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(458, 334);
		((Control)this).Controls.Add((Control)(object)deviceButton);
		((Control)this).Controls.Add((Control)(object)statusBox);
		((Control)this).Name = "GenericTest";
		((Control)this).Text = "GenericTest";
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
