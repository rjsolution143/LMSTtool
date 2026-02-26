namespace ISmart;

public interface ILocale
{
	string LanguageCode { get; }

	void SetDefaultCulture();

	void SetXlatePath(string path);

	string Xlate(string text);
}
