namespace ISmart;

public interface IStepInfo
{
	string Name { get; }

	string FriendlyName { get; }

	string Step { get; }

	string Section { get; }

	dynamic Args { get; }

	void Load(dynamic stepContent);
}
