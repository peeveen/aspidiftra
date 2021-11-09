using System.Collections;
using System.Collections.Generic;

namespace Aspidiftra
{
	/// <summary>
	///   A collection of positioned text.
	/// </summary>
	internal class PositionedTextCollection : IEnumerable<PositionedText>
	{
		private readonly IEnumerable<PositionedText> _positionedText;

		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="positionedText">All the positioned text in this collection.</param>
		/// <param name="fontSize">The size of font to display the text with.</param>
		internal PositionedTextCollection(IEnumerable<PositionedText> positionedText, float fontSize)
		{
			_positionedText = positionedText;
			FontSize = fontSize;
		}

		/// <summary>
		///   The size of font that this positioned text should be displayed with.
		/// </summary>
		public float FontSize { get; }

		public IEnumerator<PositionedText> GetEnumerator()
		{
			return _positionedText.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _positionedText.GetEnumerator();
		}
	}
}