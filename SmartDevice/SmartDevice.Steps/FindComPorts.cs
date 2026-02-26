using System.Threading.Tasks;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice.Steps;

public class FindComPorts : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		IDevice device = (IDevice)((dynamic)base.Recipe.Info.Args).Device;
		if (((dynamic)base.Info.Args).PromptText != null)
		{
			string promptText = ((dynamic)base.Info.Args).PromptText.ToString();
			promptText = Smart.Locale.Xlate(promptText);
			string title = Smart.Locale.Xlate(base.Info.Name);
			Task.Run(delegate
			{
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				device.Prompt.MessageBox(title, promptText, (MessageBoxButtons)0, (MessageBoxIcon)64);
			});
		}
		int num = 120;
		if (((dynamic)base.Info.Args).Timeout != null)
		{
			num = ((dynamic)base.Info.Args).Timeout;
		}
		foreach (object item in ((dynamic)base.Info.Args).ComPorts)
		{
			string text = (string)(dynamic)item;
			string text2 = Smart.Rsd.ComPortId(text, num);
			if (text2 == string.Empty)
			{
				string text3 = $"COM port of {text} is not found in {num} seconds";
				Smart.Log.Error(TAG, text3);
				LogResult((Result)1, "COM port not found");
				return;
			}
			base.Cache[text] = text2;
		}
		LogPass();
	}
}
