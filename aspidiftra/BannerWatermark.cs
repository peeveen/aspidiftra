using System;
using System.Collections.Immutable;
using System.Linq;
using Aspose.Pdf;
using Aspose.Pdf.Facades;

namespace Aspidiftra
{
	public class BannerWatermark : Watermark
	{
		private readonly IBannerAngle _angle;
		private readonly Appearance _appearance;
		private readonly Fitting _fit;
		private readonly Size _marginSize;
		private readonly bool _reverseDirection;
		private readonly IImmutableList<string> _text;

		/// <summary>
		///   A page edge watermark, allowing text to be placed alongside any of the
		///   four pages edges, running in either appropriate orthogonal direction.
		/// </summary>
		/// <param name="text">
		///   Text to place along the edge of the page. Can contain line breaks for
		///   multi-line watermarks.
		/// </param>
		/// <param name="appearance">Stylistic attributes for the text.</param>
		/// <param name="fit">
		///   Should the banner be made larger/smaller to fit the page more
		///   precisely? Can the text be broken up and wrapped across many lines?
		/// </param>
		/// <param name="marginSize">
		///   Size of margin, if you want to ensure the watermark isn't actually
		///   touching the page edge.
		/// </param>
		/// <param name="angle">
		///   How the banner should be orientated on the page (i.e. across which
		///   diagonal). Can also be set to an explicit angle value. In that case,
		///   a value of 0 will result in a horizontal (left to right) banner, 90
		///   will get you a vertical (bottom to top) banner. The banner is always
		///   centered on the page. Be aware that using values from 90 to 270 will
		///   result in "upside down" text.
		/// </param>
		/// <param name="pageSelector">
		///   Function that will select the pages that the watermark will appear on,
		///   from a given set of page numbers. If no value is provided for this
		///   parameter, <see cref="AspidiftraUtil.AllPagesSelector" /> will be used.
		/// </param>
		/// <param name="reverseDirection">
		///   True if the direction of the text should be reversed from the
		///   default text direction of <paramref name="angle" />.
		///   A value of true can result in text being rendered upside-down.
		/// </param>
		public BannerWatermark(string text, Appearance appearance, Fitting fit,
			Size marginSize, IBannerAngle angle, bool reverseDirection = false,
			Func<IImmutableSet<int>, IImmutableSet<int>> pageSelector = null
		) : base(appearance.Opacity, pageSelector)
		{
			_text = AspidiftraUtil.SplitTextIntoLines(text);
			_angle = angle;
			_appearance = appearance;
			_marginSize = marginSize;
			_reverseDirection = reverseDirection;
			// If the given watermark text explicitly contains line breaks, then we will
			// not attempt any kind of intelligent text wrapping.
			_fit = fit.Normalize(_text);
		}

		public override WatermarkElementCollection GetWatermarkElements(Rectangle pageSize)
		{
			// TODO: Calculate properly.
			var calculatedFontSize = _appearance.Font.GetSize(pageSize);
			var calculatedAngle = _angle.GetAngle(pageSize);
			var calculatedPosition = new Point(100.0, 100.0);
			var formattedText = new FormattedText(_text.First(),
				_appearance.Color,
				_appearance.Font.Name, EncodingType.Winansi, false, calculatedFontSize);
			var element = new WatermarkElement(calculatedPosition, formattedText);

			return new WatermarkElementCollection(new[] {element}, calculatedAngle, _appearance.IsBackground);
		}
	}
}