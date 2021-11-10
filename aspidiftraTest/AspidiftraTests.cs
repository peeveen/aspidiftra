using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Aspidiftra;
using Aspidiftra.Geometry;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using NUnit.Framework;
using Color = System.Drawing.Color;
using Font = Aspidiftra.Font;
using PageSize = Aspidiftra.PageSize;
using Point = Aspidiftra.Geometry.Point;
using Rectangle = Aspidiftra.Geometry.Rectangle;

namespace AspidiftraTest
{
	public class AspidiftraTests
	{
		/// <summary>
		///   Acceptable amount to be "off by" for floating point operations.
		/// </summary>
		internal const double GeometricTolerance = 0.0000001;

		private const string TestPdfsFolder = "..\\..\\..\\TestPDFs";
		private const string OutputPdfsFolder = "..\\..\\..\\OutputPDFs";
		private const string AsposeLicenseFolder = "..\\..\\..\\AsposeLicense";
		private const string AsposeLicenseFilename = "Aspose.Total.lic.xml";

		private const string Simple = "01_Simple_1191_842.pdf";
		private const string CropBox = "02_CropBox_800_400.pdf";
		private const string NegativeVerticalCoordinateSpace = "03_NegativeVerticalCoordinateSpace_1600_2400.pdf";
		private const string NegativeHorizontalCoordinateSpace = "04_NegativeHorizontalCoordinateSpace_1600_2400.pdf";

		private const string NegativeHorizontalAndVerticalCoordinateSpace =
			"05_NegativeHorizontalAndVerticalCoordinateSpace_1600_2400.pdf";

		private const string Portrait = "06_Portrait_750_1200.pdf";
		private const string FivePages = "07_FivePages_841.98_595.38.pdf";
		private const string DigitalSignature = "08_DigitalSignature_612_792.pdf";
		private const string LoremIpsum = "09_LoremIpsum.pdf";
		private const string Square720Pages = "10_Square720Pages.pdf";
		private const string DifferentPageSizes = "11_DifferentPageSizes.pdf";

		[SetUp]
		public void Setup()
		{
			var asposeLicensePath = Path.Join(AsposeLicenseFolder, AsposeLicenseFilename);
			if (File.Exists(asposeLicensePath))
			{
				var license = new License();
				license.SetLicense(asposeLicensePath);
			}
		}

		[Test]
		[Order(0)]
		public void LineTests()
		{
			var line1 = new Line(new Point(0.0, 0.0), new Angle(45.0, AngleUnits.Degrees));
			var line2 = new Line(new Point(10.0, 0.0), new Angle(135.0, AngleUnits.Degrees));
			var intersectionPoint1 = line1.GetIntersectionPoint(line2);
			Assert.AreEqual(new Point(5.0, 5.0), intersectionPoint1);

			var verticalLine = new Line(new Point(20.0, 50.0), Angle.Degrees270);
			var intersectionPoint2 = line1.GetIntersectionPoint(verticalLine);
			Assert.AreEqual(new Point(20.0, 20.0), intersectionPoint2);

			var page = new PageSize(500, 720);
			var pageCenterPoint = page.Center;
			var pageLines = page.Lines;
			for (var angle = 0.0; angle < 360.0; angle += 0.01)
			{
				var lineThroughPage = new Line(pageCenterPoint, new Angle(angle, AngleUnits.Degrees));
				var intersectionPoints = pageLines.Select(line => line.GetIntersectionPoint(lineThroughPage))
					.Where(point => point != null && page.Contains(point));
				Assert.AreEqual(2, intersectionPoints.Count(),
					$"There should only be two intersection points (angle was {angle})");
			}
		}

		[Test]
		[Order(0)]
		public void TextSplitTest()
		{
			// ReSharper disable line StringLiteralTypo
			var splitTestText = "   This is a     t�st of\nthe sssssssssssssstring   splitting code\n\n  .";
			var tokens = new StringTokenCollection(splitTestText).ToList();
			Assert.AreEqual(20, tokens.Count);
			var tenthToken = tokens[9];
			var fourteenthToken = tokens[13];
			Assert.IsTrue(tenthToken.String == "\n" && tenthToken.Type == StringToken.TokenType.LineBreak);
			Assert.IsTrue(fourteenthToken.String == "   " && fourteenthToken.Type == StringToken.TokenType.Whitespace);
		}

