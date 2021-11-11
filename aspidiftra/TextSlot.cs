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
		}

		/// <summary>
		///   Text slot origin.
		/// </summary>
		public Point TextOrigin { get; }

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
	}
}