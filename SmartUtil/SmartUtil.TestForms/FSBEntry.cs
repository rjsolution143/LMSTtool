using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SmartUtil.TestForms;

public class FSBEntry : Form
{
	protected DialogResult result = (DialogResult)2;

	private IContainer components;

	private GroupBox actionBox4;

	private RadioButton actionButton43;

	private RadioButton actionButton42;

	private RadioButton actionButton41;

	private PictureBox actionPic4;

	private GroupBox actionBox3;

	private RadioButton actionButton33;

	private RadioButton actionButton32;

	private RadioButton actionButton31;

	private PictureBox actionPic3;

	private GroupBox actionBox2;

	private RadioButton actionButton23;

	private RadioButton actionButton22;

	private RadioButton actionButton21;

	private PictureBox actionPic2;

	private GroupBox actionBox1;

	private RadioButton actionButton13;

	private RadioButton actionButton12;

	private RadioButton actionButton11;

	private PictureBox actionPic1;

	private ListView actionListBox;

	private ListView fsbInfoBox;

	private Button cancelButton;

	private Button okButton;

	public List<int> Selection
	{
		get
		{
			List<int> list = new List<int>();
			if (((Control)actionBox1).Enabled)
			{
				if (actionButton11.Checked)
				{
					list.Add(0);
				}
				else if (actionButton12.Checked)
				{
					list.Add(1);
				}
				else
				{
					if (!actionButton13.Checked)
					{
						return new List<int>();
					}
					list.Add(2);
				}
			}
			if (((Control)actionBox2).Enabled)
			{
				if (actionButton21.Checked)
				{
					list.Add(0);
				}
				else if (actionButton22.Checked)
				{
					list.Add(1);
				}
				else
				{
					if (!actionButton23.Checked)
					{
						return new List<int>();
					}
					list.Add(2);
				}
			}
			if (((Control)actionBox3).Enabled)
			{
				if (actionButton31.Checked)
				{
					list.Add(0);
				}
				else if (actionButton32.Checked)
				{
					list.Add(1);
				}
				else
				{
					if (!actionButton33.Checked)
					{
						return new List<int>();
					}
					list.Add(2);
				}
			}
			if (((Control)actionBox3).Enabled)
			{
				if (actionButton31.Checked)
				{
					list.Add(0);
				}
				else if (actionButton32.Checked)
				{
					list.Add(1);
				}
				else
				{
					if (!actionButton33.Checked)
					{
						return new List<int>();
					}
					list.Add(2);
				}
			}
			return list;
		}
	}

