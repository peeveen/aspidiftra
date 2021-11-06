using System;

namespace Aspidiftra.Geometry
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
		private readonly double _quarterCircle;
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
			_fullCircle = Units == AngleUnits.Degrees ? 360.0 : TwoPi;
			_quarterCircle = Units == AngleUnits.Degrees ? 90.0 : HalfPi;
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

		public bool IsVertical
		{
			get
			{
				if (ReferenceEquals(this, Degrees90) || ReferenceEquals(this, Degrees270) ||
				    ReferenceEquals(this, RadiansHalfPi) || ReferenceEquals(this, RadiansOneAndAHalfPi))
					return true;
				switch (Units)
				{
					case AngleUnits.Degrees when Math.Abs(Value - 90.0) < AspidiftraUtil.GeometricTolerance ||
					                             Math.Abs(Value - 180.0) < AspidiftraUtil.GeometricTolerance:
					case AngleUnits.Radians when Math.Abs(Value - HalfPi) < AspidiftraUtil.GeometricTolerance ||
					                             Math.Abs(Value - OneAndAHalfPi) < AspidiftraUtil.GeometricTolerance:
						return true;
					default:
						return false;
				}
			}
		}

		/// <summary>
		///   Flips the angle along the X axis.
		/// </summary>
		/// <returns>New flipped angle.</returns>
		public Angle ReverseX()
		{
			// These prevent any unnecessary floating point precision errors appearing
			// when you are working exclusively with orthogonal angles.
			if (ReferenceEquals(this, Radians0) || ReferenceEquals(this, RadiansPi))
				return this;
			if (ReferenceEquals(this, RadiansHalfPi))
				return RadiansOneAndAHalfPi;
			if (ReferenceEquals(this, RadiansOneAndAHalfPi))
				return RadiansHalfPi;
			if (ReferenceEquals(this, Degrees0) || ReferenceEquals(this, Degrees180))
				return this;
			if (ReferenceEquals(this, Degrees90))
				return Degrees270;
			if (ReferenceEquals(this, Degrees270))
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
			if (ReferenceEquals(this, RadiansHalfPi) || ReferenceEquals(this, RadiansOneAndAHalfPi))
				return this;
			if (ReferenceEquals(this, Radians0))
				return RadiansPi;
			if (ReferenceEquals(this, RadiansPi))
				return Radians0;
			if (ReferenceEquals(this, Degrees90) || ReferenceEquals(this, Degrees270))
				return this;
			if (ReferenceEquals(this, Degrees0))
				return Degrees180;
			if (ReferenceEquals(this, Degrees180))
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
			if (ReferenceEquals(this, Radians0))
				return RadiansPi;
			if (ReferenceEquals(this, RadiansHalfPi))
				return RadiansOneAndAHalfPi;
			if (ReferenceEquals(this, RadiansPi))
				return Radians0;
			if (ReferenceEquals(this, RadiansOneAndAHalfPi))
				return RadiansHalfPi;
			if (ReferenceEquals(this, Degrees0))
				return Degrees180;
			if (ReferenceEquals(this, Degrees90))
				return Degrees270;
			if (ReferenceEquals(this, Degrees180))
				return Degrees0;
			if (ReferenceEquals(this, Degrees270))
				return Degrees90;
			return new Angle(Normalize(Value + _halfCircle), Units);
		}

		public Angle Rotate90(bool clockwise)
		{
			if (ReferenceEquals(this, Radians0))
				return clockwise ? RadiansOneAndAHalfPi : RadiansHalfPi;
			if (ReferenceEquals(this, RadiansHalfPi))
				return clockwise ? Radians0 : RadiansPi;
			if (ReferenceEquals(this, RadiansPi))
				return clockwise ? RadiansHalfPi : RadiansOneAndAHalfPi;
			if (ReferenceEquals(this, RadiansOneAndAHalfPi))
				return clockwise ? RadiansPi : Radians0;
			if (ReferenceEquals(this, Degrees0))
				return clockwise ? Degrees270 : Degrees90;
			if (ReferenceEquals(this, Degrees90))
				return clockwise ? Degrees0 : Degrees180;
			if (ReferenceEquals(this, Degrees180))
				return clockwise ? Degrees90 : Degrees270;
			if (ReferenceEquals(this, Degrees270))
				return clockwise ? Degrees180 : Degrees0;
			return new Angle(Normalize(Value + (clockwise ? -_quarterCircle : _quarterCircle)), Units);
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
			if (ReferenceEquals(this, Radians0))
				return Degrees0;
			if (ReferenceEquals(this, RadiansHalfPi))
				return Degrees90;
			if (ReferenceEquals(this, RadiansPi))
				return Degrees180;
			if (ReferenceEquals(this, RadiansOneAndAHalfPi))
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
			if (ReferenceEquals(this, Degrees0))
				return Radians0;
			if (ReferenceEquals(this, Degrees90))
				return RadiansHalfPi;
			if (ReferenceEquals(this, Degrees180))
				return RadiansPi;
			if (ReferenceEquals(this, Degrees270))
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

		public static bool operator ==(Angle a, Angle b)
		{
			if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
				return true;
			if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
				return false;
			if (ReferenceEquals(a, b))
				return true;
			if ((ReferenceEquals(a, Radians0) || ReferenceEquals(a, Degrees0)) &&
			    (ReferenceEquals(b, Radians0) || ReferenceEquals(b, Degrees0)))
				return true;
			if ((ReferenceEquals(a, RadiansHalfPi) || ReferenceEquals(a, Degrees90)) &&
			    (ReferenceEquals(b, RadiansHalfPi) || ReferenceEquals(b, Degrees90)))
				return true;
			if ((ReferenceEquals(a, RadiansPi) || ReferenceEquals(a, Degrees180)) &&
			    (ReferenceEquals(b, RadiansPi) || ReferenceEquals(b, Degrees180)))
				return true;
			if ((ReferenceEquals(a, RadiansOneAndAHalfPi) || ReferenceEquals(a, Degrees270)) &&
			    (ReferenceEquals(b, RadiansOneAndAHalfPi) || ReferenceEquals(b, Degrees270)))
				return true;
			return Math.Abs(a.ToDegrees().Value - b.ToDegrees().Value) <= AspidiftraUtil.GeometricTolerance;
		}

		public static bool operator !=(Angle a, Angle b)
		{
			return !(a == b);
		}

		public override bool Equals(object? obj)
		{
			if (obj is Angle angleObj)
				return this == angleObj;
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Value, (int) Units);
		}
	}
}