using Aspidiftra.Geometry;
using Aspose.Pdf.Facades;

namespace Aspidiftra
{
	public class WatermarkElement
	{
		public WatermarkElement(Point position, FormattedText formattedText)
		{
			Position = position;
			FormattedText = formattedText;
		}

		public Point Position { get; }
		public FormattedText FormattedText { get; }
	}
}