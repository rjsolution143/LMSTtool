using System;

namespace ISmart;

public interface IFtmClient : INetworkClient, IDisposable
{
	IFtmResponse SendCommand(string command);

	IFtmResponse SendCommand(string command, bool unsolicited, bool crcCheckRequired = true);
}
