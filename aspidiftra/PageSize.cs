using System;
using Aspidiftra.Geometry;

namespace Aspidiftra
{
	public class PageSize : Rectangle
	{
		public PageSize(double width, double height) : base(0.0, 0.0, width, height)
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

		/// <summary>
		///   Calculates the angle between the X axis and the line that
		///   runs from the bottom-left corner to the top-right corner.
		/// </summary>
		/// <returns>
		///   Angle between the X axis and the line that runs from the
		///   bottom-left corner to the top-right corner.
		/// </returns>
		public Angle BottomLeftToTopRightAngle()
		{
			return new Angle(Math.Atan(Height / Width), AngleUnits.Radians);
		}

		/// <summary>
		///   Calculates the angle between the X axis and the line that
		///   runs from the bottom-right corner to the top-left corner.
		/// </summary>
		/// <returns>
		///   Angle between the X axis and the line that runs from the
		///   bottom-right corner to the top-left corner.
		/// </returns>
		public Angle BottomRightToTopLeftAngle()
		{
			return BottomLeftToTopRightAngle().ReverseY();
		}

		/// <summary>
		///   Calculates the angle between the X axis and the line that
		///   runs from the top-left corner to the bottom-right corner.
		/// </summary>
		/// <returns>
		///   Angle between the X axis and the line that runs from the
		///   top-left corner to the bottom-right corner.
		/// </returns>
		public Angle TopLeftToBottomRightAngle()
		{
			return BottomLeftToTopRightAngle().ReverseX();
		}

		/// <summary>
		///   Calculates the angle between the X axis and the line that
		///   runs from the top-right corner to the bottom-left corner.
		/// </summary>
		/// <returns>
		///   Angle between the X axis and the line that runs from the
		///   top-right corner to the bottom-left corner.
		/// </returns>
		public Angle TopRightToBottomLeftAngle()
		{
			return BottomLeftToTopRightAngle().Reverse();
		}
	}
}