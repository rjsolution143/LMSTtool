using System;
using System.Collections.Generic;
using System.IO;

namespace ISmart;

public interface ISecurity
{
	bool HasAdminRights { get; }

	string RSDUniqueID { get; set; }

	string RSDHardwareFingerprint { get; }

	string RSDStationID { get; set; }

	string RSDWeekly { get; set; }

	void Encrypt(string password, CryptoType type, Stream inputData, Stream outputData);

	void Decrypt(string password, CryptoType type, Stream inputData, Stream outputData);

	void Encrypt(byte[] key, CryptoType type, Stream inputData, Stream outputData);

	void Decrypt(byte[] key, CryptoType type, Stream inputData, Stream outputData);

	void Encrypt(byte[] key, byte[] hardcodedIv, CryptoType type, Stream inputData, Stream outputData);

	void Decrypt(byte[] key, byte[] hardcodedIv, CryptoType type, Stream inputData, Stream outputData);

	byte[] RandomBytes(int length);

	string Hash(string input);

	bool HashCheck(string input, string existingHash);

	string VerifyCalc(string content);

	bool VerifyCheck(string conent);

	string EncryptString(string plaintextString);

	string DecryptString(string encryptedString);

	bool IntegrityCheck();

	bool HostCheck();

	bool RemoteCheck();

	string SimpleHash(string input);

	void UpdateIDs();

	void SaveLogin(Login login, string filePath);

	SortedList<string, string> LoadLogin(string filePath);

	Tuple<bool, string> OsCheck();

	SortedList<string, string> StationSign();

	void ResetStation();
}
