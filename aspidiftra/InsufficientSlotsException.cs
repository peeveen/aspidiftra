﻿using System;

namespace Aspidiftra
{
	/// <summary>
	///   Exception that is thrown when a request is made to perform an operation (such
	///   as text position, slot calculation, whatever) where there is not enough space
	///   on the page.
	/// </summary>
	public class InsufficientSlotsException : Exception
	{
		public InsufficientSlotsException(int requested, int available) : base(
			$"Cannot position the {requested} rows of text into the {available} available slots.")
		{
			RequestedSlots = requested;
			AvailableSlots = available;
		}

		public int RequestedSlots { get; }
		public int AvailableSlots { get; }
	}
}