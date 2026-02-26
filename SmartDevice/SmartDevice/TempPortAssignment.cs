using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice;

public class TempPortAssignment : Form, IPortAssignment, IDisposable
{
	private SortedList<string, string> detectionInfo = new SortedList<string, string>();

	private SortedList<int, Label> portLabels = new SortedList<int, Label>();

	private IContainer components;

	private GroupBox portInfoGroup;

	private TextBox portInfoBox;

	private ComboBox portPickBox;

	private Label portPickLabel;

	private Button assignButton;

	private Label port1Label;

	private Label port2Label;

	private Label port3Label;

	private Label port4Label;

	private Label port8Label;

	private Label port7Label;

	private Label port6Label;

	private Label port5Label;

	private Button clearButton;

	private Timer refreshTimer;

	private Label scanWarningLabel;

	private Button scanButton;

	private Label port9Label;

	private Label port10Label;

	private Label port11Label;

	private Label port12Label;

	public void ShowWindow()
	{
		((Control)new TempPortAssignment()).Show();
	}

	public TempPortAssignment()
	{
		InitializeComponent();
		for (int i = 0; i < 12; i++)
		{
			portPickBox.Items.Add((object)(i + 1));
		}
		((ListControl)portPickBox).SelectedIndex = 0;
		((Control)portInfoBox).Text = "Waiting for port scan";
		portLabels[1] = port1Label;
		portLabels[2] = port2Label;
		portLabels[3] = port3Label;
		portLabels[4] = port4Label;
		portLabels[5] = port5Label;
		portLabels[6] = port6Label;
		portLabels[7] = port7Label;
		portLabels[8] = port8Label;
		portLabels[9] = port9Label;
		portLabels[10] = port10Label;
		portLabels[11] = port11Label;
		portLabels[12] = port12Label;
		((Control)assignButton).Enabled = false;
		refreshTimer.Enabled = true;
	}

	private void Scan()
	{
		((Control)portInfoBox).Text = "Scan in progress, please plug in device to new port";
		((Control)this).Refresh();
		SortedList<string, string> sortedList = Smart.UsbPorts.PortScan();
		if (sortedList.Count < 1)
		{
			((Control)portInfoBox).Text = "Scan failed, no device found";
			((Control)assignButton).Enabled = false;
			return;
		}
		int num = Smart.UsbPorts.FindPort(sortedList);
		if (num > 0 && num <= 12)
		{
			((Control)portInfoBox).Text = $"Scan found device on existing port {num}";
		}
		else
		{
			((Control)portInfoBox).Text = "Scan found device on new port";
		}
		TextBox obj = portInfoBox;
		((Control)obj).Text = ((Control)obj).Text + Environment.NewLine + Environment.NewLine + Smart.Convert.ToString("Port Details", (IEnumerable<KeyValuePair<string, string>>)sortedList);
		detectionInfo = sortedList;
		((Control)assignButton).Enabled = true;
	}

