using System;

namespace ISmart;

public interface IBase : IDisposable
{
	InterfaceType Load<InterfaceType>(string typeName);

	InterfaceType LoadCached<InterfaceType>();

	InterfaceType LoadNew<InterfaceType>(string typeName);

	void PrintBase();
}
