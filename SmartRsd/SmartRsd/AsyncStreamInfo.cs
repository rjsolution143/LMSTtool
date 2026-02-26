using System.IO;
using System.Threading;

namespace SmartRsd;

internal struct AsyncStreamInfo
{
	public FileStream AsyncFileStream { get; private set; }

	public ManualResetEvent WriteDoneEvent { get; private set; }

	public AsyncStreamInfo(FileStream asyncFileStream, ManualResetEvent writeDoneEvent)
	{
		this = default(AsyncStreamInfo);
		AsyncFileStream = asyncFileStream;
		WriteDoneEvent = writeDoneEvent;
	}
}
