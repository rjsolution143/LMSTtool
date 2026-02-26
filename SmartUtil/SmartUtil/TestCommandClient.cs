using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using ISmart;

namespace SmartUtil;

public class TestCommandClient : ITestCommandClient, INetworkClient, IDisposable
{
	public struct Request
	{
		[ThreadStatic]
		private static byte lastTag = byte.MaxValue;

		public ResponseType Flags { get; private set; }

		public byte SequenceTag { get; private set; }

		public ushort OpCode { get; private set; }

		public byte ResponseRequired { get; private set; }

		public int Length { get; private set; }

		public byte[] Data { get; private set; }

		public Request(ResponseType flags, byte sequenceTag, ushort opCode, byte responseRequired, int length, byte[] data)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			Flags = flags;
			SequenceTag = sequenceTag;
			OpCode = opCode;
			ResponseRequired = responseRequired;
			Length = length;
			Data = data;
		}

		public Request(ushort opCode, byte[] data)
			: this((ResponseType)0, NextTag(), opCode, 0, data.Length, data)
		{
		}

		public Request(string opCode, string data)
			: this(Smart.Convert.BytesToUShort(Smart.Convert.HexToBytes(opCode)), Smart.Convert.HexToBytes(data))
		{
		}

		public static byte NextTag()
		{
			byte result = 0;
			if (lastTag < byte.MaxValue)
			{
				result = (byte)(lastTag + 1);
			}
			lastTag = result;
			return result;
		}

