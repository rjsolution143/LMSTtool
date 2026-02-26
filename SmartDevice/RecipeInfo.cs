using System;
using System.Collections.Generic;
using System.Dynamic;
using ISmart;

namespace SmartDevice;

public class RecipeInfo : IRecipeInfo
{
	private string TAG => GetType().FullName;

	public string Name { get; private set; }

	public UseCase UseCase { get; private set; }

	public List<IStepInfo> Steps { get; private set; }

	public dynamic Args { get; private set; }

	public void Load(string recipeContent, UseCase useCase, string recipeFileName)
	{
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Invalid comparison between Unknown and I4
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Invalid comparison between Unknown and I4
		Args = new ExpandoObject();
		dynamic val = Smart.Json.Load(recipeContent);
		Name = val.Name;
		string text = val.UseCase;
		bool flag = false;
		if (val.BaseRecipe != null)
		{
			flag = true;
			string text2 = val.BaseRecipe;
			Args.BaseRecipe = text2;
		}
		if (!Enum.TryParse<UseCase>(text, ignoreCase: true, out UseCase result))
		{
			Smart.Log.Error(TAG, $"content.Usecase, {text}, in recipe {recipeFileName} is invalid");
		}
		else if (result != useCase)
		{
			Smart.Log.Error(TAG, $"The running usecase {((object)(UseCase)(ref useCase)).ToString()} is different than the content.UseCase, {text}, of recipe {recipeFileName}");
		}
		UseCase = useCase;
		Steps = new List<IStepInfo>();
		if ((int)useCase != 134 && (int)useCase != 141 && !flag)
		{
			IStepInfo val2 = Smart.NewStepInfo();
			dynamic val3 = Smart.Json.Load("{\"Name\": \"Check Local Configuration\", \"Step\": \"ConfigurationCheck\", \"Args\": {}}");
			val2.Load(val3);
			Steps.Add(val2);
		}
		foreach (dynamic item in val.Steps)
		{
			if (item.Name != null && item.Step != null && item.Args != null)
			{
				IStepInfo val4 = Smart.NewStepInfo();
				val4.Load(item);
				Steps.Add(val4);
			}
			else
			{
				Smart.Log.Error(TAG, "Malformed step like {} in the recipe. Skipping step");
			}
		}
	}

	public void Filter(List<string> stepExcludes, bool safe = true)
	{
		List<IStepInfo> list = new List<IStepInfo>();
		foreach (IStepInfo step in Steps)
		{
			string name = step.Name;
			bool flag = true;
			if (((dynamic)step.Args)["AllowSkip"] != null)
			{
				flag = ((dynamic)step.Args).AllowSkip;
			}
			if (stepExcludes.Contains(name) && (!safe || flag))
			{
				list.Add(step);
			}
		}
		foreach (IStepInfo item in list)
		{
			Smart.Log.Verbose(TAG, $"Removing step {item.Name}");
			Steps.Remove(item);
		}
	}

