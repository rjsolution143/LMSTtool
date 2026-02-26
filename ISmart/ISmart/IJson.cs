namespace ISmart;

public interface IJson
{
	dynamic Load(string content);

	string Dump(dynamic obj);

	dynamic Convert(object obj);

	ReturnType LoadString<ReturnType>(string json);
}
