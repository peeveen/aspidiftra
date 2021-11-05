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

		public static Point operator +(Point start, (double xOffset, double yOffset) offset)
		{
			return new Point(start.X + offset.xOffset, start.Y + offset.yOffset);
		}

		public override bool Equals(object obj)
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