	public IRecipeInfo ExpandStep(string stepName, IRecipeInfo linkedInfo)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		RecipeInfo recipeInfo = new RecipeInfo();
		recipeInfo.Name = Name;
		recipeInfo.UseCase = UseCase;
		recipeInfo.Args = (object)Args;
		List<IStepInfo> list = new List<IStepInfo>();
		foreach (IStepInfo step in Steps)
		{
			if (step.Name.ToLowerInvariant().Trim() != stepName.ToLowerInvariant().Trim())
			{
				list.Add(step);
				continue;
			}
			foreach (IStepInfo step2 in linkedInfo.Steps)
			{
				if (!(step2.Step.ToLowerInvariant().Trim() == "configurationcheck"))
				{
					list.Add(step2);
				}
			}
		}
		recipeInfo.Steps = list;
		return (IRecipeInfo)(object)recipeInfo;
	}

	public IRecipeInfo ExtendBase(IRecipeInfo baseInfo)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		RecipeInfo recipeInfo = new RecipeInfo();
		recipeInfo.Name = Name;
		recipeInfo.UseCase = UseCase;
		recipeInfo.Args = (object)Args;
		List<IStepInfo> list = new List<IStepInfo>();
		int num = 0;
		foreach (IStepInfo step2 in Steps)
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (((dynamic)step2.Args).SkipToSection != null)
			{
				text = ((dynamic)step2.Args).SkipToSection;
			}
			if (((dynamic)step2.Args).BeforeStep != null)
			{
				text3 = ((dynamic)step2.Args).BeforeStep;
			}
			if (((dynamic)step2.Args).AfterStep != null)
			{
				text3 = ((dynamic)step2.Args).AfterStep;
				flag3 = true;
			}
			if (((dynamic)step2.Args).ReplaceStep != null)
			{
				text3 = ((dynamic)step2.Args).ReplaceStep;
				flag = true;
			}
			if (((dynamic)step2.Args).DeleteStep != null)
			{
				text3 = ((dynamic)step2.Args).DeleteStep;
				flag2 = true;
			}
			if (((dynamic)step2.Args).Before != null)
			{
				text2 = ((dynamic)step2.Args).Before;
			}
			if (((dynamic)step2.Args).After != null)
			{
				text2 = ((dynamic)step2.Args).After;
				flag3 = true;
			}
			if (((dynamic)step2.Args).Replace != null)
			{
				text2 = ((dynamic)step2.Args).Replace;
				flag = true;
			}
			if (((dynamic)step2.Args).Delete != null)
			{
				text2 = ((dynamic)step2.Args).Delete;
				flag2 = true;
			}
			bool flag4 = text != null || text3 != null || text2 != null;
			bool flag5 = !flag4;
			List<IStepInfo> list2 = new List<IStepInfo>();
			if (!flag4 && num >= baseInfo.Steps.Count)
			{
				list2.Add(step2);
			}
			while (num < baseInfo.Steps.Count)
			{
				IStepInfo val = baseInfo.Steps[num];
				num++;
				if (text != null)
				{
					string section = val.Section;
					if (text.Trim().ToLowerInvariant() == section.Trim().ToLowerInvariant())
					{
						flag5 = true;
						text = null;
					}
				}
				else if (text3 != null)
				{
					string step = val.Step;
					if (text3.Trim().ToLowerInvariant() == step.Trim().ToLowerInvariant())
					{
						flag5 = true;
						text3 = null;
					}
				}
				else if (text2 != null)
				{
					string name = val.Name;
					if (text2.Trim().ToLowerInvariant() == name.Trim().ToLowerInvariant())
					{
						flag5 = true;
						text2 = null;
					}
				}
				if (!flag5)
				{
					list2.Add(val);
					continue;
				}
				if (flag2)
				{
					break;
				}
				if (flag)
				{
					list2.Add(step2);
					break;
				}
				if (flag3)
				{
					list2.Add(val);
					list2.Add(step2);
				}
				else
				{
					list2.Add(step2);
					num--;
				}
				flag4 = text != null || text3 != null || text2 != null;
				if (!flag4)
				{
					break;
				}
				flag5 = false;
			}
			if (flag4 && !flag5)
			{
				Smart.Log.Warning(TAG, $"Could not find position for step {((object)step2).ToString()}");
				list2.Insert(0, step2);
			}
			foreach (IStepInfo item in list2)
			{
				list.Add(item);
			}
		}
		if (num < baseInfo.Steps.Count)
		{
			for (int i = num; i < baseInfo.Steps.Count; i++)
			{
				list.Add(baseInfo.Steps[i]);
			}
		}
		recipeInfo.Steps = list;
		return (IRecipeInfo)(object)recipeInfo;
	}

	public override string ToString()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		string text = $"Recipe {Name} ({UseCase})";
		foreach (IStepInfo step in Steps)
		{
			string text2 = ((object)step).ToString();
			text = text + Environment.NewLine + text2;
		}
		return text;
	}

	public override bool Equals(object obj)
	{
		return ToString().Equals(obj.ToString());
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}
}
