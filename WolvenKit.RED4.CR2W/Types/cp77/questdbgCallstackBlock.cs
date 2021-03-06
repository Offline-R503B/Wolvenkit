using WolvenKit.RED4.CR2W.Reflection;
using FastMember;
using static WolvenKit.RED4.CR2W.Types.Enums;

namespace WolvenKit.RED4.CR2W.Types
{
	[REDMeta]
	public class questdbgCallstackBlock : CVariable
	{
		private CUInt64 _id;
		private CUInt64 _parentId;

		[Ordinal(0)] 
		[RED("id")] 
		public CUInt64 Id
		{
			get => GetProperty(ref _id);
			set => SetProperty(ref _id, value);
		}

		[Ordinal(1)] 
		[RED("parentId")] 
		public CUInt64 ParentId
		{
			get => GetProperty(ref _parentId);
			set => SetProperty(ref _parentId, value);
		}

		public questdbgCallstackBlock(IRed4EngineFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
