using Aspidiftra.Geometry;

namespace Aspidiftra
{
	/// <summary>
	///   Enumeration of possible page positions for a <see cref="PageEdgeTextWatermark" />.
	/// </summary>
	public enum PageEdgePosition
	{
		/// <summary>
		///   Watermark will be along top edge of page.
		///   Top edge of the text will be along the top of the page.
		/// </summary>
		Top,

		/// <summary>
		///   Watermark will be along bottom edge of page.
		///   Bottom edge of the text will be along the bottom of the page.
		/// </summary>
		Bottom,

		/// <summary>
		///   Watermark will be along the right-hand-side edge of the page.
		///   Top edge of the text will be along the right-hand-side edge of the page.
		/// </summary>
		Right,

		/// <summary>
		///   Watermark will be along the left-hand-side edge of the page.
		///   Top edge of the text will be along the left-hand-side edge of the page.
		/// </summary>
		Left
	}

	public static class PageEdgePositionExtensions
	{
		/// <summary>
		///   How should the text be angled, by default, for this page edge position?
		/// </summary>
		/// <param name="pageEdgePosition">Page edge position.</param>
		/// <returns>Default angle of text for this page position.</returns>
		public static Angle GetAngle(this PageEdgePosition pageEdgePosition)
		{
			return pageEdgePosition switch
			{
				PageEdgePosition.Right => Angle.Degrees270,
				PageEdgePosition.Left => Angle.Degrees90,
				_ => Angle.Degrees0
			};
		}

		/// <summary>
		///   Given the page size, return the width of the side of the page that is relevant to the
		///   watermark page edge position.
		/// </summary>
		/// <param name="pageEdgePosition">What page edge position will the watermark be at?</param>
		/// <param name="pageSize">The page size.</param>
		/// <param name="opposite">
		///   False to return the length of the page side that is relevant to
		///   the page edge position, or true to return the length of the other page side.
		/// </param>
		/// <returns>Length of the requested page side.</returns>
		public static double GetPageSideLength(this PageEdgePosition pageEdgePosition, PageSize pageSize,
			bool opposite = false)
		{
			return pageEdgePosition switch
			{
				// Annoying duplication here ... roll on, C# 9.0
				PageEdgePosition.Top when opposite => pageSize.Height,
				PageEdgePosition.Top => pageSize.Width,
				PageEdgePosition.Bottom when opposite => pageSize.Height,
				PageEdgePosition.Bottom => pageSize.Width,
				_ when opposite => pageSize.Width,
				_ => pageSize.Height
			};
		}
	}
}