using System.Collections.Generic;
using System.Linq;

namespace Aspidiftra
{
	internal class PageEdgeTextSlotProvider : ITextSlotProvider
	{
		private readonly PageEdgePosition _pageEdgePosition;
		private readonly bool _reverseDirection;
		private readonly IReadOnlyList<TextSlot> _slots;

		internal PageEdgeTextSlotProvider(IReadOnlyList<TextSlot> slots, PageEdgePosition pageEdgePosition,
			bool reverseDirection)
		{
			_reverseDirection = reverseDirection;
			_pageEdgePosition = pageEdgePosition;
			_slots = slots;
		}

		public IEnumerable<TextSlot> GetTextSlots(int amount)
		{
			if (amount > _slots.Count)
				throw new InsufficientSlotsException(amount, _slots.Count);
			return _pageEdgePosition switch
			{
				// Bottom is a special case, because, unlike the others, the top edge of the text is, by
				// default, not along the page edge.
				PageEdgePosition.Bottom => _reverseDirection ? _slots.Take(amount) : _slots.Take(amount).Reverse(),
				_ => _reverseDirection ? _slots.Take(amount).Reverse() : _slots.Take(amount)
			};
		}
	}
}