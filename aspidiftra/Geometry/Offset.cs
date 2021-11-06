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
	}
}