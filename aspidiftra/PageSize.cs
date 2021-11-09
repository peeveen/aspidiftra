using System;
using Aspidiftra.Geometry;

namespace Aspidiftra
{
	/// <summary>
	///   Subclass of rectangle, specifically describing the size of a page.
	///   A page size rectangle always has it's origin at (0,0).
	/// </summary>
	public class PageSize : Rectangle
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="width">Width of page.</param>
		/// <param name="height">Height of page.</param>
		internal PageSize(double width, double height) : base(0.0, 0.0, width, height)
		{
		}

		/// <summary>
		///   Applies a margin to the page size. If the margin is greater than half of the
		///   page width or height, a <see cref="MarginTooLargeException" /> exception will be thrown.
		/// </summary>
		/// <param name="amount">Size of margin.</param>
		/// <returns>Resulting page size.</returns>
		internal PageSize ApplyMargin(double amount)
		{
			try
			{
				var newRect = Deflate(amount);
				return new PageSize(newRect.Width, newRect.Height);
			}
			catch (ArgumentException ae)
			{
				throw new MarginTooLargeException(amount, ae);
			}
		}

		/// <summary>
		///   Calculates the angle between the X axis and the line that
		///   runs from the bottom-left corner to the top-right corner.
		/// </summary>
		/// <returns>
		///   Angle between the X axis and the line that runs from the
		///   bottom-left corner to the top-right corner.
		/// </returns>
		internal Angle BottomLeftToTopRightAngle()
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
		internal Angle BottomRightToTopLeftAngle()
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
		internal Angle TopLeftToBottomRightAngle()
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
		internal Angle TopRightToBottomLeftAngle()
		{
			return BottomLeftToTopRightAngle().Reverse();
		}
	}
}