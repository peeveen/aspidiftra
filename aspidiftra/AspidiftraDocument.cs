using System;
using System.Collections.Immutable;
using System.Linq;
using Aspose.Pdf;
using Aspose.Pdf.Facades;
using Rectangle = Aspidiftra.Geometry.Rectangle;
using Stamp = Aspose.Pdf.Facades.Stamp;

namespace Aspidiftra
{
	public class AspidiftraDocument : IDisposable
	{
		/// <summary>
		///   Aspose.Pdf object through which we apply watermarks.
		/// </summary>
		private readonly PdfFileStamp _fileStamp;

		/// <summary>
		///   The set of page numbers in this document.
		/// </summary>
		private readonly IImmutableSet<int> _pageNumbers;

		/// <summary>
		///   Page numbers grouped by page size.
		/// </summary>
		private readonly IImmutableDictionary<PageSize, ImmutableList<int>> _pageNumbersBySize;

		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="path">Path to the source PDF.</param>
		/// <param name="password">Any password required.</param>
		public AspidiftraDocument(string path, string? password = null)
		{
			var document = new Document(path, password);
			_pageNumbers = document.Pages.Select(page => page.Number).ToImmutableHashSet();
			// Gather up all the pages into a dictionary, keyed on normalized page size.
			// "Normalized" means that, even if the page is rotated, or has a negative coordinate space,
			// the resulting rectangle will be a simple (0, 0, width, height) rectangle that we can
			// use in Facade construction, as Facades have their own coordinates/rotation.
			//
			// When we ask a watermark to calculate it's position, etc, it can be quite a heavy operation.
			// We don't want to have to do that for every page, especially when most PDFs just contain
			// pages that are all the same size.
			// So we "group" the pages by size, and later, we can ask the watermark to calculate
			// its positions for each group, then apply those positions to all the pages that share
			// that size.
			_pageNumbersBySize = document.Pages
				.GroupBy(GetNormalizedPageSize, page => page.Number)
				.ToImmutableDictionary(grouping => grouping.Key, grouping => grouping.ToImmutableList());
			// Wrap the document in a PdfFileStamp object, for watermarking.
			// 'Cos that's why we're here, right?
			_fileStamp = new PdfFileStamp(document);
		}

		public void Dispose()
		{
			// Disposing the file stamp also disposes the wrapped Document.
			_fileStamp.Dispose();
		}

		/// <summary>
		///   Gets the normalized size of a page. "Normalized" meaning the effective visible
		///   area translated to a rectangle with (0,0) as the bottom left coordinate.
		/// </summary>
		/// <param name="page">The page to get the normalized size of.</param>
		/// <returns>Normalized page size.</returns>
		private static PageSize GetNormalizedPageSize(Page page)
		{
			// All pages have a MediaBox that defines the page size.
			// There is also the CropBox that defines how much of that page is visible.
			// In most cases, CropBox will be the same as MediaBox ... but not always.
			var pageSize = new Rectangle(page.MediaBox.Intersect(page.CropBox));
			var flip = page.Rotate == Rotation.on90 || page.Rotate == Rotation.on270;
			return new PageSize(flip ? pageSize.Height : pageSize.Width, flip ? pageSize.Width : pageSize.Height);
		}

		/// <summary>
		///   Save this document to disk.
		/// </summary>
		/// <param name="path">Path of output PDF.</param>
		public void Save(string path)
		{
			_fileStamp.Save(path);
		}

		/// <summary>
		///   Applies the given watermark to this PDF.
		/// </summary>
		/// <param name="watermark">Watermark to apply.</param>
		public void ApplyWatermark(IWatermark watermark)
		{
			// Figure out what pages this watermark should apply to.
			var applicablePageNumbers = watermark.GetApplicablePageNumbers(_pageNumbers);
			// Ask the watermark to calculate a list of positioned, scaled & angled watermark
			// elements to apply to each appropriate page. We will only ask the watermark to do
			// this once per unique page size.
			var pagesAndWatermarksBySize =
				_pageNumbersBySize.Where(kvp => kvp.Value.Intersect(applicablePageNumbers).Any())
					.Select(kvp => (kvp.Value, watermark.GetWatermarkElements(kvp.Key)));
			// Now apply those watermark elements to each appropriate page.
			foreach (var (pageNumbers, watermarkElements) in pagesAndWatermarksBySize)
			foreach (var watermarkElement in watermarkElements)
			{
				var formattedText = watermarkElement.FormattedText;
				var stamp = new Stamp
				{
					Pages = pageNumbers.ToArray(),
					// Aspose only accepts angles in degrees.
					Rotation = (float) watermarkElements.Angle.ToDegrees().Value,
					IsBackground = watermarkElements.IsBackground,
					Opacity = watermark.Opacity
				};
				stamp.BindLogo(formattedText);
				stamp.SetOrigin((float) watermarkElement.Position.X, (float) watermarkElement.Position.Y);
				_fileStamp.AddStamp(stamp);
			}
		}
	}
}