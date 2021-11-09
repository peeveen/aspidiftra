namespace Aspidiftra.Geometry
{
	/// <summary>
	///   Object representing a coordinate offset.
	/// </summary>
	public class Offset
	{
		/// <summary>
		///   "Zero offset" value.
		/// </summary>
		public static Offset None = new Offset(0.0, 0.0);

		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="x">X offset amount.</param>
		/// <param name="y">Y offset amount.</param>
		public Offset(double x, double y)
		{
			GeometryUtil.ValidateArgument(x, nameof(x));
			GeometryUtil.ValidateArgument(y, nameof(y));
			X = x;
			Y = y;
		}

		/// <summary>
		///   X offset amount.
		/// </summary>
		public double X { get; }

		/// <summary>
		///   Y offset amount.
		/// </summary>
		public double Y { get; }

		/// <summary>
		/// Scales the offset, dividing it by the given value.
		/// </summary>
		/// <param name="offset">Offset to scale.</param>
		/// <param name="divider">Amount to divide it by.</param>
		/// <returns>Scaled offset.</returns>
		public static Offset operator /(Offset offset, double divider)
		{
			GeometryUtil.ValidateArgument(divider, nameof(divider));
			return new Offset(offset.X / divider, offset.Y / divider);
		}

		/// <summary>
		/// Scales the offset, multiplying it by the given value.
		/// </summary>
		/// <param name="offset">Offset to scale.</param>
		/// <param name="multiplier">Amount to multiply it by.</param>
		/// <returns>Scaled offset.</returns>
		public static Offset operator *(Offset offset, double multiplier)
		{
			GeometryUtil.ValidateArgument(multiplier, nameof(multiplier));
			return new Offset(offset.X * multiplier, offset.Y * multiplier);
		}
	}
}