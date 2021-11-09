using System.Collections.Generic;

namespace Aspidiftra
{
	public interface ITextSlotProvider
	{
		/// <summary>
		///   Returns the text slots that should be used when positioning the
		///   given number of strings. If the provider does not contain enough
		///   slots, it should throw an <see cref="InsufficientSlotsException" /> exception."/>
		/// </summary>
		/// <param name="amount">Number of strings that are being positioned.</param>
		/// <returns>Text slots to put the strings in.</returns>
		IEnumerable<TextSlot> GetTextSlots(int amount);
	}
}