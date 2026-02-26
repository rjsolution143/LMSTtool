using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SmartUtil.TestForms;

public class SearchSelect : Form
{
	protected DialogResult result = (DialogResult)2;

	private IContainer components;

	private Button okButton;

	private ComboBox searchBox;

	private Button cancelButton;

	public string Selection
	{
		get
		{
			if (searchBox.SelectedItem != null && searchBox.SelectedItem.ToString() != string.Empty)
			{
				return searchBox.SelectedItem.ToString();
			}
			return string.Empty;
		}
	}

	public SearchSelect()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		InitializeComponent();
	}

	public static DialogResult Select(string title, string text, List<string> choices, out string choice)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		SearchSelect searchSelect = new SearchSelect();
		try
		{
			((Control)searchSelect).Text = title;
			foreach (string choice2 in choices)
			{
				searchSelect.searchBox.Items.Add((object)choice2);
			}
			((Form)searchSelect).ShowDialog();
			choice = searchSelect.Selection;
			return searchSelect.result;
		}
		finally
		{
			((IDisposable)searchSelect)?.Dispose();
		}
	}

	private void okButton_Click(object sender, EventArgs e)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (Selection != string.Empty)
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
		okButton = new Button();
		searchBox = new ComboBox();
		cancelButton = new Button();
		((Control)this).SuspendLayout();
		((Control)okButton).Location = new Point(91, 64);
		((Control)okButton).Name = "okButton";
		((Control)okButton).Size = new Size(68, 26);
		((Control)okButton).TabIndex = 0;
		((Control)okButton).Text = "OK";
		((ButtonBase)okButton).UseVisualStyleBackColor = true;
		((Control)okButton).Click += okButton_Click;
		searchBox.DropDownStyle = (ComboBoxStyle)2;
		((ListControl)searchBox).FormattingEnabled = true;
		((Control)searchBox).Location = new Point(12, 23);
		((Control)searchBox).Name = "searchBox";
		((Control)searchBox).Size = new Size(238, 21);
		((Control)searchBox).TabIndex = 1;
		cancelButton.DialogResult = (DialogResult)2;
		((Control)cancelButton).Location = new Point(182, 64);
		((Control)cancelButton).Name = "cancelButton";
		((Control)cancelButton).Size = new Size(68, 26);
		((Control)cancelButton).TabIndex = 2;
		((Control)cancelButton).Text = "Cancel";
		((ButtonBase)cancelButton).UseVisualStyleBackColor = true;
		((Control)cancelButton).Click += cancelButton_Click;
		((Form)this).AcceptButton = (IButtonControl)(object)okButton;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).CancelButton = (IButtonControl)(object)cancelButton;
		((Form)this).ClientSize = new Size(262, 104);
		((Control)this).Controls.Add((Control)(object)cancelButton);
		((Control)this).Controls.Add((Control)(object)searchBox);
		((Control)this).Controls.Add((Control)(object)okButton);
		((Control)this).Name = "SearchSelect";
		((Control)this).Text = "SearchSelect";
		((Control)this).ResumeLayout(false);
	}
}
