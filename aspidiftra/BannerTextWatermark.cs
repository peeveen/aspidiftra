using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Aspidiftra.Geometry;

namespace Aspidiftra
{
	public class BannerTextWatermark : TextWatermark
	{
		/// <summary>
		///   Angle for banners that sit along the diagonal between the bottom left corner and the top right corner of the page.
		/// </summary>
		public static IBannerAngle BottomLeftToTopRightAngle = new BottomLeftToTopRightBannerAngle();

		/// <summary>
		///   Angle for banners that sit along the diagonal between the top left corner and the bottom right corner of the page.
		/// </summary>
		public static IBannerAngle TopLeftToBottomRightAngle = new TopLeftToBottomRightBannerAngle();

		/// <summary>
		///   Angle for banners that sit along the diagonal between the top right corner and the bottom left corner of the page.
		/// </summary>
		public static IBannerAngle TopRightToBottomLeftAngle = new TopRightToBottomLeftBannerAngle();

		/// <summary>
		///   Angle for banners that sit along the diagonal between the bottom right corner and the top left corner of the page.
		/// </summary>
		public static IBannerAngle BottomRightToTopLeftAngle = new BottomRightToTopLeftBannerAngle();

		private readonly IBannerAngle _angle;

		/// <summary>
		///   A banner watermark, allowing text to be placed across the middle of the
		///   page at a chosen angle.
		/// </summary>
		/// <param name="text">
		///   Text to render. Can contain line breaks for multi-line watermarks.
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
		public BannerTextWatermark(string text, Appearance appearance, Justification justification,
			Fitting fit, Size marginSize, IBannerAngle angle,
			Func<IImmutableSet<int>, IImmutableSet<int>>? pageSelector = null)
			: base(text, appearance, justification, fit, marginSize, pageSelector)
		{
			_angle = angle;
		}

		/// <summary>
		///   A banner watermark, allowing text to be placed across the middle of the
		///   page at a chosen angle.
		/// </summary>
		/// <param name="text">
		///   Text to render. Can contain line breaks for multi-line watermarks.
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
		///   How the banner should be orientated on the page. A value of 0 will
		///   result in a horizontal (left to right) banner, 90 will get you a
		///   vertical (bottom to top) banner. The banner is always centered on
		///   the page. Be aware that using values from 90 to 270 will result in
		///   "upside down" text.
		/// </param>
		/// <param name="pageSelector">
		///   Function that will select the pages that the watermark will appear on,
		///   from a given set of page numbers. If no value is provided for this
		///   parameter, <see cref="AspidiftraUtil.AllPagesSelector" /> will be used.
		/// </param>
		public BannerTextWatermark(string text, Appearance appearance, Justification justification,
			Fitting fit, Size marginSize, Angle angle,
			Func<IImmutableSet<int>, IImmutableSet<int>>? pageSelector = null)
			: this(text, appearance, justification, fit, marginSize, new CustomBannerAngle(angle), pageSelector)
		{
		}

		protected override ITextSlotCalculator GetTextSlotCalculator(PageSize pageSize)
		{
			return new BannerTextSlotCalculator(pageSize, Fit, GetAngle(pageSize));
		}

		protected override Angle GetAngle(PageSize pageSize)
		{
			return _angle.GetAngle(pageSize);
		}

		protected override IEnumerable<MeasuredString> SelectOverflowStrings(IEnumerable<MeasuredString> strings,
			int availableLines)
		{
			return strings.Mid(availableLines);
		}
	}
}