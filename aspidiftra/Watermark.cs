using System;
using System.Collections.Immutable;

namespace Aspidiftra
{
	public abstract class Watermark : IWatermark
	{
		private readonly Func<IImmutableSet<int>, IImmutableSet<int>> _pageSelector;

		protected Watermark(float opacity, Func<IImmutableSet<int>, IImmutableSet<int>>? pageSelector = null)
		{
			Opacity = opacity;
			// If no page selector function is provided, assume all pages are to be watermarked.
			_pageSelector = pageSelector ?? AspidiftraUtil.AllPagesSelector;
		}

		public float Opacity { get; }

		public IImmutableSet<int> GetApplicablePageNumbers(IImmutableSet<int> availablePageNumbers)
		{
			return _pageSelector(availablePageNumbers);
		}

		public abstract WatermarkElementCollection GetWatermarkElements(PageSize pageMediaBox);
	}
}