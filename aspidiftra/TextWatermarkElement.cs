using Aspidiftra.Geometry;
using Aspose.Pdf.Facades;

namespace Aspidiftra
{
	/// <summary>
	///   A text watermark element. An "element" is something to display on the page.
	/// </summary>
	public class TextWatermarkElement
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="position">Position to render the element at.</param>
		/// <param name="formattedText">The formatted text object to render.</param>
		public TextWatermarkElement(Point position, FormattedText formattedText)
		{
			Position = position;
			FormattedText = formattedText;
		}

		/// <summary>
		///   Position to render the element at.
		/// </summary>
		public Point Position { get; }

		/// <summary>
		///   The formatted text object to render.
		/// </summary>
		public FormattedText FormattedText { get; }
	}
}