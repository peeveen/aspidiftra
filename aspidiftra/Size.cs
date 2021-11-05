namespace Aspidiftra
{
	public class Size
	{
		private readonly float _size;
		private readonly Sizing _sizing;

		public Size(float size, Sizing sizing)
		{
			_size = size;
			_sizing = sizing;
		}

		public Size(float size) : this(size, Sizing.Absolute)
		{
		}

		public float GetEffectiveSize(PageSize pageSize)
		{
			return (float) (_size * _sizing.GetSizingFactor(pageSize));
		}
	}
}