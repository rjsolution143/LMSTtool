using System;
using System.Collections.Generic;

namespace ISmart;

public interface ICommServerClient : INetworkClient, IDisposable
{
	TimeSpan Timeout { get; set; }

	SortedList<string, string> SendCommand(string command);

	SortedList<string, string> SendCommand(string command, string data);
}
