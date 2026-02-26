using System;
using System.Collections.Generic;
using System.Data;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace SmartUtil;

public class AsyncServerClient : IDisposable
{
	public struct ReceivedData
	{
		private string TAG => GetType().FullName;

		public DateTime Timestamp { get; private set; }

		public string Text => Smart.Convert.BytesToAscii(Data);

		public string Hex => Smart.Convert.BytesToHex(Data);

		public byte[] Data { get; private set; }

		public static ReceivedData BlankData => new ReceivedData(DateTime.MinValue, new byte[0]);

		public ReceivedData(DateTime timestamp, byte[] data)
		{
			this = default(ReceivedData);
			Timestamp = timestamp;
			Data = data;
		}

		public ReceivedData(byte[] data)
			: this(DateTime.Now, data)
		{
		}

		public static List<ReceivedData> SplitMultilineData(ReceivedData data)
		{
			List<ReceivedData> list = new List<ReceivedData>();
			string[] separator = new string[1] { "\n" };
			string[] array = data.Text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length < 2)
			{
				list.Add(data);
			}
			else
			{
				string[] array2 = array;
				foreach (string text in array2)
				{
					byte[] data2 = Smart.Convert.AsciiToBytes(text);
					list.Add(new ReceivedData(data.Timestamp, data2));
				}
			}
			return list;
		}

		public override string ToString()
		{
			return $"{Timestamp.ToLongTimeString()}: {Text}";
		}

		public override bool Equals(object obj)
		{
			return ToString().Equals(obj.ToString());
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
	}

	protected struct DataState
	{
		private string TAG => GetType().FullName;

		public TcpClient Client { get; private set; }

		public byte[] Buffer { get; private set; }

		public DataState(TcpClient client, byte[] buffer)
		{
			this = default(DataState);
			Client = client;
			Buffer = buffer;
		}

		public DataState(TcpClient client, int bufferSize)
			: this(client, GetBuffer(bufferSize))
		{
		}

		private static byte[] GetBuffer(int size)
		{
			return new byte[size];
		}
	}

	public delegate bool DataCallback(ReceivedData newData);

	private DataCallback dataCallback;

	private object clientLock = new object();

	private TcpClient client;

	private ConnectionState status;

	private List<ReceivedData> receivedData;

	private string TAG => GetType().FullName;

	public string Host { get; private set; }

	public int Port { get; private set; }

	public ConnectionState Status
	{
		get
		{
			lock (clientLock)
			{
				return status;
			}
		}
	}

	public bool IsDataAvailable
	{
		get
		{
			lock (clientLock)
			{
				return receivedData.Count > 0;
			}
		}
	}

	public AsyncServerClient(string host, int port)
		: this(host, port, null)
	{
	}

	public AsyncServerClient(string host, int port, DataCallback dataCallback)
	{
		Smart.Log.Debug(TAG, $"Creating client for {host}:{port}");
		Host = host;
		Port = port;
		this.dataCallback = dataCallback;
		lock (clientLock)
		{
			receivedData = new List<ReceivedData>();
		}
	}

	public PingReply Ping()
	{
		return new Ping().Send(Host);
	}

	public bool Connect()
	{
		Smart.Log.Debug(TAG, "Attempting to connect to server");
		Disconnect();
		lock (clientLock)
		{
			status = ConnectionState.Connecting;
			Smart.Log.Debug(TAG, $"Client connection status: {status}");
			client = new TcpClient();
			try
			{
				client.BeginConnect(Host, Port, ConnectCallback, client);
			}
			catch (SocketException ex)
			{
				Smart.Log.Assert(TAG, ex.Message.Contains("already connected"), "Exception should be about an existing connection");
				status = ConnectionState.Closed;
				Smart.Log.Debug(TAG, $"Client connection status: {status}");
				return false;
			}
		}
		return true;
	}

	public List<ReceivedData> PollData()
	{
		lock (clientLock)
		{
			List<ReceivedData> result = receivedData;
			receivedData = new List<ReceivedData>();
			return result;
		}
	}

	private void ConnectCallback(IAsyncResult clientObject)
	{
		Smart.Log.Debug(TAG, "Finishing server connection");
		bool flag = false;
		lock (clientLock)
		{
			if (Status == ConnectionState.Closed)
			{
				Smart.Log.Assert(TAG, client == null, "Client should be null");
				return;
			}
			TcpClient tcpClient = (TcpClient)clientObject.AsyncState;
			try
			{
				tcpClient.EndConnect(clientObject);
				Smart.Log.Assert(TAG, tcpClient.Connected, "Client should be connected");
				status = ConnectionState.Open;
				Smart.Log.Debug(TAG, $"Client connection status: {status}");
				DataState dataState = new DataState(tcpClient, tcpClient.ReceiveBufferSize);
				tcpClient.Client.BeginReceive(dataState.Buffer, 0, dataState.Client.ReceiveBufferSize, SocketFlags.None, ReceiveCallback, dataState);
			}
			catch (SocketException ex)
			{
				Smart.Log.Error(TAG, "Error during connecting: " + ex.Message);
				Smart.Log.Verbose(TAG, ex.ToString());
				flag = true;
			}
			catch (NullReferenceException ex2)
			{
				Smart.Log.Error(TAG, "Error during connection callback: " + ex2.Message);
				Smart.Log.Verbose(TAG, ex2.ToString());
				flag = true;
			}
		}
		if (flag)
		{
			Disconnect();
		}
	}

