using System.Collections.Generic;
using Aspidiftra.Geometry;

namespace Aspidiftra
{
	/// <summary>
	///   Text slot calculator for page edge watermarks.
	/// </summary>
	internal class PageEdgeTextSlotCalculator : ITextSlotCalculator
	{
		private readonly Angle _angle;
		private readonly double _availableTextStackSpace;
		private readonly Fitting _fit;
		private readonly Offset _logicalOffset;
		private readonly double _maximumTextLength;
		private readonly PageEdgePosition _pageEdgePosition;
		private readonly PageSize _pageSize;
		private readonly bool _reverseDirection;

		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="pageSize">Size of the page.</param>
		/// <param name="pageEdgePosition">The page edge position of the watermark.</param>
		/// <param name="angle">What angle the text will be at.</param>
		/// <param name="fit">Current best-fit constraints.</param>
		/// <param name="reverseDirection">True if the text should run in the opposite direction.</param>
		internal PageEdgeTextSlotCalculator(PageSize pageSize, PageEdgePosition pageEdgePosition,
			Angle angle, Fitting fit, bool reverseDirection)
		{
			_angle = angle;
			_fit = fit;
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

			// Each calculated slot for a multi-line page edge watermark will be offset by a certain amount.
			// We don't yet know the magnitude of the offset (it depends on the font size), but we know in
			// what "direction" it will be, so we can calculate a "logical offset" here, and multiply it by
			// the font size later.
			var xLogicalOffset = _pageEdgePosition switch
			{
				PageEdgePosition.Right => -1,
				PageEdgePosition.Left => 1,
				_ => 0
			};
			var yLogicalOffset = _pageEdgePosition switch
			{
				PageEdgePosition.Top => -1,
				PageEdgePosition.Bottom => 1,
				_ => 0
			};
			_logicalOffset = new Offset(xLogicalOffset, yLogicalOffset);
		}

		public ITextSlotProvider CalculateSlots(float fontSize)
		{
			var offset = _logicalOffset * fontSize;
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
				PageEdgePosition.Top => _reverseDirection ? pageTopRight : pageTopLeft + offset,
				PageEdgePosition.Bottom => _reverseDirection ? pageBottomRight + offset : pageBottomLeft,
				PageEdgePosition.Left => _reverseDirection ? pageTopLeft : pageBottomLeft + offset,
				_ => _reverseDirection ? pageBottomRight : pageTopRight + offset
			};

			var slots = new List<TextSlot>();
			// If we're allowed to overflow, we can use any "partial" space that's left in the stack.
			var availableTextStackSpace = _availableTextStackSpace + (_fit.HasOverflow() ? fontSize : 0.0);
			while (availableTextStackSpace > 0.0)
			{
				slots.Add(new TextSlot(startPoint, _maximumTextLength, fontSize, _angle));
				startPoint += offset;
				availableTextStackSpace -= fontSize;
			}

			return new PageEdgeTextSlotProvider(slots, _pageEdgePosition, _reverseDirection);
		}
	}
}