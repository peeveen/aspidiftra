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
			var newWidth = Width - amount * 2.0;
			var newHeight = Height - amount * 2.0;
			if (newWidth <= 0.0)
				throw new MarginTooLargeException(amount, MarginTooLargeException.PageSide.Width, newWidth);
			if (newHeight <= 0.0)
				throw new MarginTooLargeException(amount, MarginTooLargeException.PageSide.Height, newHeight);
			return new PageSize(newWidth, newHeight);
		}
	}
}