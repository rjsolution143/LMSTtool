using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using ISmart;

namespace SmartUtil;

public class CommServerClient : ICommServerClient, INetworkClient, IDisposable
{
	public struct Message
	{
		public enum MessageType
		{
			Unknown,
			Command,
			Ack,
			Nack,
			Unsolicited,
			Info,
			Data
		}

		private const string CR_ESCAPE = "&#10;";

		private const string LF_ESCAPE = "&#13;";

		private const string DOUBLE_QUOTE_ESCAPE = "&#14;";

		private const string SINGLE_QUOTE_ESCAPE = "&#15;";

		private static long lastId = -1L;

		private string TAG => GetType().FullName;

		public long Id { get; private set; }

		public DateTime Timestamp { get; private set; }

		public MessageType Type { get; private set; }

		public string Description { get; private set; }

		public string Data { get; private set; }

		public static Message BlankMessage => new Message(0L, DateTime.MinValue, MessageType.Unknown, string.Empty, string.Empty);

		public Message(long id, DateTime timestamp, MessageType type, string description, string data)
		{
			this = default(Message);
			Id = id;
			Timestamp = timestamp;
			Type = type;
			Description = description;
			if (data.Contains(" ") && (!data.StartsWith("\"") || !data.EndsWith("\"")))
			{
				data = $"\"{data}\"";
			}
			Data = data;
		}

		public Message(long id, MessageType type, string description, string data)
			: this(id, DateTime.Now, type, description, data)
		{
		}

		public Message(MessageType type, string description, string data)
			: this(NextId(), type, description, data)
		{
		}

		public Message(MessageType type, string description)
			: this(type, description, string.Empty)
		{
		}

		public Message(AsyncServerClient.ReceivedData data)
		{
			this = default(Message);
			Timestamp = data.Timestamp;
			string input = Unescape(data.Text);
			string pattern = "0x(?<id>\\w+) (?<type>\\w+) (?<description>\\S+) (?<data>.+)";
			Match match = Regex.Match(input, pattern);
			if (!match.Success)
			{
				throw new NotSupportedException($"Message '{data.ToString()}' not recognized");
			}
			string value = match.Groups["id"].Value;
			Id = long.Parse(value, NumberStyles.HexNumber);
			string value2 = match.Groups["type"].Value;
			try
			{
				Type = (MessageType)Enum.Parse(typeof(MessageType), value2, ignoreCase: true);
			}
			catch (Exception)
			{
				Smart.Log.Assert(TAG, false, "Message type should match a known type");
				Type = MessageType.Unknown;
			}
			Description = match.Groups["description"].Value;
			string text = match.Groups["data"].Value;
			if (text.Contains(" ") && (!text.StartsWith("\"") || !text.EndsWith("\"")))
			{
				text = $"\"{text}\"";
			}
			Data = text;
		}

		public KeyValuePair<string, string>[] ParseResults()
		{
			KeyValuePair<string, string>[] result = new KeyValuePair<string, string>[0];
			if (Type == MessageType.Info && Enumerable.Contains(Data, '='))
			{
				string pattern = "\"(?<key>[^\\s=]+)=(?<value>[^\"]+)\"|(?<key>[^\\s=]+)=(?<value>[^\\s]+)";
				string text = Data;
				if (Enumerable.Contains(text, ' '))
				{
					if (text.EndsWith("\"\""))
					{
						text = text.Trim(new char[1] { '"' });
						text += "\"";
					}
					else
					{
						text = text.Trim(new char[1] { '"' });
					}
				}
				MatchCollection matchCollection = Regex.Matches(text, pattern);
				if (matchCollection.Count > 0)
				{
					List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
					foreach (Match item in matchCollection)
					{
						string value = item.Groups["key"].Value;
						string value2 = item.Groups["value"].Value;
						list.Add(new KeyValuePair<string, string>(value, value2));
					}
					result = list.ToArray();
				}
				else if (!text.Trim().EndsWith('='.ToString()) && !text.Contains("Picture resolution"))
				{
					Smart.Log.Warning(TAG, "Data results should contain key value pairs: " + Data);
				}
			}
			return result;
		}

		private static long NextId()
		{
			lastId++;
			return lastId;
		}

