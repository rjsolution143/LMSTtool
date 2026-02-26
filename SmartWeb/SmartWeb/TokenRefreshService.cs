using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace SmartWeb;

public class TokenRefreshService : RestService
{
	public struct TokenRefreshInput
	{
		private string TAG => GetType().FullName;

		public string UserName { get; private set; }

		public string StationID { get; private set; }

		public string RSDUniqueID { get; private set; }

		public dynamic Fields
		{
			get
			{
				dynamic val = new ExpandoObject();
				val.username = UserName;
				val.stationid = StationID;
				val.macid = RSDUniqueID;
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

		public TokenRefreshInput(string userName, string stationID, string rsdUniqueID)
		{
			this = default(TokenRefreshInput);
			UserName = userName;
			StationID = stationID;
			RSDUniqueID = rsdUniqueID;
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

	public struct TokenRefreshOutput
	{
		private string TAG => GetType().FullName;

		public string Status { get; private set; }

		public string Message { get; private set; }

		public string UserName { get; private set; }

		public string PublicIP { get; private set; }

		public int ExpiresIn { get; private set; }

		public string Token { get; private set; }

		public string StationID { get; private set; }

		public TokenRefreshOutput(string status, string message, string userName, string publicIP, int expiresIn, string token, string stationID)
		{
			this = default(TokenRefreshOutput);
			Status = status;
			Message = message;
			UserName = userName;
			PublicIP = publicIP;
			ExpiresIn = expiresIn;
			Token = token;
			StationID = stationID;
		}

		public static TokenRefreshOutput FromDictionary(dynamic fields)
		{
			return new TokenRefreshOutput((string)fields.status, (string)fields.message, (string)fields.username, (string)fields.publicip, (int)fields.expiresin, (string)fields.token, (string)fields.stationid);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["Status"] = Status;
			dictionary["Message"] = Message;
			dictionary["UserName"] = UserName;
			dictionary["PublicIP"] = PublicIP;
			dictionary["ExpiresIn"] = ExpiresIn.ToString();
			dictionary["StationID"] = StationID;
			dictionary["Token"] = Token;
			if (Token != null && Token.Length > 10)
			{
				dictionary["Token"] = "..." + Token.Substring(Token.Length - 8, 8);
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

	private string TAG => GetType().FullName;

	public bool TRG { get; private set; }

	protected NetworkCredential Credential
	{
		get
		{
			if (TRG)
			{
				return new NetworkCredential("rsdgfs", "Moto@123");
			}
			return new NetworkCredential("rsdjira", "Moto#2015");
		}
	}

	protected override string BasicAuthentication
	{
		get
		{
			string text = $"{Credential.UserName}:{Credential.Password}";
			string arg = Smart.Convert.BytesToBase64(Smart.Convert.AsciiToBytes(text));
			return $"Basic {arg}";
		}
	}

	public TokenRefreshService()
	{
		TRG = false;
		base.Url = "https://ebiz-esb.motorola.com/rsdc/Azure/TokenRefreshClient/";
		base.Timeout = TimeSpan.FromSeconds(15.0);
	}

	public TokenRefreshService(bool trg)
	{
		if (trg)
		{
			TRG = true;
			base.Url = "https://ebiz-esb-test.motorola.com/rsdc/Azure/TokenRefreshClient/";
		}
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendRequest(request);
	}

	public TokenRefreshOutput RefreshToken(TokenRefreshInput input)
	{
		Smart.Log.Debug(TAG, $"Contacting TokenRefreshService ({base.Url})");
		base.ExtraHeaders = Smart.Security.StationSign();
		dynamic fields = input.Fields;
		dynamic val = Invoke(fields);
		TokenRefreshOutput result = TokenRefreshOutput.FromDictionary(val);
		Smart.Log.Debug(TAG, "TokenRefreshService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
