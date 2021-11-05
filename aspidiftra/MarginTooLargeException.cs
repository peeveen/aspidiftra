using System;

namespace Aspidiftra
{
	public class MarginTooLargeException : Exception
	{
		public enum PageSide
		{
			Width,
			Height
		}

		public MarginTooLargeException(double marginSize, PageSide side, double pageSideLength) : base(
			$"The requested margin size ({marginSize}) is greater than half of the page {side.ToString().ToLower()} ({pageSideLength}).")
		{
		}
	}
}