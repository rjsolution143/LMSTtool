using System;
using System.Threading;
using ISmart;

namespace SmartUtil;

public class ThreadLocker : IThreadLocked, IDisposable
{
	private object lockObj = new object();

	private Func<dynamic> getter = () => (dynamic)null;

	private Action<dynamic> setter = delegate
	{
	};

	private bool disposedValue;

	public dynamic Data
	{
		get
		{
			return getter();
		}
		set
		{
			((Action<object>)setter)(value);
		}
	}

	public ThreadLocker(object locker, Func<dynamic> getter, Action<dynamic> setter)
	{
		lockObj = locker;
		Monitor.Enter(locker);
		this.getter = getter;
		this.setter = setter;
	}

	public void Close()
	{
		Dispose();
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				Monitor.Exit(lockObj);
			}
			disposedValue = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}
}
