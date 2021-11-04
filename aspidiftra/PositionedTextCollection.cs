using System.Collections;
using System.Collections.Generic;

namespace Aspidiftra
{
	internal class PositionedTextCollection : IEnumerable<PositionedText>
	{
		private readonly IEnumerable<PositionedText> _positionedText;

		internal PositionedTextCollection(IEnumerable<PositionedText> positionedText, float fontSize)
		{
			_positionedText = positionedText;
			FontSize = fontSize;
		}

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