using System;
using System.Collections.Generic;

namespace ISmart;

public interface INet
{
	string WebRequest(string url);

	TimeSpan NistTimeOffset();

	void Browser(string url);

	long CheckSize(string url);

	long CheckSize(string url, SortedList<string, string> headers);

	void WebHit(string screen);

	bool NetworkCheck();
}
