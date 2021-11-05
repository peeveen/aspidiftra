using System;

namespace Aspidiftra
{
	public class CannotReduceFontSizeException : Exception
	{
		public CannotReduceFontSizeException(float minFontSize) : base(
			$"Even at the minimum font size of {minFontSize}, the watermark text could not fit on the page.")
		{
		}
	}
}