using System;

namespace Aspidiftra.Geometry
{
	/// <summary>
	///   Object representing a point.
	/// </summary>
	public class Point
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="x">X coordinate of point.</param>
		/// <param name="y">Y coordinate of point.</param>
		public Point(double x, double y)
		{
			GeometryUtil.ValidateArgument(x, nameof(x));
			GeometryUtil.ValidateArgument(y, nameof(y));
			X = x;
			Y = y;
		}

		/// <summary>
		///   X coordinate.
		/// </summary>
		public double X { get; }

		/// <summary>
		///   Y coordinate.
		/// </summary>
		public double Y { get; }

		/// <summary>
		///   Shifts the point by the given offset, adding the offset
		///   values to the coordinate values.
		/// </summary>
		/// <param name="start">Point to shift.</param>
		/// <param name="offset">Offset to shift it by.</param>
		/// <returns>
		///   A new point created by adding the offset values to
		///   the original coordinates.
		/// </returns>
		public static Point operator +(Point start, Offset offset)
		{
			return new Point(start.X + offset.X, start.Y + offset.Y);
		}

		/// <summary>
		///   Shifts the point by the given offset, subtracting the offset
		///   values from the coordinate values.
		/// </summary>
		/// <param name="start">Point to shift.</param>
		/// <param name="offset">Offset to shift it by.</param>
		/// <returns>
		///   A new point created by subtracting the offset values from
		///   the original coordinates.
		/// </returns>
		public static Point operator -(Point start, Offset offset)
		{
			return new Point(start.X - offset.X, start.Y - offset.Y);
		}

		/// <summary>
		///   Calculates the offset between two points.
		/// </summary>
		/// <param name="end">Second point.</param>
		/// <param name="start">First point.</param>
		/// <returns>Offset from <paramref name="start" /> to <paramref name="end" /></returns>
		public static Offset operator -(Point end, Point start)
		{
			return new Offset(end.X - start.X, end.Y - start.Y);
		}

		/// <summary>
		/// Calculates the distance from this point to the given point.
		/// </summary>
		/// <param name="otherPoint">Other point.</param>
		/// <returns>Distance from this point to <paramref name="otherPoint"/></returns>
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
				return Math.Abs(otherPoint.X - X) < GeometryUtil.Tolerance &&
				       Math.Abs(otherPoint.Y - Y) < GeometryUtil.Tolerance;
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(X, Y);
		}
	}
}