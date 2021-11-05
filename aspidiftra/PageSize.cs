using Aspidiftra.Geometry;

namespace Aspidiftra
{
	public class PageSize : Rectangle
	{
		public PageSize(double width, double height) : base(0, 0, width, height)
		{
		}

		public PageSize(Rectangle rectangle) : base(rectangle)
		{
		}

		public PageSize ApplyMargin(double amount)
		{
			var result = new PageSize(Deflate(amount));
			if (result.Left >= result.Right)
				throw new MarginTooLargeException(amount, MarginTooLargeException.PageSide.Width, result.Width);
			if (result.Bottom >= result.Top)
				throw new MarginTooLargeException(amount, MarginTooLargeException.PageSide.Height,
					result.Height);
			return result;
		}
	}
}