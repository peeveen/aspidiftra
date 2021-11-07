namespace Aspidiftra.Geometry
{
	public class Offset
	{
		public static Offset None = new Offset(0.0, 0.0);

		public Offset(double x, double y)
		{
			X = x;
			Y = y;
		}

		public double X { get; }
		public double Y { get; }

		public static Offset operator /(Offset offset, double divider)
		{
			return new Offset(offset.X / divider, offset.Y / divider);
		}

		public static Offset operator *(Offset offset, double multiplier)
		{
			return new Offset(offset.X * multiplier, offset.Y * multiplier);
		}
	}
}