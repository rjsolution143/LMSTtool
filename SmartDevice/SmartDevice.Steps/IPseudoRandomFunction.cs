using System;

namespace SmartDevice.Steps;

public interface IPseudoRandomFunction : IDisposable
{
	int HashSize { get; }

	byte[] Transform(byte[] input);
}
