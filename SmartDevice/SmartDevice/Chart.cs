using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SmartDevice;

public class Chart : Form
{
	private IContainer components;

	private Chart chart1;

	private Label label1;

	private Label noamalAirPressure;

	private Label maxPressureWithObject;

	private Label label3;

	private Label labelStableVal;

	private Label labelResult;

	private Label label5;

	private Label labelTotalMeasTime;

	private Label label7;

	private Label labelMeasCycle;

	private Label label9;

	private Label label2;

	private Label labelTrackId;

	private Label labelDroppedPressure;

	private Label label6;

	private Label labelDropLimit;

	private Label labelStableTime;

	private Label label12;

	private Label labelJumpValue;

	private Label label14;

	private Label labelJumpLimit;

	private Label labelFinalVal;

	private Label label8;

	private Label label11;

	private Label label4;

	private Label label10;

	private Button button1;

	public Chart()
	{
		InitializeComponent();
		((Control)noamalAirPressure).Text = "";
		((Control)maxPressureWithObject).Text = "";
		((Control)labelTotalMeasTime).Text = "";
		((Control)labelMeasCycle).Text = "";
		((Control)labelStableVal).Text = "";
		((Control)labelTrackId).Text = "";
		((Control)labelDroppedPressure).Text = "";
		((Control)labelDropLimit).Text = "";
		((Control)labelJumpLimit).Text = "";
		((Control)labelResult).Text = "FAIL";
		((Control)labelResult).BackColor = Color.Red;
	}

	internal bool DrawChart(string id, Dictionary<DateTime, double> timeVsReadings, double normalAirPressure, int measSecsWithObject, int measCycleMillsec, double dropLimit, double jumpLimit, double stableTimeSec, out string dynamicMsg)
	{
		string name = MethodBase.GetCurrentMethod().Name;
		dynamicMsg = string.Empty;
		((Control)noamalAirPressure).Text = normalAirPressure.ToString("f2") + " hPa";
		((Control)labelTotalMeasTime).Text = measSecsWithObject + " s";
		((Control)labelMeasCycle).Text = measCycleMillsec + " ms";
		((Control)labelStableTime).Text = stableTimeSec + " s";
		((Control)labelDropLimit).Text = "<" + dropLimit.ToString("f2") + " hPa";
		((Control)labelJumpLimit).Text = ">" + jumpLimit.ToString("f2") + " hPa";
		((Control)labelTrackId).Text = id;
		double maxPressure = 0.0;
		double finalPressue = 0.0;
		double stablePressure = 0.0;
		double droppedValue = 999.9;
		double jumpedValue = 0.0;
		int maxPressureIndex = 0;
		int finalPressureIndex = 0;
		int stablePressureIndex = 0;
		string errorMsg = string.Empty;
		bool flag = false;
		try
		{
			flag = CheckDataMeetLimit(timeVsReadings, measSecsWithObject, normalAirPressure, dropLimit, jumpLimit, out errorMsg, out maxPressure, out maxPressureIndex, stableTimeSec, out stablePressure, out stablePressureIndex, out finalPressue, out finalPressureIndex, out droppedValue, out jumpedValue);
		}
		catch (Exception ex)
		{
			errorMsg = ex.Message + Environment.NewLine + ex.StackTrace;
			Smart.Log.Info(name, errorMsg);
			return flag;
		}
		dynamicMsg = normalAirPressure.ToString("f2") + "," + maxPressure.ToString("f2") + "," + stablePressure.ToString("f2") + "," + finalPressue.ToString("f2") + "," + dropLimit + "," + jumpLimit + "," + stableTimeSec + "," + measSecsWithObject + "," + measCycleMillsec;
		((Control)maxPressureWithObject).Text = maxPressure.ToString("f2") + " hPa";
		((Control)labelStableVal).Text = stablePressure.ToString("f2") + " hPa";
		((Control)labelFinalVal).Text = finalPressue.ToString("f2") + " hPa";
		((Control)labelDroppedPressure).Text = droppedValue.ToString("f2") + " hPa";
		((Control)labelJumpValue).Text = jumpedValue.ToString("f2") + " hPa";
		if (flag)
		{
			((Control)labelResult).Text = "PASS";
			((Control)labelResult).BackColor = Color.Green;
		}
		else
		{
			((Control)labelResult).Text = "FAIL";
			((Control)labelResult).BackColor = Color.Red;
		}
		Dictionary<int, double> dictionary = new Dictionary<int, double>();
		for (int i = 0; i < timeVsReadings.Count; i++)
		{
			dictionary.Add(i, timeVsReadings.Values.ToList()[i]);
		}
		DrawChart(dictionary, flag, maxPressure, dictionary.Values.ToList().Min(), maxPressureIndex, stablePressureIndex, finalPressureIndex);
		return flag;
	}

