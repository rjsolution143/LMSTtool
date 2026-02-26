using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ISmart;

namespace SmartHelper;

public class Messages : IMessages, IDisposable
{
	private SortedList<string, MessageServer> servers = new SortedList<string, MessageServer>();

	private bool disposedValue;

	private string TAG => GetType().FullName;

	public Messages()
	{
		Smart.App.ScheduleShutdownTask("Close Messages pipes", (Action)Dispose);
	}

	public void CreateChannel(string channelName, Func<string, string> handler)
	{
		Smart.Log.Debug(TAG, $"Creating Messages channel {channelName}");
		MessageServer value = new MessageServer(channelName, handler);
		servers[channelName] = value;
	}

	public bool IsChannelCreated(string channelName)
	{
		return servers.ContainsKey(channelName);
	}

	public bool IsChannelOpen(string channelName)
	{
		using Mutex mutex = new Mutex(initiallyOwned: false, channelName);
		bool num = mutex.WaitOne(0);
		if (num)
		{
			mutex.ReleaseMutex();
		}
		return !num;
	}

	public string SendMessage(string channelName, string message)
	{
		if (message == string.Empty)
		{
			Smart.Log.Warning(TAG, "Cannot send empty Message");
			return string.Empty;
		}
		TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();
		using MessageClient messageClient = new MessageClient(channelName, taskCompletionSource.SetResult);
		messageClient.SendMessage(message);
		taskCompletionSource.Task.Wait();
		return taskCompletionSource.Task.Result;
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposedValue)
		{
			return;
		}
		if (disposing)
		{
			Smart.Log.Debug(TAG, "Closing Messages servers");
			foreach (MessageServer value in servers.Values)
			{
				value.Dispose();
			}
			servers.Clear();
		}
		disposedValue = true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
