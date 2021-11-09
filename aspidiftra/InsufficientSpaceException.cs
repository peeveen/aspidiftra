using System;

namespace Aspidiftra
{
	/// <summary>
	///   Exception that is thrown when a request is made to perform an operation (such
	///   as text position, slot calculation, whatever) where there is not enough space
	///   on the page.
	/// </summary>
	public class InsufficientSpaceException : Exception
	{
		private const string ErrorText = "There is not enough room on the page for the watermark text.";

		internal InsufficientSpaceException(Exception innerException) : base(ErrorText, innerException)
		{
		}

		internal InsufficientSpaceException() : base(ErrorText)
		{
		}
	}
}