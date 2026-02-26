namespace ISmart;

public interface IStep
{
	IRecipe Recipe { get; }

	IStepInfo Info { get; }

	bool VerifyPreContionMet();

	void Load(IRecipe recipe, IStepInfo info);

	void Setup();

	void Run();

	Result Audit();

	void TearDown();

	void Restart();

	bool CheckRetest(Result result);
}
