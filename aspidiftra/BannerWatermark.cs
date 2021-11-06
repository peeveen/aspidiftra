using System;
using System.Collections.Generic;
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
		public BannerWatermark(string text, Appearance appearance, Justification justification,
			Fitting fit, Size marginSize, IBannerAngle angle,
			Func<IImmutableSet<int>, IImmutableSet<int>>? pageSelector = null)
			: base(appearance.Opacity, pageSelector)
		{
			_text = AspidiftraUtil.SplitTextIntoLines(text);
			_angle = angle;
			_justification = justification;
			_appearance = appearance;
			_marginSize = marginSize;
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
			var marginOffset = new Offset(effectiveMarginSize, effectiveMarginSize);
			var pageSizeWithoutMargin = pageSize.ApplyMargin(effectiveMarginSize);

			// What angle will the text be at?
			// It will be an orthogonal angle, i.e. multiple of 90 degrees.
			var angle = _angle.GetAngle(pageSize);

			// This object will provide "text slots" (places to put strings) to the text position calculator.
			var pageEdgeSlotCalculator =
				new BannerTextSlotCalculator(pageSizeWithoutMargin, angle);
			// This object will do all the positioning of text, and also any fitting.
			var textPositionCalculator =
				new TextPositionCalculator(pageEdgeSlotCalculator, _appearance.Font, _justification, _fit);
			// So let's get the results of the text positioning.
			var positionedText =
				textPositionCalculator.GetPositionedText(_text, fontSize);

			// We can now convert those calculated text positions to watermark elements.
			var elements = positionedText.Select(txt => new WatermarkElement(
				txt.Position + marginOffset,
				new FormattedText(txt.Text,
					_appearance.Color,
					_appearance.Font.Name, EncodingType.Winansi, false, positionedText.FontSize)));

			return new WatermarkElementCollection(elements, angle, _appearance.IsBackground);
		}
	}

	internal class BannerTextSlotCalculator : ITextSlotCalculator
	{
		private readonly Angle _angle;
		private readonly PageSize _pageSize;
		private readonly Angle _reversedAngle;

		internal BannerTextSlotCalculator(PageSize pageSize, Angle angle)
		{
			_pageSize = pageSize;
			_angle = angle;
			_reversedAngle = angle.Reverse();
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

			// First thing we need is a line that runs through the centre of the page
			// at the desired banner angle.
			var angledCenterLine = new Line(_pageSize.Center, _angle);

			// We will be moving this line across the page (in both directions) to measure each
			// slot. Each slot will be as "tall" as the font size. The line angle will stay the
			// same, but only the point will move. Let's calculate the offset for that.
			var projectionRightAngle = _angle.Rotate90(false).ToRadians();
			var offset = new Offset(projectionRightAngle.Cos * fontSize, projectionRightAngle.Sin * fontSize);

			// The algorithm for the odd/even slot calculations are identical ... the only difference is
			// where the centre line starts. For odd, the line should be shunted off-centre by half of
			// the font size.
			var halfOffset = new Offset(offset.X / 2.0, offset.Y / 2.0);

			// Let's do the calculations!
			var pageLines = _pageSize.Lines.ToImmutableList();
			var evenSlots = CalculateSlots(angledCenterLine, offset, pageLines, fontSize);
			var oddStartLine = new Line(angledCenterLine.Point - halfOffset, angledCenterLine.Gradient);
			var oddSlots = CalculateSlots(oddStartLine, offset, pageLines, fontSize);
			return new BannerTextSlotProvider(oddSlots, evenSlots);
		}

		private ImmutableList<TextSlot> CalculateSlots(Line textLine, Offset offset, IImmutableList<Line> pageLines,
			double slotHeight)
		{
			var slots = new List<TextSlot>();
			var initialSlot = CalculateSlot(textLine, offset, pageLines, slotHeight);
			if (initialSlot != null)
			{
				slots.Add(initialSlot);

				void CalculateOffsetSlots(int offsetDirection)
				{
					for (var f = offsetDirection;; f += offsetDirection)
					{
						var newOffset = new Offset(offset.X * f, offset.Y * f);
						var newLine = new Line(textLine.Point + newOffset, textLine.Gradient);
						var newTextSlot = CalculateSlot(newLine, offset, pageLines, slotHeight);
						if (newTextSlot != null)
							slots.Add(newTextSlot);
						else
							break;
					}
				}

				CalculateOffsetSlots(1);
				// Slots have been added from the centre of the page "upwards" (whatever
				// the angle defines "up" as). This is the wrong order, so we reverse
				// them now. The next call to CalculateOffsetSlots will add them from
				// the middle "downwards", which is okay.
				slots.Reverse();
				CalculateOffsetSlots(-1);
			}

			return slots.ToImmutableList();
		}

		private TextSlot? CalculateSlot(Line textLine, Offset offset, IImmutableList<Line> pageLines, double slotHeight)
		{
			// Find where the text line crosses the page edges.
			// There should only ever be two of these.
			var intersectionPoints = pageLines.Select(line => line.GetIntersectionPoint(textLine))
				.Where(point => point != null && _pageSize.Contains(point)).Cast<Point>();
			var textSlotStartAndEndPoints = intersectionPoints.Select(point =>
			{
				// OK, offset those two intersection points by the font size offset.
				var offsetPoint = point + offset;
				// If the offset point is still on the page, then we're golden.
				if (_pageSize.Contains(offsetPoint))
					return point;
				// However, if it went off the page, we need to adjust things.
				// Let's build a line with the same angle as the text line, but using
				// the off-page intersection point.
				var offsetLine = new Line(offsetPoint, _angle);
				// Now find the intersection points between this line and the page edges.
				// There may be 2, or there may be zero.
				var offsetIntersectionPoints = pageLines.Select(line => line.GetIntersectionPoint(offsetLine))
					.Where(p => p != null && _pageSize.Contains(p))
					.Cast<Point>()
					.ToList();
				// If there aren't any, then there's no room for another slot.
				if (!offsetIntersectionPoints.Any())
					return null;
				// Find the shortest distance from the offset point (which we
				// know is off the page) to the point where the offset line
				// intersected a page edge.
				var minimumIntersectionPointDistance = offsetIntersectionPoints.Select(p => p.GetDistanceFrom(offsetPoint))
					.Min();
				// This is the amount that the original intersection points needs
				// shifted by. Hard to know in which direction, so do both, but choose
				// the result that is on the page.
				var readjustmentOffset = new Offset(_angle.Cos * minimumIntersectionPointDistance,
					_angle.Sin * minimumIntersectionPointDistance);
				var readjustmentOffsetReversed = new Offset(_reversedAngle.Cos * minimumIntersectionPointDistance,
					_reversedAngle.Sin * minimumIntersectionPointDistance);
				var pointAdjustedByAngle = point + readjustmentOffset;
				var pointAdjustedByReversedAngle = point + readjustmentOffsetReversed;
				return _pageSize.Contains(pointAdjustedByAngle) ? pointAdjustedByAngle : pointAdjustedByReversedAngle;
			}).Where(point => point != null).Cast<Point>().ToImmutableList();
			// If we didn't get a start AND end point back, then there is no room for the slot.
			if (!(textSlotStartAndEndPoints is {Count: 2}))
				return null;
			// OK, so we have two valid points. We're not entirely sure which one is the start point
			// though.
			// Find the angles between the two points in both directions. Return them in the order
			// that is closest to the desired banner angle.
			// TODO: that.
			var startPoint = textSlotStartAndEndPoints[0];
			var endPoint = textSlotStartAndEndPoints[1];
			return new TextSlot(startPoint, startPoint.GetDistanceFrom(endPoint), slotHeight, _angle);
		}
	}

	internal class BannerTextSlotProvider : ITextSlotProvider
	{
		private readonly IImmutableList<TextSlot> _evenSlots;
		private readonly IImmutableList<TextSlot> _oddSlots;

		internal BannerTextSlotProvider(IImmutableList<TextSlot> oddSlots, IImmutableList<TextSlot> evenSlots)
		{
			_oddSlots = oddSlots;
			_evenSlots = evenSlots;
		}

		public IEnumerable<TextSlot> GetTextSlots(int amount)
		{
			IEnumerable<TextSlot> GetSlots(int amount, IImmutableList<TextSlot> slots)
			{
				var availableSlots = slots.Count;
				if (availableSlots < amount)
					throw new InsufficientSlotsException(amount, availableSlots);
				var topAndTail = (availableSlots - amount) / 2;
				var topped = slots.RemoveRange(0, topAndTail);
				var tailed = topped.RemoveRange(amount, topAndTail);
				return tailed;
			}

			// If we're asked for an odd number of slots, take the middle selection from _oddSlots.
			// If we're asked for an even number of slots, take the middle selection from _evenSlots.
			return GetSlots(amount, amount % 2 == 1 ? _oddSlots : _evenSlots);
		}
	}
}