using System.Collections.Generic;

namespace ISmart;

public interface IAlerts
{
	List<IAlert> Messages { get; }

	IAlert CurrentAlert { get; set; }

	void MarkRead(IAlert alert);

	void Refresh();
}