	private bool CheckDataMeetLimit(Dictionary<DateTime, double> timeVsReadings, int measureDurationSec, double normalAirPressure, double dropLimit, double jumpLimit, out string errorMsg, out double maxPressure, out int maxPressureIndex, double waitStableSec, out double stablePressure, out int stablePressureIndex, out double finalPressue, out int finalPressureIndex, out double droppedValue, out double jumpedValue)
	{
		string name = MethodBase.GetCurrentMethod().Name;
		errorMsg = string.Empty;
		maxPressure = 0.0;
		stablePressure = 0.0;
		finalPressue = 0.0;
		maxPressure = timeVsReadings.Values.ToList().Max();
		maxPressureIndex = timeVsReadings.Values.ToList().IndexOf(maxPressure);
		DateTime key = timeVsReadings.ElementAt(maxPressureIndex).Key;
		Smart.Log.Info(name, string.Format("Max pressure {0}, measurement time {1}, index {2}.", maxPressure, key.ToString("yyyy-MM-dd-HH:mm:ss:ffff"), maxPressureIndex));
		DateTime stableTimeToCheck = key.AddSeconds(waitStableSec);
		Smart.Log.Info(name, string.Format("Wait for {0}sec to make sure measure stable, stableTimeToCheck {1}.", waitStableSec, stableTimeToCheck.ToString("yyyy-MM-dd-HH:mm:ss:ffff")));
		KeyValuePair<DateTime, double> keyValuePair = default(KeyValuePair<DateTime, double>);
		try
		{
			keyValuePair = timeVsReadings.First((KeyValuePair<DateTime, double> q) => q.Key >= stableTimeToCheck);
		}
		catch (Exception ex)
		{
			errorMsg = "Can't find the stable time point and its air pressure" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
			Smart.Log.Info(name, errorMsg);
			keyValuePair = timeVsReadings.ElementAt(maxPressureIndex);
			Smart.Log.Info(name, "it might be the last value is max value, so no stable value, just give the max value to stable value to check...");
		}
		stableTimeToCheck = keyValuePair.Key;
		stablePressure = keyValuePair.Value;
		stablePressureIndex = timeVsReadings.Keys.ToList().IndexOf(stableTimeToCheck);
		Smart.Log.Info(name, string.Format("Stable air pressure measured at {0} is {1} with index {2}.", stableTimeToCheck.ToString("yyyy-MM-dd-HH:mm:ss:ffff"), stablePressure, stablePressureIndex));
		DateTime finalMeasureTimeToCheck = stableTimeToCheck.AddSeconds(measureDurationSec);
		Smart.Log.Info(name, string.Format("To check the final air pressure value at measurement time {0}.", finalMeasureTimeToCheck.ToString("yyyy-MM-dd-HH:mm:ss:ffff")));
		KeyValuePair<DateTime, double> keyValuePair2 = default(KeyValuePair<DateTime, double>);
		try
		{
			keyValuePair2 = timeVsReadings.First((KeyValuePair<DateTime, double> q) => q.Key >= finalMeasureTimeToCheck);
		}
		catch (Exception ex2)
		{
			errorMsg = (errorMsg = "Can't find the final time point and its air pressure" + Environment.NewLine + ex2.Message + Environment.NewLine + ex2.StackTrace);
			Smart.Log.Info(name, errorMsg);
			keyValuePair2 = keyValuePair;
			Smart.Log.Info(name, "it might be the last value is stable value, so no final value to check, just give the stable value to final value to check...");
		}
		finalMeasureTimeToCheck = keyValuePair2.Key;
		finalPressue = keyValuePair2.Value;
		finalPressureIndex = timeVsReadings.Keys.ToList().IndexOf(finalMeasureTimeToCheck);
		Smart.Log.Info(name, string.Format("Final air pressure measured at {0} is {1} with index {2}.", finalMeasureTimeToCheck.ToString("yyyy-MM-dd-HH:mm:ss:ffff"), finalPressue, finalPressureIndex));
		droppedValue = stablePressure - finalPressue;
		Smart.Log.Info(name, $"Stable pressure is {stablePressure}, final air pressure is {finalPressue}, dropped {droppedValue}.");
		jumpedValue = stablePressure - normalAirPressure;
		Smart.Log.Info(name, $"Stable pressure is {stablePressure}, normal air pressure is {normalAirPressure}, jumped {jumpedValue}.");
		if (droppedValue <= dropLimit && jumpedValue >= jumpLimit)
		{
			Smart.Log.Info(name, $"Water resistance test Pass");
			return true;
		}
		Smart.Log.Info(name, $"Water resistance test Fail.");
		return false;
	}

