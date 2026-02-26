namespace ISmart;

public class ValidationItem
{
	public string Name { get; }

	public bool Enabled { get; set; }

	public string Target { get; set; }

	public string Value { get; set; }

	public Result Result { get; }

	public ValidationItem(string name, bool enabled, string target, string value, Result result)
	{
		Name = name;
		Enabled = enabled;
		Target = target;
		Value = value;
		Result = result;
	}

	public ValidationItem(string name, string value, Result result)
		: this(name, enabled: true, value, value, result)
	{
	}
}
