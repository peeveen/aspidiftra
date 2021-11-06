using Aspidiftra.Geometry;

namespace Aspidiftra
{
	public interface IBannerAngle
	{
		Angle GetAngle(PageSize pageSize);
	}

	/// <summary>
	///   Banner text will be aligned with the diagonal from
	///   the south west corner of the page to the north
	///   east corner.
	/// </summary>
	public sealed class BottomLeftToTopRightBannerAngle : IBannerAngle
	{
		public Angle GetAngle(PageSize pageSize)
		{
			return pageSize.BottomLeftToTopRightAngle();
		}
	}

	/// <summary>
	///   Banner text will be aligned with the diagonal from
	///   the north west corner of the page to the south
	///   east corner.
	/// </summary>
	public sealed class BottomRightToTopLeftBannerAngle : IBannerAngle
	{
		public Angle GetAngle(PageSize pageSize)
		{
			return pageSize.BottomRightToTopLeftAngle();
		}
	}

	/// <summary>
	///   Banner text will be aligned with the diagonal from
	///   the south west corner of the page to the north
	///   east corner.
	/// </summary>
	public sealed class TopLeftToBottomRightBannerAngle : IBannerAngle
	{
		public Angle GetAngle(PageSize pageSize)
		{
			return pageSize.TopLeftToBottomRightAngle();
		}
	}

	/// <summary>
	///   Banner text will be aligned with the diagonal from
	///   the north west corner of the page to the south
	///   east corner.
	/// </summary>
	public sealed class TopRightToBottomLeftBannerAngle : IBannerAngle
	{
		public Angle GetAngle(PageSize pageSize)
		{
			return pageSize.TopRightToBottomLeftAngle();
		}
	}

	/// <summary>
	///   Banner text will be at whatever angle you specify.
	/// </summary>
	public sealed class CustomBannerAngle : IBannerAngle
	{
		private readonly Angle _angle;

		public CustomBannerAngle(Angle angle)
		{
			_angle = angle;
		}

		public Angle GetAngle(PageSize _)
		{
			return _angle;
		}
	}
}