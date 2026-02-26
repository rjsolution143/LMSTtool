using System;
using System.Windows.Forms;

namespace ISmart;

public interface IHotKey : IDisposable
{
	int Register(Action callback, Keys key, ModifierKeys modifier);

	void Unregister(int id);
}
