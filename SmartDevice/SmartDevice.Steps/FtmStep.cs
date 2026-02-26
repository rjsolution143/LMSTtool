using ISmart;

namespace SmartDevice.Steps;

public abstract class FtmStep : BaseStep
{
	private string TAG => GetType().FullName;

	public IFtmClient ftm => (IFtmClient)base.Cache["ftm"];
}
