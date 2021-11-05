using Aspidiftra.Geometry;

namespace Aspidiftra
{
	public class TextSlot
	{
		public TextSlot(Point textOrigin, double width, double height, Angle angle)
		{
			TextOrigin = textOrigin;
			Width = width;
			Height = height;
			Angle = angle;
			EffectiveTextOrigin = CalculateEffectiveTextOrigin();
		}

		public Point TextOrigin { get; }
		public Point EffectiveTextOrigin { get; }
		public double Width { get; }
		public double Height { get; }
		public Angle Angle { get; }

		private Point CalculateEffectiveTextOrigin()
		{
			var xOffset = 0.0;
			var yOffset = 0.0;
			if (Angle <= Angle.Degrees90)
			{
				xOffset = Height * Angle.Sin;
			}
			else if (Angle <= Angle.Degrees180)
			{
				xOffset = Width * -Angle.Cos + Height * Angle.Sin;
				yOffset = Height * -Angle.Cos;
			}
			else if (Angle <= Angle.Degrees270)
			{
				xOffset = Width * -Angle.Cos;
				yOffset = Width * -Angle.Sin + Height * -Angle.Cos;
			}
			else
			{
				yOffset = Width * Angle.Cos;
			}

			return TextOrigin + (-xOffset, -yOffset);
		}
	}
}