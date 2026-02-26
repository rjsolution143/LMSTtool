using System;
using System.Collections.Generic;
using ISmart;

namespace SmartRsd;

public class JsonRecipe
{
	public string Id { get; private set; }

	public bool Default { get; private set; }

	public string Usecase { get; private set; }

	public List<JsonStep> Steps { get; private set; }

	public JsonRecipeGroup RecipeGroup { get; private set; }

	public string SrcConfig { get; private set; }

	public bool Valid { get; private set; }

	private string TAG => GetType().FullName;

	public JsonRecipe(RecipeDef recipe, JsonRecipeGroup recipeGroup)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		Valid = false;
		RecipeGroup = recipeGroup;
		Usecase = ((recipe.usecase != null) ? recipe.usecase : string.Empty).Trim();
		Id = ((recipe.id != null) ? recipe.id : string.Empty).Trim();
		Default = recipe.defaultRecipe != null && Convert.ToBoolean(recipe.defaultRecipe);
		SrcConfig = ((recipe.srcConfigs != null) ? recipe.srcConfigs : string.Empty);
		if (Usecase == "LMST_Battery_Reset")
		{
			UseCase val = (UseCase)156;
			Usecase = ((object)(UseCase)(ref val)).ToString();
		}
		if (Usecase.Length == 0)
		{
			if (RecipeGroup.Model.Matrix.FullyParsed)
			{
				Smart.Log.Error(TAG, string.Format("{0} entry is missing or empty [recipe {1}] [PhoneModel {2}] [Carrier {3}]", "usecase", Id, recipeGroup.Model.Name, recipeGroup.Model.Matrix.InternalName));
			}
			return;
		}
		if (!Enum.TryParse<UseCase>(Usecase, ignoreCase: true, out UseCase _))
		{
			if (RecipeGroup.Model.Matrix.FullyParsed)
			{
				Smart.Log.Error(TAG, $"{Usecase} is NOT a valid usecase [recipe {Id}] [PhoneModel {RecipeGroup.Model.Name}] [Carrier {RecipeGroup.Model.Matrix.InternalName}");
			}
			return;
		}
		Steps = new List<JsonStep>();
		if (recipe.steps != null)
		{
			StepDef[] steps = recipe.steps;
			for (int i = 0; i < steps.Length; i++)
			{
				JsonStep item = new JsonStep(steps[i], this);
				Steps.Add(item);
			}
		}
		if (Steps.Count == 0)
		{
			if (RecipeGroup.Model.Matrix.FullyParsed)
			{
				Smart.Log.Error(TAG, string.Format("{0} entry is missing [recipe {1}] [PhoneModel {2}] [Carrier {3}]", "steps", Id, recipeGroup.Model.Name, recipeGroup.Model.Matrix.InternalName));
			}
			return;
		}
		string key = Usecase.ToLowerInvariant();
		if (Default)
		{
			if (!recipeGroup.Model.DefaultUsecaseToRecipeLookup.ContainsKey(key))
			{
				recipeGroup.Model.DefaultUsecaseToRecipeLookup.Add(key, this);
			}
			else if (RecipeGroup.Model.Matrix.FullyParsed)
			{
				Smart.Log.Error(TAG, $"{Usecase} usecase is duplicated in default usecase table [recipe {Id}] [PhoneModel {recipeGroup.Model.Name}] [Carrier {recipeGroup.Model.Matrix.InternalName}]");
			}
		}
		Valid = true;
		if (!(SrcConfig != string.Empty) || Default)
		{
			return;
		}
		if (recipeGroup.Model.ConfigIdToJsonConfigLookup.TryGetValue(SrcConfig, out var value))
		{
			if (!value.UsecaseToRecipeLookup.ContainsKey(key))
			{
				value.UsecaseToRecipeLookup.Add(key, this);
			}
			else if (RecipeGroup.Model.Matrix.FullyParsed)
			{
				Smart.Log.Error(TAG, $"{Usecase} usecase is duplicated [recipe {Id}] [PhoneModel {recipeGroup.Model.Name}] [Carrier {recipeGroup.Model.Matrix.InternalName}]");
			}
		}
		else if (RecipeGroup.Model.Matrix.FullyParsed)
		{
			Smart.Log.Error(TAG, $"ID {SrcConfig} does not point to any JsonConfig entry [recipe {Id}] [PhoneModel {recipeGroup.Model.Name}] [Carrier {recipeGroup.Model.Matrix.InternalName}]");
		}
	}
}
