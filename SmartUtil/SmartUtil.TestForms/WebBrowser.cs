using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DotNetBrowser;
using DotNetBrowser.WinForms;

namespace SmartUtil.TestForms;

public class WebBrowser : Form
{
	private class HeaderSpy : DefaultNetworkDelegate
	{
		private WebBrowser browser;

		public HeaderSpy(WebBrowser browser)
		{
			this.browser = browser;
		}

		public override void OnHeadersReceived(HeadersReceivedParams parameters)
		{
			((DefaultNetworkDelegate)this).OnHeadersReceived(parameters);
			SortedList<string, string> sortedList = new SortedList<string, string>(parameters.Headers.GetHeaders());
			foreach (string key in sortedList.Keys)
			{
				browser.headers[key] = sortedList[key];
			}
		}
	}

	public BrowserView browser;

	public Dictionary<string, string> headers = new Dictionary<string, string>();

	public TimeSpan timeLimit = TimeSpan.FromSeconds(120.0);

	private Timer closeTimer;

	private IContainer components;

	private FolderBrowserDialog folderBrowserDialog1;

	public WebBrowser()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		InitializeComponent();
		BrowserContext defaultContext = BrowserContext.DefaultContext;
		defaultContext.NetworkService.NetworkDelegate = (NetworkDelegate)(object)new HeaderSpy(this);
		Browser val = BrowserFactory.Create(defaultContext);
		browser = (BrowserView)new WinFormsBrowserView(val);
		((Control)this).Controls.Add((Control)browser);
		((Control)browser).Dock = (DockStyle)5;
		closeTimer = new Timer();
		closeTimer.Tick += delegate
		{
			((Form)this).Close();
		};
		closeTimer.Interval = (int)timeLimit.TotalMilliseconds;
		closeTimer.Start();
	}

	public static SortedList<string, string> Browse(string url, string postData, SortedList<string, string> headers)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		WebBrowser webBrowser = new WebBrowser();
		try
		{
			string languageCode = Smart.Locale.LanguageCode;
			webBrowser.browser.Browser.Context.AcceptLanguage = languageCode;
			if (postData != null && postData.Trim().Length > 0)
			{
				Smart.Convert.AsciiToBytes(postData);
			}
			string text = string.Empty;
			foreach (string key in headers.Keys)
			{
				text += $"{key}: {headers[key]}\r\n";
			}
			LoadURLParams val = new LoadURLParams(url, postData, text);
			webBrowser.browser.Browser.LoadURL(val);
			((Form)webBrowser).ShowDialog();
			return new SortedList<string, string>(webBrowser.headers);
		}
		finally
		{
			((IDisposable)webBrowser)?.Dispose();
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
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(WebBrowser));
		folderBrowserDialog1 = new FolderBrowserDialog();
		((Control)this).SuspendLayout();
		((ContainerControl)this).AutoScaleDimensions = new SizeF(6f, 13f);
		((ContainerControl)this).AutoScaleMode = (AutoScaleMode)1;
		((Form)this).ClientSize = new Size(774, 440);
		((Form)this).Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
		((Control)this).Name = "WebBrowser";
		((Control)this).Text = "WebBrowser";
		((Control)this).ResumeLayout(false);
	}
}
