using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aspidiftra.Geometry;
using Aspose.Pdf.Facades;

namespace Aspidiftra
{
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
		///   Base class for watermarks.
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
			if (Fit.HasWrap() && Text.Contains('\n') && Text.Contains("\r\n"))
				throw new ArgumentException(
					$"Cannot use {nameof(Fitting.Wrap)} when rendering watermark text that already contains explicit line breaks.");
			Appearance = appearance;
			MarginSize = marginSize;
			Justification = justification;
			// If no page selector function is provided, assume all pages are to be watermarked.
			_pageSelector = pageSelector ?? AspidiftraUtil.AllPagesSelector;
		}

		public float Opacity => Appearance.Opacity;

		public IImmutableSet<int> GetApplicablePageNumbers(IImmutableSet<int> availablePageNumbers)
		{
			return _pageSelector(availablePageNumbers);
		}

		public WatermarkElementCollection GetWatermarkElements(PageSize pageSize)
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
			var elements = positionedText.Select(txt => new WatermarkElement(
				txt.Position + new Offset(effectiveMarginSize, effectiveMarginSize),
				new FormattedText(txt.Text, Appearance.Color, Appearance.Font.Name, EncodingType.Winansi,
					false, positionedText.FontSize)));

			return new WatermarkElementCollection(elements, GetAngle(pageSize), Appearance.IsBackground);
		}

		protected abstract ITextSlotCalculator GetTextSlotCalculator(PageSize pageSize);

		protected abstract Angle GetAngle(PageSize pageSize);

		internal PositionedTextCollection GetPositionedText(float fontSize, ITextSlotCalculator textSlotCalculator)
		{
			IImmutableList<string> originalStrings = AspidiftraUtil.SplitTextIntoLines(Text);
			var fontSizeMeasurementsCache = new FontSizeMeasurementsCache(Appearance.Font, textSlotCalculator);
			IImmutableList<string> currentStrings = originalStrings;
			for (;;)
			{
				var fontSizeMeasurements = fontSizeMeasurementsCache.GetMeasurements(fontSize);
				IReadOnlyList<MeasuredString> measuredStrings =
					fontSizeMeasurements.MeasureStrings(currentStrings).ToImmutableList();
				try
				{
					IImmutableList<TextSlot> slots = fontSizeMeasurements.TextSlotProvider.GetTextSlots(measuredStrings.Count)
						.ToImmutableList();

					IImmutableList<AllocatedTextSlot> allocatedTextSlots =
						measuredStrings.Zip(slots, (str, slot) => new AllocatedTextSlot(str, slot, Justification))
							.ToImmutableList();

					// Do the strings fit their allocated slots?
					var notAllStringsFit = allocatedTextSlots.Any(slot => !slot.TextFits);
					if (notAllStringsFit)
					{
						if (Fit.HasWrap())
							// Split the text to fit the slots. This might result in an extra line of text, but
							// if that happens, we'll just loop round to the next iteration and it will calculate
							// extra slots.
							currentStrings = fontSizeMeasurements.SplitTextForSlots(originalStrings.First(), slots);
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

		private static float ShrinkFontSize(float fontSize, float shrinkAmount)
		{
			if (Math.Abs(fontSize - MinimumFontSize) < float.Epsilon)
				throw new CannotReduceFontSizeException(MinimumFontSize);
			fontSize -= shrinkAmount;
			if (fontSize < MinimumFontSize)
				fontSize = MinimumFontSize;
			return fontSize;
		}

		private static PositionedTextCollection GetPositionedTextCollection(IEnumerable<AllocatedTextSlot> assignedSlots,
			float fontSize)
		{
			var positionedText = assignedSlots.Select(assignedSlot =>
			{
				var slotText = assignedSlot.Text;
				var slot = assignedSlot.Slot;
				var textOrigin = slot.EffectiveTextOrigin;
				var justifiedTextOrigin = textOrigin + assignedSlot.JustificationOffset;
				return new PositionedText(slotText.Text, justifiedTextOrigin);
			});
			return new PositionedTextCollection(positionedText, fontSize);
		}
	}
}