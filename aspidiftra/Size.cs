using System;

namespace Aspidiftra
{
	/// <summary>
	///   Object representing a size.
	/// </summary>
	public class Size
	{
		private readonly float _size;
		private readonly Sizing _sizing;

		/// <summary>
		///   Constructor.
		/// </summary>
		/// <param name="size">Size value. Meaning of this value depends on <paramref name="sizing" />.</param>
		/// <param name="sizing">How the size value should be interpreted.</param>
		public Size(float size, Sizing sizing)
		{
			if (float.IsInfinity(size) || float.IsNaN(size))
				throw new ArgumentException("Size value must be a normal finite number.", nameof(size));
			_size = size;
			_sizing = sizing;
		}

		/// <summary>
		///   Get the effective size, relative to the given page size.
		/// </summary>
		/// <param name="pageSize">Page size.</param>
		/// <returns>Effective size.</returns>
		public float GetEffectiveSize(PageSize pageSize)
		{
			return (float) (_size * _sizing.GetSizingFactor(pageSize));
		}
	}
}