		private static string Unescape(string text)
		{
			if (text.Contains("&#10;"))
			{
				text = text.Replace("&#10;", "\r");
			}
			if (text.Contains("&#13;"))
			{
				text = text.Replace("&#13;", "\n");
			}
			if (text.Contains("&#14;"))
			{
				text = text.Replace("&#14;", "\"");
			}
			if (text.Contains("&#15;"))
			{
				text = text.Replace("&#15;", "'");
			}
			return text;
		}

		private static string Escape(string text)
		{
			if (text.Contains("\r"))
			{
				text = text.Replace("\r", "&#10;");
			}
			if (text.Contains("\n"))
			{
				text = text.Replace("\n", "&#13;");
			}
			if (text.Contains("\""))
			{
				text = text.Replace("\"", "&#14;");
			}
			if (text.Contains("'"))
			{
				text = text.Replace("'", "&#15;");
			}
			return text;
		}

		public string ToMessageData()
		{
			string text = string.Empty;
			if (Type != MessageType.Command && Type != 0)
			{
				text = Type.ToString().ToUpperInvariant() + " ";
			}
			string format = "0x{0} {1}{2} {3}\n";
			string text2 = Data;
			if (Data.Contains(" "))
			{
				Smart.Log.Assert(TAG, Data.StartsWith("\"") && Data.EndsWith("\""), "Message data should start and end with double quotes");
				text2 = Data.Substring(1, Data.Length - 2);
				format = "0x{0} {1}{2} \"{3}\"\n";
			}
			object[] args = new object[4]
			{
				Id.ToString("X").PadLeft(8, '0'),
				text,
				Description,
				text2
			};
			return string.Format(format, args);
		}

