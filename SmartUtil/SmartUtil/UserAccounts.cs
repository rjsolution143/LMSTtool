using System;
using System.Collections.Generic;
using ISmart;

namespace SmartUtil;

public class UserAccounts : IUserAccounts
{
	private string TAG => GetType().FullName;

	public List<IUserAccount> Users { get; private set; }

	public IUserAccount CurrentUser { get; private set; }

	public string AdminHash { get; private set; }

	public bool AdminSet => AdminHash.ToString() != string.Empty;

	private static UserAccount BlankUserInfo => new UserAccount("00000", "Unknown User", "1234", "Unknown Question", "Unknown");

	public IUserAccount BlankUser => (IUserAccount)(object)BlankUserInfo;

	public UserAccounts()
	{
		Users = new List<IUserAccount>();
		CurrentUser = (IUserAccount)(object)BlankUserInfo;
		AdminHash = string.Empty;
	}

	public void Load()
	{
		string usersContent = Smart.Rsd.GetUsersContent();
		if (string.IsNullOrWhiteSpace(usersContent))
		{
			Users = new List<IUserAccount>();
			CurrentUser = (IUserAccount)(object)BlankUserInfo;
			AdminHash = string.Empty;
			Smart.Log.Debug(TAG, "User accounts file is empty, using blank user info");
			return;
		}
		if (!Smart.Security.VerifyCheck(usersContent))
		{
			throw new FormatException("The user accounts file is corrupt or incorrectly formatted.");
		}
		dynamic val = Smart.Json.Load(usersContent);
		AdminHash = val.AdminHash;
		if (AdminHash == null)
		{
			AdminHash = string.Empty;
		}
		Users = new List<IUserAccount>();
		foreach (dynamic item2 in val.Users)
		{
			UserAccount item = new UserAccount(item2);
			Users.Add((IUserAccount)(object)item);
		}
		string userQualityContent = Smart.Rsd.GetUserQualityContent();
		if (string.IsNullOrWhiteSpace(userQualityContent))
		{
			Smart.Log.Debug(TAG, "User quality file is empty, using blank quality info");
			return;
		}
		if (!Smart.Security.VerifyCheck(userQualityContent))
		{
			throw new FormatException("The user quality file is corrupt or incorrectly formatted.");
		}
		SortedList<string, UserAccount> sortedList = new SortedList<string, UserAccount>();
		foreach (UserAccount user in Users)
		{
			sortedList[user.ID] = user;
		}
		dynamic val2 = Smart.Json.Load(userQualityContent);
		foreach (dynamic item3 in val2.Users)
		{
			string key = item3.ID.ToString();
			if (sortedList.ContainsKey(key))
			{
				UserAccount userAccount2 = sortedList[key];
				int qualityPoints = int.Parse(item3.QualityPoints);
				userAccount2.QualityPoints = qualityPoints;
			}
		}
		Smart.Log.Debug(TAG, "Loaded user quality");
	}

	public void SaveQuality()
	{
		SortedList<string, object> sortedList = new SortedList<string, object>();
		List<SortedList<string, string>> list = new List<SortedList<string, string>>();
		foreach (UserAccount user in Users)
		{
			SortedList<string, string> sortedList2 = new SortedList<string, string>();
			sortedList2["ID"] = user.ID;
			sortedList2["QualityPoints"] = user.QualityPoints.ToString();
			list.Add(sortedList2);
		}
		sortedList["Users"] = list;
		sortedList["Verify"] = "VERIFY_TOKEN_LMST_SECURITY_XXXXX";
		string text = Smart.Json.Dump((object)sortedList);
		text = Smart.Security.VerifyCalc(text);
		Smart.Rsd.SaveUserQualityContent(text);
		Smart.Log.Debug(TAG, "Saved user quality");
	}

	public void Save()
	{
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		SortedList<string, object> sortedList = new SortedList<string, object>();
		sortedList["AdminHash"] = AdminHash;
		List<SortedList<string, string>> list = new List<SortedList<string, string>>();
		foreach (UserAccount user in Users)
		{
			SortedList<string, string> sortedList2 = new SortedList<string, string>();
			sortedList2["ID"] = user.ID;
			sortedList2["UserName"] = user.UserName;
			sortedList2["FullName"] = user.FullName;
			sortedList2["PinHash"] = user.PinHash;
			sortedList2["Question"] = user.Question;
			sortedList2["AnswerHash"] = user.AnswerHash;
			list.Add(sortedList2);
		}
		sortedList["Users"] = list;
		sortedList["Verify"] = "VERIFY_TOKEN_LMST_SECURITY_XXXXX";
		string text = Smart.Json.Dump((object)sortedList);
		text = Smart.Security.VerifyCalc(text);
		Smart.Rsd.UploadUsers(text, true);
		Smart.Log.Debug(TAG, "Saved user accounts");
	}

