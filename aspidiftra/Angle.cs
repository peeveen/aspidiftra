using System;

namespace Aspidiftra
{
	/// <summary>
	///   Angle units.
	/// </summary>
	public enum AngleUnits
	{
		/// <summary>
		///   Degrees (360 in a full circle).
		/// </summary>
		Degrees,

		/// <summary>
		///   Radians (2π in a full circle).
		/// </summary>
		Radians
	}

	/// <summary>
	///   Object representing an angle.
	/// </summary>
	public sealed class Angle
	{
		private const double HalfPi = Math.PI / 2.0;
		private const double OneAndAHalfPi = Math.PI + HalfPi;
		private const double TwoPi = Math.PI * 2.0;

		public static readonly Angle Degrees0 = new Angle(0.0, AngleUnits.Degrees);
		public static readonly Angle Degrees90 = new Angle(90.0, AngleUnits.Degrees);
		public static readonly Angle Degrees180 = new Angle(180.0, AngleUnits.Degrees);
		public static readonly Angle Degrees270 = new Angle(270.0, AngleUnits.Degrees);
		public static readonly Angle Radians0 = new Angle(0.0, AngleUnits.Radians);
		public static readonly Angle RadiansHalfPi = new Angle(HalfPi, AngleUnits.Radians);
		public static readonly Angle RadiansPi = new Angle(Math.PI, AngleUnits.Radians);
		public static readonly Angle RadiansOneAndAHalfPi = new Angle(OneAndAHalfPi, AngleUnits.Radians);

		private readonly double _fullCircle;
		private readonly double _halfCircle;
		private double? _cachedCosValue;

		private double? _cachedSinValue;

		/// <summary>
		///   Creates an angle.
		/// </summary>
		/// <param name="value">Value of the angle.</param>
		/// <param name="angleUnits">Units of the angle (degrees or radians). See <see cref="AngleUnits" /></param>
		public Angle(double value, AngleUnits angleUnits)
		{
			Units = angleUnits;
			// Create a couple of handy constants, depending on the units.
			_halfCircle = Units == AngleUnits.Degrees ? 180.0 : Math.PI;
			_fullCircle = Units == AngleUnits.Degrees ? 360 : TwoPi;
			Value = Normalize(value);
		}

		/// <summary>
		///   The value of the angle. Will be in degrees or radians,
		///   according to <see cref="Units" />
		/// </summary>
		public double Value { get; }

		/// <summary>
		///   Units of the angle.
		/// </summary>
		public AngleUnits Units { get; }

		public double Sin => _cachedSinValue ??= Math.Sin(ToRadians().Value);

		public double Cos => _cachedCosValue ??= Math.Cos(ToRadians().Value);

		/// <summary>
		///   Flips the angle along the X axis.
		/// </summary>
		/// <returns>New flipped angle.</returns>
		public Angle ReverseX()
		{
			// These prevent any unnecessary floating point precision errors appearing
			// when you are working exclusively with orthogonal angles.
			if (this == Radians0 || this == RadiansPi)
				return this;
			if (this == RadiansHalfPi)
				return RadiansOneAndAHalfPi;
			if (this == RadiansOneAndAHalfPi)
				return RadiansHalfPi;
			if (this == Degrees0 || this == Degrees180)
				return this;
			if (this == Degrees90)
				return Degrees270;
			if (this == Degrees270)
				return Degrees90;
			return new Angle(Normalize(-Value), Units);
		}

		/// <summary>
		///   Flips the angle along the Y axis.
		/// </summary>
		/// <returns>New flipped angle.</returns>
		public Angle ReverseY()
		{
			// These prevent any unnecessary floating point precision errors appearing
			// when you are working exclusively with orthogonal angles.
			if (this == RadiansHalfPi || this == RadiansOneAndAHalfPi)
				return this;
			if (this == Radians0)
				return RadiansPi;
			if (this == RadiansPi)
				return Radians0;
			if (this == Degrees90 || this == Degrees270)
				return this;
			if (this == Degrees0)
				return Degrees180;
			if (this == Degrees180)
				return Degrees0;
			return new Angle(Normalize(_halfCircle - Value), Units);
		}

		/// <summary>
		///   Flips the angle along both the X and Y axes.
		/// </summary>
		/// <returns>New flipped angle.</returns>
		public Angle Reverse()
		{
			// These prevent any unnecessary floating point precision errors appearing
			// when you are working exclusively with orthogonal angles.
			if (this == Radians0)
				return RadiansPi;
			if (this == RadiansHalfPi)
				return RadiansOneAndAHalfPi;
			if (this == RadiansPi)
				return Radians0;
			if (this == RadiansOneAndAHalfPi)
				return RadiansHalfPi;
			if (this == Degrees0)
				return Degrees180;
			if (this == Degrees90)
				return Degrees270;
			if (this == Degrees180)
				return Degrees0;
			if (this == Degrees270)
				return Degrees90;
			return new Angle(Normalize(Value + _halfCircle), Units);
		}

		/// <summary>
		///   Converts this angle to the same angle, but with degrees as units.
		/// </summary>
		/// <returns>Converted angle.</returns>
		public Angle ToDegrees()
		{
			// These prevent any unnecessary floating point precision errors appearing
			// when you are working exclusively with orthogonal angles.
			if (Units == AngleUnits.Degrees)
				return this;
			if (this == Radians0)
				return Degrees0;
			if (this == RadiansHalfPi)
				return Degrees90;
			if (this == RadiansPi)
				return Degrees180;
			if (this == RadiansOneAndAHalfPi)
				return Degrees270;
			return new Angle(Value / Math.PI * 180.0, AngleUnits.Degrees);
		}

		/// <summary>
		///   Converts this angle to the same angle, but with radians as units.
		/// </summary>
		/// <returns>Converted angle.</returns>
		public Angle ToRadians()
		{
			// These prevent any unnecessary floating point precision errors appearing
			// when you are working exclusively with orthogonal angles.
			if (Units == AngleUnits.Radians)
				return this;
			if (this == Degrees0)
				return Radians0;
			if (this == Degrees90)
				return RadiansHalfPi;
			if (this == Degrees180)
				return RadiansPi;
			if (this == Degrees270)
				return RadiansOneAndAHalfPi;
			return Units == AngleUnits.Radians ? this : new Angle(Value / 180.0 * Math.PI, AngleUnits.Radians);
		}

		/// <summary>
		///   Ensures that the angle is not less than zero, or greater than a full circle.
		/// </summary>
		/// <param name="angle">Value to normalize.</param>
		/// <returns>Normalized value.</returns>
		private double Normalize(double angle)
		{
			while (angle < 0.0)
				angle += _fullCircle;
			while (angle >= _fullCircle)
				angle -= _fullCircle;
			return angle;
		}

		public static bool operator <(Angle a, Angle b)
		{
			return a.ToDegrees().Value < b.ToDegrees().Value;
		}

		public static bool operator >(Angle a, Angle b)
		{
			return a.ToDegrees().Value > b.ToDegrees().Value;
		}

		public static bool operator <=(Angle a, Angle b)
		{
			if (a == b)
				return true;
			return a.ToDegrees().Value <= b.ToDegrees().Value;
		}

		public static bool operator >=(Angle a, Angle b)
		{
			if (a == b)
				return true;
			return a.ToDegrees().Value >= b.ToDegrees().Value;
		}
	}
}