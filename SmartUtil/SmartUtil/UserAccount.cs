using ISmart;

namespace SmartUtil;

public class UserAccount : IUserAccount
{
	private string TAG => GetType().FullName;

	public string ID { get; private set; }

	public string UserName { get; private set; }

	public string FullName { get; private set; }

	public string PinHash { get; private set; }

	public bool InvalidPin => Smart.Security.HashCheck(string.Empty, PinHash);

	public string Question { get; private set; }

	public string AnswerHash { get; private set; }

	public long ServerPoints { get; set; }

	public long SessionPoints { get; set; }

	public int PassStreak { get; set; }

	public int QualityPoints { get; set; }

	public decimal QualityRating
	{
		get
		{
			if (QualityPoints <= -17)
			{
				return 0m;
			}
			if (QualityPoints <= -12)
			{
				return 0.5m;
			}
			if (QualityPoints <= -8)
			{
				return 1m;
			}
			if (QualityPoints > -5)
			{
				if (QualityPoints > -2)
				{
					if (QualityPoints < 17)
					{
						if (QualityPoints < 12)
						{
							if (QualityPoints < 8)
							{
								if (QualityPoints < 5)
								{
									if (QualityPoints < 2)
									{
										return 2.5m;
									}
									return 3m;
								}
								return 3.5m;
							}
							return 4m;
						}
						return 4.5m;
					}
					return 5m;
				}
				return 2m;
			}
			return 1.5m;
		}
	}

	public UserAccount(string userName, string fullName, string pin, string question, string answer)
	{
		ID = Smart.File.Uuid();
		UserName = userName.Trim();
		FullName = fullName.Trim();
		PinHash = Smart.Security.Hash(pin.Trim());
		Question = question.Trim();
		AnswerHash = Smart.Security.Hash(answer.Trim());
		ServerPoints = 0L;
		SessionPoints = 0L;
		PassStreak = 0;
		QualityPoints = 0;
	}

	public UserAccount(string id, string userName, string fullName, string pinHash, string question, string answerHash)
	{
		ID = id;
		UserName = userName;
		FullName = fullName;
		PinHash = pinHash;
		Question = question;
		AnswerHash = answerHash;
		ServerPoints = 0L;
		SessionPoints = 0L;
		PassStreak = 0;
		QualityPoints = 0;
	}

	public UserAccount(dynamic content)
	{
		ID = (content.ID = content.ID);
		UserName = content.UserName;
		FullName = content.FullName;
		PinHash = content.PinHash;
		Question = content.Question;
		AnswerHash = content.AnswerHash;
		ServerPoints = 0L;
		SessionPoints = 0L;
		PassStreak = 0;
		QualityPoints = 0;
	}
}
