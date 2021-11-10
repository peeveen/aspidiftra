using System;
using System.Collections.Generic;

namespace Aspidiftra
{
	/// <summary>
	///   Exception that is thrown when the fitting algorithm reckons the text should be wrapped.
	/// </summary>
	internal class TextNeedsWrappedException : Exception
	{
		internal TextNeedsWrappedException(IEnumerable<string> wrappedStrings)
		{
			WrappedStrings = wrappedStrings;
		}

		public IEnumerable<string> WrappedStrings { get; }
	}
}