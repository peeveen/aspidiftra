namespace Aspidiftra
{
	/// <summary>
	///   Enumeration of ways to interpret a size value.
	/// </summary>
	public enum Sizing
	{
		/// <summary>
		///   Size is absolute.
		/// </summary>
		Absolute,

		/// <summary>
		///   Size is a percentage of the page width.
		/// </summary>
		RelativeToWidth,

		/// <summary>
		///   Size is a percentage of the page height.
		/// </summary>
		RelativeToHeight,

		/// <summary>
		///   Size is a percentage of the average of the page width and height.
		/// </summary>
		RelativeToAverageSideLength,

		/// <summary>
		///   Size is a percentage of the diagonal length of the page, from corner to opposite corner.
		/// </summary>
		RelativeToDiagonalSize,

		/// <summary>
		///   Size is a percentage of the shortest side of the page.
		/// </summary>
		RelativeToShorterSide,

		/// <summary>
		///   Size is a percentage of the longest side of the page.
		/// </summary>
		RelativeToLongerSide
	}

	public static class SizingExtensions
	{
		/// <summary>
		///   Returns the value that should be multiplied by a size value to
		///   obtain the effective size, relative to a given page size.
		/// </summary>
		/// <param name="sizing">The type of sizing.</param>
		/// <param name="pageSize">The page size.</param>
		/// <returns>The relevant sizing factor.</returns>
		public static double GetSizingFactor(this Sizing sizing, PageSize pageSize)
		{
			return sizing switch
			{
				Sizing.RelativeToWidth => pageSize.Width,
				Sizing.RelativeToHeight => pageSize.Height,
				Sizing.RelativeToAverageSideLength => pageSize.AverageSideLength(),
				Sizing.RelativeToDiagonalSize => pageSize.DiagonalLength(),
				Sizing.RelativeToShorterSide => pageSize.ShorterSideLength(),
				Sizing.RelativeToLongerSide => pageSize.LongerSideLength(),
				_ => 1.0
			};
		}
	}
}