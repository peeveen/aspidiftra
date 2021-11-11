using Aspidiftra.Geometry;

namespace Aspidiftra
{
	/// <summary>
	///   Interface for an object describing the angle of a banner watermark.
	/// </summary>
	public interface IBannerAngle
	{
		/// <summary>
		///   Calculates the effective angle of the banner watermark.
		/// </summary>
		/// <param name="pageSize">Size of the page that the banner will be on.</param>
		/// <returns>Angle of banner.</returns>
		Angle GetAngle(PageSize pageSize);
	}

	/// <summary>
	///   Banner text will be aligned with the diagonal from
	///   the south west corner of the page to the north
	///   east corner.
	/// </summary>
	internal sealed class BottomLeftToTopRightBannerAngle : IBannerAngle
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
	internal sealed class BottomRightToTopLeftBannerAngle : IBannerAngle
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
	internal sealed class TopLeftToBottomRightBannerAngle : IBannerAngle
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
	internal sealed class TopRightToBottomLeftBannerAngle : IBannerAngle
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