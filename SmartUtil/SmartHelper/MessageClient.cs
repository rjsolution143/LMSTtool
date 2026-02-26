using System;
using System.IO.Pipes;

namespace SmartHelper;

public class MessageClient : IDisposable
{
	private string TAG => GetType().FullName;

	public string ChannelName { get; private set; }

	public NamedPipeClientStream Pipe { get; private set; }

	public Action<string> Callback { get; private set; }

	public MessageClient(string channelName, Action<string> callback)
	{
		Pipe = new NamedPipeClientStream(".", $"Moto.Messages.{channelName}", PipeDirection.InOut, PipeOptions.Asynchronous);
		Callback = callback;
	}

	private void Connect()
	{
		if (!Pipe.IsConnected)
		{
			Pipe.Connect();
		}
	}

	public void SendMessage(string text)
	{
		Connect();
		Message.HidePassword(text);
		Message message = new Message(Pipe, text);
		message.Write();
		Pipe.BeginWrite(message.Buffer, 0, message.Length, HandleRequest, message);
	}

	private void HandleRequest(IAsyncResult dataStateObject)
	{
		Pipe.EndWrite(dataStateObject);
		Message message = (Message)dataStateObject.AsyncState;
		if (!message.Complete)
		{
			message.Write();
			Pipe.BeginWrite(message.Buffer, 0, message.Length, HandleRequest, message);
		}
		else
		{
			Pipe.WaitForPipeDrain();
			ReadResponse();
		}
	}

	private void ReadResponse()
	{
		Message message = new Message(Pipe);
		Pipe.BeginRead(message.Buffer, 0, message.Buffer.Length, HandleResponse, message);
	}

	private void HandleResponse(IAsyncResult dataStateObject)
	{
		int length = Pipe.EndRead(dataStateObject);
		Message message = (Message)dataStateObject.AsyncState;
		message.Read(length);
		if (!message.Complete)
		{
			Pipe.BeginRead(message.Buffer, 0, message.Buffer.Length, HandleResponse, message);
			return;
		}
		Message.HidePassword(message.Text);
		Callback(message.Text);
	}

	public void Dispose()
	{
		try
		{
		}
		finally
		{
			Pipe.Dispose();
		}
	}
}
