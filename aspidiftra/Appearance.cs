using System;
using System.Drawing;

namespace Aspidiftra
{
	/// <summary>
	///   Defines the "cosmetic" appearance of the watermark text.
	/// </summary>
	public class Appearance
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="color">Color of text.</param>
		/// <param name="opacity">
		///   How opaque should it be? A value of 1.0 is "completely opaque", and 0.0 is "completely
		///   transparent".
		/// </param>
		/// <param name="font">What font to render the text with.</param>
		/// <param name="isBackground">Should the watermark appear in the background or foreground?</param>
		public Appearance(Color color, float opacity, Font font, bool isBackground = false)
		{
			if (opacity < 0.0 || opacity > 1.0)
				throw new ArgumentException("Opacity must be between 0.0 and 1.0.", nameof(opacity));
			Color = color;
			Opacity = opacity;
			Font = font;
			IsBackground = isBackground;
		}

		/// <summary>
		///   Color of text.
		/// </summary>
		public Color Color { get; }

		/// <summary>
		///   True if text is to appear in background layer, false for foreground layer.
		/// </summary>
		public bool IsBackground { get; }

		/// <summary>
		///   Font to render the text with.
		/// </summary>
		public Font Font { get; }

		/// <summary>
		///   How opaque should the text be (0.0==completely transparent, 1.0==completely opaque).
		/// </summary>
		public float Opacity { get; }
	}
}