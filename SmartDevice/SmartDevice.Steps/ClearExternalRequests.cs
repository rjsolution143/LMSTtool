using System.Collections.Generic;

namespace SmartDevice.Steps;

public class ClearExternalRequests : ExternalRequestStep
{
	private SortedList<string, string> parameters = new SortedList<string, string>();

	private string TAG => GetType().FullName;

	public override void Run()
	{
		Smart.Log.Debug(TAG, "Clearing external request handlers");
		lock (ExternalRequestStep.requestLock)
		{
			ExternalRequestStep.handlers.Clear();
		}
		LogPass();
	}
}
