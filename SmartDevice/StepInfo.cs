using ISmart;

namespace SmartDevice;

public class StepInfo : IStepInfo
{
	private string TAG => GetType().FullName;

	public string Name { get; private set; }

	public string FriendlyName { get; private set; }

	public string Step { get; private set; }

	public string Section { get; private set; }

	public dynamic Args { get; private set; }

	public void Load(dynamic stepContent)
	{
		Name = stepContent.Name;
		string text = stepContent.Step;
		Step = text.Trim();
		FriendlyName = string.Empty;
		if (stepContent.FriendlyName != null)
		{
			FriendlyName = stepContent.FriendlyName;
		}
		Section = "General";
		if (stepContent.Section != null)
		{
			Section = stepContent.Section;
		}
		Args = (object)stepContent.Args;
	}

	public override string ToString()
	{
		string text = string.Empty;
		foreach (dynamic arg3 in Args)
		{
			string arg = arg3.Name.ToString();
			string arg2 = arg3.Value.ToString();
			string text2 = $" '{arg}': '{arg2}',";
			text += text2;
		}
		text = text.TrimEnd(new char[1] { ',' });
		string text3 = $"Step {Name} ({Step}) [{Section}]";
		if (text != string.Empty)
		{
			text3 = $"{text3} Args:{text}";
		}
		return text3;
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
