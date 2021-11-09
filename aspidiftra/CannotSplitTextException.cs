using System;

namespace Aspidiftra
{
	/// <summary>
	///   Exception thrown when it is impossible to split a string in order to make it fit a text slot.
	///   Text will only be split on whitespace, so if there is no suitable whitespace to split the text
	///   on, or if perhaps the slot is tiny, then this exception will occur.
	/// </summary>
	public class CannotSplitTextException : Exception
	{
		internal CannotSplitTextException(string text) : base($"Could not split the text '{text}' across multiple lines.")
		{
		}
	}
}