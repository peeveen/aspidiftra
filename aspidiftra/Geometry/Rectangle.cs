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
			BottomLeft = new Point(Left, Bottom);
			TopRight = new Point(Right, Top);
			Width = Math.Abs(right - left);
			Height = Math.Abs(top - bottom);
			Center = new Point((Left + Right) / 2.0, (Bottom + Top) / 2.0);
			Lines = new[]
			{
				new Line(BottomLeft, 0.0),
				new Line(BottomLeft, double.PositiveInfinity),
				new Line(TopRight, 0.0),
				new Line(TopRight, double.PositiveInfinity)
			};
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
		public Point BottomLeft { get; }
		public Point TopRight { get; }
		public Point Center { get; }
		public double Right { get; }
		public double Top { get; }
		public Line[] Lines { get; }

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

		public bool Contains(Point point)
		{
			return point.X > Left - AspidiftraUtil.GeometricTolerance &&
			       point.X < Right + AspidiftraUtil.GeometricTolerance &&
			       point.Y > Bottom - AspidiftraUtil.GeometricTolerance &&
			       point.Y < Top + AspidiftraUtil.GeometricTolerance;
		}

		public override bool Equals(object? obj)
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