	private void DrawChart(Dictionary<int, double> data, bool limitPass, double ymax, double ymin, int maxPressure_X, int stablePressure_X, int finalPreassure_X)
	{
		string text = "AirPressure";
		((Collection<Series>)(object)chart1.Series).Clear();
		chart1.Titles.Add("Real-time Air Pressure");
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisX.Title = "Point";
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisX.MajorGrid.Enabled = true;
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisX.MinorGrid.Enabled = true;
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.Title = "Air Pressure(hPa)";
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.IsStartedFromZero = false;
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.MajorGrid.Enabled = true;
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.MinorGrid.Enabled = true;
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.Maximum = ymax + 1.0;
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.Minimum = ymin - 1.0;
		chart1.Series.Add(text);
		((ChartNamedElementCollection<Series>)(object)chart1.Series)[text].ChartType = (SeriesChartType)4;
		((ChartNamedElementCollection<Series>)(object)chart1.Series)[text].Points.DataBindXY((IEnumerable)data.Keys, new IEnumerable[1] { data.Values });
		if (limitPass)
		{
			((DataPointCustomProperties)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text]).Color = Color.Green;
		}
		else
		{
			((DataPointCustomProperties)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text]).Color = Color.Red;
		}
		((DataPointCustomProperties)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text]).BorderWidth = 6;
		((DataPointCustomProperties)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text]).MarkerColor = Color.Blue;
		((DataPointCustomProperties)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text]).MarkerStyle = (MarkerStyle)2;
		((DataPointCustomProperties)((Collection<DataPoint>)(object)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text].Points)[maxPressure_X]).MarkerColor = Color.DarkGoldenrod;
		((DataPointCustomProperties)((Collection<DataPoint>)(object)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text].Points)[maxPressure_X]).MarkerStyle = (MarkerStyle)4;
		((DataPointCustomProperties)((Collection<DataPoint>)(object)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text].Points)[stablePressure_X]).MarkerColor = Color.BlanchedAlmond;
		((DataPointCustomProperties)((Collection<DataPoint>)(object)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text].Points)[stablePressure_X]).MarkerStyle = (MarkerStyle)5;
		((DataPointCustomProperties)((Collection<DataPoint>)(object)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text].Points)[finalPreassure_X]).MarkerColor = Color.Yellow;
		((DataPointCustomProperties)((Collection<DataPoint>)(object)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text].Points)[finalPreassure_X]).MarkerStyle = (MarkerStyle)1;
		((Control)chart1).Refresh();
	}

	public void DrawChart(Dictionary<int, double> data, bool limitPass, double ymax, double ymin)
	{
		string text = "AirPressure";
		((Collection<Series>)(object)chart1.Series).Clear();
		chart1.Titles.Add("Real-time Air Pressure");
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisX.Title = "Point";
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisX.MajorGrid.Enabled = true;
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisX.MinorGrid.Enabled = true;
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.Title = "Air Pressure(hPa)";
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.IsStartedFromZero = false;
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.MajorGrid.Enabled = true;
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.MinorGrid.Enabled = true;
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.Maximum = ymax + 1.0;
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.Minimum = ymin - 1.0;
		chart1.Series.Add(text);
		((ChartNamedElementCollection<Series>)(object)chart1.Series)[text].ChartType = (SeriesChartType)4;
		((ChartNamedElementCollection<Series>)(object)chart1.Series)[text].Points.DataBindXY((IEnumerable)data.Keys, new IEnumerable[1] { data.Values });
		if (limitPass)
		{
			((DataPointCustomProperties)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text]).Color = Color.Green;
		}
		else
		{
			((DataPointCustomProperties)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text]).Color = Color.Red;
		}
		((DataPointCustomProperties)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text]).BorderWidth = 6;
		((DataPointCustomProperties)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text]).MarkerColor = Color.Blue;
		((DataPointCustomProperties)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text]).MarkerStyle = (MarkerStyle)2;
		((DataPointCustomProperties)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text]).Label = "#VALX,#VAL";
		((Control)chart1).Refresh();
	}

	public void DrawChart(Dictionary<int, double> data, bool limitPass)
	{
		string text = "AirPressure";
		((Collection<Series>)(object)chart1.Series).Clear();
		chart1.Titles.Add("Real-time Air Pressure");
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisX.Title = "Time";
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisX.MajorGrid.Enabled = true;
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.Title = "Air Pressure";
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.IsStartedFromZero = false;
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.MajorGrid.Enabled = true;
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.Maximum = 1200.0;
		((Collection<ChartArea>)(object)chart1.ChartAreas)[0].AxisY.Minimum = 950.0;
		chart1.Series.Add(text);
		((ChartNamedElementCollection<Series>)(object)chart1.Series)[text].ChartType = (SeriesChartType)4;
		((ChartNamedElementCollection<Series>)(object)chart1.Series)[text].Points.DataBindXY((IEnumerable)data.Keys, new IEnumerable[1] { data.Values });
		if (limitPass)
		{
			((DataPointCustomProperties)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text]).Color = Color.Green;
		}
		else
		{
			((DataPointCustomProperties)((ChartNamedElementCollection<Series>)(object)chart1.Series)[text]).Color = Color.Red;
		}
		((Control)chart1).Refresh();
	}

	private void chart1_MouseMove(object sender, MouseEventArgs e)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		HitTestResult val = new HitTestResult();
		val = chart1.HitTest(e.X, e.Y);
		if (val.Series != null)
		{
			_ = val.Object;
		}
	}

	private void chart1_GetToolTipText(object sender, ToolTipEventArgs e)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		if ((int)e.HitTestResult.ChartElementType == 16)
		{
			int pointIndex = e.HitTestResult.PointIndex;
			DataPoint val = ((Collection<DataPoint>)(object)e.HitTestResult.Series.Points)[pointIndex];
			e.Text = string.Format("{1:F3}", val.XValue, val.YValues[0]);
		}
	}

	private void Chart_Load(object sender, EventArgs e)
	{
	}

	private void button1_Click(object sender, EventArgs e)
	{
		string name = MethodBase.GetCurrentMethod().Name;
		string text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "IP68TestImg");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string empty = string.Empty;
		empty = ((!(((Control)labelResult).Text == "PASS")) ? (((Control)labelTrackId).Text + DateTime.Now.ToString("_yyyy-MM-dd-HH-mm-ss-ffff") + "_F.png") : (((Control)labelTrackId).Text + DateTime.Now.ToString("_yyyy-MM-dd-HH-mm-ss-ffff") + "_P.png"));
		string text2 = Path.Combine(text, empty);
		Smart.Log.Info(name, string.Format("Save Image file to " + text2));
		CaptureImage(text2);
	}

	private void CaptureImage(string path)
	{
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		string name = MethodBase.GetCurrentMethod().Name;
		try
		{
			Rectangle rectangle = default(Rectangle);
			rectangle = Screen.GetWorkingArea((Control)(object)this);
			Graphics val = ((Control)this).CreateGraphics();
			Bitmap val2 = new Bitmap(rectangle.Width, rectangle.Height, val);
			Graphics obj = Graphics.FromImage((Image)val2);
			IntPtr hdc = val.GetHdc();
			IntPtr hdc2 = obj.GetHdc();
			BitBlt(hdc2, 0, 0, rectangle.Width, rectangle.Height, hdc, 0, 0, 13369376);
			val.ReleaseHdc(hdc);
			obj.ReleaseHdc(hdc2);
			((Image)val2).Save(path, ImageFormat.Png);
			MessageBox.Show("Save image ok to " + path);
		}
		catch (Exception ex)
		{
			string text = "Save image fail to " + path + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
			Smart.Log.Info(name, text);
			MessageBox.Show(text);
		}
	}

	[DllImport("gdi32.dll")]
	private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected O, but got Unknown
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Expected O, but got Unknown
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Expected O, but got Unknown
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Expected O, but got Unknown
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Expected O, but got Unknown
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Expected O, but got Unknown
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Expected O, but got Unknown
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Expected O, but got Unknown
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Expected O, but got Unknown
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Expected O, but got Unknown
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Expected O, but got Unknown
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_074d: Expected O, but got Unknown
		//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d5: Expected O, but got Unknown
		//IL_0856: Unknown result type (might be due to invalid IL or missing references)
		//IL_0860: Expected O, but got Unknown
		//IL_08db: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Expected O, but got Unknown
		//IL_0960: Unknown result type (might be due to invalid IL or missing references)
		//IL_096a: Expected O, but got Unknown
		//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f5: Expected O, but got Unknown
		//IL_0a76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a80: Expected O, but got Unknown
		//IL_0afe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b08: Expected O, but got Unknown
		//IL_0b86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b90: Expected O, but got Unknown
		//IL_0c11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1b: Expected O, but got Unknown
		//IL_0c99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca3: Expected O, but got Unknown
		//IL_0d24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2e: Expected O, but got Unknown
		//IL_0dac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db6: Expected O, but got Unknown
		//IL_0e37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e41: Expected O, but got Unknown
		//IL_0ec2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ecc: Expected O, but got Unknown
		//IL_0f4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f57: Expected O, but got Unknown
		//IL_0fd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fdf: Expected O, but got Unknown
		//IL_12e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f2: Expected O, but got Unknown
		ChartArea val = new ChartArea();
		Legend val2 = new Legend();
		Series val3 = new Series();
		chart1 = new Chart();
		label1 = new Label();
		noamalAirPressure = new Label();
		maxPressureWithObject = new Label();
		label3 = new Label();
		labelStableVal = new Label();
		labelResult = new Label();
		label5 = new Label();
		labelTotalMeasTime = new Label();
		label7 = new Label();
		labelMeasCycle = new Label();
		label9 = new Label();
		label2 = new Label();
		labelTrackId = new Label();
		labelDroppedPressure = new Label();
		label6 = new Label();
		labelDropLimit = new Label();
		labelStableTime = new Label();
		label12 = new Label();
		labelJumpValue = new Label();
		label14 = new Label();
		labelJumpLimit = new Label();
		labelFinalVal = new Label();
		label8 = new Label();
		label11 = new Label();
		label4 = new Label();
		label10 = new Label();
		button1 = new Button();
		((ISupportInitialize)chart1).BeginInit();
		((Control)this).SuspendLayout();
		((ChartNamedElement)val).Name = "ChartArea1";
		((Collection<ChartArea>)(object)chart1.ChartAreas).Add(val);
		val2.Enabled = false;
		((ChartNamedElement)val2).Name = "Legend1";
		((Collection<Legend>)(object)chart1.Legends).Add(val2);
		((Control)chart1).Location = new Point(13, 12);
		((Control)chart1).Name = "chart1";
		val3.ChartArea = "ChartArea1";
		val3.Legend = "Legend1";
		((ChartNamedElement)val3).Name = "Series1";
		((Collection<Series>)(object)chart1.Series).Add(val3);
		chart1.Size = new Size(1934, 1101);
		((Control)chart1).TabIndex = 0;
		((Control)chart1).Text = "chart1";
		chart1.GetToolTipText += chart1_GetToolTipText;
		((Control)chart1).MouseMove += new MouseEventHandler(chart1_MouseMove);
		((Control)label1).AutoSize = true;
		((Control)label1).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label1).Location = new Point(1970, 368);
		((Control)label1).Name = "label1";
		((Control)label1).Size = new Size(397, 32);
		((Control)label1).TabIndex = 1;
		((Control)label1).Text = "Normal Condition Air Pressure";
		((Control)noamalAirPressure).AutoSize = true;
		((Control)noamalAirPressure).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)noamalAirPressure).Location = new Point(1970, 418);
		((Control)noamalAirPressure).Name = "noamalAirPressure";
		((Control)noamalAirPressure).Size = new Size(114, 32);
		((Control)noamalAirPressure).TabIndex = 2;
		((Control)noamalAirPressure).Text = "995hPa";
		((Control)maxPressureWithObject).AutoSize = true;
		((Control)maxPressureWithObject).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)maxPressureWithObject).Location = new Point(1970, 516);
		((Control)maxPressureWithObject).Name = "maxPressureWithObject";
		((Control)maxPressureWithObject).Size = new Size(130, 32);
		((Control)maxPressureWithObject).TabIndex = 4;
		((Control)maxPressureWithObject).Text = "1006hPa";
		((Control)label3).AutoSize = true;
		((Control)label3).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label3).Location = new Point(1970, 470);
		((Control)label3).Name = "label3";
		((Control)label3).Size = new Size(378, 32);
		((Control)label3).TabIndex = 3;
		((Control)label3).Text = "Max Air Pressure with Object";
		((Control)labelStableVal).AutoSize = true;
		((Control)labelStableVal).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)labelStableVal).Location = new Point(1965, 621);
		((Control)labelStableVal).Name = "labelStableVal";
		((Control)labelStableVal).Size = new Size(130, 32);
		((Control)labelStableVal).TabIndex = 6;
		((Control)labelStableVal).Text = "1000hPa";
		((Control)labelResult).AutoSize = true;
		((Control)labelResult).Font = new Font("Microsoft Sans Serif", 10f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)labelResult).Location = new Point(2214, 986);
		((Control)labelResult).Name = "labelResult";
		((Control)labelResult).Size = new Size(93, 39);
		((Control)labelResult).TabIndex = 8;
		((Control)labelResult).Text = "Pass";
		((Control)label5).AutoSize = true;
		((Control)label5).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label5).Location = new Point(1976, 991);
		((Control)label5).Name = "label5";
		((Control)label5).Size = new Size(158, 32);
		((Control)label5).TabIndex = 7;
		((Control)label5).Text = "Test Result";
		((Control)labelTotalMeasTime).AutoSize = true;
		((Control)labelTotalMeasTime).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)labelTotalMeasTime).Location = new Point(1965, 130);
		((Control)labelTotalMeasTime).Name = "labelTotalMeasTime";
		((Control)labelTotalMeasTime).Size = new Size(61, 32);
		((Control)labelTotalMeasTime).TabIndex = 10;
		((Control)labelTotalMeasTime).Text = "10s";
		((Control)label7).AutoSize = true;
		((Control)label7).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label7).Location = new Point(1965, 83);
		((Control)label7).Name = "label7";
		((Control)label7).Size = new Size(406, 32);
		((Control)label7).TabIndex = 9;
		((Control)label7).Text = "Measurement Time with Object";
		((Control)labelMeasCycle).AutoSize = true;
		((Control)labelMeasCycle).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)labelMeasCycle).Location = new Point(1976, 225);
		((Control)labelMeasCycle).Name = "labelMeasCycle";
		((Control)labelMeasCycle).Size = new Size(69, 32);
		((Control)labelMeasCycle).TabIndex = 12;
		((Control)labelMeasCycle).Text = "0.5s";
		((Control)label9).AutoSize = true;
		((Control)label9).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label9).Location = new Point(1965, 176);
		((Control)label9).Name = "label9";
		((Control)label9).Size = new Size(266, 32);
		((Control)label9).TabIndex = 11;
		((Control)label9).Text = "Measurement Cycle";
		((Control)label2).AutoSize = true;
		((Control)label2).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label2).Location = new Point(1965, 31);
		((Control)label2).Name = "label2";
		((Control)label2).Size = new Size(120, 32);
		((Control)label2).TabIndex = 14;
		((Control)label2).Text = "TrackID:";
		((Control)labelTrackId).AutoSize = true;
		((Control)labelTrackId).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)labelTrackId).Location = new Point(2119, 30);
		((Control)labelTrackId).Name = "labelTrackId";
		((Control)labelTrackId).Size = new Size(112, 32);
		((Control)labelTrackId).TabIndex = 16;
		((Control)labelTrackId).Text = "TrackID";
		((Control)labelDroppedPressure).AutoSize = true;
		((Control)labelDroppedPressure).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)labelDroppedPressure).Location = new Point(1972, 809);
		((Control)labelDroppedPressure).Name = "labelDroppedPressure";
		((Control)labelDroppedPressure).Size = new Size(130, 32);
		((Control)labelDroppedPressure).TabIndex = 18;
		((Control)labelDroppedPressure).Text = "1000hPa";
		((Control)label6).AutoSize = true;
		((Control)label6).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label6).Location = new Point(1972, 758);
		((Control)label6).Name = "label6";
		((Control)label6).Size = new Size(205, 32);
		((Control)label6).TabIndex = 17;
		((Control)label6).Text = "Dropped Value";
		((Control)labelDropLimit).AutoSize = true;
		((Control)labelDropLimit).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)labelDropLimit).Location = new Point(2215, 809);
		((Control)labelDropLimit).Name = "labelDropLimit";
		((Control)labelDropLimit).Size = new Size(122, 32);
		((Control)labelDropLimit).TabIndex = 20;
		((Control)labelDropLimit).Text = "<0.5hPa";
		((Control)labelStableTime).AutoSize = true;
		((Control)labelStableTime).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)labelStableTime).Location = new Point(1976, 323);
		((Control)labelStableTime).Name = "labelStableTime";
		((Control)labelStableTime).Size = new Size(45, 32);
		((Control)labelStableTime).TabIndex = 24;
		((Control)labelStableTime).Text = "2s";
		((Control)label12).AutoSize = true;
		((Control)label12).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label12).Location = new Point(1965, 278);
		((Control)label12).Name = "label12";
		((Control)label12).Size = new Size(167, 32);
		((Control)label12).TabIndex = 23;
		((Control)label12).Text = "Stable Time";
		((Control)labelJumpValue).AutoSize = true;
		((Control)labelJumpValue).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)labelJumpValue).Location = new Point(1976, 919);
		((Control)labelJumpValue).Name = "labelJumpValue";
		((Control)labelJumpValue).Size = new Size(82, 32);
		((Control)labelJumpValue).TabIndex = 26;
		((Control)labelJumpValue).Text = "6hPa";
		((Control)label14).AutoSize = true;
		((Control)label14).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label14).Location = new Point(1976, 868);
		((Control)label14).Name = "label14";
		((Control)label14).Size = new Size(165, 32);
		((Control)label14).TabIndex = 25;
		((Control)label14).Text = "Jump Value";
		((Control)labelJumpLimit).AutoSize = true;
		((Control)labelJumpLimit).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)labelJumpLimit).Location = new Point(2215, 919);
		((Control)labelJumpLimit).Name = "labelJumpLimit";
		((Control)labelJumpLimit).Size = new Size(122, 32);
		((Control)labelJumpLimit).TabIndex = 27;
		((Control)labelJumpLimit).Text = ">2.5hPa";
		((Control)labelFinalVal).AutoSize = true;
		((Control)labelFinalVal).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)labelFinalVal).Location = new Point(1968, 718);
		((Control)labelFinalVal).Name = "labelFinalVal";
		((Control)labelFinalVal).Size = new Size(130, 32);
		((Control)labelFinalVal).TabIndex = 30;
		((Control)labelFinalVal).Text = "1000hPa";
		((Control)label8).AutoSize = true;
		((Control)label8).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label8).Location = new Point(1972, 669);
		((Control)label8).Name = "label8";
		((Control)label8).Size = new Size(240, 32);
		((Control)label8).TabIndex = 29;
		((Control)label8).Text = "Final Air Pressure";
		((Control)label11).AutoSize = true;
		((Control)label11).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label11).Location = new Point(1970, 571);
		((Control)label11).Name = "label11";
		((Control)label11).Size = new Size(259, 32);
		((Control)label11).TabIndex = 31;
		((Control)label11).Text = "Stable Air Pressure";
		((Control)label4).AutoSize = true;
		((Control)label4).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label4).Location = new Point(2215, 758);
		((Control)label4).Name = "label4";
		((Control)label4).Size = new Size(76, 32);
		((Control)label4).TabIndex = 32;
		((Control)label4).Text = "Limit";
		((Control)label10).AutoSize = true;
		((Control)label10).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)0, (GraphicsUnit)3, (byte)134);
		((Control)label10).Location = new Point(2215, 868);
		((Control)label10).Name = "label10";
		((Control)label10).Size = new Size(76, 32);
		((Control)label10).TabIndex = 33;
		((Control)label10).Text = "Limit";
		((Control)button1).Location = new Point(2064, 1045);
		((Control)button1).Name = "button1";
		((Control)button1).Size = new Size(216, 58);
		((Control)button1).TabIndex = 34;
		((Control)button1).Text = "Save Image";
		((ButtonBase)button1).UseVisualStyleBackColor = true;
		((Control)button1).Click += button1_Click;
		((ContainerControl)this).AutoScaleDimensions = new SizeF(17f, 31f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((ScrollableControl)this).AutoScroll = true;
		((Control)this).AutoSize = true;
		((Form)this).ClientSize = new Size(2412, 1125);
		((Control)this).Controls.Add((Control)(object)button1);
		((Control)this).Controls.Add((Control)(object)label10);
		((Control)this).Controls.Add((Control)(object)label4);
		((Control)this).Controls.Add((Control)(object)label11);
		((Control)this).Controls.Add((Control)(object)labelFinalVal);
		((Control)this).Controls.Add((Control)(object)label8);
		((Control)this).Controls.Add((Control)(object)labelJumpLimit);
		((Control)this).Controls.Add((Control)(object)labelJumpValue);
		((Control)this).Controls.Add((Control)(object)label14);
		((Control)this).Controls.Add((Control)(object)labelStableTime);
		((Control)this).Controls.Add((Control)(object)label12);
		((Control)this).Controls.Add((Control)(object)labelDropLimit);
		((Control)this).Controls.Add((Control)(object)labelDroppedPressure);
		((Control)this).Controls.Add((Control)(object)label6);
		((Control)this).Controls.Add((Control)(object)labelTrackId);
		((Control)this).Controls.Add((Control)(object)label2);
		((Control)this).Controls.Add((Control)(object)labelMeasCycle);
		((Control)this).Controls.Add((Control)(object)label9);
		((Control)this).Controls.Add((Control)(object)labelTotalMeasTime);
		((Control)this).Controls.Add((Control)(object)label7);
		((Control)this).Controls.Add((Control)(object)labelResult);
		((Control)this).Controls.Add((Control)(object)label5);
		((Control)this).Controls.Add((Control)(object)labelStableVal);
		((Control)this).Controls.Add((Control)(object)maxPressureWithObject);
		((Control)this).Controls.Add((Control)(object)label3);
		((Control)this).Controls.Add((Control)(object)noamalAirPressure);
		((Control)this).Controls.Add((Control)(object)label1);
		((Control)this).Controls.Add((Control)(object)chart1);
		((Control)this).Font = new Font("Microsoft Sans Serif", 8.1f, (FontStyle)1, (GraphicsUnit)3, (byte)134);
		((Control)this).Name = "Chart";
		((Form)this).StartPosition = (FormStartPosition)1;
		((Control)this).Text = "Chart";
		((Form)this).Load += Chart_Load;
		((ISupportInitialize)chart1).EndInit();
		((Control)this).ResumeLayout(false);
		((Control)this).PerformLayout();
	}
}
