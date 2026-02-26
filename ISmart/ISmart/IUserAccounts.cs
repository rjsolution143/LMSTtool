using System.Collections.Generic;

namespace ISmart;

public interface IUserAccounts
{
	IUserAccount BlankUser { get; }

	List<IUserAccount> Users { get; }

	IUserAccount CurrentUser { get; }

	string AdminHash { get; }

	bool AdminSet { get; }

	void Load();

	void Save();

	void SaveQuality();

	void LockAdmin(string adminCode);

	void AddUser(string userName, string fullName, string pin, string question, string answer);

	void ResetPin(string adminCode, string userName);

	void RemoveUser(string adminCode, string userName);

	void ChangePin(string userName, string pin, string answer, string newPin);

	void LogIn(string userName, string pin);

	void LogOut();
}
