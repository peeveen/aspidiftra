using System.Collections.Immutable;
using Aspose.Pdf;

namespace Aspidiftra
{
	public interface IWatermark
	{
		float Opacity { get; }
		IImmutableSet<int> GetApplicablePageNumbers(IImmutableSet<int> availablePageNumbers);
		WatermarkElementCollection GetWatermarkElements(Rectangle pageMediaBox);
	}
}