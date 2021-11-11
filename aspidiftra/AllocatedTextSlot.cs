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
			EffectiveTextOrigin = CalculateEffectiveTextOrigin();
		}

		/// <summary>
		///   The effective text origin is the point that we tell Aspose.Pdf to position the text at.
		///   Due to how Aspose.Pdf does positioning of rotated text, this is not always the same as
		///   the <see cref="TextSlot.TextOrigin"/> />.
		/// </summary>
		public Point EffectiveTextOrigin { get; }

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
		///   Calculates the justification offset for this allocated text slot.
		///   The offset will "push" the text along the slot by a certain amount.
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
						new Offset(spareSlotSpace * Slot.Angle.Cos, spareSlotSpace * Slot.Angle.Sin) *
						offsetMultiplier
				};
			}
		}

		/// <summary>
		///   Calculates the effective text origin.
		/// </summary>
		/// <returns>The effective text origin.</returns>
		private Point CalculateEffectiveTextOrigin()
		{
			// Aspose.Pdf is kinda weird with how it deals with the coordinates for
			// rotated text. You don't tell it the position where, say, the lower-left
			// corner of the first character of the text should be. Instead, you need
			// to imagine the orthogonal bounding box that contains the rotated text,
			// and tell it the lower left corner of that.
			// So let's calculate where that would be!
			var xOffset = 0.0;
			var yOffset = 0.0;
			var angle = Slot.Angle;
			var height = Slot.Height;
			var width = Text.Length;
			if (angle <= Angle.Degrees90)
			{
				xOffset = height * -angle.Sin;
			}
			else if (angle <= Angle.Degrees180)
			{
				xOffset = width * angle.Cos + height * -angle.Sin;
				yOffset = height * angle.Cos;
			}
			else if (angle <= Angle.Degrees270)
			{
				xOffset = width * angle.Cos;
				yOffset = width * angle.Sin + height * angle.Cos;
			}
			else
			{
				yOffset = width * angle.Sin;
			}

			return Slot.TextOrigin + new Offset(xOffset, yOffset);
		}
	}
}