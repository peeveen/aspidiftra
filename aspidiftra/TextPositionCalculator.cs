using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Aspidiftra
{
	internal class TextPositionCalculator
	{
		private readonly Fitting _fit;
		private readonly Font _font;
		private readonly Justification _justification;
		private readonly ITextSlotCalculator _textSlotCalculator;

		internal TextPositionCalculator(ITextSlotCalculator textSlotCalculator,
			Font font, Justification justification, Fitting fit)
		{
			_textSlotCalculator = textSlotCalculator;
			_font = font;
			_justification = justification;
			_fit = fit;
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