using System.Collections.Generic;
using System.Linq;

namespace ISmart;

public struct KillSwitchData
{
	private string TAG => GetType().FullName;

	public string Data { get; private set; }

	public string Password { get; private set; }

	public KillSwitchData(string data, string password)
	{
		this = default(KillSwitchData);
		Data = data;
		Password = password;
	}

	public override string ToString()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["Data"] = Data;
		dictionary["Password"] = Password;
		return Global.ToString(GetType().Name, dictionary.ToList());
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
