using Aspidiftra.Geometry;

namespace Aspidiftra
{
	/// <summary>
	///   A text slot. All watermark text is positioned "in" a text slot.
	/// </summary>
	public class TextSlot
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="textOrigin">
		///   Origin point for the text slot. This would be where (if the text slot was completely filled
		///   with text) the lower left corner of the first character of the text would be.
		/// </param>
		/// <param name="width">How wide the text slot is.</param>
		/// <param name="height">How tall the text slot is.</param>
		/// <param name="angle">What angle the text slot is at.</param>
		public TextSlot(Point textOrigin, double width, double height, Angle angle)
		{
			TextOrigin = textOrigin;
			Width = width;
			Height = height;
			Angle = angle;
			EffectiveTextOrigin = CalculateEffectiveTextOrigin();
		}

		/// <summary>
		///   Text slot origin.
		/// </summary>
		public Point TextOrigin { get; }

		/// <summary>
		///   The effective text origin is the point that we tell Aspose.Pdf to position the text at.
		///   Due to how Aspose.Pdf does positioning of rotated text, this is not always the same as
		///   <see cref="TextOrigin" />.
		/// </summary>
		public Point EffectiveTextOrigin { get; }

		/// <summary>
		///   Width of this text slot.
		/// </summary>
		public double Width { get; }

		/// <summary>
		///   Height of this text slot.
		/// </summary>
		public double Height { get; }

		/// <summary>
		///   Angle of this text slot.
		/// </summary>
		public Angle Angle { get; }

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
			if (Angle <= Angle.Degrees90)
			{
				xOffset = Height * Angle.Sin;
			}
			else if (Angle <= Angle.Degrees180)
			{
				xOffset = Width * -Angle.Cos + Height * Angle.Sin;
				yOffset = Height * -Angle.Cos;
			}
			else if (Angle <= Angle.Degrees270)
			{
				xOffset = Width * -Angle.Cos;
				yOffset = Width * -Angle.Sin + Height * -Angle.Cos;
			}
			else
			{
				yOffset = Width * -Angle.Sin;
			}

			return TextOrigin - new Offset(xOffset, yOffset);
		}
	}
}