		public byte[] ToBytes()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected I4, but got Unknown
			List<byte[]> list = new List<byte[]>();
			list.Add(new byte[1] { (byte)(int)Flags });
			list.Add(new byte[1] { SequenceTag });
			list.Add(Smart.Convert.UShortToBytes(OpCode));
			list.Add(new byte[1]);
			list.Add(new byte[1] { ResponseRequired });
			list.Add(new byte[2]);
			list.Add(Smart.Convert.IntToBytes(Length));
			list.Add(Data);
			return list.SelectMany((byte[] part) => part).ToArray();
		}

		public string ToHex()
		{
			return Smart.Convert.BytesToHex(ToBytes());
		}

		public override string ToString()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			SortedList<string, object> sortedList = new SortedList<string, object>();
			ResponseType flags = Flags;
			sortedList["flags"] = ((object)(ResponseType)(ref flags)).ToString();
			sortedList["sequenceTag"] = SequenceTag.ToString("X2");
			sortedList["opCode"] = OpCode.ToString("X4");
			sortedList["responseCode"] = ResponseRequired.ToString();
			sortedList["length"] = Length.ToString();
			sortedList["data"] = Smart.Convert.BytesToHex(Data);
			return Smart.Convert.ToString(GetType().Name, (IEnumerable<KeyValuePair<string, object>>)sortedList.ToList());
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

	public struct Response : ITestCommandResponse
	{
		public ResponseType Flags { get; private set; }

		public byte SequenceTag { get; private set; }

		public ushort OpCode { get; private set; }

		public ResponseCode ResponseCode { get; private set; }

		public int Length { get; private set; }

		public byte[] Data { get; private set; }

		public string DataHex => Smart.Convert.BytesToHex(Data);

		public bool Failed => (Flags & 1) == 1;

		public bool DataReturned => (Flags & 2) == 2;

		public bool Unsolicited => (Flags & 4) == 4;

		public bool Incomplete
		{
			get
			{
				if (Data != null)
				{
					return Length != Data.Length;
				}
				return false;
			}
		}

		public byte[] RawResponse { get; private set; }

		public static Response BlankResponse => new Response((ResponseType)129, 0, 0, (ResponseCode)7, 0, new byte[0], new byte[0]);

		public static Response Append(Response existing, byte[] extraData)
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			byte[] array = new byte[existing.Data.Length + extraData.Length];
			Array.Copy(existing.Data, array, existing.Data.Length);
			Array.Copy(extraData, 0, array, existing.Data.Length, extraData.Length);
			byte[] array2 = new byte[existing.RawResponse.Length + extraData.Length];
			Array.Copy(existing.RawResponse, array2, existing.RawResponse.Length);
			Array.Copy(extraData, 0, array2, existing.RawResponse.Length, extraData.Length);
			return new Response(existing.Flags, existing.SequenceTag, existing.OpCode, existing.ResponseCode, existing.Length, array, array2);
		}

		public Response(ResponseType flags, byte sequenceTag, ushort opCode, ResponseCode responseCode, int length, byte[] data, byte[] rawResponse)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			Flags = flags;
			SequenceTag = sequenceTag;
			OpCode = opCode;
			ResponseCode = responseCode;
			Length = length;
			Data = data;
			RawResponse = rawResponse;
		}

		public Response(byte[] raw)
			: this((ResponseType)raw[0], raw[1], Smart.Convert.BytesToUShort(raw.Skip(2).Take(2).ToArray()), (ResponseCode)raw[5], Smart.Convert.BytesToInt(raw.Skip(8).Take(4).ToArray()), raw.Skip(12).ToArray(), raw)
		{
		}

		public Response(string hexRaw)
			: this(Smart.Convert.HexToBytes(hexRaw))
		{
		}

		public override string ToString()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			SortedList<string, object> sortedList = new SortedList<string, object>();
			ResponseType flags = Flags;
			sortedList["flags"] = ((object)(ResponseType)(ref flags)).ToString();
			sortedList["sequenceTag"] = SequenceTag.ToString("X2");
			sortedList["opCode"] = OpCode.ToString("X4");
			ResponseCode responseCode = ResponseCode;
			sortedList["responseCode"] = ((object)(ResponseCode)(ref responseCode)).ToString();
			sortedList["length"] = Length.ToString();
			sortedList["data"] = Smart.Convert.BytesToHex(Data);
			sortedList["failed"] = Failed.ToString();
			sortedList["dataReturned"] = DataReturned.ToString();
			sortedList["unsolicited"] = Unsolicited.ToString();
			sortedList["incomplete"] = Incomplete.ToString();
			return Smart.Convert.ToString(GetType().Name, (IEnumerable<KeyValuePair<string, object>>)sortedList.ToList());
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

	private const int DEFAULT_PORT = 11000;

	private TimeSpan DEFAULT_TIMEOUT = TimeSpan.FromSeconds(10.0);

	private AsyncServerClient client;

	private object responseLock = new object();

	private Dictionary<byte, Response> responses = new Dictionary<byte, Response>();

	private Response incomplete = Response.BlankResponse;

	private string TAG => GetType().FullName;

	public TimeSpan Timeout { get; set; }

	public string Host { get; private set; }

	public int Port { get; private set; }

	public TestCommandClient()
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
		SetEndPoint(host, 11000);
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
		if (data.Equals(AsyncServerClient.ReceivedData.BlankData))
		{
			return true;
		}
		Smart.Log.Verbose(TAG, $"Received TCMD response: {data.Hex}");
		lock (responseLock)
		{
			Response response = new Response(data.Data);
			Smart.Log.Verbose(TAG, string.Format(response.ToString()));
			if (response.Incomplete)
			{
				Smart.Log.Debug(TAG, "Received response is incomplete");
				if (incomplete.Equals(Response.BlankResponse))
				{
					Smart.Log.Debug(TAG, "Holding incomplete response for processing");
					incomplete = response;
					return true;
				}
				Response existing = incomplete;
				Response response2 = response;
				if (response2.Length == response2.Data.Length + existing.RawResponse.Length)
				{
					existing = response;
					response2 = incomplete;
				}
				if (existing.Length != existing.Data.Length + response2.RawResponse.Length)
				{
					Smart.Log.Error(TAG, "Discarding old incomplete response for new incomplete response");
					Smart.Log.Debug(TAG, $"{existing.Data.Length} incomplete response + {response2.RawResponse.Length} extra response != {existing.Length} expected response");
					Smart.Log.Debug(TAG, $"{response2.Data.Length} extra response + {existing.RawResponse.Length} incomplete response != {response2.Length} expected response");
					incomplete = response;
					return true;
				}
				Smart.Log.Debug(TAG, "Found extra raw response for incomplete response");
				Smart.Log.Debug(TAG, $"{existing.Data.Length} incomplete response + {response2.RawResponse.Length} extra response = {existing.Length} expected response");
				response = Response.Append(existing, response2.RawResponse);
				incomplete = Response.BlankResponse;
			}
			responses[response.SequenceTag] = response;
		}
		return true;
	}

	public ITestCommandResponse SendCommand(string command)
	{
		return SendCommand(command, string.Empty);
	}

	public ITestCommandResponse SendCommand(string command, string data)
	{
		Request request = new Request(command, data);
		Send(request);
		Response response = Response.BlankResponse;
		lock (responseLock)
		{
			if (responses.ContainsKey(request.SequenceTag))
			{
				response = responses[request.SequenceTag];
			}
		}
		if (response.Failed)
		{
			throw new ArgumentOutOfRangeException($"Response fail bit set: {response.ToString()}");
		}
		return (ITestCommandResponse)(object)response;
	}

	private void Send(Request request)
	{
		Send(request, waitForResponse: true);
	}

	private void Send(Request request, bool waitForResponse)
	{
		lock (responseLock)
		{
			responses.Remove(request.SequenceTag);
		}
		if (client.Status == ConnectionState.Closed)
		{
			Connect();
		}
		if (client.Status == ConnectionState.Connecting)
		{
			WaitForConnection();
		}
		Smart.Log.Verbose(TAG, $"Sending TCMD request: {request.ToHex()}");
		Smart.Log.Verbose(TAG, request.ToString());
		client.Send(request.ToBytes());
		if (waitForResponse)
		{
			WaitForResponse(request.SequenceTag);
		}
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

	private void WaitForResponse(byte sequenceCode)
	{
		TimeSpan timeout = Timeout;
		DateTime now = DateTime.Now;
		while (DateTime.Now.Subtract(now).TotalMilliseconds < timeout.TotalMilliseconds)
		{
			lock (responseLock)
			{
				if (responses.ContainsKey(sequenceCode))
				{
					return;
				}
			}
			System.Threading.Thread.Sleep(50);
		}
		client.Disconnect();
		throw new TimeoutException("Timed out waiting for response");
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
