namespace ISmart;

public interface IFtmResponse
{
	byte ErrorCode { get; }

	byte[] Data { get; }

	byte[] Raw { get; }

	IFtmResponse UnSolicitedResponse { get; set; }

	string ToHex();
}
