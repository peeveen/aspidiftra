using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

		internal IEnumerable<MeasuredString> MeasureStrings(IEnumerable<string> strings)
		{
			return strings.Select(MeasureString);
		}

		internal IImmutableList<string> SplitTextForSlots(string text, IEnumerable<TextSlot> slots)
		{
			var words = text.Split(' ', '\t').ToList();
			var splitStrings = new List<string>();
			foreach (var slot in slots)
			{
				var availableSpace = slot.Width;
				var currentString = new StringBuilder();
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
						// The last word we added to the current string has made it too long.
						if (wordCount == 1)
							// If it was the first word we added, then oops! It's a single
							// word that's too big for the slot. Chuck an exception.
							throw new CannotSplitTextException(text);
						// Otherwise, use what we had before we added that word, put
						// that word back into the word bag, and move onto the next slot.
						splitStrings.Add(lastString);
						words.Insert(0, word);
						break;
					}

					if (!words.Any())
						// We've run out of words, and we haven't exceeded the slot width.
						// So use what we've built so far.
						splitStrings.Add(measuredString.Text);
				}
			}

			// If there are still words left over, we just return them as one big
			// string.
			if (words.Any())
				splitStrings.Add(string.Join(' ', words));
			return splitStrings.ToImmutableList();
		}

		internal class CannotSplitTextException : Exception
		{
			internal CannotSplitTextException(string text) : base($"Could not split the text '{text}' across multiple lines.")
			{
			}
		}
	}
}