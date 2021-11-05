using Aspidiftra.Geometry;
using Aspose.Pdf;

namespace Aspidiftra
{
	public enum PageEdgePosition
	{
		/// <summary>
		///   Watermark will be along top edge of page.
		///   Top edge of the text will be along the top of the page.
		/// </summary>
		North,

		/// <summary>
		///   Watermark will be along bottom edge of page.
		///   Bottom edge of the text will be along the bottom of the page.
		/// </summary>
		South,

		/// <summary>
		///   Watermark will be along the right-hand-side edge of the page.
		///   Top edge of the text will be along the right-hand-side edge of the page.
		/// </summary>
		East,

		/// <summary>
		///   Watermark will be along the left-hand-side edge of the page.
		///   Top edge of the text will be along the left-hand-side edge of the page.
		/// </summary>
		West
	}

	public static class PageEdgePositionExtensions
	{
		public static Angle GetAngle(this PageEdgePosition pageEdgePosition)
		{
			return pageEdgePosition switch
			{
				PageEdgePosition.East => Angle.Degrees270,
				PageEdgePosition.West => Angle.Degrees90,
				_ => Angle.Degrees0
			};
		}

		public static double GetPageSideLength(this PageEdgePosition pageEdgePosition, PageSize pageSize, bool opposite=false)
		{
			return pageEdgePosition switch
			{
				// Annoying duplication here ... roll on, C# 9.0
				PageEdgePosition.North when opposite => pageSize.Height,
				PageEdgePosition.North => pageSize.Width,
				PageEdgePosition.South when opposite => pageSize.Height,
				PageEdgePosition.South => pageSize.Width,
				_ when opposite => pageSize.Width,
				_ => pageSize.Height
			};
		}
	}
}