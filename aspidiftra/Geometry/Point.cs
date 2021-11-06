using System;

namespace Aspidiftra.Geometry
{
	public class Point
	{
		public Point(double x, double y)
		{
			X = x;
			Y = y;
		}

		public double X { get; }
		public double Y { get; }

		public static Point operator +(Point start, Offset offset)
		{
			return new Point(start.X + offset.X, start.Y + offset.Y);
		}

		public static Point operator -(Point start, Offset offset)
		{
			return new Point(start.X - offset.X, start.Y - offset.Y);
		}

		public double GetDistanceFrom(Point otherPoint)
		{
			var xDiff = X - otherPoint.X;
			var yDiff = Y - otherPoint.Y;
			return Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
		}

		public override bool Equals(object? obj)
		{
			if (this == obj) return true;
			if (obj is Point otherPoint)
				return Math.Abs(otherPoint.X - X) < AspidiftraUtil.GeometricTolerance &&
				       Math.Abs(otherPoint.Y - Y) < AspidiftraUtil.GeometricTolerance;
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(X, Y);
		}
	}
}