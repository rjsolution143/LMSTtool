namespace SmartDevice.Steps;

public class SetNameValues : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		foreach (object item in ((dynamic)base.Info.Args).NameValues)
		{
			string[] array = ((string)(dynamic)item).Split(new char[1] { '=' });
			string text = array[0].Trim();
			string text2 = array[1].Trim();
			base.Cache[text] = text2;
			if (text == "pcb-part-no")
			{
				base.Log.AddInfo(text, text2);
			}
			Smart.Log.Debug(TAG, $"Assign value {text2} to name {text}");
		}
		LogPass();
	}
}
