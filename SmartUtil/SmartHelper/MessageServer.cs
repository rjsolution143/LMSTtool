using System;
using System.IO.Pipes;
using System.Threading;

namespace SmartHelper;

public class MessageServer : IDisposable
{
	private string TAG => GetType().FullName;

	public string ChannelName { get; private set; }

	public Func<string, string> Handler { get; set; }

	public NamedPipeServerStream Pipe { get; private set; }

	public Mutex Mutex { get; private set; }

	public MessageServer(string channelName, Func<string, string> handler)
	{
		Mutex = new Mutex(initiallyOwned: false, channelName);
		if (!Mutex.WaitOne(0))
		{
			Mutex = null;
			throw new NotSupportedException($"Could not open channel {channelName}, name already in use");
		}
		ChannelName = channelName;
		Handler = handler;
		WaitForConnection();
	}

	private void WaitForConnection()
	{
		NamedPipeServerStream namedPipeServerStream2 = (Pipe = new NamedPipeServerStream($"Moto.Messages.{ChannelName}", PipeDirection.InOut, -1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous));
		namedPipeServerStream2.BeginWaitForConnection(HandleConnect, namedPipeServerStream2);
	}

	private void ReadRequest(NamedPipeServerStream pipe)
	{
		Message message = new Message(pipe);
		pipe.BeginRead(message.Buffer, 0, message.Buffer.Length, HandleRequest, message);
	}

	private void HandleConnect(IAsyncResult dataStateObject)
	{
		NamedPipeServerStream namedPipeServerStream = (NamedPipeServerStream)dataStateObject.AsyncState;
		namedPipeServerStream.EndWaitForConnection(dataStateObject);
		WaitForConnection();
		ReadRequest(namedPipeServerStream);
	}

	private void HandleRequest(IAsyncResult dataStateObject)
	{
		Message message = (Message)dataStateObject.AsyncState;
		NamedPipeServerStream namedPipeServerStream = (NamedPipeServerStream)message.Stream;
		int length = namedPipeServerStream.EndRead(dataStateObject);
		message.Read(length);
		if (!message.Complete)
		{
			namedPipeServerStream.BeginRead(message.Buffer, 0, message.Buffer.Length, HandleRequest, message);
			return;
		}
		Message.HidePassword(message.Text);
		try
		{
			SendResponse(message.Text, namedPipeServerStream);
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error during Messages Handle Request:");
			Smart.Log.Error(TAG, ex.ToString());
			namedPipeServerStream.Disconnect();
			WaitForConnection();
		}
	}

	private void SendResponse(string request, NamedPipeServerStream pipe)
	{
		string text = Handler(request);
		Message.HidePassword(text);
		Message message = new Message(pipe, text);
		pipe.BeginWrite(message.Buffer, 0, message.Length, HandleResponse, message);
	}

	private void HandleResponse(IAsyncResult dataStateObject)
	{
		Message message = (Message)dataStateObject.AsyncState;
		NamedPipeServerStream namedPipeServerStream = (NamedPipeServerStream)message.Stream;
		Smart.Log.Debug(TAG, "Message SERVER: Sending data");
		namedPipeServerStream.EndWrite(dataStateObject);
		if (!message.Complete)
		{
			message.Write();
			namedPipeServerStream.BeginWrite(message.Buffer, 0, message.Length, HandleResponse, message);
			return;
		}
		Smart.Log.Debug(TAG, "Message SERVER: Finished sending response");
		namedPipeServerStream.WaitForPipeDrain();
		namedPipeServerStream.Disconnect();
		Smart.Thread.Wait(TimeSpan.FromSeconds(1.0));
		namedPipeServerStream.Close();
	}

	public void Dispose()
	{
		if (Mutex != null)
		{
			Mutex.ReleaseMutex();
			Mutex.Close();
			Mutex = null;
		}
	}
}
