using System.Collections.Generic;

namespace Aspidiftra
{
	internal class FontSizeMeasurements
	{
		private readonly Font _font;
		private readonly float _fontSize;
		private readonly IDictionary<string, double> _stringMeasurements = new Dictionary<string, double>();

		internal FontSizeMeasurements(Font font, float fontSize, ITextSlotProvider textSlotProvider)
		{
			_font = font;
			_fontSize = fontSize;
			TextSlotProvider = textSlotProvider;
		}

		internal ITextSlotProvider TextSlotProvider { get; }

		public double GetTextLength(string text)
		{
			if (!_stringMeasurements.TryGetValue(text, out var size))
			{
				size = _font.MeasureString(text, _fontSize);
				_stringMeasurements[text] = size;
			}

			return size;
		}
	}
}