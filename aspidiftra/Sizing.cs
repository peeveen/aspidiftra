namespace Aspidiftra
{
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
		RelativeToShortestSide,

		/// <summary>
		///   Size is a percentage of the longest side of the page.
		/// </summary>
		RelativeToLongestSide
	}
}