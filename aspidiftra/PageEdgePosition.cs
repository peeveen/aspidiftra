using Aspidiftra.Geometry;

namespace Aspidiftra
{
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
		public static Angle GetAngle(this PageEdgePosition pageEdgePosition)
		{
			return pageEdgePosition switch
			{
				PageEdgePosition.Right => Angle.Degrees270,
				PageEdgePosition.Left => Angle.Degrees90,
				_ => Angle.Degrees0
			};
		}

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