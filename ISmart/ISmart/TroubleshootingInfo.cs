using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISmart;

public struct TroubleshootingInfo
{
	public string Title { get; private set; }

	public string URL { get; private set; }

	public int Score { get; private set; }

	public string Category { get; private set; }

	public string SubCategory { get; private set; }

	public TroubleshootingInfo(string title, string url, int score, string category, string subCategory)
	{
		Title = title;
		URL = url;
		Score = score;
		Category = category;
		SubCategory = subCategory;
	}

	public override string ToString()
	{
		SortedList<string, object> sortedList = new SortedList<string, object>();
		sortedList["Title"] = Title;
		sortedList["URL"] = URL;
		sortedList["Score"] = Score;
		sortedList["Category"] = Category;
		sortedList["SubCategory"] = SubCategory;
		return ToString(GetType().Name, sortedList.ToList());
	}

	public override bool Equals(object obj)
	{
		return ToString().Equals(obj.ToString());
	}

	public override int GetHashCode()
	{
		return ToString().GetHashCode();
	}

	private string ToString(string name, IEnumerable<KeyValuePair<string, object>> fields)
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
