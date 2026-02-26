using System;
using System.IO;
using ISmart;
using SmartBase;

internal class Smart
{
	private static IBase bases = (IBase)(object)Base.instance;

	private string TAG => GetType().FullName;

	public static IApp App => bases.LoadCached<IApp>();

	public static IADB ADB => bases.Load<IADB>("SmartDevice.ADB");

	public static ILibUsbDotNetDeviceFinder LibUsbDotNetFastbootDeviceFinder => bases.Load<ILibUsbDotNetDeviceFinder>("SmartDevice.LibUsbDotNetFastbootDeviceFinder");

	public static IPnpDeviceFinder PnpDeviceFinder => bases.Load<IPnpDeviceFinder>("SmartDevice.PnpDeviceFinder");

	public static IDeviceManager DeviceManager => bases.Load<IDeviceManager>("SmartDevice.DeviceManager");

	public static IUseCaseRunner UseCaseRunner => bases.Load<IUseCaseRunner>("SmartDevice.UseCaseRunner");

	public static IValidator Validator => bases.Load<IValidator>("SmartDevice.Validator");

	public static IConvert Convert => bases.Load<IConvert>("SmartUtil.Convert");

	public static IFile File => bases.Load<IFile>("SmartUtil.File");

	public static IGraphics Graphics => bases.Load<IGraphics>("SmartUtil.Graphics");

	public static IZip Zip => bases.Load<IZip>("SmartUtil.Zip");

	public static IJson Json => bases.Load<IJson>("SmartUtil.Json");

	public static IMessages Messages => bases.Load<IMessages>("SmartUtil.Messages");

	public static IPortChecker PortChecker => bases.Load<IPortChecker>("SmartUtil.PortChecker");

	public static ILocale Locale => bases.Load<ILocale>("SmartUtil.Locale");

	public static ILog Log => bases.Load<ILog>("SmartUtil.Log");

	public static INet Net => bases.Load<INet>("SmartUtil.Net");

	public static IThread Thread => bases.Load<IThread>("SmartUtil.Thread");

	public static ISecurity Security => bases.Load<ISecurity>("SmartUtil.Security");

	public static IUser User => bases.Load<IUser>("SmartTool.User");

	public static IAutoKillSwitch AutoKillSwitch => bases.Load<IAutoKillSwitch>("SmartTool.AutoKillSwitchViewModel");

	public static IUserAccounts UserAccounts => bases.Load<IUserAccounts>("SmartUtil.UserAccounts");

	public static IAlerts Alerts => bases.Load<IAlerts>("SmartUtil.Alerts");

	public static IWeb Web => bases.Load<IWeb>("SmartWeb.Web");

	public static IDevTest DevTest => bases.Load<IDevTest>("SmartDevice.DeviceManager");

	public static IRsd Rsd => bases.Load<IRsd>("SmartRsd.Rsd");

	public static IMotoAndroid MotoAndroid => bases.Load<IMotoAndroid>("SmartDevice.MotoAndroid");

	public static IFsb Fsb => bases.Load<IFsb>("SmartRsd.Fsb");

	public static ITestUI TestUI => bases.Load<ITestUI>("SmartUtil.TestUI");

	public static IHotKey HotKey => bases.Load<IHotKey>("SmartUtil.HotKey");

	public static IDownloadEngine DownloadEngine => bases.Load<IDownloadEngine>("SmartUtil.DownloadEngine");

	public static IMaintenance Maintenance => bases.Load<IMaintenance>("SmartUtil.Maintenance");

	public static ISession Session => bases.Load<ISession>("SmartUtil.Session");

	public static IPrintLabel PrintLabel => bases.Load<IPrintLabel>("SmartPrint.PrintLabel");

	public static ITroubleshooting Troubleshooting => bases.Load<ITroubleshooting>("SmartUtil.Troubleshooting");

	public static IRefurbInfo RefurbInfo => bases.Load<IRefurbInfo>("SmartUtil.RefurbInfo");

	public static IAutomation Automation => bases.Load<IAutomation>("SmartDevice.Automation");

	public static IPortAssignment PortAssignment => bases.Load<IPortAssignment>("SmartDevice.TempPortAssignment");

	public static IUsbPorts UsbPorts => bases.Load<IUsbPorts>("SmartDevice.UsbPorts");

	public static IDeviceListener DeviceListener => bases.Load<IDeviceListener>("SmartDevice.DeviceListener");

	public static IApp StartApp(string appName)
	{
		try
		{
			string text = string.Format("{0}.{1}", appName, "App");
			File.StorageDirName = appName;
			File.LogName = appName;
			if (appName.ToLowerInvariant() == "smarttool".ToLowerInvariant())
			{
				File.LogName = "lmst";
			}
			return bases.Load<IApp>(text);
		}
		catch (TypeInitializationException ex)
		{
			_ = ex.InnerException;
			System.IO.File.AppendAllLines(string.Format("CriticalErrorOnLaunch_{0}.txt", DateTime.Now.ToString("yyyyMM")), new string[1] { DateTime.Now.ToString("yyyyMMdd:HHmmss ") + "StartApp:" + ex.Message + Environment.NewLine + ex.StackTrace });
			throw;
		}
	}

	public static ICommServerClient NewCommServerClient()
	{
		return bases.LoadNew<ICommServerClient>("SmartUtil.CommServerClient");
	}

	public static ITestCommandClient NewTestCommandClient()
	{
		return bases.LoadNew<ITestCommandClient>("SmartUtil.TestCommandClient");
	}

	public static IFtmClient NewFtmClient()
	{
		return bases.LoadNew<IFtmClient>("SmartUtil.FtmClient");
	}

	public static IResultLogger NewResultLogger()
	{
		return bases.LoadNew<IResultLogger>("SmartUtil.ResultLogger");
	}

	public static IRecipeInfo NewRecipeInfo()
	{
		return bases.LoadNew<IRecipeInfo>("SmartDevice.RecipeInfo");
	}

	public static IStepInfo NewStepInfo()
	{
		return bases.LoadNew<IStepInfo>("SmartDevice.StepInfo");
	}

	public static IRecipe NewRecipe()
	{
		return bases.LoadNew<IRecipe>("SmartDevice.Recipe");
	}

	public static IStep NewStep(string stepName)
	{
		return bases.LoadNew<IStep>($"*.{stepName}");
	}

	public static ICsvFile NewCsvFile()
	{
		return bases.LoadNew<ICsvFile>("SmartUtil.CsvFile");
	}

	public static ITimedCache NewTimedCache()
	{
		return bases.LoadNew<ITimedCache>("SmartUtil.TimedCache");
	}

	public static IPrompt NewPrompt()
	{
		return bases.LoadNew<IPrompt>("SmartTool.Prompt");
	}

	public static IDownloadRequest NewDownloadRequest()
	{
		return bases.LoadNew<IDownloadRequest>("SmartUtil.DownloadRequest");
	}

	public static void PrintBase()
	{
		bases.PrintBase();
	}
}
