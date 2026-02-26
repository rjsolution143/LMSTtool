using System;
using ISmart;

namespace SmartUtil;

public class Alert : IAlert, IComparable<IAlert>
{
	private string TAG => GetType().FullName;

	public string ID { get; private set; }

	public AlertTypes AlertType { get; private set; }

	public AlertStatus Alertstatus { get; set; }

	public string Title { get; private set; }

	public string Message { get; private set; }

	public DateTime Timestamp { get; private set; }

	public DateTime TimeStart { get; set; }

	public DateTime TimeEnd { get; set; }

	public string LanguageCode { get; private set; }

	public bool IsRead { get; set; }

	public Alert(string id, AlertTypes alerttype, string title, string message, DateTime timestamp, DateTime timestart, DateTime timeend, string languageCode)
		: this(id, alerttype, title, message, timestamp, timestart, timeend, languageCode, isRead: false)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


	public Alert(string id, AlertTypes alerttype, string title, string message, DateTime timestamp, DateTime timestart, DateTime timeend, string languageCode, bool isRead)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		ID = id;
		AlertType = alerttype;
		Title = title;
		Message = message;
		Timestamp = timestamp;
		TimeStart = timestart;
		TimeEnd = timeend;
		LanguageCode = languageCode.ToLowerInvariant();
		IsRead = isRead;
	}

	public int CompareTo(IAlert other)
	{
		return Timestamp.CompareTo(other.Timestamp);
	}
}
