namespace ISmart;

public enum ResponseCode : byte
{
	InvalidRequestDataLength = 0,
	InadequateSecurityLevel = 1,
	NotSupportedForCurrentProtocol = 2,
	NotSupportedForCurrentMode = 3,
	InvalidOpCode = 4,
	InvalidParameter = 5,
	GenericResponse = 6,
	GenericFailure = 7,
	CannotAllocateMemory = 10,
	InternalTaskError = 11,
	TimedOutWaitingForSoftwareComponent = 12,
	NotSupportedForCurrentSubMode = 13,
	DataLengthMismatch = 14,
	IrrecoverableError = 15,
	MuxChannelFailure = 17,
	InvalidRequestDataLengthAscii = 128,
	NotSupportedForCurrentModeAscii = 131,
	InvalidOpCodeAscii = 132,
	InvalidParameterAscii = 133
}
