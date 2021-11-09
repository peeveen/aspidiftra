using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Aspidiftra.Geometry;

namespace Aspidiftra
{
	/// <summary>
	///   Collection of <see cref="TextWatermarkElement" />s, plus some additional rendering info.
	/// </summary>
	public class TextWatermarkElementCollection : IEnumerable<TextWatermarkElement>
	{
		private readonly IEnumerable<TextWatermarkElement> _elements;

		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="elements">Text elements to render.</param>
		/// <param name="angle">Angle to render them at.</param>
		/// <param name="isBackground">
		///   True if they are to be rendered in the background layer of the document, false for
		///   foreground.
		/// </param>
		public TextWatermarkElementCollection(IEnumerable<TextWatermarkElement> elements, Angle angle, bool isBackground)
		{
			Angle = angle;
			IsBackground = isBackground;
			_elements = elements.ToImmutableArray();
		}

		/// <summary>
		///   Angle to render the text elements at.
		/// </summary>
		public Angle Angle { get; }

		/// <summary>
		/// Are the text elements to be rendered in the background? Or foreground?
		/// </summary>
		public bool IsBackground { get; }

		public IEnumerator<TextWatermarkElement> GetEnumerator()
		{
			return _elements.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _elements.GetEnumerator();
		}
	}
}