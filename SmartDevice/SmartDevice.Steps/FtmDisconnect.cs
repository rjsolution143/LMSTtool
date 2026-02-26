namespace SmartDevice.Steps;

public class FtmDisconnect : ClientDisconnect
{
	private string TAG => GetType().FullName;

	protected override string ClientName => "ftm";
}
