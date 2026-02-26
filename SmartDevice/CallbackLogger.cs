using System;
using System.Collections.Generic;
using ISmart;

namespace SmartDevice;

public class CallbackLogger : IResultSubLogger, IDisposable
{
	public delegate void ResultCallback(IDevice device, string name, SortedList<string, dynamic> details);

	public delegate void InfoCallback(IDevice device, string name, string value);

	private IDevice device;

	private ResultCallback result;

	private InfoCallback info;

	private string TAG => GetType().FullName;

	public string Name => "CallbackLogger";

	public bool IsOpen => true;

	public UseCase UseCase { get; set; }

	public CallbackLogger(IDevice device, ResultCallback result = null, InfoCallback info = null)
	{
		this.device = device;
		this.result = NullResult;
		this.info = NullInfo;
		if (result != null)
		{
			this.result = result;
		}
		if (info != null)
		{
			this.info = info;
		}
	}

	private void NullResult(IDevice device, string name, SortedList<string, dynamic> details)
	{
	}

	private void NullInfo(IDevice device, string name, string value)
	{
	}

	public void AddResult(string name, SortedList<string, dynamic> details)
	{
		result(device, name, details);
	}

	public void AddInfo(string name, string value)
	{
		info(device, name, value);
	}

	public void Dispose()
	{
		result = NullResult;
		info = NullInfo;
	}
}
