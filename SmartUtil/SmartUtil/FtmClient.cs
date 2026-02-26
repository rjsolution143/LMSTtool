using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using ISmart;

namespace SmartUtil;

public class FtmClient : IFtmClient, INetworkClient, IDisposable
{
	public struct Request
	{
		public const byte StopFlag = 126;

		public const byte EscapeCharacter = 125;

		public const byte EscapeComplement = 32;

		private static ushort[] crc_16_l_table = new ushort[256]
		{
			0, 4489, 8978, 12955, 17956, 22445, 25910, 29887, 35912, 40385,
			44890, 48851, 51820, 56293, 59774, 63735, 4225, 264, 13203, 8730,
			22181, 18220, 30135, 25662, 40137, 36160, 49115, 44626, 56045, 52068,
			63999, 59510, 8450, 12427, 528, 5017, 26406, 30383, 17460, 21949,
			44362, 48323, 36440, 40913, 60270, 64231, 51324, 55797, 12675, 8202,
			4753, 792, 30631, 26158, 21685, 17724, 48587, 44098, 40665, 36688,
			64495, 60006, 55549, 51572, 16900, 21389, 24854, 28831, 1056, 5545,
			10034, 14011, 52812, 57285, 60766, 64727, 34920, 39393, 43898, 47859,
			21125, 17164, 29079, 24606, 5281, 1320, 14259, 9786, 57037, 53060,
			64991, 60502, 39145, 35168, 48123, 43634, 25350, 29327, 16404, 20893,
			9506, 13483, 1584, 6073, 61262, 65223, 52316, 56789, 43370, 47331,
			35448, 39921, 29575, 25102, 20629, 16668, 13731, 9258, 5809, 1848,
			65487, 60998, 56541, 52564, 47595, 43106, 39673, 35696, 33800, 38273,
			42778, 46739, 49708, 54181, 57662, 61623, 2112, 6601, 11090, 15067,
			20068, 24557, 28022, 31999, 38025, 34048, 47003, 42514, 53933, 49956,
			61887, 57398, 6337, 2376, 15315, 10842, 24293, 20332, 32247, 27774,
			42250, 46211, 34328, 38801, 58158, 62119, 49212, 53685, 10562, 14539,
			2640, 7129, 28518, 32495, 19572, 24061, 46475, 41986, 38553, 34576,
			62383, 57894, 53437, 49460, 14787, 10314, 6865, 2904, 32743, 28270,
			23797, 19836, 50700, 55173, 58654, 62615, 32808, 37281, 41786, 45747,
			19012, 23501, 26966, 30943, 3168, 7657, 12146, 16123, 54925, 50948,
			62879, 58390, 37033, 33056, 46011, 41522, 23237, 19276, 31191, 26718,
			7393, 3432, 16371, 11898, 59150, 63111, 50204, 54677, 41258, 45219,
			33336, 37809, 27462, 31439, 18516, 23005, 11618, 15595, 3696, 8185,
			63375, 58886, 54429, 50452, 45483, 40994, 37561, 33584, 31687, 27214,
			22741, 18780, 15843, 11370, 7921, 3960
		};

		public byte CommandCode { get; private set; }

		public byte[] RawData { get; private set; }

		public byte[] DataIn { get; private set; }

		public ushort Crc => CalculateCrc(DataIn);

		public Request(string command)
		{
			DataIn = Smart.Convert.HexToBytes(command);
			CommandCode = DataIn[0];
			string text = command.Substring(2);
			RawData = Smart.Convert.HexToBytes(text);
		}

		public byte[] ToBytes()
		{
			List<byte[]> list = new List<byte[]>();
			list.Add(new byte[1] { CommandCode });
			byte[] item = Escape(RawData);
			list.Add(item);
			byte[] array = Smart.Convert.UShortToBytes(Crc);
			Array.Reverse((Array)array);
			array = Escape(array);
			list.Add(array);
			list.Add(new byte[1] { 126 });
			return list.SelectMany((byte[] part) => part).ToArray();
		}