	private void ReceiveCallback(IAsyncResult dataStateObject)
	{
		bool flag = false;
		ReceivedData receivedData = ReceivedData.BlankData;
		lock (clientLock)
		{
			if (Status == ConnectionState.Closed)
			{
				Smart.Log.Assert(TAG, client == null, "Client should be null");
				return;
			}
			if (client == null)
			{
				Smart.Log.Warning(TAG, "Status should be closed: " + Status);
				return;
			}
			DataState dataState = (DataState)dataStateObject.AsyncState;
			int num = 0;
			try
			{
				num = dataState.Client.Client.EndReceive(dataStateObject);
			}
			catch (SocketException ex)
			{
				Smart.Log.Error(TAG, "Error during end receive: " + ex.Message);
				Smart.Log.Verbose(TAG, ex.ToString());
				flag = true;
			}
			catch (ObjectDisposedException ex2)
			{
				Smart.Log.Error(TAG, "Error during end receive: " + ex2.Message);
				Smart.Log.Verbose(TAG, ex2.ToString());
				flag = true;
			}
			catch (NullReferenceException ex3)
			{
				Smart.Log.Error(TAG, "Error during end receive: " + ex3.Message);
				Smart.Log.Verbose(TAG, ex3.ToString());
				flag = true;
			}
			if (!flag)
			{
				if (num > 0)
				{
					byte[] array = new byte[num];
					Array.Copy(dataState.Buffer, array, num);
					receivedData = new ReceivedData(array);
				}
				try
				{
					dataState.Client.Client.BeginReceive(dataState.Buffer, 0, dataState.Client.ReceiveBufferSize, SocketFlags.None, ReceiveCallback, dataState);
				}
				catch (SocketException ex4)
				{
					Smart.Log.Error(TAG, "Error during begin receive: " + ex4.Message);
					Smart.Log.Verbose(TAG, ex4.ToString());
					flag = true;
				}
				catch (ObjectDisposedException ex5)
				{
					Smart.Log.Error(TAG, "Error during begin receive: " + ex5.Message);
					Smart.Log.Verbose(TAG, ex5.ToString());
					flag = true;
				}
				catch (NullReferenceException ex6)
				{
					Smart.Log.Error(TAG, "Error during begin receive: " + ex6.Message);
					Smart.Log.Verbose(TAG, ex6.ToString());
					flag = true;
				}
			}
		}
		if (flag)
		{
			Disconnect();
		}
		if (dataCallback != null && dataCallback(receivedData))
		{
			receivedData = ReceivedData.BlankData;
		}
		if (!receivedData.Equals(ReceivedData.BlankData))
		{
			lock (clientLock)
			{
				this.receivedData.Add(receivedData);
			}
		}
	}

	public void Send(string data)
	{
		Send(Smart.Convert.AsciiToBytes(data));
	}

	public void Send(byte[] bytes)
	{
		lock (clientLock)
		{
			if (status == ConnectionState.Closed)
			{
				Smart.Log.Assert(TAG, client == null, "Client should be null");
				throw new ObjectDisposedException("Client was already closed before send");
			}
			client.Client.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, SendCallback, client);
		}
	}

	private void SendCallback(IAsyncResult dataStateObject)
	{
		try
		{
			bool flag = false;
			lock (clientLock)
			{
				if (Status == ConnectionState.Closed)
				{
					Smart.Log.Assert(TAG, client == null, "Client should be null");
					return;
				}
				TcpClient tcpClient = (TcpClient)dataStateObject.AsyncState;
				int num = 0;
				try
				{
					num = tcpClient.Client.EndSend(dataStateObject);
				}
				catch (SocketException ex)
				{
					Smart.Log.Error(TAG, "Error during end send: " + ex.Message);
					Smart.Log.Verbose(TAG, ex.ToString());
					flag = true;
				}
				if (!flag)
				{
					Smart.Log.Assert(TAG, num > 0, "Sent data should be non-zero");
				}
			}
			if (flag)
			{
				Disconnect();
			}
		}
		catch (Exception ex2)
		{
			Smart.Log.Error(TAG, "Error during data callback: " + ex2.Message);
		}
	}

	public void Disconnect()
	{
		lock (clientLock)
		{
			if (status != 0)
			{
				status = ConnectionState.Closed;
				Smart.Log.Debug(TAG, $"Client connection status: {status}");
			}
			if (client != null)
			{
				Smart.Log.Debug(TAG, "Disconnecting from server");
				client.Client.Close();
				client.Close();
				client = null;
			}
		}
	}

	public void Dispose()
	{
		Disconnect();
	}
}