	private void Assign()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Invalid comparison between Unknown and I4
		if (detectionInfo.Count < 1)
		{
			((Control)portInfoBox).Text = "Scan for a new device to assign a port";
			((Control)assignButton).Enabled = false;
			return;
		}
		int result = -1;
		if (!int.TryParse(((Control)portPickBox).Text, out result) || result < 1 || result > 12)
		{
			((Control)portInfoBox).Text = "Select a valid port 1-12 for assignment";
			return;
		}
		UsbPortStatus val = Smart.UsbPorts.PortAssign(result, detectionInfo);
		if ((int)val == 0 || (int)val == -1)
		{
			((Control)portInfoBox).Text = "Port assignment failed";
			detectionInfo = new SortedList<string, string>();
			((Control)assignButton).Enabled = false;
		}
		else
		{
			((Control)portInfoBox).Text = $"Successfully assigned port {result}";
			detectionInfo = new SortedList<string, string>();
			((Control)assignButton).Enabled = false;
		}
	}

	private void Clear()
	{
		Smart.UsbPorts.ClearPorts();
		((Control)portInfoBox).Text = "All ports cleared, scan to re-assign";
		((Control)assignButton).Enabled = false;
	}

	private void RefreshPorts()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Invalid comparison between Unknown and I4
		Smart.UsbPorts.PortRefresh();
		foreach (int key in portLabels.Keys)
		{
			Label val = portLabels[key];
			UsbPortStatus val2 = Smart.UsbPorts.PortStatus[key];
			string text = $"Port {key} {val2}";
			if (((Control)val).Text != text)
			{
				((Control)val).Text = text;
			}
			bool flag = (int)val2 > 0;
			if (((Control)val).Enabled != flag)
			{
				((Control)val).Enabled = flag;
			}
		}
	}

	private void refreshTimer_Tick(object sender, EventArgs e)
	{
		RefreshPorts();
	}

	private void scanButton_Click(object sender, EventArgs e)
	{
		Scan();
	}

	private void assignButton_Click(object sender, EventArgs e)
	{
		Assign();
	}

	private void clearButton_Click(object sender, EventArgs e)
	{
		Clear();
	}

	private void TempPortAssignment_FormClosed(object sender, FormClosedEventArgs e)
	{
		refreshTimer.Stop();
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
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_088b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0916: Unknown result type (might be due to invalid IL or missing references)
		//IL_0994: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0daf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de1: Expected O, but got Unknown
		components = new Container();
		portInfoGroup = new GroupBox();
		scanWarningLabel = new Label();
		scanButton = new Button();
		portInfoBox = new TextBox();
		portPickBox = new ComboBox();
		portPickLabel = new Label();
		assignButton = new Button();
		port1Label = new Label();
		port2Label = new Label();
		port3Label = new Label();
		port4Label = new Label();
		port8Label = new Label();
		port7Label = new Label();
		port6Label = new Label();
		port5Label = new Label();
		clearButton = new Button();
		refreshTimer = new Timer(components);
		port9Label = new Label();
		port10Label = new Label();
		port11Label = new Label();
		port12Label = new Label();
		((Control)portInfoGroup).SuspendLayout();
		((Control)this).SuspendLayout();
		((Control)portInfoGroup).Anchor = (AnchorStyles)15;
		((Control)portInfoGroup).Controls.Add((Control)(object)scanWarningLabel);
		((Control)portInfoGroup).Controls.Add((Control)(object)scanButton);
		((Control)portInfoGroup).Controls.Add((Control)(object)portInfoBox);
		((Control)portInfoGroup).Location = new Point(32, 29);
		((Control)portInfoGroup).Margin = new Padding(8, 6, 8, 6);
		((Control)portInfoGroup).Name = "portInfoGroup";
		((Control)portInfoGroup).Padding = new Padding(8, 6, 8, 6);
		((Control)portInfoGroup).Size = new Size(842, 924);
		((Control)portInfoGroup).TabIndex = 0;
		portInfoGroup.TabStop = false;
		((Control)portInfoGroup).Text = "Port Info";
		((Control)scanWarningLabel).AutoSize = true;
		((Control)scanWarningLabel).Location = new Point(168, 64);
		((Control)scanWarningLabel).Margin = new Padding(8, 0, 8, 0);
		((Control)scanWarningLabel).Name = "scanWarningLabel";
		((Control)scanWarningLabel).Size = new Size(535, 32);
		((Control)scanWarningLabel).TabIndex = 5;
		((Control)scanWarningLabel).Text = "Unplug devices from all ports before scan";
		((Control)scanButton).Location = new Point(16, 52);
		((Control)scanButton).Margin = new Padding(8, 6, 8, 6);
		((Control)scanButton).Name = "scanButton";
		((Control)scanButton).Size = new Size(136, 52);
		((Control)scanButton).TabIndex = 4;
		((Control)scanButton).Text = "Scan";
		((ButtonBase)scanButton).UseVisualStyleBackColor = true;
		((Control)scanButton).Click += scanButton_Click;
		((Control)portInfoBox).Anchor = (AnchorStyles)15;
		((Control)portInfoBox).Location = new Point(16, 122);
		((Control)portInfoBox).Margin = new Padding(8, 6, 8, 6);
		((TextBoxBase)portInfoBox).Multiline = true;
		((Control)portInfoBox).Name = "portInfoBox";
		portInfoBox.ScrollBars = (ScrollBars)2;
		((Control)portInfoBox).Size = new Size(804, 781);
		((Control)portInfoBox).TabIndex = 0;
		((Control)portPickBox).Anchor = (AnchorStyles)9;
		((ListControl)portPickBox).FormattingEnabled = true;
		((Control)portPickBox).Location = new Point(896, 87);
		((Control)portPickBox).Margin = new Padding(8, 6, 8, 6);
		((Control)portPickBox).Name = "portPickBox";
		((Control)portPickBox).Size = new Size(210, 39);
		((Control)portPickBox).TabIndex = 1;
		((Control)portPickLabel).Anchor = (AnchorStyles)9;
		((Control)portPickLabel).AutoSize = true;
		((Control)portPickLabel).Location = new Point(890, 48);
		((Control)portPickLabel).Margin = new Padding(8, 0, 8, 0);
		((Control)portPickLabel).Name = "portPickLabel";
		((Control)portPickLabel).Size = new Size(174, 32);
		((Control)portPickLabel).TabIndex = 2;
		((Control)portPickLabel).Text = "Port Number";
		((Control)assignButton).Anchor = (AnchorStyles)9;
		((Control)assignButton).Location = new Point(900, 167);
		((Control)assignButton).Margin = new Padding(8, 6, 8, 6);
		((Control)assignButton).Name = "assignButton";
		((Control)assignButton).Size = new Size(136, 52);
		((Control)assignButton).TabIndex = 3;
		((Control)assignButton).Text = "Assign";
		((ButtonBase)assignButton).UseVisualStyleBackColor = true;
		((Control)assignButton).Click += assignButton_Click;
		((Control)port1Label).Anchor = (AnchorStyles)9;
		((Control)port1Label).AutoSize = true;
		((Control)port1Label).Location = new Point(888, 248);
		((Control)port1Label).Margin = new Padding(8, 0, 8, 0);
		((Control)port1Label).Name = "port1Label";
		((Control)port1Label).Size = new Size(243, 32);
		((Control)port1Label).TabIndex = 4;
		((Control)port1Label).Text = "Port 1 is Assigned";
		((Control)port2Label).Anchor = (AnchorStyles)9;
		((Control)port2Label).AutoSize = true;
		((Control)port2Label).Location = new Point(888, 304);
		((Control)port2Label).Margin = new Padding(8, 0, 8, 0);
		((Control)port2Label).Name = "port2Label";
		((Control)port2Label).Size = new Size(243, 32);
		((Control)port2Label).TabIndex = 5;
		((Control)port2Label).Text = "Port 2 is Assigned";
		((Control)port3Label).Anchor = (AnchorStyles)9;
		((Control)port3Label).AutoSize = true;
		((Control)port3Label).Location = new Point(888, 355);
		((Control)port3Label).Margin = new Padding(8, 0, 8, 0);
		((Control)port3Label).Name = "port3Label";
		((Control)port3Label).Size = new Size(243, 32);
		((Control)port3Label).TabIndex = 6;
		((Control)port3Label).Text = "Port 3 is Assigned";
		((Control)port4Label).Anchor = (AnchorStyles)9;
		((Control)port4Label).AutoSize = true;
		((Control)port4Label).Location = new Point(888, 407);
		((Control)port4Label).Margin = new Padding(8, 0, 8, 0);
		((Control)port4Label).Name = "port4Label";
		((Control)port4Label).Size = new Size(243, 32);
		((Control)port4Label).TabIndex = 7;
		((Control)port4Label).Text = "Port 4 is Assigned";
		((Control)port8Label).Anchor = (AnchorStyles)9;
		((Control)port8Label).AutoSize = true;
		((Control)port8Label).Location = new Point(884, 622);
		((Control)port8Label).Margin = new Padding(8, 0, 8, 0);
		((Control)port8Label).Name = "port8Label";
		((Control)port8Label).Size = new Size(243, 32);
		((Control)port8Label).TabIndex = 11;
		((Control)port8Label).Text = "Port 8 is Assigned";
		((Control)port7Label).Anchor = (AnchorStyles)9;
		((Control)port7Label).AutoSize = true;
		((Control)port7Label).Location = new Point(884, 570);
		((Control)port7Label).Margin = new Padding(8, 0, 8, 0);
		((Control)port7Label).Name = "port7Label";
		((Control)port7Label).Size = new Size(243, 32);
		((Control)port7Label).TabIndex = 10;
		((Control)port7Label).Text = "Port 7 is Assigned";
		((Control)port6Label).Anchor = (AnchorStyles)9;
		((Control)port6Label).AutoSize = true;
		((Control)port6Label).Location = new Point(884, 517);
		((Control)port6Label).Margin = new Padding(8, 0, 8, 0);
		((Control)port6Label).Name = "port6Label";
		((Control)port6Label).Size = new Size(243, 32);
		((Control)port6Label).TabIndex = 9;
		((Control)port6Label).Text = "Port 6 is Assigned";
		((Control)port5Label).Anchor = (AnchorStyles)9;
		((Control)port5Label).AutoSize = true;
		((Control)port5Label).Location = new Point(884, 463);
		((Control)port5Label).Margin = new Padding(8, 0, 8, 0);
		((Control)port5Label).Name = "port5Label";
		((Control)port5Label).Size = new Size(243, 32);
		((Control)port5Label).TabIndex = 8;
		((Control)port5Label).Text = "Port 5 is Assigned";
		((Control)clearButton).Anchor = (AnchorStyles)9;
		((Control)clearButton).Location = new Point(896, 885);
		((Control)clearButton).Margin = new Padding(8, 6, 8, 6);
		((Control)clearButton).Name = "clearButton";
		((Control)clearButton).Size = new Size(136, 52);
		((Control)clearButton).TabIndex = 12;
		((Control)clearButton).Text = "Clear";
		((ButtonBase)clearButton).UseVisualStyleBackColor = true;
		((Control)clearButton).Click += clearButton_Click;
		refreshTimer.Interval = 2000;
		refreshTimer.Tick += refreshTimer_Tick;
		((Control)port9Label).Anchor = (AnchorStyles)9;
		((Control)port9Label).AutoSize = true;
		((Control)port9Label).Location = new Point(884, 678);
		((Control)port9Label).Margin = new Padding(8, 0, 8, 0);
		((Control)port9Label).Name = "port9Label";
		((Control)port9Label).Size = new Size(243, 32);
		((Control)port9Label).TabIndex = 13;
		((Control)port9Label).Text = "Port 9 is Assigned";
		((Control)port10Label).Anchor = (AnchorStyles)9;
		((Control)port10Label).AutoSize = true;
		((Control)port10Label).Location = new Point(884, 730);
		((Control)port10Label).Margin = new Padding(8, 0, 8, 0);
		((Control)port10Label).Name = "port10Label";
		((Control)port10Label).Size = new Size(259, 32);
		((Control)port10Label).TabIndex = 14;
		((Control)port10Label).Text = "Port 10 is Assigned";
		((Control)port11Label).Anchor = (AnchorStyles)9;
		((Control)port11Label).AutoSize = true;
		((Control)port11Label).Location = new Point(884, 783);
		((Control)port11Label).Margin = new Padding(8, 0, 8, 0);
		((Control)port11Label).Name = "port11Label";
		((Control)port11Label).Size = new Size(259, 32);
		((Control)port11Label).TabIndex = 15;
		((Control)port11Label).Text = "Port 11 is Assigned";
		((Control)port12Label).Anchor = (AnchorStyles)9;
		((Control)port12Label).AutoSize = true;
		((Control)port12Label).Location = new Point(884, 835);
		((Control)port12Label).Margin = new Padding(8, 0, 8, 0);
		((Control)port12Label).Name = "port12Label";
		((Control)port12Label).Size = new Size(259, 32);
		((Control)port12Label).TabIndex = 16;
		((Control)port12Label).Text = "Port 12 is Assigned";
		((ContainerControl)this).AutoScaleDimensions = new SizeF(16f, 31f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(1138, 980);
		((Control)this).Controls.Add((Control)(object)port12Label);
		((Control)this).Controls.Add((Control)(object)port11Label);
		((Control)this).Controls.Add((Control)(object)port10Label);
		((Control)this).Controls.Add((Control)(object)port9Label);
		((Control)this).Controls.Add((Control)(object)clearButton);
		((Control)this).Controls.Add((Control)(object)port8Label);
		((Control)this).Controls.Add((Control)(object)port7Label);
		((Control)this).Controls.Add((Control)(object)port6Label);
		((Control)this).Controls.Add((Control)(object)port5Label);
		((Control)this).Controls.Add((Control)(object)port4Label);
		((Control)this).Controls.Add((Control)(object)port3Label);
		((Control)this).Controls.Add((Control)(object)port2Label);
		((Control)this).Controls.Add((Control)(object)port1Label);
		((Control)this).Controls.Add((Control)(object)assignButton);
		((Control)this).Controls.Add((Control)(object)portPickLabel);
		((Control)this).Controls.Add((Control)(object)portPickBox);
		((Control)this).Controls.Add((Control)(object)portInfoGroup);
		((Form)this).Margin = new Padding(8, 6, 8, 6);
		((Control)this).Name = "TempPortAssignment";
		((Control)this).Text = "Port Assignment";
		((Form)this).FormClosed += new FormClosedEventHandler(TempPortAssignment_FormClosed);
		((Control)portInfoGroup).ResumeLayout(false);
		((Control)portInfoGroup).PerformLayout();
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
