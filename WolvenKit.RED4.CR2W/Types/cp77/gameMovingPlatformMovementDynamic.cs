using WolvenKit.RED4.CR2W.Reflection;
using FastMember;
using static WolvenKit.RED4.CR2W.Types.Enums;

namespace WolvenKit.RED4.CR2W.Types
{
	[REDMeta]
	public class gameMovingPlatformMovementDynamic : gameIMovingPlatformMovementPointToPoint
	{
		private CName _curveName;

		[Ordinal(1)] 
		[RED("curveName")] 
		public CName CurveName
		{
			get => GetProperty(ref _curveName);
			set => SetProperty(ref _curveName, value);
		}

		public gameMovingPlatformMovementDynamic(IRed4EngineFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
