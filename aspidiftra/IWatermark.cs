using System.Collections.Immutable;

namespace Aspidiftra
{
	public interface IWatermark
	{
		float Opacity { get; }
		IImmutableSet<int> GetApplicablePageNumbers(IImmutableSet<int> availablePageNumbers);
		WatermarkElementCollection GetWatermarkElements(PageSize pageMediaBox);
	}
}