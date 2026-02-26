namespace SmartDevice.Steps;

public class CommandLine
{
	public string CmdString { get; private set; }

	public int TimeOut { get; private set; }

	public long DataSize { get; private set; }

	public CommandLine(string cmdString, int timeOut, long dataSize)
	{
		CmdString = cmdString;
		TimeOut = timeOut;
		DataSize = dataSize;
	}
}
