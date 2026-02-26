using ISmart;

namespace SmartDevice.Steps;

public class FtmConnect : ClientConnect
{
	private string TAG => GetType().FullName;

	protected override string ClientName => "ftm";

	protected override INetworkClient Client()
	{
		return (INetworkClient)(object)Smart.NewFtmClient();
	}
}
