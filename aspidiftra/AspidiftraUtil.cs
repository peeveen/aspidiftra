using System;
using System.Collections.Generic;
using System.Collections.Immutable;

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
	}
}