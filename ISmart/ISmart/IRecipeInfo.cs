using System.Collections.Generic;

namespace ISmart;

public interface IRecipeInfo
{
	string Name { get; }

	UseCase UseCase { get; }

	List<IStepInfo> Steps { get; }

	dynamic Args { get; }

	void Load(string recipeContent, UseCase useCase, string recipeFileName);

	void Filter(List<string> stepExcludes, bool safe = true);

	IRecipeInfo ExtendBase(IRecipeInfo baseInfo);

	IRecipeInfo ExpandStep(string stepName, IRecipeInfo linkedInfo);
}
