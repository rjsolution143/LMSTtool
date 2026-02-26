using ISmart;

namespace SmartDevice.Steps;

public class CommServerConnect : ClientConnect
{
	private string TAG => GetType().FullName;

	protected override string ClientName => "commServer";

	protected override INetworkClient Client()
	{
		return (INetworkClient)(object)Smart.NewCommServerClient();
	}
}
