using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ISmart;

public interface IPrompt
{
	IDevice Device { get; set; }

	Result CitPrompt(string title, string type, string text);

	Result CitPrompt(string title, string type, string text, string imagePath);

	DialogResult MessageBox(string title, string content, MessageBoxButtons ButtonType, MessageBoxIcon IconType);

	void CloseMessageBox();

	DialogResult SearchSelect(string title, string text, List<string> choices, out string choice);

	DialogResult ComplaintSelect(string title, string complaintPrompt, SortedList<string, string> complaintNames, SortedList<string, string> complaintIcons, string symptomPrompt, SortedList<string, string> symptomNames, SortedList<string, string> symptomIcons, out List<string> complaintChoice, out List<string> symptomChoice);

	DialogResult FSBList(string title, List<string> columns, string buttonText, List<string> deviceInfo, SortedList<string, Tuple<string, string>> fsbs, out string fsbChoice);

	DialogResult FSBEntry(string title, List<string> columns, List<string> fsbInfo, List<Tuple<string, string, string, string, List<string>>> actions, out List<int> actionChoices);

	string InputBox(string title, string text, string waterMark = null);

	string SlectFile(string title, string initialDir);

	string SlectFile(string title, string initialDir, string filter);

	void ShowImage(string title, Image image);

	SortedList<string, string> WebBrowser(string url);

	SortedList<string, string> WebBrowser(string url, string postData, SortedList<string, string> headers);

	Result FrontcolorPrompt(string title, string type, string text, string imagePath, string frontColorImagePath);

	string FrontColorInputBox(string title, string text, List<string> imagePaths, string waterMark = null);
}
