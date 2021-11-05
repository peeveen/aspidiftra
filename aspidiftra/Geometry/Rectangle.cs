using System;

namespace Aspidiftra.Geometry
{
	public class Rectangle
	{
		public Rectangle(double left, double bottom, double right, double top)
		{
			Left = left;
			Bottom = bottom;
			Top = top;
			Right = right;
			Width = Math.Abs(right - left);
			Height = Math.Abs(top - bottom);
		}

		public Rectangle(Rectangle rectangle) : this(rectangle.Left, rectangle.Bottom, rectangle.Right, rectangle.Top)
		{
		}

		public Rectangle(Aspose.Pdf.Rectangle rectangle) : this(rectangle.LLX, rectangle.LLY, rectangle.URX, rectangle.URY)
		{
		}

		public double Width { get; }
		public double Height { get; }
		public double Left { get; }
		public double Bottom { get; }
		public double Right { get; }
		public double Top { get; }

		public Point Centre => new Point(Left + Width / 2.0, Bottom + Height / 2.0);

		public Rectangle Deflate(double amount)
		{
			return new Rectangle(Left + amount, Bottom + amount, Right - amount, Top - amount);
		}

		public Rectangle Inflate(double amount)
		{
			return Deflate(-amount);
		}

		public double DiagonalLength()
		{
			var widthSquared = Width * Width;
			var heightSquared = Height * Height;
			return Math.Sqrt(widthSquared + heightSquared);
		}

		public double AverageSideLength()
		{
			return Width + Height / 2.0;
		}

		public double ShortestSideLength()
		{
			return Math.Min(Width, Height);
		}

		public double LongestSideLength()
		{
			return Math.Max(Width, Height);
		}

		/// <summary>
		///   Calculates the angle between the X axis and the line that
		///   runs from the bottom-left corner to the top-right corner.
		/// </summary>
		/// <returns>
		///   Angle between the X axis and the line that runs from the
		///   bottom-left corner to the top-right corner.
		/// </returns>
		public Angle DiagonalAngle()
		{
			return new Angle(Math.Atan(Height / Width), AngleUnits.Radians);
		}

		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			if (obj is Rectangle otherRect)
				return Math.Abs(otherRect.Left - Left) < AspidiftraUtil.GeometricTolerance &&
				       Math.Abs(otherRect.Right - Right) < AspidiftraUtil.GeometricTolerance &&
				       Math.Abs(otherRect.Top - Top) < AspidiftraUtil.GeometricTolerance &&
				       Math.Abs(otherRect.Bottom - Bottom) < AspidiftraUtil.GeometricTolerance;
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Left, Right, Top, Bottom);
		}
	}
}