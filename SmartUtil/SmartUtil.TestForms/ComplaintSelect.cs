using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SmartUtil.TestForms;

public class ComplaintSelect : Form
{
	protected DialogResult result = (DialogResult)2;

	protected SortedList<string, string> complaintImages = new SortedList<string, string>();

	protected SortedList<string, string> symptomImages = new SortedList<string, string>();

	protected SortedList<string, string> xlate = new SortedList<string, string>();

	private IContainer components;

	private ComboBox complaintBox1;

	private Button cancelButton;

	private Button okButton;

	private PictureBox complaintImage1;

	private PictureBox complaintImage2;

	private ComboBox complaintBox2;

	private PictureBox symptomImage2;

	private ComboBox symptomBox2;

	private PictureBox symptomImage1;

	private ComboBox symptomBox1;

	private Label complaintLabel;

	private Label symptomPrompt;

	public Tuple<List<string>, List<string>> Selection
	{
		get
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			if (complaintBox1.SelectedItem != null && complaintBox1.SelectedItem.ToString() != string.Empty)
			{
				string key = complaintBox1.SelectedItem.ToString();
				string item = xlate[key];
				list.Add(item);
			}
			if (complaintBox2.SelectedItem != null && complaintBox2.SelectedItem.ToString() != string.Empty)
			{
				string key2 = complaintBox2.SelectedItem.ToString();
				string item2 = xlate[key2];
				list.Add(item2);
			}
			if (symptomBox1.SelectedItem != null && symptomBox1.SelectedItem.ToString() != string.Empty)
			{
				string key3 = symptomBox1.SelectedItem.ToString();
				string item3 = xlate[key3];
				list2.Add(item3);
			}
			if (symptomBox2.SelectedItem != null && symptomBox2.SelectedItem.ToString() != string.Empty)
			{
				string key4 = symptomBox2.SelectedItem.ToString();
				string item4 = xlate[key4];
				list2.Add(item4);
			}
			return new Tuple<List<string>, List<string>>(list, list2);
		}
	}

	public ComplaintSelect()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		InitializeComponent();
	}

	public static DialogResult Complaint(string title, string complaintPrompt, SortedList<string, string> complaintNames, SortedList<string, string> complaintIcons, string symptomPrompt, SortedList<string, string> symptomNames, SortedList<string, string> symptomIcons, out List<string> complaintChoice, out List<string> symptomChoice)
	{
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		ComplaintSelect complaintSelect = new ComplaintSelect();
		try
		{
			((Control)complaintSelect).Text = title;
			((Control)complaintSelect.complaintLabel).Text = complaintPrompt;
			((Control)complaintSelect.symptomPrompt).Text = symptomPrompt;
			complaintSelect.complaintImages = complaintIcons;
			complaintSelect.symptomImages = symptomIcons;
			foreach (string key in complaintNames.Keys)
			{
				string text = complaintNames[key];
				complaintSelect.xlate[text] = key;
				complaintSelect.complaintBox1.Items.Add((object)text);
				complaintSelect.complaintBox2.Items.Add((object)text);
			}
			foreach (string key2 in symptomNames.Keys)
			{
				string text2 = symptomNames[key2];
				complaintSelect.xlate[text2] = key2;
				complaintSelect.symptomBox1.Items.Add((object)text2);
				complaintSelect.symptomBox2.Items.Add((object)text2);
			}
			((Form)complaintSelect).ShowDialog();
			Tuple<List<string>, List<string>> selection = complaintSelect.Selection;
			complaintChoice = selection.Item1;
			symptomChoice = selection.Item2;
			return complaintSelect.result;
		}
		finally
		{
			((IDisposable)complaintSelect)?.Dispose();
		}
	}

	private void okButton_Click(object sender, EventArgs e)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Tuple<List<string>, List<string>> selection = Selection;
		if (selection.Item1.Count > 0 && selection.Item2.Count > 0)
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

	private void complaintBox1_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (complaintBox1.SelectedItem != null && complaintImages.ContainsKey(complaintBox1.SelectedItem.ToString()))
		{
			string text = complaintImages[complaintBox1.SelectedItem.ToString()];
			if (Smart.File.Exists(text))
			{
				complaintImage1.Image = Image.FromFile(text);
			}
		}
	}

	private void complaintBox2_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (complaintBox2.SelectedItem != null && complaintImages.ContainsKey(complaintBox2.SelectedItem.ToString()))
		{
			string text = complaintImages[complaintBox2.SelectedItem.ToString()];
			if (Smart.File.Exists(text))
			{
				complaintImage2.Image = Image.FromFile(text);
			}
		}
	}

	private void symptomBox1_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (symptomBox1.SelectedItem != null && symptomImages.ContainsKey(symptomBox1.SelectedItem.ToString()))
		{
			string text = symptomImages[symptomBox1.SelectedItem.ToString()];
			if (Smart.File.Exists(text))
			{
				symptomImage1.Image = Image.FromFile(text);
			}
		}
	}

	private void symptomBox2_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (symptomBox2.SelectedItem != null && symptomImages.ContainsKey(symptomBox2.SelectedItem.ToString()))
		{
			string text = symptomImages[symptomBox2.SelectedItem.ToString()];
			if (Smart.File.Exists(text))
			{
				symptomImage2.Image = Image.FromFile(text);
			}
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
		complaintBox1 = new ComboBox();
		cancelButton = new Button();
		okButton = new Button();
		complaintImage1 = new PictureBox();
		complaintImage2 = new PictureBox();
		complaintBox2 = new ComboBox();
		symptomImage2 = new PictureBox();
		symptomBox2 = new ComboBox();
		symptomImage1 = new PictureBox();
		symptomBox1 = new ComboBox();
		complaintLabel = new Label();
		symptomPrompt = new Label();
		((ISupportInitialize)complaintImage1).BeginInit();
		((ISupportInitialize)complaintImage2).BeginInit();
		((ISupportInitialize)symptomImage2).BeginInit();
		((ISupportInitialize)symptomImage1).BeginInit();
		((Control)this).SuspendLayout();
		complaintBox1.DropDownStyle = (ComboBoxStyle)2;
		((ListControl)complaintBox1).FormattingEnabled = true;
		((Control)complaintBox1).Location = new Point(55, 63);
		((Control)complaintBox1).Name = "complaintBox1";
		((Control)complaintBox1).Size = new Size(140, 21);
		((Control)complaintBox1).TabIndex = 0;
		complaintBox1.SelectedIndexChanged += complaintBox1_SelectedIndexChanged;
		cancelButton.DialogResult = (DialogResult)2;
		((Control)cancelButton).Location = new Point(331, 211);
		((Control)cancelButton).Name = "cancelButton";
		((Control)cancelButton).Size = new Size(68, 26);
		((Control)cancelButton).TabIndex = 4;
		((Control)cancelButton).Text = "Cancel";
		((ButtonBase)cancelButton).UseVisualStyleBackColor = true;
		((Control)cancelButton).Click += cancelButton_Click;
		((Control)okButton).Location = new Point(240, 211);
		((Control)okButton).Name = "okButton";
		((Control)okButton).Size = new Size(68, 26);
		((Control)okButton).TabIndex = 3;
		((Control)okButton).Text = "OK";
		((ButtonBase)okButton).UseVisualStyleBackColor = true;
		((Control)okButton).Click += okButton_Click;
		((Control)complaintImage1).Location = new Point(12, 54);
		((Control)complaintImage1).Name = "complaintImage1";
		((Control)complaintImage1).Size = new Size(37, 40);
		complaintImage1.SizeMode = (PictureBoxSizeMode)1;
		complaintImage1.TabIndex = 5;
		complaintImage1.TabStop = false;
		((Control)complaintImage2).Location = new Point(216, 54);
		((Control)complaintImage2).Name = "complaintImage2";
		((Control)complaintImage2).Size = new Size(37, 40);
		complaintImage2.SizeMode = (PictureBoxSizeMode)1;
		complaintImage2.TabIndex = 7;
		complaintImage2.TabStop = false;
		complaintBox2.DropDownStyle = (ComboBoxStyle)2;
		((ListControl)complaintBox2).FormattingEnabled = true;
		((Control)complaintBox2).Location = new Point(259, 63);
		((Control)complaintBox2).Name = "complaintBox2";
		((Control)complaintBox2).Size = new Size(140, 21);
		((Control)complaintBox2).TabIndex = 6;
		complaintBox2.SelectedIndexChanged += complaintBox2_SelectedIndexChanged;
		((Control)symptomImage2).Location = new Point(216, 156);
		((Control)symptomImage2).Name = "symptomImage2";
		((Control)symptomImage2).Size = new Size(37, 40);
		symptomImage2.SizeMode = (PictureBoxSizeMode)1;
		symptomImage2.TabIndex = 11;
		symptomImage2.TabStop = false;
		symptomBox2.DropDownStyle = (ComboBoxStyle)2;
		((ListControl)symptomBox2).FormattingEnabled = true;
		((Control)symptomBox2).Location = new Point(259, 165);
		((Control)symptomBox2).Name = "symptomBox2";
		((Control)symptomBox2).Size = new Size(140, 21);
		((Control)symptomBox2).TabIndex = 10;
		symptomBox2.SelectedIndexChanged += symptomBox2_SelectedIndexChanged;
		((Control)symptomImage1).Location = new Point(12, 156);
		((Control)symptomImage1).Name = "symptomImage1";
		((Control)symptomImage1).Size = new Size(37, 40);
		symptomImage1.SizeMode = (PictureBoxSizeMode)1;
		symptomImage1.TabIndex = 9;
		symptomImage1.TabStop = false;
		symptomBox1.DropDownStyle = (ComboBoxStyle)2;
		((ListControl)symptomBox1).FormattingEnabled = true;
		((Control)symptomBox1).Location = new Point(55, 165);
		((Control)symptomBox1).Name = "symptomBox1";
		((Control)symptomBox1).Size = new Size(140, 21);
		((Control)symptomBox1).TabIndex = 8;
		symptomBox1.SelectedIndexChanged += symptomBox1_SelectedIndexChanged;
		((Control)complaintLabel).Location = new Point(9, 9);
		((Control)complaintLabel).Name = "complaintLabel";
		((Control)complaintLabel).Size = new Size(390, 42);
		((Control)complaintLabel).TabIndex = 12;
		((Control)complaintLabel).Text = "Complaint prompt";
		((Control)symptomPrompt).Location = new Point(11, 111);
		((Control)symptomPrompt).Name = "symptomPrompt";
		((Control)symptomPrompt).Size = new Size(390, 42);
		((Control)symptomPrompt).TabIndex = 13;
		((Control)symptomPrompt).Text = "Symptom prompt";
		((Form)this).AcceptButton = (IButtonControl)(object)okButton;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).CancelButton = (IButtonControl)(object)cancelButton;
		((Form)this).ClientSize = new Size(413, 249);
		((Control)this).Controls.Add((Control)(object)symptomPrompt);
		((Control)this).Controls.Add((Control)(object)complaintLabel);
		((Control)this).Controls.Add((Control)(object)symptomImage2);
		((Control)this).Controls.Add((Control)(object)symptomBox2);
		((Control)this).Controls.Add((Control)(object)symptomImage1);
		((Control)this).Controls.Add((Control)(object)symptomBox1);
		((Control)this).Controls.Add((Control)(object)complaintImage2);
		((Control)this).Controls.Add((Control)(object)complaintBox2);
		((Control)this).Controls.Add((Control)(object)complaintImage1);
		((Control)this).Controls.Add((Control)(object)cancelButton);
		((Control)this).Controls.Add((Control)(object)okButton);
		((Control)this).Controls.Add((Control)(object)complaintBox1);
		((Control)this).Name = "ComplaintSelect";
		((Control)this).Text = "ComplaintSelect";
		((ISupportInitialize)complaintImage1).EndInit();
		((ISupportInitialize)complaintImage2).EndInit();
		((ISupportInitialize)symptomImage2).EndInit();
		((ISupportInitialize)symptomImage1).EndInit();
		((Control)this).ResumeLayout(false);
	}
}
