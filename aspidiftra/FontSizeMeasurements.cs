using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Aspidiftra
{
	/// <summary>
	///   A cache of measurements relating to a specific font and font size.
	///   The things that are cached are calculated text slots, and string widths.
	///   We cache these because, during watermark fitting, it is possible that
	///   the font size might be increased and reduced many times gradually, and we
	///   don't want to have to calculate the same text slots and string widths all
	///   over again.
	/// </summary>
	internal class FontSizeMeasurements
	{
		private readonly Font _font;
		private readonly IDictionary<string, MeasuredString> _stringMeasurements = new Dictionary<string, MeasuredString>();

		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="font">Font that the watermark is being rendered with.</param>
		/// <param name="fontSize">Specific size of the font that this item will be measuring.</param>
		/// <param name="textSlotProvider">Current text slot provider.</param>
		internal FontSizeMeasurements(Font font, float fontSize, ITextSlotProvider textSlotProvider)
		{
			_font = font;
			FontSize = fontSize;
			TextSlotProvider = textSlotProvider;
		}

		/// <summary>
		/// Size of font that these measurements are for.
		/// </summary>
		internal float FontSize { get; }

		/// <summary>
		///   The text slot provider, loaded with the slots that were calculated specifically for the
		///   font size that this set of measurements is for.
		/// </summary>
		internal ITextSlotProvider TextSlotProvider { get; }

		/// <summary>
		///   Measures the given string.
		/// </summary>
		/// <param name="text">String to measure.</param>
		/// <returns>A <see cref="MeasuredString" /> object.</returns>
		internal MeasuredString MeasureString(string text)
		{
			if (!_stringMeasurements.TryGetValue(text, out var measuredString))
			{
				var width = _font.MeasureString(text, FontSize);
				measuredString = _stringMeasurements[text] = new MeasuredString(text, width);
			}

			return measuredString;
		}

		/// <summary>
		///   Measures the given strings.
		/// </summary>
		/// <param name="text">Strings to measure.</param>
		/// <returns>A collection of <see cref="MeasuredString" /> objects.</returns>
		internal IEnumerable<MeasuredString> MeasureStrings(IEnumerable<string> text)
		{
			return text.Select(MeasureString);
		}

		/// <summary>
		///   Splits the given text tokens into lines that fit the given slots. If, after splitting
		///   the text to fit the slots, there is leftover text, a <see cref="SplitTextForSlotsOverflowException" />
		///   exception will be thrown.
		/// </summary>
		/// <param name="tokens">String tokens to build into lines.</param>
		/// <param name="slots">Slots that we want the text to fit.</param>
		/// <returns>
		///   List of strings that fit the given slots.
		/// </returns>
		internal IImmutableList<string> SplitTextForSlots(StringTokenCollection tokens, IEnumerable<TextSlot> slots)
		{
			var splitStrings = new List<string>();
			foreach (var slot in slots)
			{
				var availableSpace = slot.Width;
				var stringBuilder = new StringBuilder();
				var contentCount = 0;

				void AddLine(string line)
				{
					splitStrings.Add(line);
					stringBuilder = new StringBuilder();
					contentCount = 0;
				}

				while (tokens.Any())
				{
					var lastString = stringBuilder.ToString();
					var (content, remainder) = tokens.GetNextContent();

					if (content.Count()==1 && content.First().Type == StringToken.TokenType.LineBreak)
					{
						AddLine(stringBuilder.ToString());
					}
					else
					{
						// Content is guaranteed to only contain one line's worth of tokens.
						var contentString = content.ToString();
						stringBuilder.Append(contentString);
						++contentCount;

						var measuredString = MeasureString(stringBuilder.ToString());
						if (measuredString.Length > availableSpace)
						{
							// The last bit of content we added to the current string has made
							// it too long.
							if (contentCount == 1)
								// If it was the first bit of content we added, then oops!
								// It's a single word that's too big for the slot. Chuck an
								// exception. (it's guaranteed not to have been a whitespace
								// token).
								throw new CannotSplitTextException(contentString);
							// Otherwise, use what we had before we added that word, put
							// that word back into the word bag, and move onto the next slot.
							AddLine(lastString);
							break;
						}
					}

					if (!remainder.Any())
						// We've run out of words, and we haven't exceeded the slot width.
						// So use what we've built so far.
						AddLine(stringBuilder.ToString());
					tokens = remainder;
				}
			}

			var resultsList = splitStrings.ToImmutableList();

			// If there are still words left over, chuck an exception.
			if (tokens.Any())
				throw new SplitTextForSlotsOverflowException(resultsList, tokens.Normalize());
			return resultsList;
		}
	}
}