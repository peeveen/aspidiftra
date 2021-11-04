using System;
using System.Collections.Generic;
using System.Linq;

namespace Aspidiftra
{
	internal class TextPositionCalculator
	{
		// We will, at some point, be attempting to make some text fit better by increasing
		// or reducing the font size. The amount that we will increase or reduce by will always
		// be a multiple of this value. It will also never be less than this value, otherwise
		// we could spend an eternity making insanely small adjustments.
		private const float FontSizeDelta = 0.5f;
		private static readonly (double, double) NoOffset = (0.0, 0.0);

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

		private (double X, double Y) CalculateJustificationOffset(TextSlot slot, double textLength)
		{
			var leftJustifiedAlready = slot.Angle < Angle.RadiansPi;
			var rightJustifiedAlready = slot.Angle >= Angle.RadiansPi;
			var spareSlotSpace = slot.Width - textLength;
			return _justification switch
			{
				Justification.Left when leftJustifiedAlready => NoOffset,
				Justification.Right when rightJustifiedAlready => NoOffset,
				Justification.Centre =>
					(Math.Abs(spareSlotSpace / 2.0 * slot.Angle.Cos), Math.Abs(spareSlotSpace / 2.0 * slot.Angle.Sin)),
				_ =>
					// Due to the angle, text is appearing left or right justified by default,
					// but we want it the opposite way.
					(Math.Abs(spareSlotSpace * slot.Angle.Cos), Math.Abs(spareSlotSpace * slot.Angle.Sin))
			};
		}

		internal PositionedTextCollection GetPositionedText(IEnumerable<string> text, float fontSize)
		{
			// We may need to break up these strings, so convert them to a mutable list.
			var textList = text.ToList();
			var fontSizeMeasurementsCache = new Dictionary<double, FontSizeMeasurements>();
			for (;;)
				try
				{
					if (!fontSizeMeasurementsCache.TryGetValue(fontSize, out var fontSizeMeasurements))
					{
						fontSizeMeasurements =
							new FontSizeMeasurements(_font, fontSize, _textSlotCalculator.CalculateSlots(fontSize));
						fontSizeMeasurementsCache.Add(fontSize, fontSizeMeasurements);
					}

					var slots = fontSizeMeasurements.TextSlotProvider.GetTextSlots(textList.Count);

					// Don't really need the lambda here, but I prefer named properties over the First/Second syntax.
					var assignedSlots = textList.Zip(slots, (str, slot) => (Text: str, Slot: slot));

					var positionedText = assignedSlots.Select(assignedSlot =>
					{
						var textLength = fontSizeMeasurements.GetTextLength(assignedSlot.Text);
						var justificationOffset =
							CalculateJustificationOffset(assignedSlot.Slot, textLength);
						var textOrigin = assignedSlot.Slot.EffectiveTextOrigin;
						var justifiedTextOrigin = textOrigin.OffsetBy(justificationOffset.X, justificationOffset.Y);
						return new PositionedText(assignedSlot.Text, justifiedTextOrigin);
					});
					return new PositionedTextCollection(positionedText, fontSize);
				}
				catch (InsufficientSpaceException insufficientSpaceException)
				{
				}
		}
	}
}