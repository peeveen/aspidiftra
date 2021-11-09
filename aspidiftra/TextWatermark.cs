using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aspidiftra.Geometry;
using Aspose.Pdf.Facades;

namespace Aspidiftra
{
	/// <summary>
	///   Base class for text watermarks.
	/// </summary>
	public abstract class TextWatermark : IWatermark
	{
		// We will, at some point, be attempting to make some text fit better by increasing
		// or reducing the font size. The amount that we will increase or reduce by will always
		// be a multiple of this value. It will also never be less than this value, otherwise
		// we could spend an eternity making insanely small adjustments.
		private const float MinimumFontSizeDelta = 0.5f;

		// We will never use a font size any smaller than this.
		private const float MinimumFontSize = 1.0f;
		private readonly Func<IImmutableSet<int>, IImmutableSet<int>> _pageSelector;

		protected readonly Appearance Appearance;
		protected readonly Fitting Fit;
		protected readonly Justification Justification;
		protected readonly Size MarginSize;
		protected readonly string Text;

		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="text">
		///   Text to show.
		/// </param>
		/// <param name="justification">
		///   Justification of text in each text slot.
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
		/// <param name="fit">
		///   Should the text be made smaller/larger/wrapped to fit the area?
		/// </param>
		protected TextWatermark(string text, Appearance appearance, Justification justification,
			Fitting fit, Size marginSize,
			Func<IImmutableSet<int>, IImmutableSet<int>>? pageSelector = null)
		{
			Text = text;
			Fit = fit;
			Appearance = appearance;
			MarginSize = marginSize;
			Justification = justification;
			// If no page selector function is provided, assume all pages are to be watermarked.
			_pageSelector = pageSelector ?? AspidiftraUtil.AllPagesSelector;
		}

		/// <summary>
		///   How opaque the watermark should be.
		/// </summary>
		public float Opacity => Appearance.Opacity;

		/// <summary>
		///   What pages should the watermark apply to?
		/// </summary>
		/// <param name="availablePageNumbers">All available page numbers.</param>
		/// <returns>Subset of page numbers to apply the watermark to.</returns>
		public IImmutableSet<int> GetApplicablePageNumbers(IImmutableSet<int> availablePageNumbers)
		{
			return _pageSelector(availablePageNumbers);
		}

		/// <summary>
		///   Calculate the elements that make up this watermark for the given page size.
		/// </summary>
		/// <param name="pageSize">Page size.</param>
		/// <returns>Watermark elements to render.</returns>
		public TextWatermarkElementCollection GetWatermarkElements(PageSize pageSize)
		{
			// Get the initial requested font size. This may change,
			// depending on the value of _fit.
			var font = Appearance.Font;
			var fontSize = font.GetSize(pageSize);

			// Reduce page size length by the margin size. If the remaining space
			// is zero or less, then throw an exception (thrown by the ApplyMargin
			// function).
			var effectiveMarginSize = MarginSize.GetEffectiveSize(pageSize);
			var pageSizeWithoutMargin = pageSize.ApplyMargin(effectiveMarginSize);

			// This object will provide "text slots" (places to put strings) to the text position calculator.
			var textSlotCalculator = GetTextSlotCalculator(pageSizeWithoutMargin);
			// So let's get the results of the text positioning.
			var positionedText = GetPositionedText(fontSize, textSlotCalculator);

			// We can now convert those calculated text positions to watermark elements.
			// All calculated coordinates from the TextPositionCalculator will be for a page size
			// without the margins. We need to reapply the margin offset here.
			var elements = positionedText.Select(txt => new TextWatermarkElement(
				txt.Position + new Offset(effectiveMarginSize, effectiveMarginSize),
				new FormattedText(txt.Text, Appearance.Color, Appearance.Font.Name, EncodingType.Winansi,
					false, positionedText.FontSize)));

			return new TextWatermarkElementCollection(elements, GetAngle(pageSize), Appearance.IsBackground);
		}

		/// <summary>
		///   Gets the text slot calculator for this type of watermark.
		/// </summary>
		/// <param name="pageSize">Page size.</param>
		/// <returns>Suitable text slot calculator.</returns>
		protected abstract ITextSlotCalculator GetTextSlotCalculator(PageSize pageSize);

		/// <summary>
		///   Gets the angle for this type of watermark.
		/// </summary>
		/// <param name="pageSize">Page size.</param>
		/// <returns>Angle for this watermark.</returns>
		protected abstract Angle GetAngle(PageSize pageSize);

