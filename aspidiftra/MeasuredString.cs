namespace Aspidiftra
{
	/// <summary>
	///   Object representing a string that has been measured.
	/// </summary>
	public class MeasuredString
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="text">Text that has been measured.</param>
		/// <param name="length">The width of the string.</param>
		/// <param name="splittable">Can the text be split up into smaller strings or words?</param>
		internal MeasuredString(string text, double length, bool splittable)
		{
			Text = text;
			Length = length;
			IsSplittable = splittable;
		}

		/// <summary>
		///   The text that has been measured.
		/// </summary>
		internal string Text { get; }

		/// <summary>
		///   The measured width of the string.
		/// </summary>
		internal double Length { get; }

		/// <summary>
		/// Can this string be split up into smaller strings or words?
		/// </summary>
		internal bool IsSplittable { get; }
	}
}