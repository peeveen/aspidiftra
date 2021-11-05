using System;
using System.Collections.Immutable;
using System.Linq;
using Aspidiftra.Geometry;
using Aspose.Pdf.Facades;

namespace Aspidiftra
{
	public class BannerWatermark : Watermark
	{
		private readonly IBannerAngle _angle;
		private readonly Appearance _appearance;
		private readonly Fitting _fit;
		private readonly Justification _justification;
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
		/// <param name="justification">
		///   Should the text be left/right/centre justified?
		/// </param>
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
		public BannerWatermark(string text, Appearance appearance, Justification justification,
			Fitting fit, Size marginSize, IBannerAngle angle, bool reverseDirection = false,
			Func<IImmutableSet<int>, IImmutableSet<int>> pageSelector = null)
			: base(appearance.Opacity, pageSelector)
		{
			_text = AspidiftraUtil.SplitTextIntoLines(text);
			_angle = angle;
			_justification = justification;
			_appearance = appearance;
			_marginSize = marginSize;
			_reverseDirection = reverseDirection;
			// If the given watermark text explicitly contains line breaks, then we will
			// not attempt any kind of intelligent text wrapping.
			_fit = fit.Normalize(_text);
		}

		public override WatermarkElementCollection GetWatermarkElements(PageSize pageSize)
		{
			// Get the initial requested font size. This may change,
			// depending on the value of _fit.
			var font = _appearance.Font;
			var fontSize = font.GetSize(pageSize);

			// Reduce page size length by the margin size. If the remaining space
			// is zero or less, then throw an exception (thrown by the ApplyMargin
			// function).
			var effectiveMarginSize = _marginSize.GetEffectiveSize(pageSize);
			var pageSizeWithoutMargin = pageSize.ApplyMargin(effectiveMarginSize);

			// What angle will the text be at?
			// It will be an orthogonal angle, i.e. multiple of 90 degrees.
			var angle = _angle.GetAngle(pageSize);
			if (_reverseDirection)
				angle = angle.Reverse();

			// This object will provide "text slots" (places to put strings) to the text position calculator.
			var pageEdgeSlotCalculator =
				new BannerTextSlotCalculator(pageSizeWithoutMargin, angle, _reverseDirection);
			// This object will do all the positioning of text, and also any fitting.
			var textPositionCalculator =
				new TextPositionCalculator(pageEdgeSlotCalculator, _appearance.Font, _justification, _fit);
			// So let's get the results of the text positioning.
			var positionedText =
				textPositionCalculator.GetPositionedText(_text, fontSize);

			// We can now convert those calculated text positions to watermark elements.
			var elements = positionedText.Select(txt => new WatermarkElement(
				txt.Position+(effectiveMarginSize, effectiveMarginSize),
				new FormattedText(txt.Text,
					_appearance.Color,
					_appearance.Font.Name, EncodingType.Winansi, false, positionedText.FontSize)));

			return new WatermarkElementCollection(elements, angle, _appearance.IsBackground);
		}
	}

	internal class BannerTextSlotCalculator : ITextSlotCalculator
	{
		private readonly Angle _angle;
		private readonly bool _reverseDirection;

		internal BannerTextSlotCalculator(Rectangle pageSize, Angle angle, bool reverseDirection)
		{
			_angle = angle;
			_reverseDirection = reverseDirection;
		}

		public ITextSlotProvider CalculateSlots(float fontSize)
		{
			// OK, here's where the party is.
			// We will calculate two sets of slots.
			// One will contain an odd number of slots, the other will contain an even number.
			// Our implementation of ITextSlotProvider will choose the middle "n" slots from
			// the appropriate set, depending on whether there are an odd or even number of
			// strings to be positioned.
			// This is so that, if, for example, three strings were to be positioned, the
			// middle string (the second one) would be central on the page. On the other hand,
			// if two strings were to be positioned, we would want the centre point of the
			// page to be "between" the two strings.

			return null;
		}
	}
}