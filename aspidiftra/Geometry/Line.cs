using System;

namespace Aspidiftra.Geometry
{
	// Aspidiftra only cares about logical lines that extend infinitely.
	// So a point and an angle is all that we need to define any line.
	public class Line
	{
		public Line(Point point, Angle angle) :
			this(point, angle.IsVertical ? double.PositiveInfinity : Math.Tan(angle.ToRadians().Value))
		{
		}

		public Line(Point point, double gradient)
		{
			Point = point;
			Gradient = gradient;
			Constant = IsVertical ? 0.0 : Point.Y - Gradient * Point.X;
		}

		public Point Point { get; }
		public double Gradient { get; }
		public double Constant { get; }
		public bool IsVertical => double.IsPositiveInfinity(Gradient);

		public Point? GetPointAtX(double x)
		{
			return IsVertical ? null : new Point(x, Gradient * x + Constant);
		}

		public Point? GetIntersectionPoint(Line line)
		{
			// Floating-point arithmetic doesn't play nice with infinity,
			// so get the special cases dealt with here.
			var thisIsVertical = IsVertical;
			var otherLineIsVertical = line.IsVertical;
			switch (thisIsVertical)
			{
				// Lines are parallel, forget it.
				// Even if they overlap, there would be infinite intersection points.
				case true when otherLineIsVertical:
					return null;
				// So this line is vertical. Solve other line's equation for
				// this line's X value.
				case true:
					return line.GetPointAtX(Point.X);
			}

			if (otherLineIsVertical)
				// So the other line is vertical. Solve this line's equation for
				// the other line's X value.
				return GetPointAtX(line.Point.X);

			// So now we know that neither line is vertical. We can do the
			// usual solution.
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
			return GetPointAtX(constantDiff / gradientDiff);
		}
	}
}