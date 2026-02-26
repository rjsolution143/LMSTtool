using System.Collections.Generic;

namespace ISmart;

public interface IValidator
{
	List<string> FindRecipes(IDevice device);

	List<string> FindProfiles(IDevice device, string recipe);

	void SaveProfile(IDevice device, string profileName, string recipe, List<ValidationItem> options);

	List<ValidationItem> LoadProfile(IDevice device, string recipe, string profile);

	void DeleteProfile(IDevice device, string profileName);

	void InitRecipe(IDevice device, string recipe);

	List<ValidationItem> FindItems(IDevice device);

	void RunRecipe(IDevice device, string recipe, string profile);

	List<ValidationItem> FindResults(IDevice device, string recipe, string profile);
}
