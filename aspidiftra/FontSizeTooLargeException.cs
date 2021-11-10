using System;

namespace Aspidiftra
{
	/// <summary>
	/// Exception that is thrown when the fitting algorithm reckons that the font size should
	/// be reduced.
	/// </summary>
	internal class FontSizeTooLargeException : Exception
	{
		internal FontSizeTooLargeException(float reducedSize)
		{
			ReducedFontSize = reducedSize;
		}

		public float ReducedFontSize { get; }
	}
}