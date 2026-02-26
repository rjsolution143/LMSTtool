using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace SmartWeb;

public class ETokenValidationService : RestService
{
	public struct ETokenValidationOutput
	{
		private string TAG => GetType().FullName;

		public string ETokenIp { get; private set; }

		public string Status { get; private set; }

		public string ResponseResult { get; private set; }

		public string ResponseMessage { get; private set; }

		public ETokenValidationOutput(string eTokenIp, string status, string responseResult, string responseMessage)
		{
			this = default(ETokenValidationOutput);
			ETokenIp = eTokenIp;
			Status = status;
			ResponseResult = responseResult;
			ResponseMessage = responseMessage;
		}

		public static ETokenValidationOutput FromDictionary(dynamic fields)
		{
			string eTokenIp = fields.etokenip;
			string status = fields.status;
			string responseResult = fields.result;
			string responseMessage = fields.message;
			return new ETokenValidationOutput(eTokenIp, status, responseResult, responseMessage);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["ETokenIp"] = ETokenIp;
			dictionary["Status"] = Status;
			dictionary["ResponseResult"] = ResponseResult;
			dictionary["ResponseMessage"] = ResponseMessage;
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

	public ETokenValidationService()
	{
		base.Url = "https://ebiz-esb.motorola.com/EToken/SRPEtokenValidationService/";
	}

	public override dynamic Invoke(dynamic request)
	{
		string text = request.eTokenIp;
		return SendGet(text.Trim());
	}

	public ETokenValidationOutput GetTokenStatus(string eTokenIp)
	{
		Smart.Log.Debug(TAG, $"Contacting ETokenValidationService ({base.Url})");
		Smart.Log.Verbose(TAG, eTokenIp);
		dynamic val = new ExpandoObject();
		val.eTokenIp = eTokenIp;
		dynamic val2 = Invoke(val);
		ETokenValidationOutput result = ETokenValidationOutput.FromDictionary(val2);
		Smart.Log.Debug(TAG, "ETokenValidationService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
