using System;

namespace Aspidiftra.Geometry
{
	internal class GeometryUtil
	{
		/// <summary>
		///   Acceptable amount to be "off by" for geometric floating point operations.
		/// </summary>
		internal const double Tolerance = 0.0000001;

		/// <summary>
		///   Validates a given argument value. Throws an exception if the
		///   value is not a standard numeric value (i.e. is infinite, or
		///   not-a-number).
		/// </summary>
		/// <param name="value">Value of the argument.</param>
		/// <param name="name">Name of the argument.</param>
		internal static void ValidateArgument(double value, string name)
		{
			if (double.IsInfinity(value) || double.IsNaN(value))
				throw new ArgumentException("Value must be a normal finite number.", name);
		}
	}
}