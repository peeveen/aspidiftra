using System;
using Aspidiftra.Geometry;

namespace Aspidiftra
{
	/// <summary>
	///   A text slot that has had a string allocated to it.
	/// </summary>
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
				var offsetMultiplier = _justification == Justification.Centre ? 0.5 : 1.0;
				return _justification switch
				{
					Justification.Left when leftJustifiedAlready => Offset.None,
					Justification.Right when rightJustifiedAlready => Offset.None,
					_ =>
						new Offset(Math.Abs(spareSlotSpace * Slot.Angle.Cos), Math.Abs(spareSlotSpace * Slot.Angle.Sin)) *
						offsetMultiplier
				};
			}
		}
	}
}