using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice;

public class FakeFixtureWindow : Form
{
	private enum ButtonStatus
	{
		Idle,
		Press,
		Release
	}

	private bool fakeError;

	private bool scanning;

	private bool init;

	private int currentFixture = 1;

	private IDevice currentDevice;

	private string statusText = string.Empty;

	private SortedList<int, FixtureStatus> fixtureStatus = new SortedList<int, FixtureStatus>();

	private SortedList<int, string> fixtureResult = new SortedList<int, string>();

	private SortedList<int, ButtonStatus> fixtureButton = new SortedList<int, ButtonStatus>();

	private SortedList<int, string> fixtureDevices = new SortedList<int, string>();

	private SortedList<string, int> autos = new SortedList<string, int>();

	public Action<string> Logger = delegate(string x)
	{
		Smart.Log.Debug("FakeLogger", x);
	};

	private IContainer components;

	private ListBox deviceListBox;

	private GroupBox deviceGroupBox;

	private CheckBox autoCheckBox;

	private Button assignButton;

	private GroupBox fixtureGroupBox;

	private Button errorButton;

	private Button offlineButton;

	private Button disconnectButton;

	private Button connectedButton;

	private Label releaseLaabel;

	private Label pressLabel;

	private TextBox fixtureTextBox;

	private ListBox fixtureListBox;

	private Button removeButton;

	private Timer tickTimer;

	private Label deviceLabel;

	private Label fixtureLabel;

	private Label resultLabel;

	private Label label1;

	public string ErrorMessage { get; private set; }

	public bool IsConnected => init;

	public bool RecodeInnerLog { get; set; }

	public static FakeFixtureWindow ShowWindow()
	{
		FakeFixtureWindow fakeFixtureWindow = new FakeFixtureWindow();
		((Control)fakeFixtureWindow).Show();
		return fakeFixtureWindow;
	}

	public FakeFixtureWindow()
		: this("127.0.0.1")
	{
	}

	public FakeFixtureWindow(string ipPort)
	{
		InitializeComponent();
		for (int i = 0; i < 8; i++)
		{
			fixtureStatus[i + 1] = FixtureStatus.Idle;
			fixtureResult[i + 1] = string.Empty;
			fixtureButton[i + 1] = ButtonStatus.Idle;
			fixtureListBox.Items.Add((object)("Fixture " + (i + 1)));
		}
	}

	private void RefreshUI()
	{
		ShowFixtureStatus();
		ShowButtonStatus();
		ShowDeviceStatus();
	}

	private void ShowDeviceStatus()
	{
		int num = currentFixture;
		if (fixtureStatus.ContainsKey(num))
		{
			((Control)fixtureLabel).Text = "Fixture " + num;
		}
		else
		{
			((Control)fixtureLabel).Text = string.Empty;
		}
		IDevice val = currentDevice;
		if (val == null)
		{
			((Control)assignButton).Enabled = false;
			((Control)removeButton).Enabled = false;
			((Control)autoCheckBox).Enabled = false;
			((Control)deviceLabel).Text = string.Empty;
		}
		else
		{
			((Control)assignButton).Enabled = true;
			((Control)removeButton).Enabled = true;
			((Control)autoCheckBox).Enabled = true;
			((Control)deviceLabel).Text = val.ID;
		}
		if (!scanning)
		{
			return;
		}
		List<string> list = new List<string>();
		SortedList<int, string> sortedList = new SortedList<int, string>();
		foreach (IDevice value in Smart.DeviceManager.Devices.Values)
		{
			string iD = value.ID;
			list.Add(iD);
			if (autos.ContainsKey(iD) && value.PortIndex != autos[iD])
			{
				value.PortIndex = autos[iD];
			}
			if (fixtureStatus.ContainsKey(value.PortIndex))
			{
				sortedList[value.PortIndex] = iD;
			}
		}
		fixtureDevices = sortedList;
		List<object> list2 = new List<object>();
		List<string> list3 = new List<string>(list);
		foreach (object item2 in deviceListBox.Items)
		{
			string item = item2.ToString();
			if (!list.Contains(item))
			{
				list2.Add(item2);
			}
			else
			{
				list3.Remove(item);
			}
		}
		foreach (object item3 in list2)
		{
			deviceListBox.Items.Remove(item3);
		}
		foreach (string item4 in list3)
		{
			deviceListBox.Items.Add((object)item4);
		}
	}

