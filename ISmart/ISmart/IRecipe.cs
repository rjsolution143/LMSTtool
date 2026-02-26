using System.Collections.Generic;

namespace ISmart;

public interface IRecipe
{
	IRecipeInfo Info { get; }

	IResultLogger Log { get; }

	List<IStep> Steps { get; }

	SortedList<string, dynamic> Cache { get; }

	void Load(IRecipeInfo info);

	void Run();
}
