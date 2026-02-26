using System.Collections.Generic;
using System.Text;

namespace ISmart;

public class Global
{
	public static string RecipeNameTemp;

	public static UseCase SelectUsecase;

	public static string NewLMSTVersion;

	public static string ToString(string name, IEnumerable<KeyValuePair<string, object>> fields)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(name);
		foreach (KeyValuePair<string, object> field in fields)
		{
			stringBuilder.Append("    ");
			stringBuilder.Append(field.Key);
			stringBuilder.Append(": ");
			if (field.Value != null)
			{
				stringBuilder.AppendLine(field.Value.ToString());
			}
			else
			{
				stringBuilder.AppendLine("[NULL]");
			}
		}
		return stringBuilder.ToString();
	}
}
