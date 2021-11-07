using System;

namespace Aspidiftra
{
	public class MarginTooLargeException : Exception
	{
		public MarginTooLargeException(double marginSize, Exception innerException) : base(
			$"Could not reduce page size by the requested margin size of ({marginSize}).", innerException)
		{
		}
	}
}