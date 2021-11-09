using System.Collections.Generic;

namespace Aspidiftra
{
	/// <summary>
	///   A cache of <see cref="FontSizeMeasurements" /> for various font sizes.
	/// </summary>
	internal class FontSizeMeasurementsCache
	{
		private readonly IDictionary<float, FontSizeMeasurements>
			_cache = new Dictionary<float, FontSizeMeasurements>();

		private readonly Font _font;
		private readonly ITextSlotCalculator _textSlotCalculator;

		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="font">The font that we are using.</param>
		/// <param name="textSlotCalculator">A watermark text slot calculator.</param>
		internal FontSizeMeasurementsCache(Font font, ITextSlotCalculator textSlotCalculator)
		{
			_font = font;
			_textSlotCalculator = textSlotCalculator;
		}

		/// <summary>
		/// Gets the <see cref="FontSizeMeasurements"/> pertaining to the given font size.
		/// </summary>
		/// <param name="fontSize">Font size.</param>
		/// <returns>A <see cref="FontSizeMeasurements"/> object for the given font size.</returns>
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