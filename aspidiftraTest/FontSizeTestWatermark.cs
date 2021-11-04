using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Aspidiftra;
using Aspose.Pdf;
using Aspose.Pdf.Facades;
using Watermark = Aspidiftra.Watermark;

namespace AspidiftraTest
{
	public class FontSizeTestWatermark : Watermark
	{
		private readonly Appearance _appearance;
		private readonly string _text;

		/// <summary>
		///   A page edge watermark, allowing text to be placed alongside any of the
		///   four pages edges, running in either appropriate orthogonal direction.
		/// </summary>
		/// <param name="text">Text to place along the edge of the page.</param>
		/// <param name="appearance">Stylistic attributes for the text.</param>
		/// <param name="pageSelector">
		///   Function that will select the pages that the watermark will appear on,
		///   from a given set of page numbers. If no value is provided for this
		///   parameter, <see cref="AspidiftraUtil.AllPagesSelector" /> will be used.
		/// </param>
		public FontSizeTestWatermark(string text, Appearance appearance,
			Func<IImmutableSet<int>, IImmutableSet<int>> pageSelector = null)
			: base(appearance.Opacity, pageSelector)
		{
			_text = text;
			_appearance = appearance;
		}

		public override WatermarkElementCollection GetWatermarkElements(Rectangle pageMediaBox)
		{
			var calculatedFontSize = _appearance.Font.GetSize(pageMediaBox);
			var calculatedAngle = Angle.Degrees0;
			var formattedText = new FormattedText(_text,
				_appearance.Color,
				_appearance.Font.Name, EncodingType.Winansi, false, calculatedFontSize);
			var elements = new List<WatermarkElement>();
			for (var f = 0; f < 10; ++f)
			{
				var calculatedPosition = new Point(0.0, 100.0 * f);
				var element = new WatermarkElement(calculatedPosition, formattedText);
				elements.Add(element);
			}

			return new WatermarkElementCollection(elements, calculatedAngle, _appearance.IsBackground);
		}
	}
}