		public override string ToString()
		{
			return $"{Timestamp.ToLongDateString()}: {ToMessageData()}";
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

	private const int DEFAULT_PORT = 2631;

	private TimeSpan DEFAULT_TIMEOUT = TimeSpan.FromSeconds(30.0);

	private AsyncServerClient client;

	private object responseLock = new object();

	private Dictionary<long, List<Message>> responses = new Dictionary<long, List<Message>>();

	private string TAG => GetType().FullName;

	public TimeSpan Timeout { get; set; }

	public string Host { get; private set; }

	public int Port { get; private set; }

	public CommServerClient()
	{
		Timeout = DEFAULT_TIMEOUT;
	}

	public void SetEndPoint(string host, int port)
	{
		Host = host;
		Port = port;
		client = new AsyncServerClient(host, port, DataCallback);
	}

	public void SetEndPoint(string host)
	{
		SetEndPoint(host, 2631);
	}

	public void Connect()
	{
		client.Connect();
	}

	public bool IsConnected()
	{
		if (client != null)
		{
			return client.Status == ConnectionState.Open;
		}
		return false;
	}

	private bool DataCallback(AsyncServerClient.ReceivedData data)
	{
		foreach (AsyncServerClient.ReceivedData item in AsyncServerClient.ReceivedData.SplitMultilineData(data))
		{
			_ = item;
			if (data.Equals(AsyncServerClient.ReceivedData.BlankData))
			{
				continue;
			}
			Message blankMessage = Message.BlankMessage;
			try
			{
				blankMessage = new Message(data);
			}
			catch (Exception ex)
			{
				Smart.Log.Warning(TAG, "Error parsing message: " + ex.Message);
				Smart.Log.Verbose(TAG, ex.ToString());
				break;
			}
			if (blankMessage.Type == Message.MessageType.Unsolicited)
			{
				if (blankMessage.Description == "PING")
				{
					RespondToPing(blankMessage);
				}
				else if (!(blankMessage.Description == "LOST_FOCUS") && !(blankMessage.Description == "USER_PASS_FAIL") && !(blankMessage.Description == "PASS"))
				{
					Smart.Log.Warning(TAG, blankMessage.ToString());
				}
				continue;
			}
			Smart.Log.Verbose(TAG, blankMessage.ToString());
			lock (responseLock)
			{
				if (!responses.ContainsKey(blankMessage.Id))
				{
					responses[blankMessage.Id] = new List<Message>();
				}
				responses[blankMessage.Id].Add(blankMessage);
			}
		}
		return true;
	}

	private void RespondToPing(Message message)
	{
		Smart.Log.Assert(TAG, message.Description == "PING", "Should be responding to a PING");
		Smart.Log.Assert(TAG, message.Data.Contains("HEY PC YOU THERE?"), "Should be normal PING message");
		Send(new Message(message.Id, Message.MessageType.Ack, message.Description, message.Data), waitForAck: false);
	}

	public SortedList<string, string> SendCommand(string command)
	{
		return SendCommand(command, string.Empty);
	}

	public SortedList<string, string> SendCommand(string command, string data)
	{
		Message message = new Message(Message.MessageType.Command, command, data);
		Send(message);
		SortedList<string, string> sortedList = new SortedList<string, string>();
		List<Message> list = new List<Message>();
		lock (responseLock)
		{
			if (responses.ContainsKey(message.Id))
			{
				list.AddRange(responses[message.Id]);
			}
		}
		foreach (Message item in list)
		{
			if (item.Type == Message.MessageType.Nack)
			{
				throw new ArgumentOutOfRangeException($"NACK returned from CommServer for command {command} {data}: {item.Data}");
			}
			KeyValuePair<string, string>[] array = item.ParseResults();
			for (int i = 0; i < array.Length; i++)
			{
				KeyValuePair<string, string> keyValuePair = array[i];
				if (sortedList.ContainsKey(keyValuePair.Key))
				{
					Smart.Log.Debug(TAG, "Results should be unique");
				}
				sortedList[keyValuePair.Key] = keyValuePair.Value;
			}
		}
		return sortedList;
	}

	private void Send(Message message)
	{
		Send(message, waitForAck: true);
	}

	private void Send(Message message, bool waitForAck)
	{
		bool flag = false;
		TimeSpan timeSpan = TimeSpan.FromSeconds(60.0);
		if (Timeout < DEFAULT_TIMEOUT)
		{
			timeSpan = Timeout;
		}
		DateTime now = DateTime.Now;
		do
		{
			flag = false;
			if (client.Status == ConnectionState.Closed)
			{
				Connect();
			}
			if (client.Status == ConnectionState.Connecting)
			{
				WaitForConnection();
			}
			string text = message.ToMessageData();
			if (message.Description != "PING")
			{
				Smart.Log.Verbose(TAG, text);
			}
			client.Send(text);
			if (!waitForAck)
			{
				continue;
			}
			try
			{
				WaitForAckOrNack(message.Id);
				break;
			}
			catch (TimeoutException ex)
			{
				if (DateTime.Now.Subtract(now).TotalMilliseconds < timeSpan.TotalMilliseconds)
				{
					Smart.Log.Error(TAG, ex.Message);
					message = new Message(message.Type, message.Description, message.Data);
					if (message.Description.ToLowerInvariant() == "STOP".ToLowerInvariant())
					{
						waitForAck = false;
					}
					flag = true;
					continue;
				}
				throw;
			}
		}
		while (flag);
	}

	private void WaitForConnection()
	{
		TimeSpan timeout = Timeout;
		DateTime now = DateTime.Now;
		while (DateTime.Now.Subtract(now).TotalMilliseconds < timeout.TotalMilliseconds)
		{
			if (IsConnected())
			{
				return;
			}
			System.Threading.Thread.Sleep(50);
		}
		client.Disconnect();
		throw new TimeoutException("Timed out waiting for connection");
	}

	private void WaitForAckOrNack(long id)
	{
		TimeSpan timeout = Timeout;
		DateTime now = DateTime.Now;
		while (DateTime.Now.Subtract(now).TotalMilliseconds < timeout.TotalMilliseconds)
		{
			lock (responseLock)
			{
				if (responses.ContainsKey(id))
				{
					foreach (Message item in responses[id])
					{
						if (item.Type == Message.MessageType.Ack || item.Type == Message.MessageType.Nack)
						{
							return;
						}
					}
				}
			}
			System.Threading.Thread.Sleep(50);
		}
		client.Disconnect();
		throw new TimeoutException("Timed out waiting for ACK/NACK");
	}

	public void Dispose()
	{
		if (client != null)
		{
			client.Dispose();
			client = null;
		}
	}
}
