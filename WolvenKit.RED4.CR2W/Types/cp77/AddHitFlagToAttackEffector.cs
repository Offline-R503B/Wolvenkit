using WolvenKit.RED4.CR2W.Reflection;
using FastMember;
using static WolvenKit.RED4.CR2W.Types.Enums;

namespace WolvenKit.RED4.CR2W.Types
{
	[REDMeta]
	public class AddHitFlagToAttackEffector : ModifyAttackEffector
	{
		private CEnum<hitFlag> _hitFlag;

		[Ordinal(0)] 
		[RED("hitFlag")] 
		public CEnum<hitFlag> HitFlag
		{
			get => GetProperty(ref _hitFlag);
			set => SetProperty(ref _hitFlag, value);
		}

		public AddHitFlagToAttackEffector(IRed4EngineFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
