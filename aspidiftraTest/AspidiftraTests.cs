using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Aspidiftra;
using Aspidiftra.Geometry;
using Aspose.Pdf;
using Aspose.Pdf.Facades;
using NUnit.Framework;
using Color = System.Drawing.Color;
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
		private const string LoremIpsum = "09_LoremIpsum_595_841.pdf";
		private const string Square720Pages = "10_Square720Pages_600_600.pdf";
		private const string DifferentPageSizes = "11_DifferentPageSizes.pdf";
		private const string Bookmarks = "12_Bookmarks.pdf";

		private const string SeenAndNotSeenLyrics = "He would see faces in movies, on T.V., in magazines, and in books,\n" +
		                                            "He thought that some of these faces might be right for him,\n" +
		                                            "And through the years, by keeping an ideal facial structure fixed in his mind,\n" +
		                                            "Or somewhere in the back of his mind,\n" +
		                                            "That he might, by force of will, cause his face to approach those of his ideal,\n" +
		                                            "The change would be very subtle,\n" +
		                                            "It might take ten years or so,\n" +
		                                            "Gradually his face would change its shape,\n" +
		                                            "A more hooked nose,\n" +
		                                            "Wider, thinner lips,\n" +
		                                            "Beady eyes,\n" +
		                                            "A larger forehead,\n" +
		                                            "He imagined that this was an ability he shared with most other people,\n" +
		                                            "They had also molded their faces according to some ideal,\n" +
		                                            "Maybe they imagined that their new face would better suit their personality,\n" +
		                                            "Or maybe they imagined that their personality would be forced to change,\n" +
		                                            "To fit the new appearance,\n" +
		                                            "This is why first impressions are often correct,\n" +
		                                            "Although some people might have made mistakes,\n" +
		                                            "They may have arrived at an appearance that bears no relationship to them,\n" +
		                                            "They may have picked an ideal appearance based on some childish whim,\n" +
		                                            "Or momentary impulse,\n" +
		                                            "Some may have gotten half way there, and then changed their minds,\n" +
		                                            "He wonders if he too might have made a similar mistake.";

		/// <summary>
		///   Applies the Aspose license, if available.
		/// </summary>
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

		/// <summary>
		///   Some basic tests to ensure that the Line class is working correctly.
		/// </summary>
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

		/// <summary>
		///   Some basic tests to ensure that StringTokenCollection class works properly.
		/// </summary>
		[Test]
		[Order(0)]
		public void TextSplitTest()
		{
			// ReSharper disable line StringLiteralTypo
			var splitTestText = "\n   This is a     tést of\nthe sssssssssssssstring   splitting code\n\n  .";
			var tokens = new StringTokenCollection(splitTestText).ToList();
			Assert.AreEqual(20, tokens.Count);
			var tenthToken = tokens[9];
			var fourteenthToken = tokens[13];
			Assert.IsTrue(tenthToken.String == "\n" && tenthToken.Type == StringToken.TokenType.LineBreak);
			Assert.IsTrue(fourteenthToken.String == "   " && fourteenthToken.Type == StringToken.TokenType.Whitespace);
		}

		/// <summary>
		///   Some basic tests to ensure that the Angle class works properly.
		/// </summary>
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

			var turned90Clockwise = reversedRadians.Rotate90(true);
			Assert.AreEqual(Math.PI * (1.0 / 6.0), turned90Clockwise.Value, GeometricTolerance);
			var turned90Anticlockwise = reversedRadians.Rotate90(false);
			Assert.AreEqual(Math.PI * (7.0 / 6.0), turned90Anticlockwise.Value, GeometricTolerance);
		}

		/// <summary>
		///   Some basic tests to make sure that the PageSize class works properly.
		/// </summary>
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

		/// <summary>
		///   Applies a long banner watermark at a jaunty angle.
		/// </summary>
		[Test]
		[Order(2)]
		public void ApplyPageEdgeWatermark()
		{
			var testPdfPath = Path.Join(TestPdfsFolder, DifferentPageSizes);
			var outputPdfPath = Path.Join(OutputPdfsFolder, "ReversedPageEdgeWatermark.pdf");
			var watermarkFont = new Font("Helvetica", FontStyles.Italic, new Size(.025f, Sizing.RelativeToDiagonalSize));
			var watermarkAppearance = new Appearance(Color.Red, 0.6f, watermarkFont);
			var pageEdgeWatermark = new PageEdgeTextWatermark(
				"This is a page edge watermark test that I hope\nwill execute successfully.",
				watermarkAppearance,
				PageEdgePosition.Bottom, Justification.Centre, Fitting.Wrap | Fitting.Shrink | Fitting.Grow,
				new Size(0.03f, Sizing.RelativeToAverageSideLength), Offset.None, true);

			AspidiftraUtil.WatermarkPdf(testPdfPath, new[] {pageEdgeWatermark}, outputPdfPath);
		}

		[Test]
		[Order(2)]
		public void ApplyBannerWatermark()
		{
			var testPdfPath = Path.Join(TestPdfsFolder, DifferentPageSizes);
			var outputPdfPath = Path.Join(OutputPdfsFolder, "LongBannerWatermark.pdf");
			var watermarkFont = new Font("Helvetica", FontStyles.Regular, new Size(.035f, Sizing.RelativeToDiagonalSize));
			var watermarkAppearance = new Appearance(Color.Green, 0.6f, watermarkFont);
			var bannerWatermark = new BannerTextWatermark(
				"This is a banner watermark test that I sincerely hope\nwill execute successfully, mainly because the maths involved was bloody difficult, and I haven't done any of that sort of thing since school.",
				watermarkAppearance, Justification.Centre, Fitting.Wrap | Fitting.Shrink,
				new Size(0.08f, Sizing.RelativeToAverageSideLength),
				new CustomBannerAngle(new Angle(123.4, AngleUnits.Degrees)), Offset.None);

			AspidiftraUtil.WatermarkPdf(testPdfPath, new[] {bannerWatermark}, outputPdfPath);
		}

		[Test]
		[Order(2)]
		public void ApplyBothTypesOfWatermark()
		{
			var testPdfPath = Path.Join(TestPdfsFolder, LoremIpsum);
			var outputPdfPath = Path.Join(OutputPdfsFolder, "BothTypesOfWatermark.pdf");
			var impactFont = new Font("Impact", FontStyles.Bold, new Size(.045f, Sizing.RelativeToShorterSide));
			var courierFont = new Font("Courier New", FontStyles.Regular, new Size(.025f, Sizing.RelativeToWidth));
			var blueVioletImpact = new Appearance(Color.BlueViolet, 0.75f, impactFont); // 75% opacity
			var maroonCourier = new Appearance(Color.Maroon, 0.5f, courierFont); // 50% opacity

			var topPageEdgeWatermark = new PageEdgeTextWatermark(
				"This is a page edge watermark with\nan explicit line break in it", // Watermark text
				maroonCourier, // Cosmetic appearance of the text
				PageEdgePosition.Top, // Where to place the watermark
				Justification.Left, // Justification of text
				Fitting.Wrap, // Permitted fitting constraints
				new Size(0.025f, Sizing.RelativeToDiagonalSize), // Margin
				Offset.None);

			var rightPageEdgeWatermark = new PageEdgeTextWatermark(
				"This is a page edge watermark that has got a huge amount of text " +
				"in it, and so we will probably find that it needs to be wrapped " +
				"across multiple lines",
				maroonCourier, // Cosmetic appearance of the text
				PageEdgePosition.Right, // Where to place the watermark
				Justification.Left, // Justification of text
				Fitting.Wrap, // Permitted fitting constraints
				new Size(0.01f, Sizing.RelativeToDiagonalSize), // Margin
				Offset.None);

			var bannerWatermark = new BannerTextWatermark(
				"This banner text also has lots and lots and lots and lots and lots " +
				"of text in it and will undoubtedly not all fit on one line",
				blueVioletImpact, // Cosmetic appearance of the text
				Justification.Centre, // Justification
				Fitting.Wrap | Fitting.Shrink | Fitting.Grow, // Permitted fitting constraints.
				new Size(0.08f, Sizing.RelativeToAverageSideLength), // Margin
				BannerTextWatermark.BottomLeftToTopRightAngle, // Angle of banner
				Offset.None);

			using var doc = new AspidiftraDocument(testPdfPath);
			doc.ApplyWatermark(bannerWatermark);
			doc.ApplyWatermark(topPageEdgeWatermark);
			doc.ApplyWatermark(rightPageEdgeWatermark);
			doc.Save(outputPdfPath);
		}

		/// <summary>
		///   Applies a banner watermark to each page of a 720 page document,
		///   rotating it by 0.5 degrees each time.
		/// </summary>
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
				var angle = (n - 1) * 0.5;
				var requiredPageNumbers = new[] {n}.ToImmutableHashSet();
				var bannerWatermark = new BannerTextWatermark(
					$"Banner at {angle} degrees",
					watermarkAppearance, Justification.Centre, Fitting.Shrink | Fitting.Grow,
					new Size(0.08f, Sizing.RelativeToAverageSideLength),
					new CustomBannerAngle(new Angle(angle, AngleUnits.Degrees)),
					new Offset(50, 50),
					pages => requiredPageNumbers);
				watermarks.Add(bannerWatermark);
			}

			AspidiftraUtil.WatermarkPdf(testPdfPath, watermarks, outputPdfPath);
		}

		/// <summary>
		///   Applies a banner watermark with a MASSIVE font size.
		///   Overflow is allowed.
		/// </summary>
		[Test]
		[Order(2)]
		public void HugeOverflowBannerWatermark()
		{
			var testPdfPath = Path.Join(TestPdfsFolder, LoremIpsum);
			var outputPdfPath = Path.Join(OutputPdfsFolder, "HugeOverflowBanner.pdf");
			var watermarkFont = new Font("Impact", FontStyles.Bold, new Size(0.66f, Sizing.RelativeToWidth));
			var watermarkAppearance = new Appearance(Color.Purple, 0.6f, watermarkFont);
			var watermarks = new List<IWatermark>();

			var bannerWatermark = new BannerTextWatermark(
				"BOOM SHAKALAK\nBOOM SHAKALAK\nBOOM SHAKALAK\nBOOM SHAKALAK\nBOOM SHAKALAK\nBOOM SHAKALAK",
				watermarkAppearance,
				Justification.Centre,
				Fitting.Overflow,
				new Size(0.04f, Sizing.RelativeToAverageSideLength),
				BannerTextWatermark.BottomLeftToTopRightAngle,
				Offset.None);
			watermarks.Add(bannerWatermark);

			AspidiftraUtil.WatermarkPdf(testPdfPath, watermarks, outputPdfPath);
		}

		/// <summary>
		///   Applies a page edge watermark with a MASSIVE font size.
		///   Overflow is allowed.
		/// </summary>
		[Test]
		[Order(2)]
		public void HugeOverflowPageEdgeWatermark()
		{
			var testPdfPath = Path.Join(TestPdfsFolder, LoremIpsum);
			var outputPdfPath = Path.Join(OutputPdfsFolder, "HugeOverflowPageEdge.pdf");
			var watermarkFont = new Font("Impact", FontStyles.Bold, new Size(0.66f, Sizing.RelativeToWidth));
			var watermarkAppearance = new Appearance(Color.Purple, 0.6f, watermarkFont);
			var watermarks = new List<IWatermark>();

			var pageEdgeWatermark = new PageEdgeTextWatermark(
				"Pagans, Saints and Sages: The Angels Of Our Ages",
				watermarkAppearance,
				PageEdgePosition.Top,
				Justification.Right,
				Fitting.Overflow,
				new Size(0.04f, Sizing.RelativeToAverageSideLength),
				Offset.None);
			watermarks.Add(pageEdgeWatermark);
			AspidiftraUtil.WatermarkPdf(testPdfPath, watermarks, outputPdfPath);
		}

		/// <summary>
		///   Applies a page edge watermark with a MASSIVE font size.
		///   Overflow is allowed.
		/// </summary>
		[Test]
		[Order(2)]
		public void LotsOfLinesOverflowPageEdgeWatermark()
		{
			var testPdfPath = Path.Join(TestPdfsFolder, LoremIpsum);
			var outputPdfPath = Path.Join(OutputPdfsFolder, "LotsOfLinesOverflowPageEdge.pdf");
			var watermarkFont = new Font("Impact", FontStyles.Bold, new Size(0.06f, Sizing.RelativeToHeight));
			var watermarkAppearance = new Appearance(Color.Purple, 0.6f, watermarkFont);
			var watermarks = new List<IWatermark>();

			var pageEdgeWatermark = new PageEdgeTextWatermark(SeenAndNotSeenLyrics,
				watermarkAppearance,
				PageEdgePosition.Top,
				Justification.Left,
				Fitting.Overflow,
				new Size(0.04f, Sizing.RelativeToAverageSideLength),
				Offset.None);
			watermarks.Add(pageEdgeWatermark);
			AspidiftraUtil.WatermarkPdf(testPdfPath, watermarks, outputPdfPath);
		}

		/// <summary>
		///   Applies a page edge watermark with a MASSIVE font size.
		///   Overflow is allowed.
		/// </summary>
		[Test]
		[Order(2)]
		public void LotsOfLinesBannerWatermark()
		{
			var testPdfPath = Path.Join(TestPdfsFolder, LoremIpsum);
			var outputPdfPath = Path.Join(OutputPdfsFolder, "LotsOfLinesOverflowBanner.pdf");
			var watermarkFont = new Font("Impact", FontStyles.Bold, new Size(0.06f, Sizing.RelativeToHeight));
			var watermarkAppearance = new Appearance(Color.Purple, 0.6f, watermarkFont);
			var watermarks = new List<IWatermark>();

			var doc = new Document(testPdfPath);
			var pageEdgeWatermark = new BannerTextWatermark(SeenAndNotSeenLyrics,
				watermarkAppearance,
				Justification.Left,
				Fitting.Overflow,
				new Size(0.04f, Sizing.RelativeToAverageSideLength),
				new CustomBannerAngle(new Angle(12.3, AngleUnits.Degrees)),
				Offset.None);
			watermarks.Add(pageEdgeWatermark);
			AspidiftraUtil.WatermarkPdf(doc, watermarks);
			doc.Save(outputPdfPath);
		}

		[Test]
		[Order(2)]
		public void GetDocumentSize()
		{
			var testPdfPath = Path.Join(TestPdfsFolder, Square720Pages);
			new AspidiftraDocument(testPdfPath);
		}

		[Test]
		[Order(2)]
		public void BookmarkTest()
		{
			var paths = new[]
			{
				Path.Join(TestPdfsFolder, LoremIpsum),
				Path.Join(TestPdfsFolder, FivePages),
				Path.Join(TestPdfsFolder, Bookmarks)
			};
			var outputPath = Path.Join(OutputPdfsFolder, "BookmarkConcatenationTest.pdf");
			var pdfPaths = paths.ToImmutableList();
			if (!pdfPaths.Any())
				throw new ArgumentException("At least one PDF must be provided.", nameof(paths));
			var concatenatePdfPaths = pdfPaths.TakeLast(pdfPaths.Count - 1);
			// Open first document
			var pdfsToDispose = new List<Document>();
			var pageOffset = 0;
			var bookmarks = new List<Bookmarks>();
			using (var originalPdf = new Document(pdfPaths.First()))
			{
				pageOffset += originalPdf.Pages.Count;

				// Stick the others on, in order.
				foreach (var nextPdfPath in concatenatePdfPaths)
				{
					Bookmarks? nextPdfBookmarks;
					// Create PdfBookmarkEditor
					using (var bookmarkEditor = new PdfBookmarkEditor())
					{
						// Open PDF file
						bookmarkEditor.BindPdf(nextPdfPath);
						// Extract bookmarks
						nextPdfBookmarks = bookmarkEditor.ExtractBookmarks(true);
					}

					var nextPdf = new Document(nextPdfPath);
					pdfsToDispose.Add(nextPdf);
					originalPdf.Pages.Add(nextPdf.Pages);
					if (nextPdfBookmarks != null)
					{
						foreach (var bookmark in nextPdfBookmarks)
						{
							bookmark.PageNumber += pageOffset;
							foreach (var subBookmark in bookmark.ChildItems)
								subBookmark.PageNumber += pageOffset;
						}

						bookmarks.Add(nextPdfBookmarks);
					}

					pageOffset += nextPdf.Pages.Count;
				}

				var infoStrings = new Dictionary<string, string>();
				infoStrings["Author"] = "Bobcat Goldthwaite";
				infoStrings["Subject"] = "Nothing much";
				foreach (var (key, value) in infoStrings)
					originalPdf.Info[key] = value;

				originalPdf.Save(outputPath);
			}

			foreach (var pdf in pdfsToDispose)
				pdf.Dispose();

			using (var bookmarkEditor = new PdfBookmarkEditor())
			{
				// Bind PDF document
				bookmarkEditor.BindPdf(outputPath);
				// Create bookmarks
				foreach (var subBookmark in bookmarks.SelectMany(b => b))
					bookmarkEditor.CreateBookmarks(subBookmark);
				// Save updated document
				bookmarkEditor.Save(outputPath);
			}
		}
	}
}