		public string ToHex()
		{
			return Smart.Convert.BytesToHex(ToBytes());
		}

		public override string ToString()
		{
			SortedList<string, object> sortedList = new SortedList<string, object>();
			sortedList["commandCode"] = CommandCode.ToString("X2");
			sortedList["rawData"] = Smart.Convert.BytesToHex(RawData);
			sortedList["crc"] = Crc.ToString("X4");
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

		public static byte[] Escape(byte[] data)
		{
			List<byte> list = new List<byte>();
			list.Add(125);
			list.Add(126);
			List<int> list2 = new List<int>();
			for (int i = 0; i < data.Length; i++)
			{
				foreach (byte item in list)
				{
					if (item == data[i])
					{
						list2.Add(i);
						break;
					}
				}
			}
			if (list2.Count < 1)
			{
				return data;
			}
			byte[] array = new byte[data.Length + list2.Count];
			int num = 0;
			for (int j = 0; j < data.Length; j++)
			{
				if (!list2.Contains(j))
				{
					array[num] = data[j];
					num++;
				}
				else
				{
					array[num] = 125;
					array[num + 1] = (byte)(data[j] ^ 0x20u);
					num += 2;
				}
			}
			return array;
		}

		public static byte[] UnEscape(byte[] data)
		{
			int num = data.Count((byte value) => value == 125);
			if (num < 1)
			{
				return data;
			}
			byte[] array = new byte[data.Length - num];
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				byte b = data[num2];
				num2++;
				if (b == 125)
				{
					b = (byte)(data[num2] ^ 0x20u);
					num2++;
				}
				array[i] = b;
			}
			return data;
		}

		public static ushort CalculateCrc(byte[] data)
		{
			return CalculateCRC(new List<byte>(data), data.Length);
		}

		private static ushort CalculateCRC(List<byte> dataBytes, int totalBytesToCalculate)
		{
			ushort num = 33800;
			int num2 = totalBytesToCalculate * 8;
			int num3 = 0;
			ushort num4 = ushort.MaxValue;
			while (num2 >= 8)
			{
				num4 = (ushort)(crc_16_l_table[(num4 ^ dataBytes[num3]) & 0xFF] ^ (num4 >> 8));
				num2 -= 8;
				num3++;
			}
			if (num2 != 0)
			{
				ushort num5 = (ushort)(dataBytes[num3] << 8);
				while (num2-- != 0)
				{
					if (((uint)(num4 ^ num5) & (true ? 1u : 0u)) != 0)
					{
						num4 >>= 1;
						num4 ^= num;
					}
					else
					{
						num4 >>= 1;
					}
					num5 >>= 1;
				}
			}
			return (ushort)(~num4);
		}
	}

	public struct Response : IFtmResponse
	{
		public byte ErrorCode { get; private set; }

		public byte[] Data { get; private set; }

		public byte[] Raw { get; private set; }

		public IFtmResponse UnSolicitedResponse { get; set; }

