using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ISmart;

namespace SmartUtil.TestForms;

public class Validation : Form
{
	private const string UNKNOWN = "UNKNOWN";

	private Timer deviceTimer = new Timer();

	private Timer statusTimer = new Timer();

	private IDevice currentDevice;

	private string deviceDetails = "UNKNOWN";

	private bool populated;

	private IContainer components;

	private ListView stepList;

	private Button runButton;

	private ComboBox recipeListBox;

	private ColumnHeader numberHeader;

	private ColumnHeader stepHeader;

	private ColumnHeader readingHeader;

	private ColumnHeader valueHeader;

	private Label deviceLabel;

	private Label statusText;

	private Button clearButton;

	private ComboBox profileListBox;

	private Label userProfileLabel;

	private Label selectedRecipeLabel;

	private Button saveProfileButton;

	private ColumnHeader resultHeader;

	private string TAG => ((object)this).GetType().FullName;

	public Validation()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		InitializeComponent();
		ResetUI();
		deviceTimer.Interval = 2000;
		deviceTimer.Tick += DeviceTimer_Tick;
		deviceTimer.Start();
		statusTimer.Interval = 1000;
		statusTimer.Tick += StatusTimer_Tick;
		statusTimer.Start();
		ShowUserOptions();
	}

	private void ResetUI()
	{
		((Control)deviceLabel).Text = "No Device";
		((Control)statusText).Text = "Not Running";
		deviceDetails = "UNKNOWN";
		((Control)saveProfileButton).Enabled = false;
		currentDevice = null;
		populated = false;
		stepList.Items.Clear();
		profileListBox.Items.Clear();
		recipeListBox.Items.Clear();
	}

	private void ShowDeviceDetails()
	{
		if (currentDevice == null)
		{
			deviceDetails = "UNKNOWN";
			return;
		}
		string arg = "UNKNOWN";
		string arg2 = "UNKNOWN";
		string arg3 = "UNKNOWN";
		if (currentDevice.ModelId != string.Empty)
		{
			arg = currentDevice.ModelId;
		}
		if (currentDevice.SerialNumber != string.Empty)
		{
			arg2 = currentDevice.SerialNumber;
		}
		if (currentDevice.ID != string.Empty)
		{
			arg3 = currentDevice.ID;
		}
		string text = $"{arg}: {arg2} {arg3}";
		((Control)deviceLabel).Text = text;
	}

	private void ShowUserOptions()
	{
		string text = ((Control)recipeListBox).Text.ToString();
		recipeListBox.Items.Clear();
		foreach (string item in Smart.Validator.FindRecipes(currentDevice))
		{
			recipeListBox.Items.Add((object)item);
		}
		if (recipeListBox.Items.Contains((object)text))
		{
			recipeListBox.SelectedItem = text;
		}
		if (text == string.Empty && recipeListBox.Items.Count > 0)
		{
			text = recipeListBox.Items[0].ToString();
		}
		List<string> list = Smart.Validator.FindProfiles(currentDevice, text);
		bool flag = list.Count == profileListBox.Items.Count;
		foreach (string item2 in list)
		{
			bool flag2 = false;
			foreach (object item3 in profileListBox.Items)
			{
				if (item2 == item3.ToString())
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			return;
		}
		profileListBox.Items.Clear();
		foreach (string item4 in list)
		{
			profileListBox.Items.Add((object)item4);
		}
	}

	public static Color ResultColor(Result result)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected I4, but got Unknown
		return (int)result switch
		{
			0 => Control.DefaultBackColor, 
			7 => Color.White, 
			2 => Color.Orange, 
			5 => Color.OrangeRed, 
			4 => Color.Red, 
			3 => Color.Pink, 
			6 => Color.Yellow, 
			1 => Color.Red, 
			8 => Color.LightGreen, 
			_ => throw new NotSupportedException("Result type not supported " + ((object)(Result)(ref result)).ToString()), 
		};
	}

	private void PopulateSteps(List<ValidationItem> items, bool showResults)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		stepList.Items.Clear();
		int num = 1;
		foreach (ValidationItem item in items)
		{
			string[] obj = new string[5]
			{
				num.ToString(),
				item.Name,
				item.Target,
				item.Value,
				null
			};
			Result result = item.Result;
			obj[4] = ((object)(Result)(ref result)).ToString();
			ListViewItem val = new ListViewItem(obj);
			val.Checked = item.Enabled;
			if (showResults && item.Enabled)
			{
				val.BackColor = ResultColor(item.Result);
			}
			else if (showResults)
			{
				val.BackColor = Color.LightGray;
			}
			stepList.Items.Add(val);
			num++;
		}
		populated = true;
	}

	private List<ValidationItem> SaveSteps()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Expected O, but got Unknown
		List<ValidationItem> list = new List<ValidationItem>();
		foreach (ListViewItem item2 in stepList.Items)
		{
			ListViewItem val = item2;
			string text = val.SubItems[1].Text;
			string text2 = val.SubItems[2].Text;
			string text3 = val.SubItems[3].Text;
			Result val2 = (Result)8;
			ValidationItem item = new ValidationItem(text, val.Checked, text2, text3, val2);
			list.Add(item);
		}
		return list;
	}

	private void InitRecipe()
	{
		((Control)saveProfileButton).Enabled = true;
		((Control)statusText).Text = "Initializing";
		string text = ((Control)recipeListBox).Text.ToString();
		Smart.Validator.InitRecipe(currentDevice, text);
		Smart.Thread.Wait(TimeSpan.FromSeconds(60.0), (Checker<bool>)FinishedValidation);
		List<ValidationItem> items = Smart.Validator.FindItems(currentDevice);
		PopulateSteps(items, showResults: false);
		((Control)statusText).Text = "Finished Init";
	}

	private bool FinishedValidation()
	{
		IResultLogger log = currentDevice.Log;
		if (log.Results.Count < 1)
		{
			Smart.Thread.Wait(TimeSpan.FromSeconds(1.0));
			return false;
		}
		if (log.Results[log.Results.Count - 1].Item1 != "LMST_Validation")
		{
			Smart.Thread.Wait(TimeSpan.FromSeconds(1.0));
			return false;
		}
		return true;
	}

	private void ValidateRecipe()
	{
		((Control)saveProfileButton).Enabled = false;
		((Control)statusText).Text = "Running Validation";
		string text = ((Control)recipeListBox).Text.ToString();
		string text2 = ((Control)profileListBox).Text.ToString();
		Smart.Validator.RunRecipe(currentDevice, text, text2);
		Smart.Thread.Wait(TimeSpan.FromSeconds(60.0), (Checker<bool>)FinishedValidation);
		List<ValidationItem> items = Smart.Validator.FindResults(currentDevice, text, text2);
		PopulateSteps(items, showResults: true);
		((Control)statusText).Text = "Finished Validation";
	}

	private void DeviceTimer_Tick(object sender, EventArgs e)
	{
		if (currentDevice != null)
		{
			if (deviceDetails.StartsWith("UNKNOWN"))
			{
				ShowDeviceDetails();
				ShowUserOptions();
			}
		}
		else
		{
			SortedList<string, IDevice> devices = Smart.DeviceManager.Devices;
			if (devices.Count >= 1)
			{
				currentDevice = devices.Values[0];
			}
		}
	}

	private void StatusTimer_Tick(object sender, EventArgs e)
	{
		if (currentDevice != null && currentDevice.Log != null && currentDevice.Locked)
		{
			((Control)statusText).Text = "Running Step: " + currentDevice.Log.CurrentStep;
		}
	}

	private void runButton_Click(object sender, EventArgs e)
	{
		if (currentDevice == null)
		{
			((Control)statusText).Text = "Cannot run without connected device";
		}
		else if (!populated)
		{
			InitRecipe();
		}
		else
		{
			ValidateRecipe();
		}
	}

	private void clearButton_Click(object sender, EventArgs e)
	{
		ResetUI();
	}

	private void recipeListBox_SelectionChangeCommitted(object sender, EventArgs e)
	{
		try
		{
			ShowUserOptions();
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, ex.ToString());
		}
	}

	private void profileListBox_SelectionChangeCommitted(object sender, EventArgs e)
	{
		try
		{
			string text = ((Control)recipeListBox).Text.ToString();
			string text2 = ((Control)profileListBox).Text.ToString();
			if (!(text2.Trim() == string.Empty))
			{
				List<ValidationItem> items = Smart.Validator.LoadProfile(currentDevice, text, text2);
				PopulateSteps(items, showResults: false);
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, ex.ToString());
		}
	}

	private void saveProfileButton_Click(object sender, EventArgs e)
	{
		try
		{
			string text = ((Control)profileListBox).Text.ToString();
			if (!(text.Trim() == string.Empty))
			{
				string text2 = recipeListBox.SelectedItem.ToString();
				List<ValidationItem> list = SaveSteps();
				Smart.Validator.SaveProfile(currentDevice, text, text2, list);
			}
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, ex.ToString());
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
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		stepList = new ListView();
		numberHeader = new ColumnHeader();
		stepHeader = new ColumnHeader();
		readingHeader = new ColumnHeader();
		valueHeader = new ColumnHeader();
		runButton = new Button();
		recipeListBox = new ComboBox();
		deviceLabel = new Label();
		statusText = new Label();
		clearButton = new Button();
		profileListBox = new ComboBox();
		userProfileLabel = new Label();
		selectedRecipeLabel = new Label();
		saveProfileButton = new Button();
		resultHeader = new ColumnHeader();
		((Control)this).SuspendLayout();
		((Control)stepList).Anchor = (AnchorStyles)14;
		stepList.CheckBoxes = true;
		stepList.Columns.AddRange((ColumnHeader[])(object)new ColumnHeader[5] { numberHeader, stepHeader, readingHeader, valueHeader, resultHeader });
		stepList.FullRowSelect = true;
		((Control)stepList).Location = new Point(12, 89);
		((Control)stepList).Name = "stepList";
		((Control)stepList).Size = new Size(599, 274);
		((Control)stepList).TabIndex = 0;
		stepList.UseCompatibleStateImageBehavior = false;
		stepList.View = (View)1;
		numberHeader.Text = "#";
		stepHeader.Text = "Step";
		stepHeader.Width = 215;
		readingHeader.DisplayIndex = 3;
		readingHeader.Text = "Reading";
		readingHeader.Width = 61;
		valueHeader.DisplayIndex = 2;
		valueHeader.Text = "Target Value";
		valueHeader.Width = 86;
		((Control)runButton).Location = new Point(12, 27);
		((Control)runButton).Name = "runButton";
		((Control)runButton).Size = new Size(85, 28);
		((Control)runButton).TabIndex = 1;
		((Control)runButton).Text = "Run";
		((ButtonBase)runButton).UseVisualStyleBackColor = true;
		((Control)runButton).Click += runButton_Click;
		recipeListBox.DropDownStyle = (ComboBoxStyle)2;
		((ListControl)recipeListBox).FormattingEnabled = true;
		((Control)recipeListBox).Location = new Point(209, 32);
		((Control)recipeListBox).Name = "recipeListBox";
		((Control)recipeListBox).Size = new Size(121, 21);
		((Control)recipeListBox).TabIndex = 2;
		recipeListBox.SelectionChangeCommitted += recipeListBox_SelectionChangeCommitted;
		((Control)deviceLabel).AutoSize = true;
		((Control)deviceLabel).Location = new Point(13, 8);
		((Control)deviceLabel).Name = "deviceLabel";
		((Control)deviceLabel).Size = new Size(77, 13);
		((Control)deviceLabel).TabIndex = 6;
		((Control)deviceLabel).Text = "DEVICE TEXT";
		((Control)statusText).Location = new Point(12, 68);
		((Control)statusText).Name = "statusText";
		((Control)statusText).Size = new Size(388, 18);
		((Control)statusText).TabIndex = 7;
		((Control)statusText).Text = "STATUS TEXT";
		((Control)clearButton).Location = new Point(547, 62);
		((Control)clearButton).Name = "clearButton";
		((Control)clearButton).Size = new Size(63, 24);
		((Control)clearButton).TabIndex = 8;
		((Control)clearButton).Text = "Clear";
		((ButtonBase)clearButton).UseVisualStyleBackColor = true;
		((Control)clearButton).Click += clearButton_Click;
		((ListControl)profileListBox).FormattingEnabled = true;
		((Control)profileListBox).Location = new Point(415, 32);
		((Control)profileListBox).Name = "profileListBox";
		((Control)profileListBox).Size = new Size(121, 21);
		((Control)profileListBox).TabIndex = 9;
		profileListBox.SelectionChangeCommitted += profileListBox_SelectionChangeCommitted;
		((Control)userProfileLabel).AutoSize = true;
		((Control)userProfileLabel).Location = new Point(345, 35);
		((Control)userProfileLabel).Name = "userProfileLabel";
		((Control)userProfileLabel).Size = new Size(64, 13);
		((Control)userProfileLabel).TabIndex = 10;
		((Control)userProfileLabel).Text = "User Profile:";
		((Control)selectedRecipeLabel).AutoSize = true;
		((Control)selectedRecipeLabel).Location = new Point(114, 35);
		((Control)selectedRecipeLabel).Name = "selectedRecipeLabel";
		((Control)selectedRecipeLabel).Size = new Size(89, 13);
		((Control)selectedRecipeLabel).TabIndex = 11;
		((Control)selectedRecipeLabel).Text = "Selected Recipe:";
		((Control)saveProfileButton).Enabled = false;
		((Control)saveProfileButton).Location = new Point(547, 29);
		((Control)saveProfileButton).Name = "saveProfileButton";
		((Control)saveProfileButton).Size = new Size(63, 24);
		((Control)saveProfileButton).TabIndex = 12;
		((Control)saveProfileButton).Text = "Save";
		((ButtonBase)saveProfileButton).UseVisualStyleBackColor = true;
		((Control)saveProfileButton).Click += saveProfileButton_Click;
		resultHeader.Text = "Result";
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(623, 375);
		((Control)this).Controls.Add((Control)(object)saveProfileButton);
		((Control)this).Controls.Add((Control)(object)selectedRecipeLabel);
		((Control)this).Controls.Add((Control)(object)userProfileLabel);
		((Control)this).Controls.Add((Control)(object)profileListBox);
		((Control)this).Controls.Add((Control)(object)clearButton);
		((Control)this).Controls.Add((Control)(object)statusText);
		((Control)this).Controls.Add((Control)(object)deviceLabel);
		((Control)this).Controls.Add((Control)(object)recipeListBox);
		((Control)this).Controls.Add((Control)(object)runButton);
		((Control)this).Controls.Add((Control)(object)stepList);
		((Control)this).Name = "Validation";
		((Control)this).Text = "Validation";
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
