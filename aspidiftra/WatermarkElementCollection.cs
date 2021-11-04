using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Aspidiftra
{
	public class WatermarkElementCollection : IEnumerable<WatermarkElement>
	{
		private readonly IEnumerable<WatermarkElement> _elements;

		public WatermarkElementCollection(IEnumerable<WatermarkElement> elements, Angle angle, bool isBackground)
		{
			Angle = angle;
			IsBackground = isBackground;
			_elements = elements.ToImmutableArray();
		}

		public Angle Angle { get; }
		public bool IsBackground { get; }

		public IEnumerator<WatermarkElement> GetEnumerator()
		{
			return _elements.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _elements.GetEnumerator();
		}
	}
}