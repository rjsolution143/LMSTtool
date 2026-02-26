namespace ISmart;

public interface ITestCommandResponse
{
	ResponseType Flags { get; }

	byte SequenceTag { get; }

	ushort OpCode { get; }

	ResponseCode ResponseCode { get; }

	int Length { get; }

	byte[] Data { get; }

	string DataHex { get; }

	bool Failed { get; }

	bool DataReturned { get; }

	bool Unsolicited { get; }

	bool Incomplete { get; }

	byte[] RawResponse { get; }
}
