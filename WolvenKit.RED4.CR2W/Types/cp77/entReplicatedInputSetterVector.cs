using WolvenKit.RED4.CR2W.Reflection;
using FastMember;
using static WolvenKit.RED4.CR2W.Types.Enums;

namespace WolvenKit.RED4.CR2W.Types
{
	[REDMeta]
	public class entReplicatedInputSetterVector : entReplicatedInputSetterBase
	{
		private Vector4 _value;

		[Ordinal(2)] 
		[RED("value")] 
		public Vector4 Value
		{
			get => GetProperty(ref _value);
			set => SetProperty(ref _value, value);
		}

		public entReplicatedInputSetterVector(IRed4EngineFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
