using System.Collections.Immutable;

namespace Aspidiftra
{
	/// <summary>
	///   Interface for a watermark.
	/// </summary>
	public interface IWatermark
	{
		/// <summary>
		///   How opaque is the watermark?
		/// </summary>
		float Opacity { get; }

		/// <summary>
		///   What page numbers should the watermark be on?
		/// </summary>
		/// <param name="availablePageNumbers">Available page numbers.</param>
		/// <returns>The subset of the given page numbers to apply the watermark to.</returns>
		IImmutableSet<int> GetApplicablePageNumbers(IImmutableSet<int> availablePageNumbers);

		/// <summary>
		///   Calculates the watermark elements that should appear on a page with the given page size.
		/// </summary>
		/// <param name="pageSize">Page size.</param>
		/// <returns>Collection of watermark elements.</returns>
		TextWatermarkElementCollection GetWatermarkElements(PageSize pageSize);
	}
}