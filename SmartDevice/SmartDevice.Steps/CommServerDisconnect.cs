namespace SmartDevice.Steps;

public class CommServerDisconnect : ClientDisconnect
{
	private string TAG => GetType().FullName;

	protected override string ClientName => "commServer";
}
