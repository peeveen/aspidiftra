using System.Collections.Generic;

namespace Aspidiftra
{
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