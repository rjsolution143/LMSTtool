namespace SmartDevice.Steps;

public class TestCommandDisconnect : ClientDisconnect
{
	private string TAG => GetType().FullName;

	protected override string ClientName => "tcmd";
}
