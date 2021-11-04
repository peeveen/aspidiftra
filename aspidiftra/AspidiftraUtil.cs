using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Aspose.Pdf;
using Aspose.Pdf.Facades;
using Color = System.Drawing.Color;

namespace Aspidiftra
{
	public static class AspidiftraUtil
	{
		public static double GetSizingFactor(this Sizing sizing, Rectangle rect)
		{
			return sizing switch
			{
				Sizing.RelativeToWidth => rect.Width,
				Sizing.RelativeToHeight => rect.Height,
				Sizing.RelativeToAverageSideLength => rect.AverageSideLength(),
				Sizing.RelativeToDiagonalSize => rect.DiagonalLength(),
				Sizing.RelativeToShortestSide => rect.ShortestSideLength(),
				Sizing.RelativeToLongestSide => rect.LongestSideLength(),
				_ => 1.0
			};
		}

		public static FontColor ToFontColor(this Color color)
		{
			return new FontColor(color.R, color.G, color.B);
		}

		public static Rectangle Normalize(this Rectangle rect, Rotation rotation)
		{
			var flip = rotation == Rotation.on90 || rotation == Rotation.on270;
			var width = flip ? rect.Height : rect.Width;
			var height = flip ? rect.Width : rect.Height;
			return new Rectangle(0, 0, width, height);
		}

		public static double DiagonalLength(this Rectangle rect)
		{
			var widthSquared = rect.Width * rect.Width;
			var heightSquared = rect.Height * rect.Height;
			return Math.Sqrt(widthSquared + heightSquared);
		}

		public static double AverageSideLength(this Rectangle rect)
		{
			return rect.Width + rect.Height / 2.0;
		}

		public static double ShortestSideLength(this Rectangle rect)
		{
			return Math.Min(rect.Width, rect.Height);
		}

		public static double LongestSideLength(this Rectangle rect)
		{
			return Math.Max(rect.Width, rect.Height);
		}

		/// <summary>
		///   Calculates the angle between the X axis and the line that
		///   runs from the bottom-left corner to the top-right corner.
		/// </summary>
		/// <param name="rect">This rectangle.</param>
		/// <returns>
		///   Angle between the X axis and the line that runs from the
		///   bottom-left corner to the top-right corner.
		/// </returns>
		public static Angle DiagonalAngle(this Rectangle rect)
		{
			return new Angle(Math.Atan(rect.Height / rect.Width), AngleUnits.Radians);
		}

		public static Angle GetAngle(this PageEdgePosition pageEdgePosition)
		{
			return pageEdgePosition switch
			{
				PageEdgePosition.East => Angle.Degrees270,
				PageEdgePosition.West => Angle.Degrees90,
				_ => Angle.Degrees0
			};
		}

		public static Fitting Normalize(this Fitting fitting, IEnumerable<string> text)
		{
			return text.Count() > 1 ? fitting &= ~Fitting.Wrap : fitting;
		}

		public static Point OffsetBy(this Point point, double xOffset, double yOffset)
		{
			return new Point(point.X + xOffset, point.Y + yOffset);
		}

		public static IImmutableList<string> SplitTextIntoLines(string text)
		{
			// Platform neuter the text, with regard to newlines.
			text = text.Replace("\r\n", "\n");
			var lines = text.Split('\n').Select(line => line.Trim());
			return lines.ToImmutableList();
		}

		internal static IImmutableSet<int> AllPagesSelector(IImmutableSet<int> pages)
		{
			return pages;
		}

		public static void ConcatenatePdfs(IEnumerable<string> paths, string outputPath)
		{
			if (paths == null)
				throw new ArgumentNullException(nameof(paths));
			if (outputPath == null)
				throw new ArgumentNullException(nameof(outputPath));
			outputPath = outputPath.Trim();
			if (string.IsNullOrEmpty(outputPath))
				throw new ArgumentException("Output path must not be blank.", nameof(outputPath));
			var pdfPaths = paths.ToImmutableList();
			if (!pdfPaths.Any())
				throw new ArgumentException("At least one PDF must be provided.", nameof(paths));
			var concatenatePdfPaths = pdfPaths.TakeLast(pdfPaths.Count - 1);
			// Open first document
			var pdfsToDispose = new List<Document>();
			using (var originalPdf = new Document(pdfPaths.First()))
			{
				// Stick the others on, in order.
				foreach (var nextPdfPath in concatenatePdfPaths)
				{
					var nextPdf = new Document(nextPdfPath);
					pdfsToDispose.Add(nextPdf);
					originalPdf.Pages.Add(nextPdf.Pages);
				}

				originalPdf.Save(outputPath);
			}

			foreach (var pdf in pdfsToDispose)
				pdf.Dispose();
		}
	}
}