using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace SmartWeb;

public class OAuthTokenService : RestService
{
	public struct OAuthTokenOutput
	{
		private string TAG => GetType().FullName;

		public string AccessToken { get; private set; }

		public string Scope { get; private set; }

		public string TokenType { get; private set; }

		public int ExpiresIn { get; private set; }

		public OAuthTokenOutput(string accessToken, string scope, string tokenType, int expiresIn)
		{
			this = default(OAuthTokenOutput);
			AccessToken = accessToken;
			Scope = scope;
			TokenType = tokenType;
			ExpiresIn = expiresIn;
		}

		public static OAuthTokenOutput FromDictionary(dynamic fields)
		{
			return new OAuthTokenOutput((string)fields.access_token, (string)fields.scope, (string)fields.token_type, (int)fields.expires_in);
		}

		public override string ToString()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["AccessToken"] = AccessToken;
			dictionary["Scope"] = Scope;
			dictionary["TokenType"] = TokenType;
			dictionary["ExpiresIn"] = ExpiresIn;
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

	public string Key { get; set; }

	public string Secret { get; set; }

	protected NetworkCredential Credential => new NetworkCredential(Key, Secret);

	protected override string BasicAuthentication
	{
		get
		{
			string text = $"{Credential.UserName}:{Credential.Password}";
			string arg = Smart.Convert.BytesToBase64(Smart.Convert.AsciiToBytes(text));
			return $"Basic {arg}";
		}
	}

	public OAuthTokenService(string url, string key, string secret)
	{
		base.Url = url;
		Key = key;
		Secret = secret;
	}

	public override dynamic Invoke(dynamic request)
	{
		return SendForm(request);
	}

	public OAuthTokenOutput GetToken()
	{
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		Smart.Log.Debug(TAG, $"Contacting OAuthTokenService ({base.Url})");
		dynamic val = new ExpandoObject();
		val.grant_type = "client_credentials";
		dynamic val2 = Invoke(val);
		if (val2.error != null)
		{
			string text = string.Format("OAuth request failed with error {0}: {1}", val2.error, val2.error_description);
			Smart.Log.Error(TAG, text);
			throw new WebException(text);
		}
		OAuthTokenOutput result = OAuthTokenOutput.FromDictionary(val2);
		Smart.Log.Debug(TAG, "OAuthTokenService request completed");
		Smart.Log.Verbose(TAG, result.ToString());
		return result;
	}
}
