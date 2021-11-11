namespace Aspidiftra
{
	public interface ITextSlotCalculator
	{
		/// <summary>
		///   For the given font size, calculate as many text slots as possible.
		///   This method will never be called multiple times with the
		///   same <paramref name="fontSize" /> argument, so there is no need for any
		///   internal caching.
		///   If there is a need to calculate slots that cannot fit on the page, then only
		///   return those that are at least partially on the page (flagged with IsOverflow).
		///   Never return slots that are 100% off-page.
		/// </summary>
		/// <param name="fontSize">Font size to calculate the slots for.</param>
		/// <returns>An object describing all the text slots that can fit on the page.</returns>
		ITextSlotProvider CalculateSlots(float fontSize);
	}
}