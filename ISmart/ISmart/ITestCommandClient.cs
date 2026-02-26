using System;

namespace ISmart;

public interface ITestCommandClient : INetworkClient, IDisposable
{
	TimeSpan Timeout { get; set; }

	ITestCommandResponse SendCommand(string command);

	ITestCommandResponse SendCommand(string command, string data);
}
