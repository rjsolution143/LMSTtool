using System;

namespace ISmart;

public interface IAlert : IComparable<IAlert>
{
	string ID { get; }

	AlertTypes AlertType { get; }

	AlertStatus Alertstatus { get; set; }

	string Title { get; }

	string Message { get; }

	DateTime Timestamp { get; }

	DateTime TimeStart { get; set; }

	DateTime TimeEnd { get; set; }

	string LanguageCode { get; }

	bool IsRead { get; set; }
}
