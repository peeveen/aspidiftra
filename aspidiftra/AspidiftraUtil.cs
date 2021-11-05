using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aspose.Pdf;

namespace Aspidiftra
{
	public static class AspidiftraUtil
	{
		/// <summary>
		///   Acceptable amount to be "off by" for floating point operations.
		/// </summary>
		internal const double GeometricTolerance = 0.0000001;

		public static IImmutableList<string> SplitTextIntoLines(string text)
		{
			// Platform neuter the text, with regard to newlines.
			text = text.Replace("\r\n", "\n");
			var lines = text.Split('\n').Select(line => line.Trim());
			return lines.ToImmutableList();
		}

		internal static IImmutableSet<int> AllPagesSelector(IImmutableSet<int> pages)
		{
			return pages;
		}

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