using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ISmart;

namespace SmartDevice;

public class DummyWindow : NativeWindow
{
	protected static bool AddTimeToLogMessage = true;

	public bool savelogdata;

	public StringBuilder LogData = new StringBuilder();

	public ILog Logger { get; set; }

	public string TAG { get; set; }

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern uint RegisterWindowMessage(string lpString);

	protected override void WndProc(ref Message m)
	{
		if (((Message)(ref m)).Msg == RegisterWindowMessage("WM_FBFLASH_STATUS_MSG"))
		{
			string text = Marshal.PtrToStringAnsi(((Message)(ref m)).LParam);
			Logger.Verbose(TAG, text);
			if (savelogdata)
			{
				LogData.AppendLine(text);
			}
		}
		((NativeWindow)this).WndProc(ref m);
	}
}
