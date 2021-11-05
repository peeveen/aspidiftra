using System;
using Aspidiftra.Geometry;

namespace Aspidiftra
{
	internal class AssignedTextSlot
	{
		// Handle constant for a zero offset.
		private static readonly (double, double) NoOffset = (0.0, 0.0);

		private readonly Justification _justification;

		internal AssignedTextSlot(MeasuredString text, TextSlot slot, Justification justification)
		{
			Text = text;
			Slot = slot;
			_justification = justification;
		}

		internal MeasuredString Text { get; }
		internal TextSlot Slot { get; }

		internal (double X, double Y) JustificationOffset
		{
			get
			{
				var leftJustifiedAlready = Slot.Angle < Angle.RadiansPi;
				var rightJustifiedAlready = Slot.Angle >= Angle.RadiansPi;
				var spareSlotSpace = Slot.Width - Text.Length;
				return _justification switch
				{
					Justification.Left when leftJustifiedAlready => NoOffset,
					Justification.Right when rightJustifiedAlready => NoOffset,
					Justification.Centre =>
						(Math.Abs(spareSlotSpace / 2.0 * Slot.Angle.Cos), Math.Abs(spareSlotSpace / 2.0 * Slot.Angle.Sin)),
					_ =>
						// Due to the angle, text is appearing left or right justified by default,
						// but we want it the opposite way.
						(Math.Abs(spareSlotSpace * Slot.Angle.Cos), Math.Abs(spareSlotSpace * Slot.Angle.Sin))
				};
			}
		}
	}
}