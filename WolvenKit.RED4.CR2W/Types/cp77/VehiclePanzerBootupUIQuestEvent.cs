using WolvenKit.RED4.CR2W.Reflection;
using FastMember;
using static WolvenKit.RED4.CR2W.Types.Enums;

namespace WolvenKit.RED4.CR2W.Types
{
	[REDMeta]
	public class VehiclePanzerBootupUIQuestEvent : redEvent
	{
		private CEnum<panzerBootupUI> _mode;

		[Ordinal(0)] 
		[RED("mode")] 
		public CEnum<panzerBootupUI> Mode
		{
			get => GetProperty(ref _mode);
			set => SetProperty(ref _mode, value);
		}

		public VehiclePanzerBootupUIQuestEvent(IRed4EngineFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
