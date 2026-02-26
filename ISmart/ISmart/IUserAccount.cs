namespace ISmart;

public interface IUserAccount
{
	string ID { get; }

	string UserName { get; }

	string FullName { get; }

	string PinHash { get; }

	bool InvalidPin { get; }

	string Question { get; }

	string AnswerHash { get; }

	long ServerPoints { get; set; }

	long SessionPoints { get; set; }

	int PassStreak { get; set; }

	int QualityPoints { get; set; }

	decimal QualityRating { get; }
}
