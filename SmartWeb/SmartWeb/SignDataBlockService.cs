using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using SmartWeb.DataBlockSignServiceV3;

namespace SmartWeb;

public class SignDataBlockService : SoapService
{
	public struct SignDataBlockInput
	{
		private string TAG => GetType().FullName;

		public string MascId { get; private set; }

		public string RsdLogId { get; private set; }

		public string ClientIpAddress { get; private set; }

		public string ClientRequestType { get; private set; }

		public string NewSerialNumber { get; private set; }

		public string OldSerialNumber { get; private set; }

		public string OriginalSerialNumber { get; private set; }

		public string PasswordChangeRequired { get; private set; }

		public string Crc32 { get; private set; }

		public byte[] RequestParam { get; private set; }

		public SignDataBlockInput(string mascId, string rsdLogId, string clientIpAddress, string clientRequestType, string newSerialNumber, string oldSerialNumber, string originalSerialNumber, string passwordChangeRequired, byte[] requestParam)
		{
			this = default(SignDataBlockInput);
			MascId = mascId;
			RsdLogId = rsdLogId;
			ClientIpAddress = clientIpAddress;
			ClientRequestType = clientRequestType;
			NewSerialNumber = newSerialNumber;
			OldSerialNumber = oldSerialNumber;
			OriginalSerialNumber = originalSerialNumber;
			PasswordChangeRequired = passwordChangeRequired;
			RequestParam = requestParam;
			Crc32 = GenerateCrc(MascId, ClientIpAddress, ClientRequestType, NewSerialNumber, OldSerialNumber, PasswordChangeRequired, rsdLogId, RequestParam);
		}

		public RequestBean ToServiceInput()
		{
			return new RequestBean
			{
				MASCID = MascId,
				rsd_log_id = RsdLogId,
				clientIP = ClientIpAddress,
				clientReqType = ClientRequestType,
				newIMEI = NewSerialNumber,
				oldIMEI = OldSerialNumber,
				passChgRequd = PasswordChangeRequired,
				reqParam = RequestParam,
				crc32 = Crc32
			};
		}

		private static string GenerateCrc(string mascId, string clientIpAddress, string clientRequestType, string newSerialNumber, string oldSerialNumber, string passwordChangeRequired, string rsdLogId, byte[] requestParam)
		{
			List<byte> list = new List<byte>();
			list.AddRange(Smart.Convert.AsciiToBytes(clientIpAddress));
			list.AddRange(Smart.Convert.AsciiToBytes(mascId));
			list.AddRange(Smart.Convert.AsciiToBytes(clientRequestType));
			list.AddRange(Smart.Convert.AsciiToBytes(oldSerialNumber));
			list.AddRange(Smart.Convert.AsciiToBytes(newSerialNumber));
			list.AddRange(Smart.Convert.AsciiToBytes(passwordChangeRequired));
			list.AddRange(Smart.Convert.AsciiToBytes(rsdLogId));
			list.AddRange(requestParam);
			int num = Crc.Compute(list.ToArray());
			return Smart.Convert.BytesToHex(Smart.Convert.IntToBytes(num));
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["MascId"] = MascId;
			dictionary["ClientIpAddress"] = ClientIpAddress;
			dictionary["ClientRequestType"] = ClientRequestType;
			dictionary["NewSerialNumber"] = NewSerialNumber;
			dictionary["OldSerialNumber"] = OldSerialNumber;
			dictionary["OriginalSerialNumber"] = OriginalSerialNumber;
			dictionary["PasswordChangeRequired"] = PasswordChangeRequired;
			dictionary["RsdLogId"] = RsdLogId;
			dictionary["Crc32"] = Crc32;
			string value = "[NULL]";
			if (RequestParam != null)
			{
				value = Smart.Convert.BytesToHex(RequestParam);
			}
			dictionary["RequestParam"] = value;
			return Smart.Convert.ToString(GetType().Name, (IEnumerable<KeyValuePair<string, object>>)dictionary.ToList());
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

	public struct SignDataBlockOutput
	{
		private string TAG => GetType().FullName;

		public string StatusCode { get; private set; }

		public string StatusData { get; private set; }

		public string TransactionId { get; private set; }

		public byte[] PkiResponse { get; private set; }

		public SignDataBlockOutput(string statusCode, string statusData, string transactionId, byte[] pkiResponse)
		{
			this = default(SignDataBlockOutput);
			StatusCode = statusCode;
			StatusData = statusData;
			TransactionId = transactionId;
			PkiResponse = pkiResponse;
		}

		public static SignDataBlockOutput FromServiceOutput(ClientResponse output)
		{
			return new SignDataBlockOutput(output.statusCode, output.statusData, output.transactionID, output.pkiResponse);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["StatusCode"] = StatusCode;
			dictionary["StatusData"] = StatusData;
			dictionary["TransactionId"] = TransactionId;
			string value = "[NULL]";
			if (PkiResponse != null)
			{
				value = Smart.Convert.BytesToHex(PkiResponse);
			}
			dictionary["PkiResponse"] = value;
			return Smart.Convert.ToString(GetType().Name, (IEnumerable<KeyValuePair<string, object>>)dictionary.ToList());
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

	private string TAG => GetType().FullName;

	public SignDataBlockService()
	{
		base.Url = "https://ebiz-esb.motorola.com/iqs-0.0.1-SNAPSHOT/webservice/IQS_DataBlockSign_3.5";
	}

	public override dynamic Invoke(dynamic request)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		QS_DataBlockSign_10Client qS_DataBlockSign_10Client = new QS_DataBlockSign_10Client(binding, new EndpointAddress(base.Url));
		KeyValuePair<string, string> item = new KeyValuePair<string, string>("Authorization", BasicAuthentication);
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		list.Add(item);
		KeyValuePair<string, string> item2 = new KeyValuePair<string, string>("originalimei", request.OriginalSerialNumber);
		list.Add(item2);
		SortedList<string, string> sortedList = Smart.Security.StationSign();
		foreach (string key in sortedList.Keys)
		{
			list.Add(new KeyValuePair<string, string>(key, sortedList[key]));
		}
		((ClientBase<IQS_DataBlockSign_10>)qS_DataBlockSign_10Client).Endpoint.EndpointBehaviors.Add((IEndpointBehavior)(object)new SoapListener(base.SentRequest, base.ReceivedReply, list));
		dynamic val = qS_DataBlockSign_10Client.signDataBlock(request.ToServiceInput());
		return SignDataBlockOutput.FromServiceOutput(val);
	}

	public SignDataBlockOutput SignDataBlock(SignDataBlockInput input)
	{
		Smart.Log.Debug(TAG, $"Contacting SignDataBlockService  ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		SignDataBlockOutput result = Invoke(input);
		Smart.Log.Debug(TAG, "SignDataBlockService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
