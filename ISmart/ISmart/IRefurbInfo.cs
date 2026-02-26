namespace ISmart;

public interface IRefurbInfo
{
	bool Enabled { get; }

	RefurbInfo CollectInfo(IDevice device);
}
