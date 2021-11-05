using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

		internal MeasuredString MeasureString(string text)
		{
			if (!_stringMeasurements.TryGetValue(text, out var size))
			{
				size = _font.MeasureString(text, _fontSize);
				_stringMeasurements[text] = size;
			}

			return new MeasuredString(text, size);
		}

		internal IEnumerable<MeasuredString> SplitTextToFit(string text, double availableSpace)
		{
			var words = text.Split(' ', '\t').ToList();
			var splitStrings = new List<MeasuredString>();
			var currentString = new StringBuilder();
			var lastStringLength = 0.0;
			var wordCount = 0;
			while (words.Any())
			{
				var lastString = currentString.ToString();
				var word = words[0];
				words.RemoveAt(0);
				if (wordCount++ > 0)
					currentString.Append(' ');
				currentString.Append(word);

				var measuredString = MeasureString(currentString.ToString());
				if (measuredString.Length > availableSpace)
				{
					if (wordCount == 1)
					{
						splitStrings.Add(new MeasuredString(measuredString, false));
					}
					else
					{
						splitStrings.Add(new MeasuredString(lastString, lastStringLength));
						words.Insert(0, word);
					}

					wordCount = 0;
					currentString = new StringBuilder();
				}
				else if (!words.Any())
				{
					splitStrings.Add(new MeasuredString(measuredString, wordCount == 1));
				}

				lastStringLength = measuredString.Length;
			}

			if (splitStrings.Count == 1)
				throw new CannotSplitTextException(text);
			return splitStrings;
		}

		internal IEnumerable<MeasuredString> MeasureStrings(IEnumerable<string> strings)
		{
			return strings.Select(MeasureString);
		}

		internal class CannotSplitTextException : Exception
		{
			internal CannotSplitTextException(string text) : base($"Could not split the text '{text}' across multiple lines.")
			{
			}
		}
	}
}