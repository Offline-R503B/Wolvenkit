using WolvenKit.RED4.CR2W.Reflection;
using FastMember;
using static WolvenKit.RED4.CR2W.Types.Enums;

namespace WolvenKit.RED4.CR2W.Types
{
	[REDMeta]
	public class UIVendorItemsSoldEvent : redEvent
	{
		private CInt32 _requestID;
		private CArray<gameItemID> _itemsID;
		private CArray<CInt32> _quantity;
		private CArray<CInt32> _piecesPrice;

		[Ordinal(0)] 
		[RED("requestID")] 
		public CInt32 RequestID
		{
			get => GetProperty(ref _requestID);
			set => SetProperty(ref _requestID, value);
		}

		[Ordinal(1)] 
		[RED("itemsID")] 
		public CArray<gameItemID> ItemsID
		{
			get => GetProperty(ref _itemsID);
			set => SetProperty(ref _itemsID, value);
		}

		[Ordinal(2)] 
		[RED("quantity")] 
		public CArray<CInt32> Quantity
		{
			get => GetProperty(ref _quantity);
			set => SetProperty(ref _quantity, value);
		}

		[Ordinal(3)] 
		[RED("piecesPrice")] 
		public CArray<CInt32> PiecesPrice
		{
			get => GetProperty(ref _piecesPrice);
			set => SetProperty(ref _piecesPrice, value);
		}

		public UIVendorItemsSoldEvent(IRed4EngineFile cr2w, CVariable parent, string name) : base(cr2w, parent, name) { }
	}
}
