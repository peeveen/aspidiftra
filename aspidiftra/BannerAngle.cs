using Aspose.Pdf;

namespace Aspidiftra
{
	public interface IBannerAngle
	{
		Angle GetAngle(Rectangle pageSize);
	}

	/// <summary>
	///   Banner text will be aligned with the diagonal from
	///   the south west corner of the page to the north
	///   east corner.
	/// </summary>
	public sealed class SouthWestToNorthEastBannerAngle : IBannerAngle
	{
		public Angle GetAngle(Rectangle pageSize)
		{
			return pageSize.DiagonalAngle();
		}
	}

	/// <summary>
	///   Banner text will be aligned with the diagonal from
	///   the north west corner of the page to the south
	///   east corner.
	/// </summary>
	public sealed class NorthWestToSouthEastBannerAngle : IBannerAngle
	{
		public Angle GetAngle(Rectangle pageSize)
		{
			return pageSize.DiagonalAngle().ReverseX();
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

		public Angle GetAngle(Rectangle pageSize)
		{
			return _angle;
		}
	}
}