		public Response(Request request, byte[] raw, bool crcMatchedRequired)
		{
			UnSolicitedResponse = null;
			if (raw.Length < 4)
			{
				throw new IndexOutOfRangeException("Response must be at least 4 bytes");
			}
			if (raw[^1] != 126)
			{
				throw new MissingFieldException("Response must end in Stop Flag");
			}
			Raw = raw;
			string text = Smart.Convert.BytesToHex(raw);
			string text2 = request.ToHex();
			text2 = text2.Substring(0, text2.Length - 6);
			int length = 0;
			for (int num = text2.Length - 2; num >= 0; num -= 2)
			{
				if (text2.Substring(num, 2) != "00")
				{
					length = num + 2;
					break;
				}
			}
			text2 = text2.Substring(0, length);
			if (!text.StartsWith(text2))
			{
				ErrorCode = raw[0];
				Data = new byte[0];
				return;
			}
			raw = Request.UnEscape(raw);
			ushort num2 = Request.CalculateCrc(raw.Take(raw.Length - 3).ToArray());
			byte[] array = new byte[2]
			{
				raw[^2],
				raw[^3]
			};
			ushort num3 = Smart.Convert.BytesToUShort(array);
			if (num2 != num3)
			{
				if (crcMatchedRequired)
				{
					throw new NotSupportedException("Received CRC is incorrect");
				}
				Smart.Log.Debug("Response", $"CRCs are not matched - calcCRC: {num2}, recCRC: {num3}");
			}
			ErrorCode = 0;
			int num4 = text2.Length / 2;
			int num5 = raw.Length - num4 - 3;
			Data = new byte[num5];
			if (num5 > 0)
			{
				Array.Copy(raw, num4, Data, 0, num5);
			}
		}

		public string ToHex()
		{
			return Smart.Convert.BytesToHex(Data);
		}

		public override string ToString()
		{
			SortedList<string, object> sortedList = new SortedList<string, object>();
			sortedList["errorCode"] = ErrorCode.ToString("X2");
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

	private const int DEFAULT_PORT = 11008;

	private TimeSpan DEFAULT_TIMEOUT = TimeSpan.FromSeconds(50.0);

	private object clientLock = new object();

	private TcpClient client;

	private string TAG => GetType().FullName;

	public string Host { get; private set; }

	public int Port { get; private set; }

	public void SetEndPoint(string host, int port)
	{
		Host = host;
		Port = port;
		lock (clientLock)
		{
			client = new TcpClient();
		}
	}

	public void SetEndPoint(string host)
	{
		SetEndPoint(host, 11008);
	}

	public void Connect()
	{
		lock (clientLock)
		{
			client.Connect(Host, Port);
			Smart.Log.Verbose(TAG, $"Connected to FTM socket: {Host}:{Port}");
		}
	}

	public bool IsConnected()
	{
		if (client != null)
		{
			return client.Connected;
		}
		return false;
	}

	public IFtmResponse SendCommand(string command)
	{
		return SendCommand(command, unsolicited: false);
	}

	public IFtmResponse SendCommand(string command, bool unsolicited, bool crcMatchedRequired = true)
	{
		lock (this)
		{
			if (!IsConnected())
			{
				Connect();
			}
			Request request = new Request(command);
			Smart.Log.Verbose(TAG, $"Sending FTM request: {request.ToString()}");
			byte[] array = Send(request, write: true);
			Smart.Log.Verbose(TAG, $"Received raw FTM solicited data: {Smart.Convert.BytesToHex(array)}");
			Response response = new Response(request, array, crcMatchedRequired);
			Smart.Log.Verbose(TAG, $"Received FTM solicited response: {response.ToString()}");
			if (unsolicited)
			{
				array = Send(request, write: false);
				Smart.Log.Verbose(TAG, $"Received raw FTM unsolicited data: {Smart.Convert.BytesToHex(array)}");
				Response response2 = new Response(request, array, crcMatchedRequired);
				response.UnSolicitedResponse = (IFtmResponse)(object)response2;
				Smart.Log.Verbose(TAG, $"Received FTM unsolicited response: {response2.ToString()}");
			}
			return (IFtmResponse)(object)response;
		}
	}

	private byte[] Send(Request request, bool write)
	{
		byte[] array = request.ToBytes();
		NetworkStream stream = client.GetStream();
		if (write)
		{
			stream.Write(array, 0, array.Length);
		}
		List<byte> list = new List<byte>();
		byte b = 0;
		do
		{
			int num = stream.ReadByte();
			if (num >= 0)
			{
				b = (byte)num;
				list.Add(b);
			}
		}
		while (b != 126);
		return list.ToArray();
	}

	public void Dispose()
	{
		if (client != null)
		{
			client.Close();
			client = null;
		}
	}
}
