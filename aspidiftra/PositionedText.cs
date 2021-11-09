using Aspidiftra.Geometry;

namespace Aspidiftra
{
	/// <summary>
	///   A piece of positioned text.
	/// </summary>
	internal class PositionedText
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <param name="position">The position.</param>
		internal PositionedText(string text, Point position)
		{
			Position = position;
			Text = text;
		}

		/// <summary>
		///   Position for the text.
		/// </summary>
		internal Point Position { get; }

		/// <summary>
		///   The positioned text.
		/// </summary>
		internal string Text { get; }
	}
}