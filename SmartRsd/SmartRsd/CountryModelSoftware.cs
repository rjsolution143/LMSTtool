namespace SmartRsd;

public class CountryModelSoftware
{
	public string Country { get; } = string.Empty;


	public string Model { get; } = string.Empty;


	public string InternalVersion { get; } = string.Empty;


	public CountryModelSoftware(string country, string model, string internalVersion)
	{
		Country = country;
		Model = model;
		InternalVersion = internalVersion;
	}

	public override string ToString()
	{
		return $"Country: {Country}, Model: {Model}, InternalVersion: {InternalVersion}";
	}
}
