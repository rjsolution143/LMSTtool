namespace ISmart;

public struct Login
{
	public string UserName { get; set; }

	public string Password { get; set; }

	public static Login Default => new Login(string.Empty, string.Empty);

	public Login(string userName, string password)
	{
		UserName = userName;
		Password = password;
	}
}
