using System;

namespace Aspidiftra.Geometry
{
	// Aspidiftra only cares about logical lines that extend infinitely.
	// So a point and an angle is all that we need to define any line.
	public class Line
	{
		public Line(Point point, Angle angle)
		{
			Point = point;
			// A vertical line has infinite gradient. But lucky for us,
			// the radian values for 90 or 270 degrees are 0.5*PI and
			// 1.5*PI respectively, and neither of those values can be
			// represented in floating-point precisely enough for the
			// Math.Tan operation to return actual infinity.
			// Instead, it will "only" return a very, very large number,
			// which we can happily perform arithmetic with.
			Gradient = Math.Tan(angle.ToRadians().Value);
			Constant = Point.Y - Gradient * Point.X;
		}

		public Point Point { get; }
		public double Gradient { get; }
		public double Constant { get; }

		public Point? GetIntersectionPoint(Line line)
		{
			var gradientDiff = Gradient - line.Gradient;
			if (Math.Abs(gradientDiff) < AspidiftraUtil.GeometricTolerance)
				return null;
			// So the lines will cross at the X coordinate where:
			// thisGradient*x + thisConstant = otherGradient*x + otherConstant
			// which becomes:
			// (thisGradient - otherGradient)*x = otherConstant - thisConstant
			var constantDiff = line.Constant - Constant;
			// So now:
			// x = constantDiff/gradientDiff
			var x = constantDiff / gradientDiff;
			// Solve for Y in either line equation.
			var y = Gradient * x + Constant;
			return new Point(x, y);
		}
	}
}