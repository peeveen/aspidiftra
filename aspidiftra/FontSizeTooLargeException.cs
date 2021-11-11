using System;

namespace Aspidiftra
{
	/// <summary>
	/// Exception that is thrown when the fitting algorithm reckons that the font size should
	/// be reduced.
	/// </summary>
	internal class FontSizeTooLargeException : Exception
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="reducedSize">The suggested reduced font size.</param>
		internal FontSizeTooLargeException(float reducedSize)
		{
			ReducedFontSize = reducedSize;
		}

		/// <summary>
		/// The suggested reduced font size.
		/// </summary>
		public float ReducedFontSize { get; }
	}
}