using System;
using System.Collections.Generic;
using System.Threading;
using ISmart;

namespace SmartUtil;

public class Thread : IThread
{
	private SortedList<string, Tuple<Timer, ThreadStart>> timers = new SortedList<string, Tuple<Timer, ThreadStart>>();

	private string TAG => GetType().FullName;

	public SortedList<string, object> LockCache { get; private set; } = new SortedList<string, object>();


	public void Run(ThreadStart task)
	{
		Run(task, setSta: false);
	}

	public void Run(ThreadStart task, bool setSta)
	{
		RunThread(task, setSta);
	}

	public void Run<ReturnValue>(Func<ReturnValue> task, Action<ReturnValue> callback)
	{
		Run(task, callback, setSta: false);
	}

	public void Run<ReturnValue>(Func<ReturnValue> task, Action<ReturnValue> callback, bool setSta)
	{
		ThreadStart task2 = delegate
		{
			ReturnValue obj = task();
			callback(obj);
		};
		Run(task2, setSta);
	}

	public ReturnValue RunAndWait<ReturnValue>(Func<ReturnValue> task)
	{
		return RunAndWait(task, setSta: false);
	}

	public ReturnValue RunAndWait<ReturnValue>(Func<ReturnValue> task, bool setSta)
	{
		ReturnValue returned = default(ReturnValue);
		ThreadStart task2 = delegate
		{
			returned = task();
		};
		RunThread(task2, setSta).Join();
		return returned;
	}

	public System.Threading.Thread RunThread(ThreadStart task)
	{
		return RunThread(task, setSta: false);
	}

	public System.Threading.Thread RunThread(ThreadStart task, bool setSta)
	{
		System.Threading.Thread thread = new System.Threading.Thread((ThreadStart)delegate
		{
			try
			{
				task();
			}
			catch (Exception ex)
			{
				Smart.Log.Critical(TAG, "Unhandled exception in thread: " + ex.Message);
				Smart.Log.Verbose(TAG, ex.ToString());
			}
		});
		thread.IsBackground = true;
		if (setSta)
		{
			thread.SetApartmentState(ApartmentState.STA);
		}
		thread.Start();
		return thread;
	}

	public void Wait(TimeSpan waitTime)
	{
		Wait(waitTime, null);
	}

	public bool Wait(TimeSpan waitTime, Checker<bool> returnIfTrue)
	{
		return Wait(waitTime, returnIfTrue, valueToWaitFor: true);
	}

	public ReturnType Wait<ReturnType>(TimeSpan waitTime, Checker<ReturnType> checker, ReturnType valueToWaitFor)
	{
		bool flag = waitTime.TotalSeconds > 1.0;
		bool flag2 = waitTime.TotalMilliseconds < 50.0;
		DateTime now = DateTime.Now;
		ReturnType result = default(ReturnType);
		while (DateTime.Now.Subtract(now).TotalMilliseconds < waitTime.TotalMilliseconds)
		{
			if (checker != null)
			{
				result = checker.Invoke();
				if (result.Equals(valueToWaitFor))
				{
					return result;
				}
			}
			if (flag)
			{
				System.Threading.Thread.Sleep((int)TimeSpan.FromMilliseconds(100.0).TotalMilliseconds);
			}
			else if (!flag2)
			{
				System.Threading.Thread.Sleep((int)TimeSpan.FromMilliseconds(10.0).TotalMilliseconds);
			}
		}
		return result;
	}

	public Checker<ReturnType> AddDelay<ReturnType>(Checker<ReturnType> checker, ReturnType valueToWaitFor, TimeSpan delay)
	{
		return () => DelayedCheck(checker, valueToWaitFor, delay);
	}

	private ReturnType DelayedCheck<ReturnType>(Checker<ReturnType> checker, ReturnType valueToWaitFor, TimeSpan delay)
	{
		ReturnType result = checker.Invoke();
		if (!result.Equals(valueToWaitFor))
		{
			Wait(delay);
		}
		return result;
	}

	public IThreadLocked CreateLock(object locker, Func<dynamic> getter, Action<dynamic> setter)
	{
		return (IThreadLocked)(object)new ThreadLocker(locker, getter, setter);
	}

	public void DelayedCallback(ThreadStart task, TimeSpan delay)
	{
		string text = Smart.File.Uuid();
		Timer item = new Timer(CallBackHelper, text, (int)delay.TotalMilliseconds, -1);
		timers[text] = new Tuple<Timer, ThreadStart>(item, task);
	}

	private void CallBackHelper(object state)
	{
		string text = state.ToString();
		Smart.Log.Debug(TAG, "Calling delayed thread task: " + text);
		ThreadStart item = timers[text].Item2;
		Timer item2 = timers[text].Item1;
		timers.Remove(text);
		try
		{
			item();
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Error running delayed thread: " + ex.Message);
			Smart.Log.Debug(TAG, ex.ToString());
		}
		finally
		{
			item2.Dispose();
		}
	}
}
