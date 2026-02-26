namespace SmartDevice;

public enum FixtureCommand
{
	PressKey = 1,
	ReleaseKey,
	ReportPass,
	ReportFail,
	ReportError,
	ReportUnderDetection,
	ReportUnderTesting,
	ReportUsbLostConnection
}
