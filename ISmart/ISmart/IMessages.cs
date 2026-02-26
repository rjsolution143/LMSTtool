using System;

namespace ISmart;

public interface IMessages : IDisposable
{
	void CreateChannel(string channelName, Func<string, string> handler);

	bool IsChannelCreated(string channelName);

	bool IsChannelOpen(string channelName);

	string SendMessage(string channelName, string message);
}
