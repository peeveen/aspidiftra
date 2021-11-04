using System.Drawing;

namespace Aspidiftra
{
	public class Appearance
	{
		public Appearance(Color color, float opacity, Font font, bool isBackground = false)
		{
			Color = color;
			Opacity = opacity;
			Font = font;
			IsBackground = isBackground;
		}

		public Color Color { get; }
		public bool IsBackground { get; }
		public Font Font { get; }
		public float Opacity { get; }
	}
}