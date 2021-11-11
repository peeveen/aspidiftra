using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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
		///   Returns the middle N elements from a collection.
		/// </summary>
		/// <typeparam name="T">The generic type.</typeparam>
		/// <param name="collection">The collection.</param>
		/// <param name="amount">The number of elements to return.</param>
		/// <returns>
		///   The elements from the middle of the collection, or as near
		///   as it can get if an odd number of elements need to be removed.
		/// </returns>
		public static IEnumerable<T> Mid<T>(this IEnumerable<T> collection, int amount)
		{
			var asList = collection.ToList();
			var collectionSize = asList.Count;
			if (amount > collectionSize)
				throw new ArgumentException(
					$"Cannot take the middle {amount} elements from a collection that only has {collectionSize} elements in it.");
			var tooMany = collectionSize - amount;
			var amountToRemoveEachSide = tooMany / 2;
			return asList.GetRange(amountToRemoveEachSide, amount);
		}
	}
}