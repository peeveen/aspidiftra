using System;

namespace Aspidiftra
{
	/// <summary>
	///   Exception that is thrown when the requested margin size is greater than half of the
	///   page width or height.
	/// </summary>
	public class MarginTooLargeException : Exception
	{
		internal MarginTooLargeException(double marginSize, Exception innerException) : base(
			$"Could not reduce page size by the requested margin size of ({marginSize}).", innerException)
		{
		}
	}
}