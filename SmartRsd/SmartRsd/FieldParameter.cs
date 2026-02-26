namespace SmartRsd;

internal class FieldParameter
{
	public string Value { get; private set; }

	public string ContentType { get; private set; }

	public FieldParameter(string value, string contentType)
	{
		Value = value;
		ContentType = contentType;
	}
}
