using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Aspidiftra
{
	internal class TextPositionCalculator
	{
		// We will, at some point, be attempting to make some text fit better by increasing
		// or reducing the font size. The amount that we will increase or reduce by will always
		// be a multiple of this value. It will also never be less than this value, otherwise
		// we could spend an eternity making insanely small adjustments.
		private const float MinimumFontSizeDelta = 0.5f;

		// We will never use a font size any smaller than this.
		private const float MinimumFontSize = 1.0f;

		private readonly Fitting _fitting;
		private readonly Font _font;
		private readonly Justification _justification;
		private readonly ITextSlotCalculator _textSlotCalculator;

		internal TextPositionCalculator(ITextSlotCalculator textSlotCalculator,
			Font font, Justification justification, Fitting fitting)
		{
			_textSlotCalculator = textSlotCalculator;
			_font = font;
			_justification = justification;
			_fitting = fitting;
		}

		internal PositionedTextCollection GetPositionedText(IEnumerable<string> text, float fontSize)
		{
			// We may need to break up these strings, so convert them to a mutable list.
			var originalStrings = text.ToImmutableList();
			var fontSizeMeasurementsCache = new FontSizeMeasurementsCache(_font, _textSlotCalculator);
			var currentStrings = originalStrings;
			for (;;)
			{
				var fontSizeMeasurements = fontSizeMeasurementsCache.GetMeasurements(fontSize);
				IReadOnlyList<MeasuredString> measuredStrings =
					fontSizeMeasurements.MeasureStrings(currentStrings).ToImmutableList();
				var shrinkRequired = false;
				try
				{
					var slots = fontSizeMeasurements.TextSlotProvider.GetTextSlots(measuredStrings.Count);

					var assignedSlots =
						measuredStrings.Zip(slots, (str, slot) => new AssignedTextSlot(str, slot, _justification))
							.ToImmutableList();
					var (splitStrings, newShrinkRequired) =
						HandleExcessivelyLengthyStrings(assignedSlots, fontSizeMeasurements);
					shrinkRequired |= newShrinkRequired;

					// If we didn't split any strings, and no shrinking is required, then we can return what we have.
					if (measuredStrings.Count == splitStrings.Count && !shrinkRequired)
						return GetPositionedTextCollection(assignedSlots, fontSize);

					// Otherwise, let's go round again.
					// If a font size reduction is required, then any split strings are no longer valid.
					currentStrings = shrinkRequired ? originalStrings : splitStrings.Select(x => x.Text).ToImmutableList();
				}
				catch (InsufficientSlotsException)
				{
					// There are not enough slots for the text.
					// Wrapping lines will only make this worse.
					// If we're not allowed to shrink the font size, then we are out of options, so
					// the exception will just have to bubble up.
					if (!_fitting.HasShrink())
						throw;
					shrinkRequired = true;
					// TODO: Make an estimate of shrink magnitude?
					//var availableSlots = insufficientSlotsException.AvailableSlots;
					//var requestedSlots = insufficientSlotsException.RequestedSlots;
				}

				if (shrinkRequired)
					fontSize = ShrinkFontSize(fontSize, MinimumFontSizeDelta);
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

		private PositionedTextCollection GetPositionedTextCollection(IEnumerable<AssignedTextSlot> assignedSlots,
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

		private (IImmutableList<MeasuredString> Text, bool ShrinkRequired) HandleExcessivelyLengthyStrings(
			IEnumerable<AssignedTextSlot> assignedSlots, FontSizeMeasurements fontSizeMeasurements)
		{
			var shrinkRequired = false;
			var handledStringSubCollections = assignedSlots.Select(assignedSlot =>
			{
				var slotText = assignedSlot.Text;
				var slot = assignedSlot.Slot;
				var measuredString = fontSizeMeasurements.MeasureString(slotText.Text);
				if (measuredString.Length > slot.Width)
				{
					var (handledText, newShrinkRequired) =
						HandleExcessivelyLengthyString(measuredString, fontSizeMeasurements, slot.Width);
					shrinkRequired |= newShrinkRequired;
					return handledText;
				}

				return new[] {measuredString};
			});
			var handledStrings = handledStringSubCollections.SelectMany(s => s).ToImmutableList();
			return (handledStrings, shrinkRequired);
		}

		private (IEnumerable<MeasuredString> Text, bool ShrinkRequired) HandleExcessivelyLengthyString(
			MeasuredString measuredString,
			FontSizeMeasurements fontSizeMeasurements, double slotWidth)
		{
			var shrinkRequired = false;
			if (_fitting.HasWrap() && measuredString.IsSplittable)
				try
				{
					return (fontSizeMeasurements.SplitTextToFit(measuredString.Text, slotWidth), false);
				}
				catch (FontSizeMeasurements.CannotSplitTextException)
				{
					if (!_fitting.HasShrink())
						throw;
					shrinkRequired = true;
				}
			else if (_fitting.HasShrink())
				shrinkRequired = true;

			return (new[] {measuredString}, shrinkRequired);
		}

		internal class FontSizeMeasurementsCache
		{
			private readonly IDictionary<float, FontSizeMeasurements>
				_cache = new Dictionary<float, FontSizeMeasurements>();

			private readonly Font _font;
			private readonly ITextSlotCalculator _textSlotCalculator;

			internal FontSizeMeasurementsCache(Font font, ITextSlotCalculator textSlotCalculator)
			{
				_font = font;
				_textSlotCalculator = textSlotCalculator;
			}

			internal FontSizeMeasurements GetMeasurements(float fontSize)
			{
				if (!_cache.TryGetValue(fontSize, out var fontSizeMeasurements))
				{
					fontSizeMeasurements =
						new FontSizeMeasurements(_font, fontSize, _textSlotCalculator.CalculateSlots(fontSize));
					_cache.Add(fontSize, fontSizeMeasurements);
				}

				return fontSizeMeasurements;
			}
		}
	}
}