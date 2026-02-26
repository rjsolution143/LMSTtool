using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SmartUtil.TestForms;

public class DownloadManager : Form
{
	private IContainer components;

	private TextBox logBox;

	private Timer tickTimer;

	public DownloadEngine Engine { get; set; }

	public DownloadManager()
	{
		InitializeComponent();
	}

	private void RefreshStatus()
	{
		((Control)logBox).Text = Engine.Print();
	}

	private void tickTimer_Tick(object sender, EventArgs e)
	{
		RefreshStatus();
	}

	private void DownloadManager_Load(object sender, EventArgs e)
	{
		Engine.Start();
		tickTimer.Start();
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
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		components = new Container();
		logBox = new TextBox();
		tickTimer = new Timer(components);
		((Control)this).SuspendLayout();
		((Control)logBox).Location = new Point(12, 339);
		((TextBoxBase)logBox).Multiline = true;
		((Control)logBox).Name = "logBox";
		((Control)logBox).Size = new Size(599, 212);
		((Control)logBox).TabIndex = 1;
		tickTimer.Tick += tickTimer_Tick;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(623, 563);
		((Control)this).Controls.Add((Control)(object)logBox);
		((Control)this).Name = "DownloadManager";
		((Control)this).Text = "DownloadManager";
		((Form)this).Load += DownloadManager_Load;
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
