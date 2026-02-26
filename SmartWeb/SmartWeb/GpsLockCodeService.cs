using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using SmartWeb.GpsRsuServiceV2;

namespace SmartWeb;

public class GpsLockCodeService : SoapService
{
	public struct GpsLockCodeInput
	{
		private string TAG => GetType().FullName;

		public string SerialNumber { get; private set; }

		public string TrackID { get; private set; }

		public string SerialNumber2 { get; private set; }

		public string Nwscp { get; private set; }

		public string Sscp { get; private set; }

		public string ServicePasscode { get; private set; }

		public string DeviceSecretKey { get; private set; }

		public string ESimEid { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.serialNumber = SerialNumber;
				val.trackID = TrackID;
				val.serialNumber2 = SerialNumber2;
				val.nwscp = Nwscp;
				val.sscp = Sscp;
				val.servicePasscode = ServicePasscode;
				val.deviceSecretKey = DeviceSecretKey;
				val.eSimEid = ESimEid;
				IDictionary<string, object> dictionary = val;
				foreach (string item in new List<string>(dictionary.Keys))
				{
					if (dictionary[item] == null)
					{
						dictionary.Remove(item);
					}
				}
				return val;
			}
		}

		public GpsLockCodeInput(string serialNumber, string trackID, string serialNumber2, string nwscp, string sscp, string servicePasscode, string deviceSecretKey, string eSimEid)
		{
			this = default(GpsLockCodeInput);
			SerialNumber = serialNumber;
			TrackID = trackID;
			SerialNumber2 = serialNumber2;
			Nwscp = nwscp;
			Sscp = sscp;
			ServicePasscode = servicePasscode;
			DeviceSecretKey = deviceSecretKey;
			ESimEid = eSimEid;
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> item in (IDictionary<string, object>)Fields)
			{
				dictionary[item.Key] = item.Value.ToString();
			}
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

	public struct GpsLockCodeOutput
	{
		private string TAG => GetType().FullName;

		public string ErrorCode { get; private set; }

		public string ErrorMessage { get; private set; }

		public GpsLockCodeOutput(string errorCode, string errorMessage)
		{
			this = default(GpsLockCodeOutput);
			ErrorCode = errorCode;
			ErrorMessage = errorMessage;
		}

		public static GpsLockCodeOutput FromServiceOutput(saveSubsidyLockCodesForServiceResponse output)
		{
			return new GpsLockCodeOutput(output.errorInfo.errorCode.ToString(), output.errorInfo.errorMessage);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["ErrorCode"] = ErrorCode;
			dictionary["ErrorMessage"] = ErrorMessage;
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

	public GpsLockCodeService()
	{
		base.Url = "https://wsgw.motorola.com/GPSTrustonicRSUService/RSUService";
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
	}

	public override dynamic Invoke(dynamic request)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		GPSTrustonicRSUServiceImplClient gPSTrustonicRSUServiceImplClient = new GPSTrustonicRSUServiceImplClient(binding, new EndpointAddress(base.Url));
		KeyValuePair<string, string> item = new KeyValuePair<string, string>("Authorization", BasicAuthentication);
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		list.Add(item);
		((ClientBase<GPSTrustonicRSUServiceImpl>)gPSTrustonicRSUServiceImplClient).Endpoint.EndpointBehaviors.Add((IEndpointBehavior)(object)new SoapListener(base.SentRequest, base.ReceivedReply, list));
		dynamic val = gPSTrustonicRSUServiceImplClient.SaveSubsidyLockCodesForService(request.TrackID, request.SerialNumber, request.SerialNumber2, request.Nwscp, request.Sscp, request.ServicePasscode, request.DeviceSecretKey, request.ESimEid);
		return GpsLockCodeOutput.FromServiceOutput(val);
	}

	public GpsLockCodeOutput GpsLockCode(GpsLockCodeInput input)
	{
		Smart.Log.Debug(TAG, $"Contacting GpsLockCodeService ({base.Url})");
		Smart.Log.Verbose(TAG, input.ToString());
		GpsLockCodeOutput result = Invoke(input);
		Smart.Log.Debug(TAG, "GpsLockCodeService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
