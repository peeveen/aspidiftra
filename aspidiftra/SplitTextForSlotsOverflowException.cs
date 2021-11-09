using System;
using System.Collections.Immutable;

namespace Aspidiftra
{
	/// <summary>
	///   Exception that is thrown when a request to split text across multiple text slots
	///   results in an overflow (too much text to fit into the slots).
	/// </summary>
	internal class SplitTextForSlotsOverflowException : Exception
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="splitStrings">The results of the split operation.</param>
		/// <param name="overflowTokens">The string tokens that make up the overflow text.</param>
		internal SplitTextForSlotsOverflowException(IImmutableList<string> splitStrings,
			StringTokenCollection overflowTokens)
		{
			SplitStrings = splitStrings;
			OverflowTokens = overflowTokens;
		}

		internal IImmutableList<string> SplitStrings { get; }
		internal StringTokenCollection OverflowTokens { get; }
	}
}