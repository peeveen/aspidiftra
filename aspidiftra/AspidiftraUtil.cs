using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aspose.Pdf;

namespace Aspidiftra
{
	public static class AspidiftraUtil
	{
		internal static IImmutableSet<int> AllPagesSelector(IImmutableSet<int> pages)
		{
			return pages;
		}

		/// <summary>
		///   Watermarks the given source PDF, and saves to a new path.
		/// </summary>
		/// <param name="inputPath">Path to the source PDF.</param>
		/// <param name="watermarks">The watermarks to apply.</param>
		/// <param name="outputPath">The path to write the watermarked PDF to.</param>
		public static void WatermarkPdf(string inputPath, IEnumerable<IWatermark> watermarks, string outputPath)
		{
			if (inputPath == null)
				throw new ArgumentNullException(nameof(inputPath));
			if (outputPath == null)
				throw new ArgumentNullException(nameof(outputPath));
			if (watermarks == null)
				throw new ArgumentNullException(nameof(watermarks));

			using var sourcePdf = new AspidiftraDocument(inputPath);
			foreach (var watermark in watermarks)
				sourcePdf.ApplyWatermark(watermark);
			sourcePdf.Save(outputPath);
		}

		/// <summary>
		/// Concatenates multiple PDF documents together into one.
		/// </summary>
		/// <param name="paths">Paths to the PDFs that you want to join, in order.</param>
		/// <param name="outputPath">Path to write the new concatenated PDF to.</param>
		public static void ConcatenatePdfs(IEnumerable<string> paths, string outputPath)
		{
			if (paths == null)
				throw new ArgumentNullException(nameof(paths));
			if (outputPath == null)
				throw new ArgumentNullException(nameof(outputPath));
			outputPath = outputPath.Trim();
			if (string.IsNullOrEmpty(outputPath))
				throw new ArgumentException("Output path must not be blank.", nameof(outputPath));
			var pdfPaths = paths.ToImmutableList();
			if (!pdfPaths.Any())
				throw new ArgumentException("At least one PDF must be provided.", nameof(paths));
			var concatenatePdfPaths = pdfPaths.TakeLast(pdfPaths.Count - 1);
			// Open first document
			var pdfsToDispose = new List<Document>();
			using (var originalPdf = new Document(pdfPaths.First()))
			{
				// Stick the others on, in order.
				foreach (var nextPdfPath in concatenatePdfPaths)
				{
					var nextPdf = new Document(nextPdfPath);
					pdfsToDispose.Add(nextPdf);
					originalPdf.Pages.Add(nextPdf.Pages);
				}

				originalPdf.Save(outputPath);
			}

			foreach (var pdf in pdfsToDispose)
				pdf.Dispose();
		}
	}
}