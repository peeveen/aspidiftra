using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aspose.Pdf;
using Aspose.Pdf.Facades;

namespace Aspidiftra
{
	public class PageEdgeWatermark : Watermark
	{
		private readonly Appearance _appearance;
		private readonly Fitting _fit;
		private readonly Justification _justification;
		private readonly Size _marginSize;
		private readonly PageEdgePosition _pageEdgePosition;
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
		/// <param name="position">What page edge should it be placed along?</param>
		/// <param name="justification">
		///   Justification of text (relative to the default text direction of
		///   <paramref name="position" />).
		/// </param>
		/// <param name="appearance">Stylistic attributes for the text.</param>
		/// <param name="marginSize">
		///   Size of margin, if you want to ensure the watermark isn't actually
		///   touching the page edge.
		/// </param>
		/// <param name="pageSelector">
		///   Function that will select the pages that the watermark will appear on,
		///   from a given set of page numbers. If no value is provided for this
		///   parameter, <see cref="AspidiftraUtil.AllPagesSelector" /> will be used.
		/// </param>
		/// <param name="reverseDirection">
		///   True if the direction of the text should be reversed from the
		///   default text direction of <paramref name="position" />.
		///   A value of true can result in text being rendered upside-down if
		///   <paramref name="position" /> is North or South.
		/// </param>
		/// <param name="fit">
		///   Should the text be made smaller/larger/wrapped to fit the area?
		///   Note that using <see cref="Fitting.Grow" /> is not generally
		///   a good idea, as it can result in enormous text. Also, there is no
		///   mechanism in place to prevent page edge watermarks on different page
		///   edges from overlapping.
		/// </param>
		public PageEdgeWatermark(string text, Appearance appearance, PageEdgePosition position,
			Justification justification, Fitting fit, Size marginSize, bool reverseDirection = false,
			Func<IImmutableSet<int>, IImmutableSet<int>> pageSelector = null
			) : base(appearance.Opacity, pageSelector)
		{
			_text = AspidiftraUtil.SplitTextIntoLines(text);
			_appearance = appearance;
			_marginSize = marginSize;
			_reverseDirection = reverseDirection;
			_justification = justification;
			_pageEdgePosition = position;
			// If the given watermark text explicitly contains line breaks, then we will
			// not attempt any kind of intelligent text wrapping.
			_fit = fit.Normalize(_text);
		}

		public override WatermarkElementCollection GetWatermarkElements(Rectangle pageSize)
		{
			// Get the initial requested font size. This may change,
			// depending on the value of _fit.
			var font = _appearance.Font;
			var fontSize = font.GetSize(pageSize);

			// What angle will the text be at?
			// It will be an orthogonal angle, i.e. multiple of 90 degrees.
			var angle = _pageEdgePosition.GetAngle();
			if (_reverseDirection)
				angle = angle.Reverse();

			// This object will provide "text slots" (places to put strings) to the text position calculator.
			var pageEdgeSlotCalculator =
				new PageEdgeTextSlotCalculator(pageSize, _pageEdgePosition, _marginSize, angle, _reverseDirection);
			// This object will do all the positioning of text, and also any fitting.
			var textPositionCalculator = new TextPositionCalculator(pageEdgeSlotCalculator, _appearance.Font, _justification, _fit);
			// So let's get the results of the text positioning.
			var positionedText =
				textPositionCalculator.GetPositionedText(_text, fontSize);

			// We can now convert those calculated text positions to watermark elements.
			var elements = positionedText.Select(txt => new WatermarkElement(txt.Position, new FormattedText(txt.Text,
				_appearance.Color,
				_appearance.Font.Name, EncodingType.Winansi, false, positionedText.FontSize)));

			return new WatermarkElementCollection(elements, angle, _appearance.IsBackground);
		}
	}

	internal class PageEdgeTextSlotCalculator : ITextSlotCalculator
	{
		private readonly Angle _angle;
		private readonly double _availableTextStackSpace;
		private readonly double _maximumTextLength;
		private readonly Point _pageBottomLeft;
		private readonly Point _pageBottomRight;
		private readonly PageEdgePosition _pageEdgePosition;
		private readonly Point _pageTopLeft;
		private readonly Point _pageTopRight;
		private readonly bool _reverseDirection;
		private readonly int _xStepMultiplier;
		private readonly int _yStepMultiplier;

