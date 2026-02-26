namespace ISmart;

public interface IUseCaseRunner
{
	bool ResultsSaved(UseCase useCase, IDevice device);

	void Run(UseCase useCase, IDevice device, bool newThread = true, bool skipPassed = false);
}
