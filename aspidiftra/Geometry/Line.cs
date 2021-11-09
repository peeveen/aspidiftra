using System;

namespace Aspidiftra.Geometry
{
	/// <summary>
	///   Object defining a line.
	///   Aspidiftra only cares about logical lines that extend infinitely.
	///   So a point and an angle or gradient is all that we need to define
	///   any line.
	/// </summary>
	public class Line
	{
		private readonly double _constant;

		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="point">A point that the line passes through.</param>
		/// <param name="angle">Angle of the line. Direction is not important.</param>
		public Line(Point point, Angle angle) :
			this(point, angle.IsVertical ? double.PositiveInfinity : Math.Tan(angle.ToRadians().Value))
		{
		}

		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="point">A point that the line passes through.</param>
		/// <param name="gradient">Gradient of the line.</param>
		public Line(Point point, double gradient)
		{
			Point = point;
			Gradient = gradient;
			_constant = IsVertical ? 0.0 : Point.Y - Gradient * Point.X;
		}

		/// <summary>
		///   A point that this line passes through.
		/// </summary>
		public Point Point { get; }

		/// <summary>
		///   The gradient of this line.
		/// </summary>
		public double Gradient { get; }

		/// <summary>
		///   True if this line is vertical.
		/// </summary>
		public bool IsVertical => double.IsPositiveInfinity(Gradient);

		/// <summary>
		///   Solves the line equation for X, returning the point on the
		///   line. If the line is vertical, the result will be null.
		/// </summary>
		/// <param name="x">X coordinate of a point on the line.</param>
		/// <returns>Point on the line with the given X coordinate.</returns>
		public Point? GetPointAtX(double x)
		{
			return IsVertical ? null : new Point(x, Gradient * x + _constant);
		}

		/// <summary>
		///   Gets the point of intersection between this line and the given line.
		/// </summary>
		/// <param name="line">Other line.</param>
		/// <returns>
		///   Point of intersection between the two lines, or null if
		///   there is no intersection.
		/// </returns>
		public Point? GetIntersectionPoint(Line line)
		{
			// Floating-point arithmetic doesn't play nice with infinity,
			// so get the special infinite-gradient cases dealt with first.
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

			// If the lines are parallel, then no result.
			if (Math.Abs(gradientDiff) < GeometryUtil.Tolerance)
				return null;

			// So the lines will cross at the X coordinate where:
			// thisGradient*x + thisConstant = otherGradient*x + otherConstant
			// which becomes:
			// (thisGradient - otherGradient)*x = otherConstant - thisConstant
			var constantDiff = line._constant - _constant;
			// So now:
			// x = constantDiff/gradientDiff
			return GetPointAtX(constantDiff / gradientDiff);
		}
	}
}