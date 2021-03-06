using WolvenKit.RED4.CR2W.Reflection;
using FastMember;
using static WolvenKit.RED4.CR2W.Types.Enums;

namespace WolvenKit.RED4.CR2W.Types
{
	[REDMeta]
	public class VRoomFeed : redEvent
	{
		private CBool _on;

		[Ordinal(0)] 
		[RED("On")] 
		public CBool On
		{
			get => GetProperty(ref _on);
			set => SetProperty(ref _on, value);
		}

		public VRoomFeed(IRed4EngineFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
