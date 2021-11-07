using System.Collections.Generic;
using Aspidiftra.Geometry;

namespace Aspidiftra
{
	internal class PageEdgeTextSlotCalculator : ITextSlotCalculator
	{
		private readonly Angle _angle;
		private readonly double _availableTextStackSpace;
		private readonly double _maximumTextLength;
		private readonly PageEdgePosition _pageEdgePosition;
		private readonly PageSize _pageSize;
		private readonly bool _reverseDirection;
		private readonly int _xStepMultiplier;
		private readonly int _yStepMultiplier;

		internal PageEdgeTextSlotCalculator(PageSize pageSize, PageEdgePosition pageEdgePosition,
			Angle angle, bool reverseDirection)
		{
			_angle = angle;
			_pageSize = pageSize;
			_pageEdgePosition = pageEdgePosition;
			_reverseDirection = reverseDirection;

			// Get length of appropriate page side.
			var appositePageSizeLength = pageEdgePosition.GetPageSideLength(_pageSize);

			// Get the length of the other page side.
			// If a multi-line page edge watermark is requested, then
			// it's theoretically possible that there might be too many lines
			// to actually fit on the page, so we'll check for that.
			var oppositePageSizeLength = pageEdgePosition.GetPageSideLength(_pageSize, true);

			_maximumTextLength = appositePageSizeLength;
			_availableTextStackSpace = oppositePageSizeLength;

			_xStepMultiplier = _pageEdgePosition switch
			{
				PageEdgePosition.Right => -1,
				PageEdgePosition.Left => 1,
				_ => 0
			};
			_yStepMultiplier = _pageEdgePosition switch
			{
				PageEdgePosition.Top => -1,
				PageEdgePosition.Bottom => 1,
				_ => 0
			};
		}

		public ITextSlotProvider CalculateSlots(float fontSize)
		{
			var pageTopLeft = new Point(0, _pageSize.Height);
			var pageTopRight = new Point(_pageSize.Width, _pageSize.Height);
			var pageBottomLeft = new Point(0, 0);
			var pageBottomRight = new Point(_pageSize.Width, 0);


			// Build the list of text slots, starting at the outside edge, working in.

			// Figure out the text origin start point for the first line.
			// The "origin" of a line of text is the lower left corner of the first
			// character.
			var startPoint = _pageEdgePosition switch
			{
				PageEdgePosition.Top => _reverseDirection
					? pageTopRight
					: pageTopLeft + new Offset(0, fontSize * _yStepMultiplier),
				PageEdgePosition.Bottom => _reverseDirection
					? pageBottomRight + new Offset(0, fontSize * _yStepMultiplier)
					: pageBottomLeft,
				PageEdgePosition.Left => _reverseDirection
					? pageTopLeft
					: pageBottomLeft + new Offset(fontSize * _xStepMultiplier, 0),
				_ => _reverseDirection
					? pageBottomRight
					: pageTopRight + new Offset(fontSize * _xStepMultiplier, 0)
			};

			var slots = new List<TextSlot>();
			var availableTextStackSpace = _availableTextStackSpace;
			while (availableTextStackSpace > 0.0)
			{
				slots.Add(new TextSlot(startPoint, _maximumTextLength, fontSize, _angle));
				startPoint = startPoint + new Offset(fontSize * _xStepMultiplier, fontSize * _yStepMultiplier);
				availableTextStackSpace -= fontSize;
			}

			return new PageEdgeTextSlotProvider(slots, _pageEdgePosition, _reverseDirection);
		}
	}
}