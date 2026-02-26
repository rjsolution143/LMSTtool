using ISmart;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmartUtil;

public class Json : IJson
{
	private string TAG => GetType().FullName;

	public string Dump(dynamic obj)
	{
		return JsonConvert.SerializeObject(obj, (Formatting)1);
	}

	public dynamic Load(string content)
	{
		return JsonConvert.DeserializeObject(content);
	}

	public dynamic Convert(object obj)
	{
		return JToken.FromObject(obj);
	}

	public ReturnType LoadString<ReturnType>(string json)
	{
		return JsonConvert.DeserializeObject<ReturnType>(json);
	}
}
