using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Aspidiftra;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using NUnit.Framework;
using Color = System.Drawing.Color;
using Font = Aspidiftra.Font;

namespace AspidiftraTest
{
	public class AspidiftraTests
	{
		/// <summary>
		///   Acceptable amount to be "off by" for floating point operations.
		/// </summary>
		private const double AcceptableVariance = 0.0000001;

		private const string TestPdfsFolder = "..\\..\\..\\TestPDFs";
		private const string OutputPdfsFolder = "..\\..\\..\\OutputPDFs";
		private const string AsposeLicenseFolder = "..\\..\\..\\AsposeLicense";
		private const string AsposeLicenseFilename = "Aspose.Total.lic.xml";
		private const string ConcatenatedPdfFilename = "ConcatenatedTestFiles.pdf";

		private const string Simple = "01_Simple_1191_842.pdf";
		private const string CropBox = "02_CropBox_800_400.pdf";
		private const string NegativeVerticalCoordinateSpace = "03_NegativeVerticalCoordinateSpace_1600_2400.pdf";
		private const string NegativeHorizontalCoordinateSpace = "04_NegativeHorizontalCoordinateSpace_1600_2400.pdf";

		private const string NegativeHorizontalAndVerticalCoordinateSpace =
			"05_NegativeHorizontalAndVerticalCoordinateSpace_1600_2400.pdf";

		private const string Portrait = "06_Portrait_750_1200.pdf";
		private const string FivePages = "07_FivePages_841.98_595.38.pdf";
		private const string DigitalSignature = "08_DigitalSignature_612_792.pdf";

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
		public void AngleTests()
		{
			var checkNormalizationDegrees1 = new Angle(700.0, AngleUnits.Degrees);
			Assert.AreEqual(340.0, checkNormalizationDegrees1.Value, AcceptableVariance);
			var checkNormalizationDegrees2 = new Angle(-1000000.0, AngleUnits.Degrees);
			Assert.AreEqual(80.0, checkNormalizationDegrees2.Value, AcceptableVariance);
			var checkNormalizationRadians1 = new Angle(Math.PI * (7.0 / 3.0), AngleUnits.Radians);
			Assert.AreEqual(Math.PI * (1.0 / 3.0), checkNormalizationRadians1.Value, AcceptableVariance);
			var checkNormalizationRadians2 = new Angle(-Math.PI * (10000.0 / 3.0), AngleUnits.Radians);
			Assert.AreEqual(Math.PI * (2.0 / 3.0), checkNormalizationRadians2.Value, AcceptableVariance);

			var thirtyDegrees = new Angle(30.0, AngleUnits.Degrees);
			var xReversedThirtyDegrees = thirtyDegrees.ReverseX();
			Assert.AreEqual(330.0, xReversedThirtyDegrees.Value, AcceptableVariance);
			var yReversedThirtyDegrees = thirtyDegrees.ReverseY();
			Assert.AreEqual(150.0, yReversedThirtyDegrees.Value, AcceptableVariance);
			var reversedThirtyDegrees = thirtyDegrees.Reverse();
			Assert.AreEqual(210.0, reversedThirtyDegrees.Value, AcceptableVariance);

			// 300 degrees.
			var largeRadiansAngle = new Angle(Math.PI * (5.0 / 3.0), AngleUnits.Radians);
			var xReversedRadians = largeRadiansAngle.ReverseX();
			Assert.AreEqual(Math.PI * (1.0 / 3.0), xReversedRadians.Value, AcceptableVariance);
			var yReversedRadians = largeRadiansAngle.ReverseY();
			Assert.AreEqual(Math.PI * (4.0 / 3.0), yReversedRadians.Value, AcceptableVariance);
			var reversedRadians = largeRadiansAngle.Reverse();
			Assert.AreEqual(Math.PI * (2.0 / 3.0), reversedRadians.Value, AcceptableVariance);
		}

