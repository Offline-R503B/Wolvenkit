using System.IO;
using System.Runtime.Serialization;
using WolvenKit.RED3.CR2W.Reflection;
using FastMember;
using static WolvenKit.RED3.CR2W.Types.Enums;


namespace WolvenKit.RED3.CR2W.Types
{
	[DataContract(Namespace = "")]
	[REDMeta]
	public class SBlockDesc : CVariable
	{
		[Ordinal(1)] [RED("ock")] 		public CPtr<CQuestGraphBlock> Ock { get; set;}

		[Ordinal(2)] [RED("putName")] 		public CName PutName { get; set;}

		public SBlockDesc(IRed3EngineFile cr2w, CVariable parent, string name) : base(cr2w, parent, name){ }

		

		public override void Read(BinaryReader file, uint size) => base.Read(file, size);

		public override void Write(BinaryWriter file) => base.Write(file);

	}
}