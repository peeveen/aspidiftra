using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aspidiftra.Geometry;

namespace Aspidiftra
{
	/// <summary>
	///   Object responsible for calculating the positions of banner text slots.
	/// </summary>
	internal class BannerTextSlotCalculator : ITextSlotCalculator
	{
		/// <summary>
		///   Angle of the banner.
		/// </summary>
		private readonly Angle _angle;

		/// <summary>
		///   Size of the page that we are putting the banner on.
		/// </summary>
		private readonly PageSize _pageSize;

		/// <summary>
		///   Reverse of the desired banner angle. Calculated and stored here for
		///   efficiency.
		/// </summary>
		private readonly Angle _reversedAngle;

		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="pageSize">Size of the page that the banner will be on.</param>
		/// <param name="angle">Desired angle of banner.</param>
		internal BannerTextSlotCalculator(PageSize pageSize, Angle angle)
		{
			_pageSize = pageSize;
			_angle = angle;
			_reversedAngle = angle.Reverse();
		}

		/// <summary>
		///   Calculates the position of all possible text slots for the given font size.
		/// </summary>
		/// <param name="fontSize">Font size.</param>
		/// <returns>A text slot provider, loaded with all the calculated slot positions.</returns>
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
			var halfOffset = offset / 2.0;

			// Calculation involves finding the intersection points between the angled center line, and
			// the page edges, which are represented by four Line objects here.
			var pageLines = _pageSize.Lines.ToImmutableList();

			// Let's do the calculations!
			// Get the even slots.
			var evenSlots = CalculateSlots(angledCenterLine, offset, pageLines, fontSize);

			// Get the odd slots.
			var oddStartLine = new Line(angledCenterLine.Point - halfOffset, angledCenterLine.Gradient);
			var oddSlots = CalculateSlots(oddStartLine, offset, pageLines, fontSize);

			// Return our special provider, loaded with both even and odd slot collections.
			return new BannerTextSlotProvider(oddSlots, evenSlots);
		}

		/// <summary>
		///   Calculates all the possible text slots.
		/// </summary>
		/// <param name="textLine">Starting text line.</param>
		/// <param name="offset">Amount to offset the text line by for each slot.</param>
		/// <param name="pageLines">Line that define the page edges.</param>
		/// <param name="slotHeight">How "tall" each slot should be (currently font size, really).</param>
		/// <returns>Collection of calculated text slots.</returns>
		private ImmutableList<TextSlot> CalculateSlots(Line textLine, Offset offset, IImmutableList<Line> pageLines,
			double slotHeight)
		{
			var slots = new List<TextSlot>();
			// We start by calculating the slot that is "on" the given text line.
			var initialSlot = CalculateSlot(textLine, offset, pageLines, slotHeight);
			if (initialSlot != null)
			{
				// Add the initial slot to the collection.
				slots.Add(initialSlot);

				// This function will repeatedly move the text line by the offset until
				// it is no longer in a position that is viable for a text slot.
				// The offsetDirection parameter will be 1 or -1 to indicate what
				// "direction" we will offset the line by each time.
				void CalculateOffsetSlots(int offsetDirection)
				{
					for (var f = offsetDirection;; f += offsetDirection)
					{
						var newOffset = offset * f;
						var newLine = new Line(textLine.Point + newOffset, textLine.Gradient);
						var newTextSlot = CalculateSlot(newLine, offset, pageLines, slotHeight);
						if (newTextSlot != null)
							slots.Add(newTextSlot);
						else
							break;
					}
				}

				// Calculate the slots in the "upwards" direction (whatever
				// the angle defines "up" as!).
				CalculateOffsetSlots(1);
				// Slots have been added from the centre of the page "upwards".
				// This is the wrong order, so we reverse them now. 
				slots.Reverse();
				// This next call to CalculateOffsetSlots will add them from the
				// middle "downwards", which is okay.
				CalculateOffsetSlots(-1);
			}

			return slots.ToImmutableList();
		}

		/// <summary>
		/// Calculates a text slot on the given line.
		/// </summary>
		/// <param name="textLine">Line that the resulting text slot must be on.</param>
		/// <param name="offset">The current text line offset.</param>
		/// <param name="pageLines">Lines that define the page edges.</param>
		/// <param name="slotHeight">How "tall" each slot should be (currently font size, really).</param>
		/// <returns>The text slot for the given line, or null if no text slot can fit on this line.</returns>
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
			// If the line is running from corner to corner, we might get more than 2 points back.
			// Remove non-unique ones.
			textSlotStartAndEndPoints = textSlotStartAndEndPoints.Distinct().ToImmutableList();
			// If we didn't get a start AND end point back, then there is no room for the slot.
			if (!(textSlotStartAndEndPoints is {Count: 2}))
				return null;
			// OK, so we have two valid points. We're not entirely sure which one is the start point
			// though.
			// Find the angles between the two points in both directions. Return them in the order
			// that is closest to the desired banner angle.
			var firstPoint = textSlotStartAndEndPoints[0];
			var secondPoint = textSlotStartAndEndPoints[1];
			var offsetFromFirstToSecond = secondPoint - firstPoint;
			var offsetFromSecondToFirst = firstPoint - secondPoint;
			var angleFromFirstToSecond =
				new Angle(Math.Atan2(offsetFromFirstToSecond.Y, offsetFromFirstToSecond.X), AngleUnits.Radians);
			var angleFromSecondToFirst =
				new Angle(Math.Atan2(offsetFromSecondToFirst.Y, offsetFromSecondToFirst.X), AngleUnits.Radians);
			var angleInRadians = _angle.ToRadians().Value;
			var firstToSecondAngleDiff = Math.Abs(angleInRadians - angleFromFirstToSecond.Value);
			var secondToFirstAngleDiff = Math.Abs(angleInRadians - angleFromSecondToFirst.Value);
			if (secondToFirstAngleDiff < firstToSecondAngleDiff)
				(secondPoint, firstPoint) = (firstPoint, secondPoint);
			return new TextSlot(firstPoint, firstPoint.GetDistanceFrom(secondPoint), slotHeight, _angle);
		}
	}
}