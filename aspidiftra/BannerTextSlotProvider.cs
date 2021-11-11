using System.Collections.Generic;
using System.Collections.Immutable;

namespace Aspidiftra
{
	/// <summary>
	///   Text slot provider for banner watermarks.
	/// </summary>
	internal class BannerTextSlotProvider : ITextSlotProvider
	{
		private readonly IImmutableList<TextSlot> _evenSlots;
		private readonly IImmutableList<TextSlot> _oddSlots;

		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="oddSlots">Calculated odd slots.</param>
		/// <param name="evenSlots">Calculated even slots.</param>
		internal BannerTextSlotProvider(IImmutableList<TextSlot> oddSlots, IImmutableList<TextSlot> evenSlots)
		{
			_oddSlots = oddSlots;
			_evenSlots = evenSlots;
		}

		/// <summary>
		///   Returns the suitable set of text slots for the given number of strings.
		/// </summary>
		/// <param name="amount">Number of strings we want to place on the page.</param>
		/// <returns>
		///   The suitable set of text slots, if we have enough. If not, this
		///   function will throw an <see cref="InsufficientSlotsException" /> exception.
		/// </returns>
		public IEnumerable<TextSlot> GetTextSlots(int amount)
		{
			// If we're asked for an odd number of slots, take the middle selection from _oddSlots.
			// If we're asked for an even number of slots, take the middle selection from _evenSlots.
			var slots = amount % 2 == 1 ? _oddSlots : _evenSlots;
			var availableSlots = slots.Count;
			if (availableSlots < amount)
				throw new InsufficientSlotsException(amount, availableSlots);
			return slots.Mid(amount);
		}
	}
}