using WolvenKit.RED4.CR2W.Reflection;
using FastMember;
using static WolvenKit.RED4.CR2W.Types.Enums;

namespace WolvenKit.RED4.CR2W.Types
{
	[REDMeta]
	public class ToggleLightByNameEvent : ToggleLightEvent
	{
		private CName _componentName;

		[Ordinal(2)] 
		[RED("componentName")] 
		public CName ComponentName
		{
			get => GetProperty(ref _componentName);
			set => SetProperty(ref _componentName, value);
		}

		public ToggleLightByNameEvent(IRed4EngineFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
