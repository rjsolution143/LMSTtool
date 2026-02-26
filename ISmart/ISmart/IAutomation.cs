using System;

namespace ISmart;

public interface IAutomation : IDisposable
{
	bool Running { get; }

	void Start();

	void Stop();
}