	private void Refresh()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		string empty = string.Empty;
		if ((int)Smart.Rsd.DownloadUsers(ref empty) != 0)
		{
			throw new NotSupportedException("Cannot refresh user.json data");
		}
		if (empty.Trim() == string.Empty)
		{
			Smart.Log.Warning(TAG, "No users.json content downloaded");
			return;
		}
		Smart.Rsd.SaveUsersContent(empty);
		Load();
	}

	public void LockAdmin(string adminCode)
	{
		Refresh();
		if (AdminSet)
		{
			throw new AccessViolationException("Admin code is already set");
		}
		AdminHash = Smart.Security.Hash(adminCode.Trim());
	}

	public void AddUser(string userName, string fullName, string pin, string question, string answer)
	{
		if (userName == BlankUser.UserName)
		{
			throw new NotSupportedException($"Illegal user name {userName}");
		}
		Refresh();
		foreach (UserAccount user in Users)
		{
			if (user.UserName.Trim().ToLowerInvariant() == userName.Trim().ToLowerInvariant())
			{
				throw new NotSupportedException(string.Format("Cannot add user that already exists", userName));
			}
		}
		UserAccount item = new UserAccount(userName, fullName, pin, question, answer);
		Users.Add((IUserAccount)(object)item);
		Smart.Log.Debug(TAG, $"Added user {userName}");
	}

	public void ResetPin(string adminCode, string userName)
	{
		Refresh();
		if (!Smart.Security.HashCheck(adminCode, AdminHash))
		{
			throw new AccessViolationException("Admin code is invalid");
		}
		UserAccount userAccount = null;
		bool flag = false;
		foreach (UserAccount user in Users)
		{
			if (user.UserName.Trim().ToLowerInvariant() == userName.Trim().ToLowerInvariant())
			{
				userAccount = user;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			throw new NotSupportedException($"Could not find user {userName}");
		}
		UserAccount item = new UserAccount(userAccount.ID, userAccount.UserName, userAccount.FullName, Smart.Security.Hash(string.Empty), userAccount.Question, userAccount.AnswerHash);
		Users.Remove((IUserAccount)(object)userAccount);
		Users.Add((IUserAccount)(object)item);
		Smart.Log.Debug(TAG, $"Reset PIN for user {userName}");
	}

	public void RemoveUser(string adminCode, string userName)
	{
		Refresh();
		if (!Smart.Security.HashCheck(adminCode, AdminHash))
		{
			throw new AccessViolationException("Admin code is invalid");
		}
		foreach (UserAccount item in new List<IUserAccount>(Users))
		{
			if (item.UserName.Trim().ToLowerInvariant() == userName.Trim().ToLowerInvariant())
			{
				Users.Remove((IUserAccount)(object)item);
				Smart.Log.Debug(TAG, $"Removed user {userName}");
				return;
			}
		}
		throw new NotSupportedException($"Could not find user {userName}");
	}

	public void ChangePin(string userName, string pin, string answer, string newPin)
	{
		Refresh();
		UserAccount userAccount = null;
		bool flag = false;
		foreach (UserAccount user in Users)
		{
			if (user.UserName.Trim().ToLowerInvariant() == userName.Trim().ToLowerInvariant())
			{
				userAccount = user;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			throw new NotSupportedException($"Could not find user {userName}");
		}
		if (!userAccount.InvalidPin)
		{
			if (Smart.Security.HashCheck(pin, userAccount.PinHash))
			{
				Smart.Log.Debug(TAG, $"Validated user {userName} PIN");
			}
			else
			{
				if (!Smart.Security.HashCheck(answer, userAccount.AnswerHash))
				{
					throw new AccessViolationException("No valid PIN or security answer entered");
				}
				Smart.Log.Debug(TAG, $"Validated user {userName} security answer");
			}
		}
		else
		{
			Smart.Log.Debug(TAG, $"Setting user {userName} PIN to valid value");
		}
		UserAccount item = new UserAccount(userAccount.ID, userAccount.UserName, userAccount.FullName, Smart.Security.Hash(newPin), userAccount.Question, userAccount.AnswerHash);
		Users.Remove((IUserAccount)(object)userAccount);
		Users.Add((IUserAccount)(object)item);
		Smart.Log.Debug(TAG, $"Changed PIN for user {userName}");
	}

	public void LogIn(string userName, string pin)
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Refresh();
		}
		catch (Exception ex)
		{
			Smart.Log.Error(TAG, "Could not download latest users.json file: " + ex.ToString());
		}
		foreach (UserAccount item in new List<IUserAccount>(Users))
		{
			if (item.UserName.Trim().ToLowerInvariant() == userName.Trim().ToLowerInvariant())
			{
				if (item.InvalidPin || !Smart.Security.HashCheck(pin, item.PinHash))
				{
					throw new AccessViolationException("PIN check failed");
				}
				string s = string.Empty;
				if ((int)Smart.Rsd.GetGamificationPoint(item.ID, ref s) != 0)
				{
					Smart.Log.Error(TAG, "Could not read user's point total");
					s = "0";
				}
				CurrentUser = (IUserAccount)(object)item;
				CurrentUser.ServerPoints = long.Parse(s);
				return;
			}
		}
		throw new NotSupportedException($"Could not find user {userName}");
	}

	public void LogOut()
	{
		CurrentUser = BlankUser;
	}
}
