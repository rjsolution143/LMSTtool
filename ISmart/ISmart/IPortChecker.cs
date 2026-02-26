using System;

namespace ISmart;

public interface IPortChecker : IDisposable
{
	bool Running { get; set; }
}