		internal PageEdgeTextSlotCalculator(Rectangle pageSize, PageEdgePosition pageEdgePosition, Size marginSize,
			Angle angle, bool reverseDirection)
		{
			_angle = angle;
			_pageEdgePosition = pageEdgePosition;
			_reverseDirection = reverseDirection;

			// Get length of appropriate page side.
			var appositePageSizeLength = pageEdgePosition switch
			{
				// Annoying duplicate here ... roll on, C# 9.0
				PageEdgePosition.North => pageSize.Width,
				PageEdgePosition.South => pageSize.Width,
				_ => pageSize.Height
			};

			// Get the length of the other page side.
			// If a multi-line page edge watermark is requested, then
			// it's theoretically possible that there might be too many lines
			// to actually fit on the page, so we'll check for that.
			var oppositePageSizeLength = pageEdgePosition switch
			{
				// Annoying duplicate here ... roll on, C# 9.0
				PageEdgePosition.North => pageSize.Height,
				PageEdgePosition.South => pageSize.Height,
				_ => pageSize.Width
			};

			// Reduce page size length by the margin size. If the remaining space
			// is zero or less, then throw an exception.
			double effectiveMarginSize = marginSize.GetEffectiveSize(pageSize);

			// Handy exception function.
			void ThrowInsufficientSpaceException(bool apposite, double length)
			{
				const string width = "width";
				const string height = "height";
				var widthString = apposite ? width : height;
				var heightString = apposite ? height : width;
				var northOrSouth = pageEdgePosition == PageEdgePosition.North || pageEdgePosition == PageEdgePosition.South;
				var pageEdgeDescriptor = northOrSouth ? widthString : heightString;
				throw new InsufficientSpaceException(
					$"The requested margin size ({marginSize}) is greater than half of the page {pageEdgeDescriptor} ({length}).");
			}

			_maximumTextLength = appositePageSizeLength - effectiveMarginSize * 2.0;
			if (_maximumTextLength <= 0.0)
				ThrowInsufficientSpaceException(true, appositePageSizeLength);
			_availableTextStackSpace = oppositePageSizeLength - effectiveMarginSize * 2.0;
			if (_availableTextStackSpace <= 0.0)
				ThrowInsufficientSpaceException(false, oppositePageSizeLength);

			_pageTopLeft = new Point(effectiveMarginSize, pageSize.Height - effectiveMarginSize);
			_pageTopRight = new Point(pageSize.Width - effectiveMarginSize, pageSize.Height - effectiveMarginSize);
			_pageBottomLeft = new Point(effectiveMarginSize, effectiveMarginSize);
			_pageBottomRight = new Point(pageSize.Width - effectiveMarginSize, effectiveMarginSize);

			_xStepMultiplier = _pageEdgePosition switch
			{
				PageEdgePosition.East => -1,
				PageEdgePosition.West => 1,
				_ => 0
			};
			_yStepMultiplier = _pageEdgePosition switch
			{
				PageEdgePosition.North => -1,
				PageEdgePosition.South => 1,
				_ => 0
			};
		}

		public ITextSlotProvider CalculateSlots(float fontSize)
		{
			// Build the list of text slots, starting at the outside edge, working in.

			// Figure out the text origin start point for the first line.
			// The "origin" of a line of text is the lower left corner of the first
			// character.
			var startPoint = _pageEdgePosition switch
			{
				PageEdgePosition.North => _reverseDirection
					? _pageTopRight
					: _pageTopLeft.OffsetBy(0, fontSize * _yStepMultiplier),
				PageEdgePosition.South => _reverseDirection
					? _pageBottomRight.OffsetBy(0, fontSize * _yStepMultiplier)
					: _pageBottomLeft,
				PageEdgePosition.West => _reverseDirection
					? _pageTopLeft
					: _pageBottomLeft.OffsetBy(fontSize * _xStepMultiplier, 0),
				_ => _reverseDirection
					? _pageBottomRight
					: _pageTopRight.OffsetBy(fontSize * _xStepMultiplier, 0)
			};

			var slots = new List<TextSlot>();
			var availableTextStackSpace = _availableTextStackSpace;
			while (availableTextStackSpace > 0.0)
			{
				slots.Add(new TextSlot(startPoint, _maximumTextLength, fontSize, _angle));
				startPoint = startPoint.OffsetBy(fontSize * _xStepMultiplier, fontSize * _yStepMultiplier);
				availableTextStackSpace -= fontSize;
			}

			return new PageEdgeTextSlotProvider(slots, _pageEdgePosition, _reverseDirection);
		}
	}

	internal class PageEdgeTextSlotProvider : ITextSlotProvider
	{
		private readonly PageEdgePosition _pageEdgePosition;
		private readonly bool _reverseDirection;
		private readonly IReadOnlyList<TextSlot> _slots;

		internal PageEdgeTextSlotProvider(IReadOnlyList<TextSlot> slots, PageEdgePosition pageEdgePosition,
			bool reverseDirection)
		{
			_reverseDirection = reverseDirection;
			_pageEdgePosition = pageEdgePosition;
			_slots = slots;
		}

		public IEnumerable<TextSlot> GetTextSlots(int amount)
		{
			if (amount > _slots.Count)
				throw new InsufficientSlotsException(amount, _slots.Count);
			return _pageEdgePosition switch
			{
				// South is a special case, because, unlike the others, the top edge of the text is, by
				// default, not along the page edge.
				PageEdgePosition.South => _reverseDirection ? _slots.Take(amount) : _slots.Take(amount).Reverse(),
				_ => _reverseDirection ? _slots.Take(amount).Reverse() : _slots.Take(amount)
			};
		}
	}
}