		[Test]
		[Order(0)]
		public void PageSizeFunctions()
		{
			const double rectWidth = 10.0;
			const double rectHeight = 20.0;
			var diagonalAngleRadians = Math.Atan(rectHeight / rectWidth);
			var diagonalAngleDegrees = diagonalAngleRadians * 180.0 / Math.PI;
			var rect = new Rectangle(0, 0, rectWidth, rectHeight);
			Assert.AreEqual(Math.Sqrt(rectWidth * rectWidth + rectHeight * rectHeight), rect.DiagonalLength(),
				AcceptableVariance);
			Assert.AreEqual(rectWidth + rectHeight / 2.0, rect.AverageSideLength(), AcceptableVariance);
			Assert.AreEqual(rectHeight, rect.LongestSideLength());
			Assert.AreEqual(rectWidth, rect.ShortestSideLength());
			Assert.AreEqual(diagonalAngleDegrees, rect.DiagonalAngle());
		}

		[Test]
		[Order(0)]
		public void Concatenate()
		{
			var outputPdfPath = Path.Join(OutputPdfsFolder, ConcatenatedPdfFilename);
			var testPdfPaths = Directory.GetFiles(TestPdfsFolder)
				.Select(Path.GetFullPath)
				.ToImmutableList();
			AspidiftraUtil.ConcatenatePdfs(testPdfPaths, outputPdfPath);

			// TODO: Test the output.
		}

		[Test]
		public void TestFontHeight()
		{
			var testPdfPath = Path.Join(OutputPdfsFolder, ConcatenatedPdfFilename);
			var outputPdfPath = Path.Join(OutputPdfsFolder, "FontHeightTest.pdf");
			var watermarkFont = new Font("Helvetica", FontStyles.Italic, new Size(100.0f, Sizing.Absolute));
			var watermarkAppearance = new Appearance(Color.Red, 0.8f, watermarkFont);
			var pageEdgeWatermark = new FontSizeTestWatermark("This is a page edge watermark test", watermarkAppearance);

			using var aspDoc = new AspidiftraDocument(testPdfPath);
			aspDoc.ApplyWatermarks(new[] {pageEdgeWatermark});
			aspDoc.Save(outputPdfPath);

			// TODO: Test the output, somehow?
		}

		[Test]
		public void ApplyWatermark()
		{
			var testPdfPath = Path.Join(OutputPdfsFolder, ConcatenatedPdfFilename);
			var outputPdfPath = Path.Join(OutputPdfsFolder, "PageEdgeWatermarked.pdf");
			var watermarkFont = new Font("Helvetica", FontStyles.Italic, new Size(.025f, Sizing.RelativeToDiagonalSize));
			var watermarkAppearance = new Appearance(Color.Red, 0.6f, watermarkFont);
			var pageEdgeWatermark = new PageEdgeWatermark("This is a page edge watermark test\nGood, innit?",
				watermarkAppearance,
				PageEdgePosition.West, Justification.Centre, Fitting.Wrap | Fitting.Shrink,
				new Size(0.03f, Sizing.RelativeToAverageSideLength), true);

			using var aspDoc = new AspidiftraDocument(testPdfPath);
			aspDoc.ApplyWatermarks(new[] {pageEdgeWatermark});
			aspDoc.Save(outputPdfPath);

			// TODO: Test the output, somehow?
		}

		[Test]
		public void GenerateDoc()
		{
			var doc = new Document();
			for (var f = 0; f < 5; ++f)
			{
				var page = doc.Pages.Add();
				page.MediaBox = new Rectangle(0, 0, 841.98, 595.38);
				var text = new TextFragment($"FivePages\nPage {f + 1}\nPageSize = (0, 0, 841.98, 595.38)")
				{
					Position = new Position(100, 100)
				};
				page.Paragraphs.Add(text);
			}

			var outputPath = Path.Join(TestPdfsFolder, "16_FivePages_841.98_595.38.pdf");
			doc.Save(outputPath);
		}

		[Test]
		public void OpenDoc()
		{
			var testPdfPath = Path.Join(TestPdfsFolder, "05_NegativeHorizontalAndVerticalCoordinateSpace_1600_2400.pdf");
			var doc = new AspidiftraDocument(testPdfPath);
			// TODO: Test the output, somehow?
		}
	}
}