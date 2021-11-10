using System;

namespace Aspidiftra
{
	/// <summary>
	///   Exception that is thrown when it impossible to fit the text on the page, and the
	///   current font size cannot be reduced any further.
	/// </summary>
	public class CannotReduceFontSizeException : Exception
	{
		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="minFontSize">Minimum font size that has been reached.</param>
		/// <param name="innerException">The exception that prompted the font size reduction attempt.</param>
		internal CannotReduceFontSizeException(float minFontSize, Exception? innerException = null) : base(
			$"Even at the minimum font size of {minFontSize}, the watermark text could not fit on the page.", innerException)
		{
		}
	}
}