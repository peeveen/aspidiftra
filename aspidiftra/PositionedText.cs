using Aspidiftra.Geometry;

namespace Aspidiftra
{
	internal class PositionedText
	{
		internal PositionedText(string text, Point position)
		{
			Position = position;
			Text = text;
		}

		internal Point Position { get; }
		internal string Text { get; }
	}
}