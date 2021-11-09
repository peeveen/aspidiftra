using Aspose.Pdf.Text;

namespace Aspidiftra
{
	/// <summary>
	///   Font to render a watermark with.
	/// </summary>
	public class Font
	{
		private readonly Aspose.Pdf.Text.Font _font;
		private readonly Size _size;

		/// <summary>
		///   Definition of the font to use for watermarking.
		/// </summary>
		/// <param name="typeface">Typeface to use.</param>
		/// <param name="styles">Font styles, e.g., bold, italic, etc.</param>
		/// <param name="size">Size of font.</param>
		public Font(string typeface, FontStyles styles, Size size)
		{
			// This will throw an exception if the font is not available.
			_font = FontRepository.FindFont(typeface, styles, true);
			_size = size;
		}

		/// <summary>
		///   Name of font.
		/// </summary>
		public string Name => _font.FontName;

		/// <summary>
		///   Gets the font size, possibly relative to the page size.
		/// </summary>
		/// <param name="pageSize">Current page size.</param>
		/// <returns>Effective font size.</returns>
		public float GetSize(PageSize pageSize)
		{
			return _size.GetEffectiveSize(pageSize);
		}

		/// <summary>
		///   Returns the width of the given string.
		/// </summary>
		/// <param name="text">String to measure.</param>
		/// <param name="fontSize">Size of font to measure with.</param>
		/// <returns>Width of given string.</returns>
		public double MeasureString(string text, float fontSize)
		{
			return _font.MeasureString(text, fontSize);
		}
	}
}