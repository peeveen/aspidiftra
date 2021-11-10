# aspidiftra
PDF text watermarking library, for use with Aspose PDF.

* Targets .NET Standard 2.0, compatible with most .NET Framework or .NET Core apps.
* Arbitrary minimum version of Aspose PDF is 20.12. Standard Aspose licensing restrictions apply.

# Example usages
```
var fontSize = new Size(.025f, Sizing.RelativeToDiagonalSize);
var watermarkFont = new Font("Helvetica", FontStyles.Italic, fontSize);
var watermarkRedAppearance = new Appearance(Color.Red, 1.0f, watermarkFont);
var watermarkGreenAppearance = new Appearance(Color.Green, 0.8f, watermarkFont);

var pageEdgeWatermark = new PageEdgeTextWatermark(
	"This is a page edge watermark", // Watermark text
	watermarkRedAppearance, // Cosmetic appearance of the text
	PageEdgePosition.Bottom, // Where to place the watermark
	Justification.Centre, // Justification of text
	Fitting.Wrap | Fitting.Shrink, // Permitted best-fitting constraints
	new Size(0.03f, Sizing.RelativeToAverageSideLength), // Margin
	true); // Reverse the direction of the text

var bannerWatermark = new BannerTextWatermark(
	"This is my banner text.", // Watermark text
	watermarkGreenAppearance, // Cosmetic appearance of the text
	Justification.Centre, // Justification of the text
	Fitting.Wrap | Fitting.Shrink | Fitting.Grow, // Permitted best-fitting constraints
	new Size(0.08f, Sizing.RelativeToAverageSideLength), // Margin
	new CustomBannerAngle(new Angle(123.4, AngleUnits.Degrees))); // Angle of banner

var watermarks = new IWatermark[] {pageEdgeWatermark, bannerWatermark};
AspidiftraUtil.WatermarkPdf("C:\\LoremIpsum.pdf", watermarks, "C:\\Watermarked.pdf");
```
![AspidiftraSample1](https://github.com/peeveen/aspidiftra/media/watermarkedDocument1.png)
```
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
	new Size(0.025f, Sizing.RelativeToDiagonalSize)); // Margin

var rightPageEdgeWatermark = new PageEdgeTextWatermark(
	"This is a page edge watermark that has got a huge amount of text " +
	"in it, and so we will probably find that it needs to be wrapped " +
	"across multiple lines",
	maroonCourier, // Cosmetic appearance of the text
	PageEdgePosition.Right, // Where to place the watermark
	Justification.Left, // Justification of text
	Fitting.Wrap, // Permitted fitting constraints
	new Size(0.01f, Sizing.RelativeToDiagonalSize)); // Reverse the direction of the text

var bannerWatermark = new BannerTextWatermark(
	"This banner text also has lots and lots and lots and lots and lots " +
	"of text in it and will undoubtedly not all fit on one line",
	blueVioletImpact, // Cosmetic appearance of the text
	Justification.Centre, // Justification
	Fitting.Wrap | Fitting.Shrink | Fitting.Grow, // Permitted fitting constraints.
	new Size(0.08f, Sizing.RelativeToAverageSideLength), // Margin
	new BottomLeftToTopRightBannerAngle()); // Angle of banner

using var doc = new AspidiftraDocument("C:\\LoremIpsum.pdf");
doc.ApplyWatermark(bannerWatermark);
doc.ApplyWatermark(topPageEdgeWatermark);
doc.ApplyWatermark(rightPageEdgeWatermark);
doc.Save("C:\\Watermarked.pdf");
```
![AspidiftraSample2](https://github.com/peeveen/aspidiftra/media/watermarkedDocument2.png)

# Features
* All watermark constructors can take an optional "page selector" lambda argument, filtering the pages that the watermark appears on.
* Font sizes and margin sizes can be specified as absolute or relative to various page dimensions.
