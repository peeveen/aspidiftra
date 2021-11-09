using System;

namespace Aspidiftra.Geometry
{
	/// <summary>
	///   Object representing a rectangle. Rectangles are always simple, non-rotated,
	///   rectangles, with the lower-left corner point having lower coordinate values
	///   than the upper-right corner point.
	/// </summary>
	public class Rectangle
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="left">X value for the left edge of the rectangle.</param>
		/// <param name="bottom">Y value for the bottom edge of the rectangle.</param>
		/// <param name="right">X value for the right edge of the rectangle.</param>
		/// <param name="top">Y value for the top edge of the rectangle.</param>
		public Rectangle(double left, double bottom, double right, double top)
		{
			GeometryUtil.ValidateArgument(left, nameof(left));
			GeometryUtil.ValidateArgument(right, nameof(right));
			GeometryUtil.ValidateArgument(top, nameof(top));
			GeometryUtil.ValidateArgument(bottom, nameof(bottom));
			if (left > right)
				throw new ArgumentException("Invalid rectangle: left > right.");
			if (bottom > top)
				throw new ArgumentException("Invalid rectangle: bottom > top.");
			Left = left;
			Bottom = bottom;
			Top = top;
			Right = right;
			Width = Math.Abs(right - left);
			Height = Math.Abs(top - bottom);
			Center = new Point((Left + Right) / 2.0, (Bottom + Top) / 2.0);
			Point bottomLeft = new Point(Left, Bottom);
			Point topRight = new Point(Right, Top);
			Lines = new[]
			{
				new Line(bottomLeft, 0.0),
				new Line(bottomLeft, double.PositiveInfinity),
				new Line(topRight, 0.0),
				new Line(topRight, double.PositiveInfinity)
			};
		}

		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="rectangle">Rectangle from Aspose.Pdf.</param>
		public Rectangle(Aspose.Pdf.Rectangle rectangle) : this(rectangle.LLX, rectangle.LLY, rectangle.URX, rectangle.URY)
		{
		}

		/// <summary>
		///   Width of this rectangle.
		/// </summary>
		public double Width { get; }

		/// <summary>
		///   Height of this rectangle.
		/// </summary>
		public double Height { get; }

		/// <summary>
		///   X coordinate of the left edge of this rectangle.
		/// </summary>
		public double Left { get; }

		/// <summary>
		///   Y coordinate of the bottom edge of this triangle.
		/// </summary>
		public double Bottom { get; }

		/// <summary>
		///   Center point of this rectangle.
		/// </summary>
		public Point Center { get; }

		/// <summary>
		///   X coordinate of the right edge of this rectangle.
		/// </summary>
		public double Right { get; }

		/// <summary>
		///   Y coordinate of the top edge of this rectangle.
		/// </summary>
		public double Top { get; }

		/// <summary>
		///   The four lines that define this rectangle.
		/// </summary>
		public Line[] Lines { get; }

		/// <summary>
		///   Reduces this rectangle by the given amount. If <paramref name="amount" /> is more
		///   than half of the width or height of this rectangle, then an <see cref="ArgumentException" /> is thrown.
		/// </summary>
		/// <param name="amount">Amount to deflate by.</param>
		/// <returns>Deflated rectangle.</returns>
		public Rectangle Deflate(double amount)
		{
			var amountTimesTwo = amount * 2.0;
			if (amountTimesTwo > Width)
				throw new ArgumentException($"Deflating the rectangle by {amount} would result in a negative width.");
			if (amountTimesTwo > Height)
				throw new ArgumentException($"Deflating the rectangle by {amount} would result in a negative height.");
			return new Rectangle(Left + amount, Bottom + amount, Right - amount, Top - amount);
		}

		/// <summary>
		///   Calculates the distance between opposite corners of this rectangle.
		/// </summary>
		/// <returns>The distance between opposite corners of this rectangle.</returns>
		public double DiagonalLength()
		{
			var widthSquared = Width * Width;
			var heightSquared = Height * Height;
			return Math.Sqrt(widthSquared + heightSquared);
		}

		/// <summary>
		///   Calculates the average side length.
		/// </summary>
		/// <returns>The average side length.</returns>
		public double AverageSideLength()
		{
			return Width + Height / 2.0;
		}

		/// <summary>
		///   Calculates the shorter side length.
		/// </summary>
		/// <returns>The shorter side length.</returns>
		public double ShorterSideLength()
		{
			return Math.Min(Width, Height);
		}

		/// <summary>
		///   Calculates the longer side length.
		/// </summary>
		/// <returns>The longer side length.</returns>
		public double LongerSideLength()
		{
			return Math.Max(Width, Height);
		}

		/// <summary>
		///   Calculates whether the given point is within (or on the edge of) this rectangle.
		/// </summary>
		/// <param name="point">Point to check.</param>
		/// <returns>True if the point is in (or on the edge of) this rectangle.</returns>
		public bool Contains(Point point)
		{
			return point.X > Left - GeometryUtil.Tolerance &&
			       point.X < Right + GeometryUtil.Tolerance &&
			       point.Y > Bottom - GeometryUtil.Tolerance &&
			       point.Y < Top + GeometryUtil.Tolerance;
		}

		public override bool Equals(object? obj)
		{
			if (this == obj) return true;
			if (obj is Rectangle otherRect)
				return Math.Abs(otherRect.Left - Left) < GeometryUtil.Tolerance &&
				       Math.Abs(otherRect.Right - Right) < GeometryUtil.Tolerance &&
				       Math.Abs(otherRect.Top - Top) < GeometryUtil.Tolerance &&
				       Math.Abs(otherRect.Bottom - Bottom) < GeometryUtil.Tolerance;
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Left, Right, Top, Bottom);
		}
	}
}