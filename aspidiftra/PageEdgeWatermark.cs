using System;
using System.Collections.Immutable;
using Aspidiftra.Geometry;

namespace Aspidiftra
{
	public class PageEdgeWatermark : Watermark
	{
		private readonly Angle _angle;
		private readonly PageEdgePosition _pageEdgePosition;
		private readonly bool _reverseDirection;

		/// <summary>
		///   A page edge watermark, allowing text to be placed alongside any of the
		///   four pages edges, running in either appropriate orthogonal direction.
		/// </summary>
		/// <param name="text">
		///   Text to place along the edge of the page. Can contain line breaks for
		///   multi-line watermarks.
		/// </param>
		/// <param name="position">What page edge should it be placed along?</param>
		/// <param name="justification">
		///   Justification of text (relative to the default text direction of
		///   <paramref name="position" />).
		/// </param>
		/// <param name="appearance">Stylistic attributes for the text.</param>
		/// <param name="marginSize">
		///   Size of margin, if you want to ensure the watermark isn't actually
		///   touching the page edge.
		/// </param>
		/// <param name="pageSelector">
		///   Function that will select the pages that the watermark will appear on,
		///   from a given set of page numbers. If no value is provided for this
		///   parameter, <see cref="AspidiftraUtil.AllPagesSelector" /> will be used.
		/// </param>
		/// <param name="reverseDirection">
		///   True if the direction of the text should be reversed from the
		///   default text direction of <paramref name="position" />.
		///   A value of true can result in text being rendered upside-down if
		///   <paramref name="position" /> is Top or Bottom.
		/// </param>
		/// <param name="fit">
		///   Should the text be made smaller/larger/wrapped to fit the area?
		///   Note that using <see cref="Fitting.Grow" /> is not generally
		///   a good idea, as it can result in enormous text. Also, there is no
		///   mechanism in place to prevent page edge watermarks on different page
		///   edges from overlapping.
		/// </param>
		public PageEdgeWatermark(string text, Appearance appearance, PageEdgePosition position,
			Justification justification, Fitting fit, Size marginSize, bool reverseDirection = false,
			Func<IImmutableSet<int>, IImmutableSet<int>>? pageSelector = null
		) : base(text, appearance, justification, fit, marginSize, pageSelector)
		{
			_reverseDirection = reverseDirection;
			_pageEdgePosition = position;
			// What angle will the text be at?
			// It will be an orthogonal angle, i.e. multiple of 90 degrees.
			_angle = _pageEdgePosition.GetAngle();
			if (reverseDirection)
				_angle = _angle.Reverse();
		}

		protected override ITextSlotCalculator GetTextSlotCalculator(PageSize pageSize)
		{
			return new PageEdgeTextSlotCalculator(pageSize, _pageEdgePosition, _angle, _reverseDirection);
		}

		protected override Angle GetWatermarkAngle(PageSize pageSize)
		{
			return _angle;
		}
	}
}