namespace SmartBase;

public class Launch
{
	private string TAG => GetType().FullName;

	public static void StartApp(string appName)
	{
		Base.instance.Load();
		Smart.StartApp(appName).Run();
	}
}
