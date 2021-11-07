using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aspidiftra.Geometry;

namespace Aspidiftra
{
	public class BannerWatermark : Watermark
	{
		private readonly IBannerAngle _angle;

		/// <summary>
		///   A page edge watermark, allowing text to be placed alongside any of the
		///   four pages edges, running in either appropriate orthogonal direction.
		/// </summary>
		/// <param name="text">
		///   Text to place along the edge of the page. Can contain line breaks for
		///   multi-line watermarks.
		/// </param>
		/// <param name="appearance">Stylistic attributes for the text.</param>
		/// <param name="justification">
		///   Should the text be left/right/centre justified?
		/// </param>
		/// <param name="fit">
		///   Should the banner be made larger/smaller to fit the page more
		///   precisely? Can the text be broken up and wrapped across many lines?
		/// </param>
		/// <param name="marginSize">
		///   Size of margin, if you want to ensure the watermark isn't actually
		///   touching the page edge.
		/// </param>
		/// <param name="angle">
		///   How the banner should be orientated on the page (i.e. across which
		///   diagonal). Can also be set to an explicit angle value. In that case,
		///   a value of 0 will result in a horizontal (left to right) banner, 90
		///   will get you a vertical (bottom to top) banner. The banner is always
		///   centered on the page. Be aware that using values from 90 to 270 will
		///   result in "upside down" text.
		/// </param>
		/// <param name="pageSelector">
		///   Function that will select the pages that the watermark will appear on,
		///   from a given set of page numbers. If no value is provided for this
		///   parameter, <see cref="AspidiftraUtil.AllPagesSelector" /> will be used.
		/// </param>
		public BannerWatermark(string text, Appearance appearance, Justification justification,
			Fitting fit, Size marginSize, IBannerAngle angle,
			Func<IImmutableSet<int>, IImmutableSet<int>>? pageSelector = null)
			: base(text,appearance,justification,fit,marginSize,pageSelector)
		{
			_angle = angle;
		}

		protected override ITextSlotCalculator GetTextSlotCalculator(PageSize pageSize)
		{
			return new BannerTextSlotCalculator(pageSize, GetWatermarkAngle(pageSize));
		}

		protected override Angle GetWatermarkAngle(PageSize pageSize)
		{
			return _angle.GetAngle(pageSize);
		}
	}
}