	public FSBEntry()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		InitializeComponent();
	}

	public static DialogResult Entry(string title, List<string> columns, List<string> fsbInfo, List<Tuple<string, string, string, string, List<string>>> actions, out List<int> actionChoices)
	{
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		FSBEntry fSBEntry = new FSBEntry();
		try
		{
			((Control)fSBEntry).Text = title;
			foreach (string item5 in fsbInfo)
			{
				fSBEntry.fsbInfoBox.Items.Add(item5);
			}
			foreach (string column in columns)
			{
				fSBEntry.actionListBox.Columns.Add(column);
			}
			foreach (Tuple<string, string, string, string, List<string>> action in actions)
			{
				ListViewItem val = new ListViewItem(new string[5]
				{
					action.Item1,
					action.Item2,
					action.Item3,
					action.Item4,
					Smart.Convert.ToCommaSeparated((IEnumerable)action.Item5)
				});
				fSBEntry.actionListBox.Items.Add(val);
			}
			if (actions.Count > 0)
			{
				((Control)fSBEntry.actionBox1).Text = actions[0].Item1;
				string item = actions[0].Item3;
				if (Smart.File.Exists(item))
				{
					fSBEntry.actionPic1.Image = Image.FromFile(item);
				}
				((Control)fSBEntry.actionButton11).Text = actions[0].Item5[0];
				((Control)fSBEntry.actionButton12).Text = actions[0].Item5[1];
				((Control)fSBEntry.actionButton13).Text = actions[0].Item5[2];
			}
			else
			{
				((Control)fSBEntry.actionBox1).Visible = false;
				((Control)fSBEntry.actionBox1).Enabled = false;
			}
			if (actions.Count > 1)
			{
				((Control)fSBEntry.actionBox2).Text = actions[1].Item1;
				string item2 = actions[1].Item3;
				if (Smart.File.Exists(item2))
				{
					fSBEntry.actionPic2.Image = Image.FromFile(item2);
				}
				((Control)fSBEntry.actionButton21).Text = actions[1].Item5[0];
				((Control)fSBEntry.actionButton22).Text = actions[1].Item5[1];
				((Control)fSBEntry.actionButton23).Text = actions[1].Item5[2];
			}
			else
			{
				((Control)fSBEntry.actionBox2).Visible = false;
				((Control)fSBEntry.actionBox2).Enabled = false;
			}
			if (actions.Count > 2)
			{
				((Control)fSBEntry.actionBox3).Text = actions[2].Item1;
				string item3 = actions[2].Item3;
				if (Smart.File.Exists(item3))
				{
					fSBEntry.actionPic3.Image = Image.FromFile(item3);
				}
				((Control)fSBEntry.actionButton31).Text = actions[2].Item5[0];
				((Control)fSBEntry.actionButton32).Text = actions[2].Item5[1];
				((Control)fSBEntry.actionButton33).Text = actions[2].Item5[2];
			}
			else
			{
				((Control)fSBEntry.actionBox3).Visible = false;
				((Control)fSBEntry.actionBox3).Enabled = false;
			}
			if (actions.Count > 3)
			{
				((Control)fSBEntry.actionBox4).Text = actions[3].Item1;
				string item4 = actions[3].Item3;
				if (Smart.File.Exists(item4))
				{
					fSBEntry.actionPic4.Image = Image.FromFile(item4);
				}
				((Control)fSBEntry.actionButton41).Text = actions[3].Item5[0];
				((Control)fSBEntry.actionButton42).Text = actions[3].Item5[1];
				((Control)fSBEntry.actionButton43).Text = actions[3].Item5[2];
			}
			else
			{
				((Control)fSBEntry.actionBox4).Visible = false;
				((Control)fSBEntry.actionBox4).Enabled = false;
			}
			((Form)fSBEntry).ShowDialog();
			actionChoices = fSBEntry.Selection;
			return fSBEntry.result;
		}
		finally
		{
			((IDisposable)fSBEntry)?.Dispose();
		}
	}

	private void okButton_Click(object sender, EventArgs e)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (Selection.Count > 0)
		{
			result = (DialogResult)1;
			((Form)this).Close();
		}
	}

	private void cancelButton_Click(object sender, EventArgs e)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		result = (DialogResult)2;
		((Form)this).Close();
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
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Expected O, but got Unknown
		actionBox4 = new GroupBox();
		actionButton43 = new RadioButton();
		actionButton42 = new RadioButton();
		actionButton41 = new RadioButton();
		actionPic4 = new PictureBox();
		actionBox3 = new GroupBox();
		actionButton33 = new RadioButton();
		actionButton32 = new RadioButton();
		actionButton31 = new RadioButton();
		actionPic3 = new PictureBox();
		actionBox2 = new GroupBox();
		actionButton23 = new RadioButton();
		actionButton22 = new RadioButton();
		actionButton21 = new RadioButton();
		actionPic2 = new PictureBox();
		actionBox1 = new GroupBox();
		actionButton13 = new RadioButton();
		actionButton12 = new RadioButton();
		actionButton11 = new RadioButton();
		actionPic1 = new PictureBox();
		actionListBox = new ListView();
		fsbInfoBox = new ListView();
		cancelButton = new Button();
		okButton = new Button();
		((Control)actionBox4).SuspendLayout();
		((ISupportInitialize)actionPic4).BeginInit();
		((Control)actionBox3).SuspendLayout();
		((ISupportInitialize)actionPic3).BeginInit();
		((Control)actionBox2).SuspendLayout();
		((ISupportInitialize)actionPic2).BeginInit();
		((Control)actionBox1).SuspendLayout();
		((ISupportInitialize)actionPic1).BeginInit();
		((Control)this).SuspendLayout();
		((Control)actionBox4).Controls.Add((Control)(object)actionButton43);
		((Control)actionBox4).Controls.Add((Control)(object)actionButton42);
		((Control)actionBox4).Controls.Add((Control)(object)actionButton41);
		((Control)actionBox4).Controls.Add((Control)(object)actionPic4);
		((Control)actionBox4).Location = new Point(263, 396);
		((Control)actionBox4).Name = "actionBox4";
		((Control)actionBox4).Size = new Size(245, 125);
		((Control)actionBox4).TabIndex = 13;
		actionBox4.TabStop = false;
		((Control)actionBox4).Text = "Action 4";
		((Control)actionButton43).AutoSize = true;
		((Control)actionButton43).Location = new Point(147, 72);
		((Control)actionButton43).Name = "actionButton43";
		((Control)actionButton43).Size = new Size(73, 17);
		((Control)actionButton43).TabIndex = 4;
		actionButton43.TabStop = true;
		((Control)actionButton43).Text = "Action 4-3";
		((ButtonBase)actionButton43).UseVisualStyleBackColor = true;
		((Control)actionButton42).AutoSize = true;
		((Control)actionButton42).Location = new Point(147, 47);
		((Control)actionButton42).Name = "actionButton42";
		((Control)actionButton42).Size = new Size(73, 17);
		((Control)actionButton42).TabIndex = 3;
		actionButton42.TabStop = true;
		((Control)actionButton42).Text = "Action 4-2";
		((ButtonBase)actionButton42).UseVisualStyleBackColor = true;
		((Control)actionButton41).AutoSize = true;
		((Control)actionButton41).Location = new Point(147, 24);
		((Control)actionButton41).Name = "actionButton41";
		((Control)actionButton41).Size = new Size(73, 17);
		((Control)actionButton41).TabIndex = 2;
		actionButton41.TabStop = true;
		((Control)actionButton41).Text = "Action 4-1";
		((ButtonBase)actionButton41).UseVisualStyleBackColor = true;
		((Control)actionPic4).Location = new Point(6, 19);
		((Control)actionPic4).Name = "actionPic4";
		((Control)actionPic4).Size = new Size(127, 94);
		actionPic4.SizeMode = (PictureBoxSizeMode)1;
		actionPic4.TabIndex = 1;
		actionPic4.TabStop = false;
		((Control)actionBox3).Controls.Add((Control)(object)actionButton33);
		((Control)actionBox3).Controls.Add((Control)(object)actionButton32);
		((Control)actionBox3).Controls.Add((Control)(object)actionButton31);
		((Control)actionBox3).Controls.Add((Control)(object)actionPic3);
		((Control)actionBox3).Location = new Point(12, 396);
		((Control)actionBox3).Name = "actionBox3";
		((Control)actionBox3).Size = new Size(245, 125);
		((Control)actionBox3).TabIndex = 12;
		actionBox3.TabStop = false;
		((Control)actionBox3).Text = "Action 3";
		((Control)actionButton33).AutoSize = true;
		((Control)actionButton33).Location = new Point(140, 72);
		((Control)actionButton33).Name = "actionButton33";
		((Control)actionButton33).Size = new Size(73, 17);
		((Control)actionButton33).TabIndex = 7;
		actionButton33.TabStop = true;
		((Control)actionButton33).Text = "Action 3-3";
		((ButtonBase)actionButton33).UseVisualStyleBackColor = true;
		((Control)actionButton32).AutoSize = true;
		((Control)actionButton32).Location = new Point(140, 48);
		((Control)actionButton32).Name = "actionButton32";
		((Control)actionButton32).Size = new Size(73, 17);
		((Control)actionButton32).TabIndex = 6;
		actionButton32.TabStop = true;
		((Control)actionButton32).Text = "Action 3-2";
		((ButtonBase)actionButton32).UseVisualStyleBackColor = true;
		((Control)actionButton31).AutoSize = true;
		((Control)actionButton31).Location = new Point(140, 24);
		((Control)actionButton31).Name = "actionButton31";
		((Control)actionButton31).Size = new Size(73, 17);
		((Control)actionButton31).TabIndex = 5;
		actionButton31.TabStop = true;
		((Control)actionButton31).Text = "Action 3-1";
		((ButtonBase)actionButton31).UseVisualStyleBackColor = true;
		((Control)actionPic3).Location = new Point(6, 19);
		((Control)actionPic3).Name = "actionPic3";
		((Control)actionPic3).Size = new Size(127, 94);
		actionPic3.SizeMode = (PictureBoxSizeMode)1;
		actionPic3.TabIndex = 1;
		actionPic3.TabStop = false;
		((Control)actionBox2).Controls.Add((Control)(object)actionButton23);
		((Control)actionBox2).Controls.Add((Control)(object)actionButton22);
		((Control)actionBox2).Controls.Add((Control)(object)actionButton21);
		((Control)actionBox2).Controls.Add((Control)(object)actionPic2);
		((Control)actionBox2).Location = new Point(263, 265);
		((Control)actionBox2).Name = "actionBox2";
		((Control)actionBox2).Size = new Size(245, 125);
		((Control)actionBox2).TabIndex = 11;
		actionBox2.TabStop = false;
		((Control)actionBox2).Text = "Action 2";
		((Control)actionButton23).AutoSize = true;
		((Control)actionButton23).Location = new Point(140, 68);
		((Control)actionButton23).Name = "actionButton23";
		((Control)actionButton23).Size = new Size(73, 17);
		((Control)actionButton23).TabIndex = 4;
		actionButton23.TabStop = true;
		((Control)actionButton23).Text = "Action 2-3";
		((ButtonBase)actionButton23).UseVisualStyleBackColor = true;
		((Control)actionButton22).AutoSize = true;
		((Control)actionButton22).Location = new Point(140, 44);
		((Control)actionButton22).Name = "actionButton22";
		((Control)actionButton22).Size = new Size(73, 17);
		((Control)actionButton22).TabIndex = 3;
		actionButton22.TabStop = true;
		((Control)actionButton22).Text = "Action 2-2";
		((ButtonBase)actionButton22).UseVisualStyleBackColor = true;
		((Control)actionButton21).AutoSize = true;
		((Control)actionButton21).Location = new Point(140, 20);
		((Control)actionButton21).Name = "actionButton21";
		((Control)actionButton21).Size = new Size(73, 17);
		((Control)actionButton21).TabIndex = 2;
		actionButton21.TabStop = true;
		((Control)actionButton21).Text = "Action 2-1";
		((ButtonBase)actionButton21).UseVisualStyleBackColor = true;
		((Control)actionPic2).Location = new Point(6, 19);
		((Control)actionPic2).Name = "actionPic2";
		((Control)actionPic2).Size = new Size(127, 94);
		actionPic2.SizeMode = (PictureBoxSizeMode)1;
		actionPic2.TabIndex = 1;
		actionPic2.TabStop = false;
		((Control)actionBox1).Controls.Add((Control)(object)actionButton13);
		((Control)actionBox1).Controls.Add((Control)(object)actionButton12);
		((Control)actionBox1).Controls.Add((Control)(object)actionButton11);
		((Control)actionBox1).Controls.Add((Control)(object)actionPic1);
		((Control)actionBox1).Location = new Point(12, 265);
		((Control)actionBox1).Name = "actionBox1";
		((Control)actionBox1).Size = new Size(245, 125);
		((Control)actionBox1).TabIndex = 10;
		actionBox1.TabStop = false;
		((Control)actionBox1).Text = "Action 1";
		((Control)actionButton13).AutoSize = true;
		((Control)actionButton13).Location = new Point(142, 68);
		((Control)actionButton13).Name = "actionButton13";
		((Control)actionButton13).Size = new Size(73, 17);
		((Control)actionButton13).TabIndex = 7;
		actionButton13.TabStop = true;
		((Control)actionButton13).Text = "Action 1-3";
		((ButtonBase)actionButton13).UseVisualStyleBackColor = true;
		((Control)actionButton12).AutoSize = true;
		((Control)actionButton12).Location = new Point(142, 44);
		((Control)actionButton12).Name = "actionButton12";
		((Control)actionButton12).Size = new Size(73, 17);
		((Control)actionButton12).TabIndex = 6;
		actionButton12.TabStop = true;
		((Control)actionButton12).Text = "Action 1-2";
		((ButtonBase)actionButton12).UseVisualStyleBackColor = true;
		((Control)actionButton11).AutoSize = true;
		((Control)actionButton11).Location = new Point(142, 20);
		((Control)actionButton11).Name = "actionButton11";
		((Control)actionButton11).Size = new Size(73, 17);
		((Control)actionButton11).TabIndex = 5;
		actionButton11.TabStop = true;
		((Control)actionButton11).Text = "Action 1-1";
		((ButtonBase)actionButton11).UseVisualStyleBackColor = true;
		((Control)actionPic1).Location = new Point(8, 20);
		((Control)actionPic1).Name = "actionPic1";
		((Control)actionPic1).Size = new Size(127, 94);
		actionPic1.SizeMode = (PictureBoxSizeMode)1;
		actionPic1.TabIndex = 0;
		actionPic1.TabStop = false;
		((Control)actionListBox).Location = new Point(12, 129);
		((Control)actionListBox).Name = "actionListBox";
		((Control)actionListBox).Size = new Size(490, 130);
		((Control)actionListBox).TabIndex = 8;
		actionListBox.UseCompatibleStateImageBehavior = false;
		actionListBox.View = (View)1;
		((Control)fsbInfoBox).Location = new Point(12, 12);
		((Control)fsbInfoBox).Name = "fsbInfoBox";
		((Control)fsbInfoBox).Size = new Size(490, 111);
		((Control)fsbInfoBox).TabIndex = 7;
		fsbInfoBox.UseCompatibleStateImageBehavior = false;
		fsbInfoBox.View = (View)3;
		cancelButton.DialogResult = (DialogResult)2;
		((Control)cancelButton).Location = new Point(419, 534);
		((Control)cancelButton).Name = "cancelButton";
		((Control)cancelButton).Size = new Size(68, 26);
		((Control)cancelButton).TabIndex = 15;
		((Control)cancelButton).Text = "Cancel";
		((ButtonBase)cancelButton).UseVisualStyleBackColor = true;
		((Control)cancelButton).Click += cancelButton_Click;
		((Control)okButton).Location = new Point(328, 534);
		((Control)okButton).Name = "okButton";
		((Control)okButton).Size = new Size(68, 26);
		((Control)okButton).TabIndex = 14;
		((Control)okButton).Text = "OK";
		((ButtonBase)okButton).UseVisualStyleBackColor = true;
		((Control)okButton).Click += okButton_Click;
		((Form)this).AcceptButton = (IButtonControl)(object)okButton;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).CancelButton = (IButtonControl)(object)cancelButton;
		((Form)this).ClientSize = new Size(513, 572);
		((Control)this).Controls.Add((Control)(object)cancelButton);
		((Control)this).Controls.Add((Control)(object)okButton);
		((Control)this).Controls.Add((Control)(object)actionBox4);
		((Control)this).Controls.Add((Control)(object)actionBox3);
		((Control)this).Controls.Add((Control)(object)actionBox2);
		((Control)this).Controls.Add((Control)(object)actionBox1);
		((Control)this).Controls.Add((Control)(object)actionListBox);
		((Control)this).Controls.Add((Control)(object)fsbInfoBox);
		((Control)this).Name = "FSBEntry";
		((Control)this).Text = "FSBEntry";
		((Control)actionBox4).ResumeLayout(false);
		((Control)actionBox4).PerformLayout();
		((ISupportInitialize)actionPic4).EndInit();
		((Control)actionBox3).ResumeLayout(false);
		((Control)actionBox3).PerformLayout();
		((ISupportInitialize)actionPic3).EndInit();
		((Control)actionBox2).ResumeLayout(false);
		((Control)actionBox2).PerformLayout();
		((ISupportInitialize)actionPic2).EndInit();
		((Control)actionBox1).ResumeLayout(false);
		((Control)actionBox1).PerformLayout();
		((ISupportInitialize)actionPic1).EndInit();
		((Control)this).ResumeLayout(false);
	}
}
