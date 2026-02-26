using System;

namespace ISmart;

public interface IThreadLocked : IDisposable
{
	dynamic Data { get; set; }

	void Close();
}
