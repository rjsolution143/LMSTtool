using System.Collections.Generic;

namespace SmartRsd;

public class JsonRecipeGroup
{
	public JsonPhoneModel Model { get; private set; }

	public List<JsonRecipe> Recipes { get; private set; }

	public bool Valid { get; private set; }

	private string TAG => GetType().FullName;

	public JsonRecipeGroup(RecipeDef[] recipes, JsonPhoneModel phoneModel)
	{
		Model = phoneModel;
		Recipes = new List<JsonRecipe>();
		Valid = false;
		for (int i = 0; i < recipes.Length; i++)
		{
			JsonRecipe jsonRecipe = new JsonRecipe(recipes[i], this);
			if (jsonRecipe.Valid)
			{
				Recipes.Add(jsonRecipe);
			}
		}
		if (Recipes.Count > 0)
		{
			Valid = true;
		}
	}
}
