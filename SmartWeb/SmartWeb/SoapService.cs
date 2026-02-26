using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;
using System.Text;

namespace SmartWeb;

public abstract class SoapService : WebService
{
	protected class SoapListener : IClientMessageInspector, IEndpointBehavior
	{
		private Action<string> requestCallback { get; set; }

		private Action<string> replyCallback { get; set; }

		private List<KeyValuePair<string, string>> extraHeaders { get; set; }

		public SoapListener(Action<string> requestCallback, Action<string> replyCallback, List<KeyValuePair<string, string>> extraHeaders)
		{
			this.requestCallback = requestCallback;
			this.replyCallback = replyCallback;
			this.extraHeaders = extraHeaders;
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			replyCallback(((object)reply).ToString());
		}

		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Expected O, but got Unknown
			object obj = default(object);
			foreach (KeyValuePair<string, string> extraHeader in extraHeaders)
			{
				string key = extraHeader.Key;
				string value = extraHeader.Value;
				Smart.Log.Verbose("SoapService", $"Adding '{key}' header");
				if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, ref obj))
				{
					HttpRequestMessageProperty val = (HttpRequestMessageProperty)((obj is HttpRequestMessageProperty) ? obj : null);
					if (string.IsNullOrEmpty(val.Headers[key]))
					{
						val.Headers[key] = value;
					}
				}
				else
				{
					HttpRequestMessageProperty val = new HttpRequestMessageProperty();
					val.Headers.Add(key, value);
					request.Properties.Add(HttpRequestMessageProperty.Name, (object)val);
				}
			}
			requestCallback(((object)request).ToString());
			return null;
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
			clientRuntime.MessageInspectors.Add((IClientMessageInspector)(object)this);
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
		}

		public void Validate(ServiceEndpoint endpoint)
		{
		}
	}

	private string TAG => GetType().FullName;

	protected virtual Binding binding
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Expected O, but got Unknown
			BasicHttpBinding val = new BasicHttpBinding
			{
				CloseTimeout = TimeSpan.FromMinutes(1.0),
				OpenTimeout = TimeSpan.FromMinutes(1.0),
				ReceiveTimeout = TimeSpan.FromMinutes(10.0),
				SendTimeout = TimeSpan.FromMinutes(1.0),
				AllowCookies = false,
				BypassProxyOnLocal = false,
				HostNameComparisonMode = (HostNameComparisonMode)0,
				MessageEncoding = (WSMessageEncoding)0,
				TextEncoding = Encoding.UTF8,
				TransferMode = (TransferMode)0,
				UseDefaultWebProxy = true
			};
			val.Security.Mode = (BasicHttpSecurityMode)1;
			val.Security.Transport.ClientCredentialType = (HttpClientCredentialType)0;
			val.Security.Transport.ProxyCredentialType = (HttpProxyCredentialType)0;
			val.Security.Transport.Realm = "";
			val.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Default;
			return (Binding)val;
		}
	}
}