		[Test]
		[Order(0)]
		public void AngleTests()
		{
			Assert.AreEqual(Angle.Degrees180, Angle.RadiansPi);

			var checkNormalizationDegrees1 = new Angle(700.0, AngleUnits.Degrees);
			Assert.AreEqual(340.0, checkNormalizationDegrees1.Value, GeometricTolerance);
			var checkNormalizationDegrees2 = new Angle(-1000000.0, AngleUnits.Degrees);
			Assert.AreEqual(80.0, checkNormalizationDegrees2.Value, GeometricTolerance);
			var checkNormalizationRadians1 = new Angle(Math.PI * (7.0 / 3.0), AngleUnits.Radians);
			Assert.AreEqual(Math.PI * (1.0 / 3.0), checkNormalizationRadians1.Value, GeometricTolerance);
			var checkNormalizationRadians2 = new Angle(-Math.PI * (10000.0 / 3.0), AngleUnits.Radians);
			Assert.AreEqual(Math.PI * (2.0 / 3.0), checkNormalizationRadians2.Value, GeometricTolerance);

			var thirtyDegrees = new Angle(30.0, AngleUnits.Degrees);
			var xReversedThirtyDegrees = thirtyDegrees.ReverseX();
			Assert.AreEqual(330.0, xReversedThirtyDegrees.Value, GeometricTolerance);
			var yReversedThirtyDegrees = thirtyDegrees.ReverseY();
			Assert.AreEqual(150.0, yReversedThirtyDegrees.Value, GeometricTolerance);
			var reversedThirtyDegrees = thirtyDegrees.Reverse();
			Assert.AreEqual(210.0, reversedThirtyDegrees.Value, GeometricTolerance);

			// 300 degrees.
			var largeRadiansAngle = new Angle(Math.PI * (5.0 / 3.0), AngleUnits.Radians);
			var xReversedRadians = largeRadiansAngle.ReverseX();
			Assert.AreEqual(Math.PI * (1.0 / 3.0), xReversedRadians.Value, GeometricTolerance);
			var yReversedRadians = largeRadiansAngle.ReverseY();
			Assert.AreEqual(Math.PI * (4.0 / 3.0), yReversedRadians.Value, GeometricTolerance);
			var reversedRadians = largeRadiansAngle.Reverse();
			Assert.AreEqual(Math.PI * (2.0 / 3.0), reversedRadians.Value, GeometricTolerance);
		}

		[Test]
		[Order(0)]
		public void PageSizeFunctions()
		{
			const double rectWidth = 10.0;
			const double rectHeight = 20.0;
			var diagonalAngleRadians = new Angle(Math.Atan(rectHeight / rectWidth), AngleUnits.Radians);
			var rect = new Rectangle(0, 0, rectWidth, rectHeight);
			Assert.AreEqual(Math.Sqrt(rectWidth * rectWidth + rectHeight * rectHeight), rect.DiagonalLength(),
				GeometricTolerance);
			Assert.AreEqual(rectWidth + rectHeight / 2.0, rect.AverageSideLength(), GeometricTolerance);
			Assert.AreEqual(rectHeight, rect.LongerSideLength());
			Assert.AreEqual(rectWidth, rect.ShorterSideLength());
			Assert.AreEqual(diagonalAngleRadians, new PageSize(rect.Width, rect.Height).BottomLeftToTopRightAngle());
		}

		[Test]
		[Order(2)]
		public void ApplyPageEdgeWatermark()
		{
			var testPdfPath = Path.Join(TestPdfsFolder, DifferentPageSizes);
			var outputPdfPath = Path.Join(OutputPdfsFolder, "PageEdgeWatermarked.pdf");
			var watermarkFont = new Font("Helvetica", FontStyles.Italic, new Size(.025f, Sizing.RelativeToDiagonalSize));
			var watermarkAppearance = new Appearance(Color.Red, 0.6f, watermarkFont);
			var pageEdgeWatermark = new PageEdgeTextWatermark(
				"This is a page edge watermark test that I hope\nwill execute successfully.",
				watermarkAppearance,
				PageEdgePosition.Bottom, Justification.Centre, Fitting.Wrap | Fitting.Shrink | Fitting.Grow,
				new Size(0.03f, Sizing.RelativeToAverageSideLength), true);

			AspidiftraUtil.WatermarkPdf(testPdfPath, new[] {pageEdgeWatermark}, outputPdfPath);

			// TODO: Test the output, somehow?
		}

