using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SmartUtil.TestForms;

public class FSBList : Form
{
	protected DialogResult result = (DialogResult)2;

	private IContainer components;

	private ListView deviceInfoBox;

	private ListView fsbBox;

	private Label testLabel;

	public string Selection
	{
		get
		{
			if (fsbBox.SelectedItems.Count > 0)
			{
				return fsbBox.SelectedItems[0].Text;
			}
			return string.Empty;
		}
	}

	public FSBList()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		InitializeComponent();
	}

	public static DialogResult List(string title, List<string> columns, string buttonText, List<string> deviceInfo, SortedList<string, Tuple<string, string>> fsbs, out string fsbChoice)
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		FSBList fSBList = new FSBList();
		try
		{
			((Control)fSBList).Text = title;
			foreach (string item in deviceInfo)
			{
				fSBList.deviceInfoBox.Items.Add(item);
			}
			foreach (string column in columns)
			{
				fSBList.fsbBox.Columns.Add(column);
			}
			foreach (string key in fsbs.Keys)
			{
				ListViewItem val = new ListViewItem(new string[4]
				{
					key,
					fsbs[key].Item1,
					fsbs[key].Item2,
					buttonText
				});
				fSBList.fsbBox.Items.Add(val);
			}
			((Form)fSBList).ShowDialog();
			fsbChoice = fSBList.Selection;
			return fSBList.result;
		}
		finally
		{
			((IDisposable)fSBList)?.Dispose();
		}
	}

	private void fsbBox_DoubleClick(object sender, EventArgs e)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (Selection != string.Empty)
		{
			result = (DialogResult)1;
			((Form)this).Close();
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
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		deviceInfoBox = new ListView();
		fsbBox = new ListView();
		testLabel = new Label();
		((Control)this).SuspendLayout();
		((Control)deviceInfoBox).Location = new Point(12, 12);
		((Control)deviceInfoBox).Name = "deviceInfoBox";
		((Control)deviceInfoBox).Size = new Size(490, 111);
		((Control)deviceInfoBox).TabIndex = 0;
		deviceInfoBox.UseCompatibleStateImageBehavior = false;
		deviceInfoBox.View = (View)3;
		((Control)fsbBox).Location = new Point(12, 144);
		((Control)fsbBox).Name = "fsbBox";
		((Control)fsbBox).Size = new Size(490, 371);
		((Control)fsbBox).TabIndex = 1;
		fsbBox.UseCompatibleStateImageBehavior = false;
		fsbBox.View = (View)1;
		((Control)fsbBox).DoubleClick += fsbBox_DoubleClick;
		((Control)testLabel).AutoSize = true;
		((Control)testLabel).Location = new Point(12, 128);
		((Control)testLabel).Name = "testLabel";
		((Control)testLabel).Size = new Size(461, 13);
		((Control)testLabel).TabIndex = 2;
		((Control)testLabel).Text = "THIS IS FOR TESTING AND WILL BE REMOVED: PLEASE DOUBLE-CLICK TO SELECT FSB";
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(514, 527);
		((Control)this).Controls.Add((Control)(object)testLabel);
		((Control)this).Controls.Add((Control)(object)fsbBox);
		((Control)this).Controls.Add((Control)(object)deviceInfoBox);
		((Control)this).Name = "FSBList";
		((Control)this).Text = "FSBList";
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
