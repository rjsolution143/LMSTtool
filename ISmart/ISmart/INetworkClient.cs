using System;

namespace ISmart;

public interface INetworkClient : IDisposable
{
	string Host { get; }

	int Port { get; }

	void SetEndPoint(string host, int port);

	void SetEndPoint(string host);

	void Connect();

	bool IsConnected();
}
