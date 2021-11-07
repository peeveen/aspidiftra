using System.Collections.Generic;
using System.Collections.Immutable;

namespace Aspidiftra
{
	internal class BannerTextSlotProvider : ITextSlotProvider
	{
		private readonly IImmutableList<TextSlot> _evenSlots;
		private readonly IImmutableList<TextSlot> _oddSlots;

		internal BannerTextSlotProvider(IImmutableList<TextSlot> oddSlots, IImmutableList<TextSlot> evenSlots)
		{
			_oddSlots = oddSlots;
			_evenSlots = evenSlots;
		}

		public IEnumerable<TextSlot> GetTextSlots(int amount)
		{
			// If we're asked for an odd number of slots, take the middle selection from _oddSlots.
			// If we're asked for an even number of slots, take the middle selection from _evenSlots.
			var slots = amount % 2 == 1 ? _oddSlots : _evenSlots;
			var availableSlots = slots.Count;
			if (availableSlots < amount)
				throw new InsufficientSlotsException(amount, availableSlots);
			var topAndTail = (availableSlots - amount) / 2;
			var topped = slots.RemoveRange(0, topAndTail);
			var tailed = topped.RemoveRange(amount, topAndTail);
			return tailed;
		}
	}
}