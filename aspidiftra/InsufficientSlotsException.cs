using System;

namespace Aspidiftra
{
	/// <summary>
	///   Exception that is thrown when we want to show N lines of text, but
	///   there fewer than N text slots available.
	/// </summary>
	public class InsufficientSlotsException : Exception
	{
		internal InsufficientSlotsException(int requested, int available) : base(
			$"Cannot position the {requested} rows of text into the {available} available slots.")
		{
			RequestedSlots = requested;
			AvailableSlots = available;
		}

		public int RequestedSlots { get; }
		public int AvailableSlots { get; }
	}
}