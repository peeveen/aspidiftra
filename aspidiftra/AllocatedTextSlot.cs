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

		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="text">Text that has been allocated to a slot.</param>
		/// <param name="slot">The slot it has been allocated to.</param>
		/// <param name="justification">The justification of the text within the slot.</param>
		internal AllocatedTextSlot(MeasuredString text, TextSlot slot, Justification justification)
		{
			Text = text;
			Slot = slot;
			_justification = justification;
			TextFits = text.Length <= slot.Width;
		}

		/// <summary>
		///   The text that has been allocated to the slot.
		/// </summary>
		internal MeasuredString Text { get; }

		/// <summary>
		///   The slot that the text has been allocated to.
		/// </summary>
		internal TextSlot Slot { get; }

		/// <summary>
		///   True if the text fits the slot, false otherwise.
		/// </summary>
		internal bool TextFits { get; }

		/// <summary>
		/// Calculates the justification offset for this allocated text slot.
		/// The offset will "push" the text along the slot by a certain amount.
		/// </summary>
		internal Offset JustificationOffset
		{
			get
			{
				// Text that is rotated between 0 and 180 degrees will automatically
				// be rendered left justified by Aspose.Pdf (i.e. the FIRST character
				// of the text will be at the coordinates we specify to render the text
				// at).
				var leftJustifiedAlready = Slot.Angle < Angle.RadiansPi;
				// Text that is rotated between 180 and 360 degrees will automatically
				// be rendered right justified by Aspose.Pdf (i.e. the LAST character
				// of the text will be at the coordinates we specify to render the text
				// at).
				var rightJustifiedAlready = Slot.Angle >= Angle.RadiansPi;
				var spareSlotSpace = Slot.Width - Text.Length;
				// If we're centering, we just want to move the text along by half of
				// spare space.
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