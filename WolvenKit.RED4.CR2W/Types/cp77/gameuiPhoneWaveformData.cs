using WolvenKit.RED4.CR2W.Reflection;
using FastMember;
using static WolvenKit.RED4.CR2W.Types.Enums;

namespace WolvenKit.RED4.CR2W.Types
{
	[REDMeta]
	public class gameuiPhoneWaveformData : IScriptable
	{
		private CArray<Vector4> _points;

		[Ordinal(0)] 
		[RED("points")] 
		public CArray<Vector4> Points
		{
			get => GetProperty(ref _points);
			set => SetProperty(ref _points, value);
		}

		public gameuiPhoneWaveformData(IRed4EngineFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
