using System;

namespace Aspidiftra
{
	/// <summary>
	///   Exception that is thrown when a request is made to perform an operation (such
	///   as text position, slot calculation, whatever) where there is not enough space
	///   on the page.
	/// </summary>
	public class InsufficientSpaceException : InvalidOperationException
	{
		public InsufficientSpaceException(string message) : base(message)
		{
		}
	}
}