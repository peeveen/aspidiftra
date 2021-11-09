using System.Collections.Generic;
using System.Linq;

namespace Aspidiftra
{
	/// <summary>
	/// Text slot provider for page edge watermarks.
	/// </summary>
	internal class PageEdgeTextSlotProvider : ITextSlotProvider
	{
		private readonly PageEdgePosition _pageEdgePosition;
		private readonly bool _reverseDirection;
		private readonly IReadOnlyList<TextSlot> _slots;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="slots">All calculated slots.</param>
		/// <param name="pageEdgePosition">The page edge position of the watermark.</param>
		/// <param name="reverseDirection">True if the text is running in the opposite direction.</param>
		internal PageEdgeTextSlotProvider(IReadOnlyList<TextSlot> slots, PageEdgePosition pageEdgePosition,
			bool reverseDirection)
		{
			_reverseDirection = reverseDirection;
			_pageEdgePosition = pageEdgePosition;
			_slots = slots;
		}

		/// <summary>
		/// Returns the suitable slots for the given number of strings.
		/// </summary>
		/// <param name="amount">Number of strings that we want to place.</param>
		/// <returns>The text slots to use for that number of strings.</returns>
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