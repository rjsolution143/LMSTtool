using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ISmart;

public interface ITestUI
{
	void Test();

	DialogResult SearchSelect(string title, string text, List<string> choices, out string choice);

	DialogResult ComplaintSelect(string title, string complaintPrompt, SortedList<string, string> complaintNames, SortedList<string, string> complaintIcons, string symptomPrompt, SortedList<string, string> symptomNames, SortedList<string, string> symptomIcons, out List<string> complaintChoice, out List<string> symptomChoice);

	DialogResult FSBList(string title, List<string> columns, string buttonText, List<string> deviceInfo, SortedList<string, Tuple<string, string>> fsbs, out string fsbChoice);

	DialogResult FSBEntry(string title, List<string> columns, List<string> deviceInfo, List<Tuple<string, string, string, string, List<string>>> actions, out List<int> actionChoices);

	SortedList<string, string> WebBrowser(string url);

	SortedList<string, string> WebBrowser(string url, string postData, SortedList<string, string> headers);

	void BulkMotoFocus();
}
