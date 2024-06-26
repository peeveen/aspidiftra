﻿using System;

namespace Aspidiftra
{
	/// <summary>
	///   Flags that can be used to specify how a watermark should fit the page.
	///   These can be combined with a bitwise OR.
	/// </summary>
	[Flags]
	public enum Fitting
	{
		/// <summary>
		///   No fitting is applied.
		/// </summary>
		None = 0,

		/// <summary>
		///   Text will be broken into multiple lines if it is too big. This
		///   takes precedence over <see cref="Shrink" /> or <see cref="Grow" />,
		///   and will be applied first.
		///   Text will only be broken on whitespace.
		///   If the provided watermark text already contains line breaks, then
		///   this flag will be ignored, as it will be assumed that the line breaks
		///   are positionally important.
		///   If you specify this flag with <see cref="Shrink" />, then shrinking will
		///   only occur if the broken-up text contains a very long sequence of
		///   non-whitespace characters that are longer than the available space.
		///   If you specify this flag with <see cref="Grow" />, then the broken-up
		///   text will be increased in size until it fits the available space as closely
		///   as possible.
		/// </summary>
		Wrap = 1,

		/// <summary>
		///   Text can be made smaller if it does not fit.
		/// </summary>
		Shrink = 2,

		/// <summary>
		///   Text can be made larger to fit the given area as closely as possible.
		/// </summary>
		Grow = 4,

		/// <summary>
		///   Text is allowed to overflow the page boundaries.
		/// </summary>
		Overflow = 8
	}

	public static class FittingExtensions
	{
		public static bool Has(this Fitting fitting, Fitting check)
		{
			return (fitting & check) == check;
		}

		public static bool HasWrap(this Fitting fitting)
		{
			return fitting.Has(Fitting.Wrap);
		}

		public static bool HasShrink(this Fitting fitting)
		{
			return fitting.Has(Fitting.Shrink);
		}

		public static bool HasGrow(this Fitting fitting)
		{
			return fitting.Has(Fitting.Grow);
		}

		public static bool HasOverflow(this Fitting fitting)
		{
			return fitting.Has(Fitting.Overflow);
		}
	}
}