	private void ShowFixtureStatus()
	{
		string empty = string.Empty;
		int num = currentFixture;
		if (!fixtureStatus.ContainsKey(num))
		{
			((Control)connectedButton).Enabled = false;
			((Control)disconnectButton).Enabled = false;
			((Control)offlineButton).Enabled = false;
			((Control)errorButton).Enabled = false;
			return;
		}
		((Control)connectedButton).Enabled = true;
		((Control)disconnectButton).Enabled = true;
		((Control)offlineButton).Enabled = true;
		((Control)errorButton).Enabled = true;
		if ((fixtureStatus[currentFixture] == FixtureStatus.Idle || fixtureStatus[currentFixture] == FixtureStatus.Ready) && fixtureResult[currentFixture] != string.Empty)
		{
			string text = $"Fixture {currentFixture} Status {fixtureResult[currentFixture]}";
			if (text != ((Control)resultLabel).Text)
			{
				((Control)resultLabel).Text = text;
				((Control)resultLabel).Visible = true;
			}
		}
		else
		{
			((Control)resultLabel).Visible = false;
		}
		empty = empty + $"Fixture {num}:" + Environment.NewLine;
		empty = empty + $"     Status {fixtureStatus[currentFixture]}" + Environment.NewLine;
		if (fixtureDevices.ContainsKey(currentFixture))
		{
			empty = empty + $"     Device {fixtureDevices[currentFixture]}" + Environment.NewLine;
		}
		empty = empty + $"     Result {fixtureResult[currentFixture]}" + Environment.NewLine;
		empty = empty + $"     Button Instructions: {fixtureButton[currentFixture]}" + Environment.NewLine;
		if (empty != statusText)
		{
			statusText = empty;
			((Control)fixtureTextBox).Text = statusText;
		}
		ShowButtonStatus();
	}

	private void ShowButtonStatus()
	{
		int key = currentFixture;
		if (!fixtureStatus.ContainsKey(key))
		{
			((Control)pressLabel).Visible = false;
			((Control)releaseLaabel).Visible = false;
			return;
		}
		switch (fixtureButton[key])
		{
		case ButtonStatus.Idle:
			((Control)pressLabel).Visible = false;
			((Control)releaseLaabel).Visible = false;
			break;
		case ButtonStatus.Press:
			((Control)pressLabel).Visible = true;
			((Control)releaseLaabel).Visible = false;
			break;
		case ButtonStatus.Release:
			((Control)pressLabel).Visible = false;
			((Control)releaseLaabel).Visible = true;
			break;
		default:
			((Control)pressLabel).Visible = false;
			((Control)releaseLaabel).Visible = false;
			break;
		}
	}

	private void ConnectCable(int fixtureId)
	{
		if (fixtureStatus.ContainsKey(fixtureId))
		{
			switch (fixtureStatus[fixtureId])
			{
			case FixtureStatus.Unknown:
				fixtureStatus[fixtureId] = FixtureStatus.Ready;
				break;
			case FixtureStatus.Idle:
				fixtureStatus[fixtureId] = FixtureStatus.Ready;
				break;
			case FixtureStatus.Ready:
				fixtureStatus[fixtureId] = FixtureStatus.Ready;
				break;
			case FixtureStatus.Offline:
				fixtureStatus[fixtureId] = FixtureStatus.Ready;
				break;
			case FixtureStatus.Error:
				fixtureStatus[fixtureId] = FixtureStatus.Ready;
				break;
			case FixtureStatus.Testing:
				break;
			}
		}
	}

	private void DisconnectCable(int fixtureId)
	{
		if (fixtureStatus.ContainsKey(fixtureId))
		{
			switch (fixtureStatus[fixtureId])
			{
			case FixtureStatus.Unknown:
				fixtureStatus[fixtureId] = FixtureStatus.Idle;
				break;
			case FixtureStatus.Idle:
				fixtureStatus[fixtureId] = FixtureStatus.Idle;
				break;
			case FixtureStatus.Ready:
				fixtureStatus[fixtureId] = FixtureStatus.Idle;
				break;
			case FixtureStatus.Testing:
				fixtureStatus[fixtureId] = FixtureStatus.Idle;
				break;
			case FixtureStatus.Offline:
				fixtureStatus[fixtureId] = FixtureStatus.Idle;
				break;
			case FixtureStatus.Error:
				fixtureStatus[fixtureId] = FixtureStatus.Idle;
				break;
			}
		}
	}

