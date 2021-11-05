using Aspose.Pdf.Text;

namespace Aspidiftra
{
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

		public string Name => _font.FontName;

		public float GetSize(PageSize pageSize)
		{
			return _size.GetEffectiveSize(pageSize);
		}

		public double MeasureString(string text, float fontSize)
		{
			return _font.MeasureString(text, fontSize);
		}
	}
}