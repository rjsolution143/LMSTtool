using System;
using System.Collections.Generic;

namespace SmartDevice.Steps;

public abstract class ExternalRequestStep : BaseStep
{
	protected enum ExternalResponseType
	{
		Error = -1,
		Data
	}

	protected static List<Func<string, string>> handlers = new List<Func<string, string>>();

	protected static object requestLock = new object();

	protected static bool multiRequestActive = false;

	private string TAG => GetType().FullName;

	public override void Run()
	{
		string text = "ExternalRequest";
		if (!Smart.Messages.IsChannelCreated(text))
		{
			Smart.Log.Debug(TAG, "Creating External Request channel");
			Smart.Messages.CreateChannel(text, (Func<string, string>)RequestHandler);
		}
		bool flag = false;
		if (((dynamic)base.Info.Args).MultiRequest != null)
		{
			flag = ((dynamic)base.Info.Args).MultiRequest;
		}
		if (!flag)
		{
			Smart.Log.Debug(TAG, "Clearing External Request handlers");
			lock (requestLock)
			{
				handlers.Clear();
				return;
			}
		}
		multiRequestActive = true;
	}

	protected static string SendResponse(ExternalResponseType type, string response)
	{
		return $"{type.ToString()}:{response}";
	}

	private static string RequestHandler(string request)
	{
		lock (requestLock)
		{
			if (handlers.Count < 1)
			{
				Smart.Log.Error("Smart.ExternalRequest.RequestHandler", "No request handler registered, sending generic error response");
				return SendResponse(ExternalResponseType.Error, "Unexpected external request, no handler available");
			}
			Func<string, string> func = handlers[0];
			if (multiRequestActive)
			{
				handlers.RemoveAt(0);
			}
			Smart.Log.Debug("Smart.ExternalRequest.RequestHandler", "Processing external request with registered handler");
			return func(request);
		}
	}
}
