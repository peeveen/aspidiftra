using System;
using Aspidiftra.Geometry;

namespace Aspidiftra
{
	internal class AllocatedTextSlot
	{
		private readonly Justification _justification;

		internal AllocatedTextSlot(MeasuredString text, TextSlot slot, Justification justification)
		{
			Text = text;
			Slot = slot;
			_justification = justification;
			TextFits = text.Length <= slot.Width;
		}

		internal MeasuredString Text { get; }
		internal TextSlot Slot { get; }
		internal bool TextFits { get; }

		internal Offset JustificationOffset
		{
			get
			{
				var leftJustifiedAlready = Slot.Angle < Angle.RadiansPi;
				var rightJustifiedAlready = Slot.Angle >= Angle.RadiansPi;
				var spareSlotSpace = Slot.Width - Text.Length;
				return _justification switch
				{
					Justification.Left when leftJustifiedAlready => Offset.None,
					Justification.Right when rightJustifiedAlready => Offset.None,
					Justification.Centre =>
						new Offset(Math.Abs(spareSlotSpace / 2.0 * Slot.Angle.Cos),
							Math.Abs(spareSlotSpace / 2.0 * Slot.Angle.Sin)),
					_ =>
						// Due to the angle, text is appearing left or right justified by default,
						// but we want it the opposite way.
						new Offset(Math.Abs(spareSlotSpace * Slot.Angle.Cos), Math.Abs(spareSlotSpace * Slot.Angle.Sin))
				};
			}
		}
	}
}