		/// <summary>
		///   Gets the positioned text elements for this watermark.
		/// </summary>
		/// <param name="fontSize">Current font size.</param>
		/// <param name="textSlotCalculator">The text slot calculator for this type of watermark.</param>
		/// <returns>The positioned text collection.</returns>
		private PositionedTextCollection GetPositionedText(float fontSize, ITextSlotCalculator textSlotCalculator)
		{
			var textTokens = new StringTokenCollection(Text);
			IImmutableList<string> currentStrings = textTokens.GetStrings().ToImmutableList();

			var fontSizeMeasurementsCache = new FontSizeMeasurementsCache(Appearance.Font, textSlotCalculator);
			for (;;)
			{
				var fontSizeMeasurements = fontSizeMeasurementsCache.GetMeasurements(fontSize);
				try
				{
					IImmutableList<TextSlot> slots = fontSizeMeasurements.TextSlotProvider.GetTextSlots(currentStrings.Count)
						.ToImmutableList();

					IReadOnlyList<MeasuredString> measuredStrings =
						fontSizeMeasurements.MeasureStrings(currentStrings).ToImmutableList();

					IImmutableList<AllocatedTextSlot> allocatedTextSlots =
						measuredStrings.Zip(slots, (str, slot) => new AllocatedTextSlot(str, slot, Justification))
							.ToImmutableList();

					// Do the strings fit their allocated slots?
					var notAllStringsFit = allocatedTextSlots.Any(slot => !slot.TextFits);
					if (notAllStringsFit)
					{
						if (Fit.HasWrap())
							try
							{
								// Split the text to fit the slots.
								currentStrings = fontSizeMeasurements.SplitTextForSlots(textTokens, slots);
							}
							catch (SplitTextForSlotsOverflowException splitTextForSlotsOverflowException)
							{
								// There is too much text to fit the original number of slots. Re-iterate, and
								// we will request additional slots.
								currentStrings = splitTextForSlotsOverflowException.SplitStrings;
								currentStrings = currentStrings.AddRange(splitTextForSlotsOverflowException.OverflowTokens.GetStrings());
							}
						else if (Fit.HasShrink())
							// TODO: Make an estimate of shrink magnitude?
							fontSize = ShrinkFontSize(fontSize, MinimumFontSizeDelta);
						else
							// Not enough room on the page, and we're not allowed to do anything about it!
							throw new InsufficientSpaceException();
					}
					else
					{
						// If we didn't split any strings, and no shrinking is required, then we can return what we have.
						return GetPositionedTextCollection(allocatedTextSlots, fontSize);
					}
				}
				catch (CannotSplitTextException cannotSplitTextException)
				{
					// A line of text cannot be split at a suitable point.
					// If we're not allowed to shrink the font size, then we are out of options, so
					// the exception will just have to bubble up.
					if (Fit.HasShrink())
						fontSize = ShrinkFontSize(fontSize, MinimumFontSizeDelta);
					else
						throw new InsufficientSpaceException(cannotSplitTextException);
				}
				catch (InsufficientSlotsException insufficientSlotsException)
				{
					// There are not enough slots for the text.
					// Wrapping lines will only make this worse.
					// If we're not allowed to shrink the font size, then we are out of options, so
					// the exception will just have to bubble up.
					if (Fit.HasShrink())
						fontSize = ShrinkFontSize(fontSize, MinimumFontSizeDelta);
					else
						throw new InsufficientSpaceException(insufficientSlotsException);
				}
				catch (CannotReduceFontSizeException cannotReduceFontSizeException)
				{
					throw new InsufficientSpaceException(cannotReduceFontSizeException);
				}
			}
		}

		/// <summary>
		///   Reduces the given font size by the given amount.
		/// </summary>
		/// <param name="fontSize">
		///   Font size to reduce. If this is <see cref="MinimumFontSize" />, then a
		///   <see cref="CannotReduceFontSizeException" /> exception will be thrown.
		/// </param>
		/// <param name="shrinkAmount">Amount to reduce it by.</param>
		/// <returns>Reduced font size. It will never be reduced below <see cref="MinimumFontSize" />."/></returns>
		private static float ShrinkFontSize(float fontSize, float shrinkAmount)
		{
			if (Math.Abs(fontSize - MinimumFontSize) < float.Epsilon)
				throw new CannotReduceFontSizeException(MinimumFontSize);
			fontSize -= shrinkAmount;
			if (fontSize < MinimumFontSize)
				fontSize = MinimumFontSize;
			return fontSize;
		}

		/// <summary>
		///   Utility function for converting a list of <see cref="AllocatedTextSlot" />s into a
		///   <see cref="PositionedTextCollection" />.
		/// </summary>
		/// <param name="allocatedSlots">The allocated text slots.</param>
		/// <param name="fontSize">The current font size.</param>
		/// <returns>A positioned text collection.</returns>
		private static PositionedTextCollection GetPositionedTextCollection(IEnumerable<AllocatedTextSlot> allocatedSlots,
			float fontSize)
		{
			var positionedText = allocatedSlots.Select(allocatedSlot =>
			{
				var slotText = allocatedSlot.Text;
				var slot = allocatedSlot.Slot;
				var textOrigin = slot.EffectiveTextOrigin;
				var justifiedTextOrigin = textOrigin + allocatedSlot.JustificationOffset;
				return new PositionedText(slotText.Text, justifiedTextOrigin);
			});
			return new PositionedTextCollection(positionedText, fontSize);
		}
	}
}