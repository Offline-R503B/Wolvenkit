using WolvenKit.RED4.CR2W.Reflection;
using FastMember;
using static WolvenKit.RED4.CR2W.Types.Enums;

namespace WolvenKit.RED4.CR2W.Types
{
	[REDMeta]
	public class gameuiAdvertGlitchEvent : redEvent
	{
		private CFloat _glitchValue;

		[Ordinal(0)] 
		[RED("glitchValue")] 
		public CFloat GlitchValue
		{
			get => GetProperty(ref _glitchValue);
			set => SetProperty(ref _glitchValue, value);
		}

		public gameuiAdvertGlitchEvent(IRed4EngineFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