	private void GoOffline(int fixtureId)
	{
		if (fixtureStatus.ContainsKey(fixtureId))
		{
			fixtureStatus[fixtureId] = FixtureStatus.Offline;
		}
	}

	private void ErrorState(int fixtureId)
	{
		if (fixtureStatus.ContainsKey(fixtureId))
		{
			fixtureStatus[fixtureId] = FixtureStatus.Error;
		}
	}

	private void connectedButton_Click(object sender, EventArgs e)
	{
		ConnectCable(currentFixture);
	}

	private void disconnectButton_Click(object sender, EventArgs e)
	{
		DisconnectCable(currentFixture);
	}

	private void offlineButton_Click(object sender, EventArgs e)
	{
		GoOffline(currentFixture);
	}

	private void errorButton_Click(object sender, EventArgs e)
	{
		ErrorState(currentFixture);
	}

	private void assignButton_Click(object sender, EventArgs e)
	{
		if (currentDevice == null)
		{
			return;
		}
		string iD = currentDevice.ID;
		int num = currentFixture;
		if (fixtureStatus.ContainsKey(num))
		{
			currentDevice.PortIndex = num;
			if (autoCheckBox.Checked)
			{
				autos[iD] = num;
			}
		}
	}

	private void removeButton_Click(object sender, EventArgs e)
	{
		if (currentDevice != null)
		{
			currentDevice.PortIndex = 0;
			string iD = currentDevice.ID;
			if (autos.ContainsKey(iD))
			{
				autos.Remove(iD);
			}
		}
	}

