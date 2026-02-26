using System;
using System.Collections.Generic;
using System.Threading;

namespace ISmart;

public interface IThread
{
	SortedList<string, object> LockCache { get; }

	void Run(ThreadStart task);

	void Run(ThreadStart task, bool setSta);

	void Run<ReturnValue>(Func<ReturnValue> task, Action<ReturnValue> callback);

	void Run<ReturnValue>(Func<ReturnValue> task, Action<ReturnValue> callback, bool setSta);

	ReturnValue RunAndWait<ReturnValue>(Func<ReturnValue> task);

	ReturnValue RunAndWait<ReturnValue>(Func<ReturnValue> task, bool setSta);

	Thread RunThread(ThreadStart task);

	Thread RunThread(ThreadStart task, bool setSta);

	void Wait(TimeSpan waitTime);

	bool Wait(TimeSpan waitTime, Checker<bool> returnIfTrue);

	ReturnType Wait<ReturnType>(TimeSpan waitTime, Checker<ReturnType> checker, ReturnType valueToWaitFor);

	Checker<ReturnType> AddDelay<ReturnType>(Checker<ReturnType> checker, ReturnType valueToWaitFor, TimeSpan delay);

	IThreadLocked CreateLock(object locker, Func<dynamic> getter, Action<dynamic> setter);

	void DelayedCallback(ThreadStart task, TimeSpan delay);
}
