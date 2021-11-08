using System;

namespace Aspidiftra.Geometry
{
	internal class GeometryUtil
	{
		internal static void ValidateGeometricValue(double value, string name)
		{
			if (!double.IsFinite(value))
				throw new ArgumentException("Value must be a normal finite number.", name);
		}
	}
}