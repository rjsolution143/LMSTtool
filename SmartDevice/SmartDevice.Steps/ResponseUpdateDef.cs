using System.Collections.Generic;

namespace SmartDevice.Steps;

public class ResponseUpdateDef
{
	public string CreatedDate;

	public Dictionary<string, ShellResponse.ShellCmdType> ExeStringToShellCmdType;

	public Dictionary<string, ShellResponse.ShellCmdStatus> MTekFlashResponseToStatus;

	public Dictionary<string, ShellResponse.ShellCmdStatus> QComFlashResponseToStatus;

	public Dictionary<string, ShellResponse.ShellCmdStatus> SPFlashResponseToStatus;

	public Dictionary<string, ShellResponse.ShellCmdStatus> QFilFlashResponseToStatus;

	public Dictionary<string, ShellResponse.ShellCmdStatus> RDFlashResponseToStatus;

	public Dictionary<string, ShellResponse.ShellCmdStatus> MTekProgResponseToStatus;

	public Dictionary<string, ShellResponse.ShellCmdStatus> MmmProgResponseToStatus;

	public Dictionary<string, ShellResponse.ShellCmdStatus> MobaProgResponseToStatus;

	public Dictionary<string, ShellResponse.ShellCmdStatus> JavaProgResponseToStatus;

	public Dictionary<string, ShellResponse.ShellCmdStatus> LMProgResponseToStatus;

	public Dictionary<string, ShellResponse.ShellCmdStatus> P410ProgResponseToStatus;

	public Dictionary<string, ShellResponse.ShellCmdStatus> QCBlankFlashResponseToStatus;

	public Dictionary<string, ShellResponse.ShellCmdStatus> ZxProgResponseToStatus;

	public Dictionary<string, ShellResponse.ShellCmdStatus> LqProgResponseToStatus;

	public Dictionary<string, ShellResponse.ShellCmdStatus> GenericResponseToStatus;
}
