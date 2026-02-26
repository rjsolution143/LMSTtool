using ISmart;

namespace SmartDevice.Steps;

public class TestCommandConnect : ClientConnect
{
	private string TAG => GetType().FullName;

	protected override string ClientName => "tcmd";

	protected override INetworkClient Client()
	{
		return (INetworkClient)(object)Smart.NewTestCommandClient();
	}
}