		[Test]
		[Order(2)]
		public void ApplyBannerWatermark()
		{
			var testPdfPath = Path.Join(TestPdfsFolder, DifferentPageSizes);
			var outputPdfPath = Path.Join(OutputPdfsFolder, "BannerWatermarked.pdf");
			var watermarkFont = new Font("Helvetica", FontStyles.Regular, new Size(.035f, Sizing.RelativeToDiagonalSize));
			var watermarkAppearance = new Appearance(Color.Green, 0.6f, watermarkFont);
			var bannerWatermark = new BannerTextWatermark(
				"This is a banner watermark test that I sincerely hope\nwill execute successfully, mainly because the maths involved was bloody difficult.",
				watermarkAppearance, Justification.Centre, Fitting.Wrap | Fitting.Shrink,
				new Size(0.08f, Sizing.RelativeToAverageSideLength),
				new CustomBannerAngle(new Angle(123.4, AngleUnits.Degrees)));

			AspidiftraUtil.WatermarkPdf(testPdfPath, new[] {bannerWatermark}, outputPdfPath);
			// TODO: Test the output, somehow?
		}

		[Test]
		[Order(2)]
		public void ApplyBoth()
		{
			var testPdfPath = Path.Join(TestPdfsFolder, LoremIpsum);
			var outputPdfPath = Path.Join(OutputPdfsFolder, "BothWatermarked.pdf");
			var watermarkFont = new Font("Helvetica", FontStyles.Italic, new Size(.025f, Sizing.RelativeToDiagonalSize));
			var watermarkRedAppearance = new Appearance(Color.Red, 1.0f, watermarkFont);
			var watermarkGreenAppearance = new Appearance(Color.Green, 0.8f, watermarkFont);

			var pageEdgeWatermark = new PageEdgeTextWatermark(
				"This is a page edge watermark", // Watermark text
				watermarkRedAppearance, // Cosmetic appearance of the text
				PageEdgePosition.Bottom, // Where to place the watermark
				Justification.Centre, // Justification of text
				Fitting.Wrap | Fitting.Shrink, // How to best fit the text if it bigger than the page.
				new Size(0.03f, Sizing.RelativeToAverageSideLength), // Margin
				true); // Reverse the direction of the text

			var bannerWatermark = new BannerTextWatermark(
				"This is my banner text.",
				watermarkGreenAppearance,
				Justification.Centre,
				Fitting.Wrap | Fitting.Shrink | Fitting.Grow,
				new Size(0.08f, Sizing.RelativeToAverageSideLength),
				new CustomBannerAngle(new Angle(123.4, AngleUnits.Degrees)));

			var watermarks = new IWatermark[] {pageEdgeWatermark, bannerWatermark};
			AspidiftraUtil.WatermarkPdf(testPdfPath, watermarks, outputPdfPath);
		}

		[Test]
		[Order(2)]
		public void TestBannerRotation()
		{
			var testPdfPath = Path.Join(TestPdfsFolder, Square720Pages);
			var outputPdfPath = Path.Join(OutputPdfsFolder, "BannerRotationTest.pdf");
			var watermarkFont = new Font("Helvetica", FontStyles.Regular, new Size(.035f, Sizing.RelativeToDiagonalSize));
			var watermarkAppearance = new Appearance(Color.Blue, 0.6f, watermarkFont);
			var watermarks = new List<IWatermark>();
			for (var n = 1; n <= 720; ++n)
			{
				var angle = (n-1) * 0.5;
				var requiredPageNumbers = new[] {n}.ToImmutableHashSet();
				var bannerWatermark = new BannerTextWatermark(
					$"Banner at {angle} degrees",
					watermarkAppearance, Justification.Centre, Fitting.Shrink | Fitting.Grow,
					new Size(0.08f, Sizing.RelativeToAverageSideLength),
					new CustomBannerAngle(new Angle(angle, AngleUnits.Degrees)),
					pages => requiredPageNumbers);
				watermarks.Add(bannerWatermark);
			}

			AspidiftraUtil.WatermarkPdf(testPdfPath, watermarks, outputPdfPath);
		}
	}
}