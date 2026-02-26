using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LabelManager2;

[ComImport]
[CompilerGenerated]
[InterfaceType(2)]
[DefaultMember("_Name")]
[Guid("3624B9C6-9E5D-11D3-A896-00C04F324E22")]
[TypeIdentifier]
public interface IDocument
{
	void _VtblGap1_4();

	[DispId(0)]
	string _Name
	{
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(0)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		[DispId(0)]
		set;
	}
}
