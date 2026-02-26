using ISmart;

namespace SmartDevice.Steps;

public class WebTest : BaseStep
{
	private string TAG => GetType().FullName;

	public override void Run()
	{
		string text = ((IDevice)((dynamic)base.Recipe.Info.Args).Device).Prompt.InputBox("Serial Number", "Enter serial number (or leave blank for default)", (string)null);
		Smart.Web.WarrantyRequest(text, false);
		Smart.Web.GetGppdId(text);
		LogPass();
	}
}