	private void fixtureListBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		int key = ((ListControl)fixtureListBox).SelectedIndex + 1;
		if (fixtureStatus.ContainsKey(key))
		{
			currentFixture = key;
		}
	}

	private void deviceListBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (!scanning)
		{
			return;
		}
		object selectedItem = deviceListBox.SelectedItem;
		if (selectedItem == null)
		{
			return;
		}
		string text = selectedItem.ToString();
		foreach (IDevice value in Smart.DeviceManager.Devices.Values)
		{
			if (value.ID == text)
			{
				currentDevice = value;
			}
		}
	}

	private void deviceListBox_Click(object sender, EventArgs e)
	{
		if (!scanning)
		{
			deviceListBox.Items.Clear();
			scanning = true;
		}
	}

	private void tickTimer_Tick(object sender, EventArgs e)
	{
		RefreshUI();
	}

	public static string Protocol()
	{
		throw new NotSupportedException("Protocol not supported");
	}

	public bool Init(string ipAddress, out string errorMsg)
	{
		errorMsg = string.Empty;
		if (fakeError)
		{
			errorMsg = "Fake Error";
			return false;
		}
		init = true;
		tickTimer.Enabled = true;
		return true;
	}

	public int QueryFixtureStatus(int fixtureId, out string errorMsg)
	{
		errorMsg = string.Empty;
		if (!init)
		{
			errorMsg = "Not intialized";
			return 4;
		}
		if (fakeError)
		{
			errorMsg = "Fake Error";
			return 4;
		}
		if (!fixtureStatus.ContainsKey(fixtureId))
		{
			errorMsg = "Unrecognized Fixture ID: " + fixtureId;
			return 4;
		}
		return (int)fixtureStatus[fixtureId];
	}

	public bool WriteToFixture(int fixtureId, int value, out string errorMsg)
	{
		errorMsg = string.Empty;
		if (!init)
		{
			errorMsg = "Not intialized";
			return false;
		}
		if (fakeError)
		{
			errorMsg = "Fake Error";
			return false;
		}
		if (!fixtureStatus.ContainsKey(fixtureId))
		{
			errorMsg = "Unrecognized Fixture ID: " + fixtureId;
			return false;
		}
		if (value < 1 || value > 5)
		{
			errorMsg = "Unrecognized Command: " + value;
			return false;
		}
		FixtureCommand fixtureCommand = (FixtureCommand)value;
		switch (fixtureCommand)
		{
		case FixtureCommand.PressKey:
			fixtureButton[fixtureId] = ButtonStatus.Press;
			fixtureStatus[fixtureId] = FixtureStatus.Testing;
			fixtureResult[fixtureId] = string.Empty;
			return true;
		case FixtureCommand.ReleaseKey:
		{
			int fixtureIdNow = fixtureId;
			fixtureButton[fixtureIdNow] = ButtonStatus.Release;
			Smart.Thread.Run((ThreadStart)delegate
			{
				Releaser(fixtureIdNow);
			});
			return true;
		}
		case FixtureCommand.ReportPass:
			fixtureResult[fixtureId] = "Pass";
			DisconnectCable(fixtureId);
			return true;
		case FixtureCommand.ReportFail:
			fixtureResult[fixtureId] = "Fail";
			DisconnectCable(fixtureId);
			return true;
		case FixtureCommand.ReportError:
			fixtureResult[fixtureId] = "Error";
			DisconnectCable(fixtureId);
			return true;
		default:
			errorMsg = "Unsupported Command: " + value;
			return false;
		}
	}

	private void Releaser(int fixtureId)
	{
		Smart.Thread.Wait(TimeSpan.FromSeconds(10.0));
		fixtureButton[fixtureId] = ButtonStatus.Idle;
	}

	public Task<int> QueryFixtureStatusAsync(int fixtureId)
	{
		throw new NotSupportedException("FakeFixture does not support Async");
	}

	public Task<bool> WriteToFixtureAsync(int fixtureId, int value)
	{
		throw new NotSupportedException("FakeFixture does not support Async");
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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Expected O, but got Unknown
		//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b5: Expected O, but got Unknown
		//IL_093a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0944: Expected O, but got Unknown
		components = new Container();
		deviceListBox = new ListBox();
		deviceGroupBox = new GroupBox();
		fixtureLabel = new Label();
		deviceLabel = new Label();
		removeButton = new Button();
		autoCheckBox = new CheckBox();
		assignButton = new Button();
		fixtureGroupBox = new GroupBox();
		resultLabel = new Label();
		errorButton = new Button();
		offlineButton = new Button();
		disconnectButton = new Button();
		connectedButton = new Button();
		releaseLaabel = new Label();
		pressLabel = new Label();
		fixtureTextBox = new TextBox();
		fixtureListBox = new ListBox();
		tickTimer = new Timer(components);
		label1 = new Label();
		((Control)deviceGroupBox).SuspendLayout();
		((Control)fixtureGroupBox).SuspendLayout();
		((Control)this).SuspendLayout();
		((ListControl)deviceListBox).FormattingEnabled = true;
		deviceListBox.Items.AddRange(new object[1] { "Click to Scan" });
		((Control)deviceListBox).Location = new Point(6, 19);
		((Control)deviceListBox).Name = "deviceListBox";
		((Control)deviceListBox).Size = new Size(240, 277);
		((Control)deviceListBox).TabIndex = 0;
		deviceListBox.Click += deviceListBox_Click;
		deviceListBox.SelectedIndexChanged += deviceListBox_SelectedIndexChanged;
		((Control)deviceGroupBox).Anchor = (AnchorStyles)7;
		((Control)deviceGroupBox).Controls.Add((Control)(object)label1);
		((Control)deviceGroupBox).Controls.Add((Control)(object)fixtureLabel);
		((Control)deviceGroupBox).Controls.Add((Control)(object)deviceLabel);
		((Control)deviceGroupBox).Controls.Add((Control)(object)removeButton);
		((Control)deviceGroupBox).Controls.Add((Control)(object)autoCheckBox);
		((Control)deviceGroupBox).Controls.Add((Control)(object)assignButton);
		((Control)deviceGroupBox).Controls.Add((Control)(object)deviceListBox);
		((Control)deviceGroupBox).Location = new Point(12, 12);
		((Control)deviceGroupBox).Name = "deviceGroupBox";
		((Control)deviceGroupBox).Size = new Size(252, 402);
		((Control)deviceGroupBox).TabIndex = 1;
		deviceGroupBox.TabStop = false;
		((Control)deviceGroupBox).Text = "Devices";
		((Control)fixtureLabel).AutoSize = true;
		((Control)fixtureLabel).Location = new Point(84, 329);
		((Control)fixtureLabel).Name = "fixtureLabel";
		((Control)fixtureLabel).Size = new Size(38, 13);
		((Control)fixtureLabel).TabIndex = 8;
		((Control)fixtureLabel).Text = "Fixture";
		((Control)deviceLabel).AutoSize = true;
		((Control)deviceLabel).Location = new Point(6, 304);
		((Control)deviceLabel).Name = "deviceLabel";
		((Control)deviceLabel).Size = new Size(39, 13);
		((Control)deviceLabel).TabIndex = 8;
		((Control)deviceLabel).Text = "device";
		((Control)removeButton).Location = new Point(6, 356);
		((Control)removeButton).Name = "removeButton";
		((Control)removeButton).Size = new Size(61, 30);
		((Control)removeButton).TabIndex = 4;
		((Control)removeButton).Text = "Remove";
		((ButtonBase)removeButton).UseVisualStyleBackColor = true;
		((Control)removeButton).Click += removeButton_Click;
		((Control)autoCheckBox).AutoSize = true;
		((Control)autoCheckBox).Location = new Point(166, 328);
		((Control)autoCheckBox).Name = "autoCheckBox";
		((Control)autoCheckBox).Size = new Size(48, 17);
		((Control)autoCheckBox).TabIndex = 3;
		((Control)autoCheckBox).Text = "Auto";
		((ButtonBase)autoCheckBox).UseVisualStyleBackColor = true;
		((Control)assignButton).Location = new Point(6, 320);
		((Control)assignButton).Name = "assignButton";
		((Control)assignButton).Size = new Size(61, 30);
		((Control)assignButton).TabIndex = 2;
		((Control)assignButton).Text = "Assign";
		((ButtonBase)assignButton).UseVisualStyleBackColor = true;
		((Control)assignButton).Click += assignButton_Click;
		((Control)fixtureGroupBox).Anchor = (AnchorStyles)15;
		((Control)fixtureGroupBox).Controls.Add((Control)(object)resultLabel);
		((Control)fixtureGroupBox).Controls.Add((Control)(object)errorButton);
		((Control)fixtureGroupBox).Controls.Add((Control)(object)offlineButton);
		((Control)fixtureGroupBox).Controls.Add((Control)(object)disconnectButton);
		((Control)fixtureGroupBox).Controls.Add((Control)(object)connectedButton);
		((Control)fixtureGroupBox).Controls.Add((Control)(object)releaseLaabel);
		((Control)fixtureGroupBox).Controls.Add((Control)(object)pressLabel);
		((Control)fixtureGroupBox).Controls.Add((Control)(object)fixtureTextBox);
		((Control)fixtureGroupBox).Controls.Add((Control)(object)fixtureListBox);
		((Control)fixtureGroupBox).Location = new Point(270, 12);
		((Control)fixtureGroupBox).Name = "fixtureGroupBox";
		((Control)fixtureGroupBox).Size = new Size(492, 402);
		((Control)fixtureGroupBox).TabIndex = 2;
		fixtureGroupBox.TabStop = false;
		((Control)fixtureGroupBox).Text = "Fixtures";
		((Control)resultLabel).AutoSize = true;
		((Control)resultLabel).Font = new Font("Microsoft Sans Serif", 18f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)resultLabel).Location = new Point(108, 357);
		((Control)resultLabel).Name = "resultLabel";
		((Control)resultLabel).Size = new Size(181, 29);
		((Control)resultLabel).TabIndex = 8;
		((Control)resultLabel).Text = "Fixture Result ";
		((Control)resultLabel).Visible = false;
		((Control)errorButton).Location = new Point(265, 320);
		((Control)errorButton).Name = "errorButton";
		((Control)errorButton).Size = new Size(70, 30);
		((Control)errorButton).TabIndex = 7;
		((Control)errorButton).Text = "Error";
		((ButtonBase)errorButton).UseVisualStyleBackColor = true;
		((Control)errorButton).Click += errorButton_Click;
		((Control)offlineButton).Location = new Point(189, 320);
		((Control)offlineButton).Name = "offlineButton";
		((Control)offlineButton).Size = new Size(70, 30);
		((Control)offlineButton).TabIndex = 6;
		((Control)offlineButton).Text = "Offline";
		((ButtonBase)offlineButton).UseVisualStyleBackColor = true;
		((Control)offlineButton).Click += offlineButton_Click;
		((Control)disconnectButton).Location = new Point(113, 320);
		((Control)disconnectButton).Name = "disconnectButton";
		((Control)disconnectButton).Size = new Size(70, 30);
		((Control)disconnectButton).TabIndex = 5;
		((Control)disconnectButton).Text = "Disconnect";
		((ButtonBase)disconnectButton).UseVisualStyleBackColor = true;
		((Control)disconnectButton).Click += disconnectButton_Click;
		((Control)connectedButton).Location = new Point(6, 320);
		((Control)connectedButton).Name = "connectedButton";
		((Control)connectedButton).Size = new Size(70, 30);
		((Control)connectedButton).TabIndex = 4;
		((Control)connectedButton).Text = "Connected";
		((ButtonBase)connectedButton).UseVisualStyleBackColor = true;
		((Control)connectedButton).Click += connectedButton_Click;
		((Control)releaseLaabel).AutoSize = true;
		((Control)releaseLaabel).Font = new Font("Microsoft Sans Serif", 18f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)releaseLaabel).Location = new Point(47, 276);
		((Control)releaseLaabel).Name = "releaseLaabel";
		((Control)releaseLaabel).Size = new Size(412, 29);
		((Control)releaseLaabel).TabIndex = 3;
		((Control)releaseLaabel).Text = "Release Power/Volume Keys Now";
		((Control)releaseLaabel).Visible = false;
		((Control)pressLabel).AutoSize = true;
		((Control)pressLabel).Font = new Font("Microsoft Sans Serif", 18f, (FontStyle)1, (GraphicsUnit)3, (byte)0);
		((Control)pressLabel).Location = new Point(63, 247);
		((Control)pressLabel).Name = "pressLabel";
		((Control)pressLabel).Size = new Size(382, 29);
		((Control)pressLabel).TabIndex = 2;
		((Control)pressLabel).Text = "Press Power/Volume Keys Now";
		((Control)pressLabel).Visible = false;
		((Control)fixtureTextBox).Location = new Point(171, 19);
		((TextBoxBase)fixtureTextBox).Multiline = true;
		((Control)fixtureTextBox).Name = "fixtureTextBox";
		fixtureTextBox.ScrollBars = (ScrollBars)2;
		((Control)fixtureTextBox).Size = new Size(315, 225);
		((Control)fixtureTextBox).TabIndex = 1;
		((ListControl)fixtureListBox).FormattingEnabled = true;
		((Control)fixtureListBox).Location = new Point(6, 19);
		((Control)fixtureListBox).Name = "fixtureListBox";
		((Control)fixtureListBox).Size = new Size(159, 225);
		((Control)fixtureListBox).TabIndex = 0;
		fixtureListBox.SelectedIndexChanged += fixtureListBox_SelectedIndexChanged;
		tickTimer.Interval = 500;
		tickTimer.Tick += tickTimer_Tick;
		((Control)label1).AutoSize = true;
		((Control)label1).ForeColor = Color.Firebrick;
		((Control)label1).Location = new Point(65, 304);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(181, 13);
		((Control)label1).TabIndex = 9;
		((Control)label1).Text = "Use Port Assignment window instead";
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(774, 426);
		((Control)this).Controls.Add((Control)(object)fixtureGroupBox);
		((Control)this).Controls.Add((Control)(object)deviceGroupBox);
		((Control)this).Name = "FakeFixtureWindow";
		((Control)this).Text = "Fixture Test";
		((Control)deviceGroupBox).ResumeLayout(false);
		((Control)deviceGroupBox).PerformLayout();
		((Control)fixtureGroupBox).ResumeLayout(false);
		((Control)fixtureGroupBox).PerformLayout();
		((Control)this).ResumeLayout(false);
	}
}
