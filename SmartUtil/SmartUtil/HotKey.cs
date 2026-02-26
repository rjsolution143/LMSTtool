using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ISmart;

namespace SmartUtil;

public class HotKey : IHotKey, IDisposable
{
	public class HotKeyWindow : NativeWindow, IDisposable
	{
		private const int WM_HOTKEY = 786;

		private const int ID_START = 1337;

		private SortedList<int, Action> callbacks = new SortedList<int, Action>();

		private int offset;

		private bool disposedValue;

		[DllImport("user32.dll")]
		public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

		[DllImport("user32.dll")]
		public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		public HotKeyWindow()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			((NativeWindow)this).CreateHandle(new CreateParams());
		}

		public int Register(Action callback, int modifiers, Keys key)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected I4, but got Unknown
			int num = 1337 + offset;
			RegisterCombo(num, modifiers, (int)key, callback);
			offset++;
			return num;
		}

		public void Unregister(int id)
		{
			UnregisterHotKey(((NativeWindow)this).Handle, id);
		}

		private void RegisterCombo(int ID, int fsModifiers, int vlc, Action callback)
		{
			if (RegisterHotKey(((NativeWindow)this).Handle, ID, fsModifiers, vlc))
			{
				callbacks.Add(ID, callback);
			}
		}

		protected override void WndProc(ref Message m)
		{
			int msg = ((Message)(ref m)).Msg;
			if (msg == 786)
			{
				int key = ((Message)(ref m)).WParam.ToInt32();
				callbacks[key]();
			}
			((NativeWindow)this).WndProc(ref m);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposedValue)
			{
				return;
			}
			if (disposing)
			{
				foreach (int key in callbacks.Keys)
				{
					UnregisterHotKey(((NativeWindow)this).Handle, key);
				}
				((NativeWindow)this).DestroyHandle();
			}
			disposedValue = true;
		}

		public void Dispose()
		{
			Dispose(disposing: true);
		}
	}

	private HotKeyWindow window;

	private string TAG => GetType().FullName;

	public int Register(Action callback, Keys key, ModifierKeys modifiers)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected I4, but got Unknown
		if (window == null)
		{
			window = new HotKeyWindow();
			Smart.App.ScheduleShutdownTask("Shut down hotkeys", (Action)Dispose);
		}
		return window.Register(callback, (int)modifiers, key);
	}

	public void Unregister(int id)
	{
		window.Unregister(id);
	}

	public void Dispose()
	{
		if (window != null)
		{
			window.Dispose();
			window = null;
		}
	}
}
