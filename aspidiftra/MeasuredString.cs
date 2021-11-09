namespace Aspidiftra
{
	/// <summary>
	///   Object representing a string that has been measured.
	/// </summary>
	internal class MeasuredString
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="text">Text that has been measured.</param>
		/// <param name="length">The width of the string.</param>
		internal MeasuredString(string text, double length)
		{
			Text = text;
			Length = length;
		}

		/// <summary>
		///   The text that has been measured.
		/// </summary>
		internal string Text { get; }

		/// <summary>
		///   The measured width of the string.
		/// </summary>
		internal double Length { get; }
	}
}