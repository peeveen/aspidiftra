# aspidiftra
PDF text watermarking library, for use with Aspose PDF.

![Uncle](/media/uncle.jpg)

* Targets .NET Standard 2.0, .NET Framework 4.8, .NET Core 3.1.
* Arbitrary minimum version of Aspose PDF is 20.12. Standard Aspose licensing restrictions apply.
* Currently provides page edge watermarks or banner watermarks.
* All watermark constructors can take an optional `pageSelector` lambda argument, filtering the pages that the watermark appears on.
* Font sizes and margin sizes can be specified as absolute or relative to various page dimensions.
* Custom per-watermark opacity/transparency.
* If you try to apply a watermark that cannot be fit onto the page, an `InsufficientSpaceException` will be thrown unless you use the `Fitting.Overflow` best-fit constraint.
* Position of watermarks can be offset by a user-defined amount. This is applied after all fitting logic.

# Examples
```csharp
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
	Offset.None, // User-defined positional offset
	true); // Reverse the direction of the text

var bannerWatermark = new BannerTextWatermark(
	"This is my banner text.", // Watermark text
	watermarkGreenAppearance, // Cosmetic appearance of the text
	Justification.Centre, // Justification of the text
	Fitting.Wrap | Fitting.Shrink | Fitting.Grow, // Permitted best-fitting constraints
	new Size(0.08f, Sizing.RelativeToAverageSideLength), // Margin
	new CustomBannerAngle(new Angle(123.4, AngleUnits.Degrees)), // Angle of banner
	Offset.None); // User-defined positional offset

var watermarks = new IWatermark[] {pageEdgeWatermark, bannerWatermark};
AspidiftraUtil.WatermarkPdf("C:\\LoremIpsum.pdf", watermarks, "C:\\Watermarked.pdf");
```
![AspidiftraSample1](/media/watermarkedDocument1.png)
```csharp
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
	Offset.None); // User-defined positional offset.

var rightPageEdgeWatermark = new PageEdgeTextWatermark(
	"This is a page edge watermark that has got a huge amount of text " +
	"in it, and so we will probably find that it needs to be wrapped " +
	"across multiple lines",
	maroonCourier, // Cosmetic appearance of the text
	PageEdgePosition.Right, // Where to place the watermark
	Justification.Left, // Justification of text
	Fitting.Wrap, // Permitted fitting constraints
	new Size(0.01f, Sizing.RelativeToDiagonalSize), // Margin
	Offset.None); // User-defined positional offset.

var bannerWatermark = new BannerTextWatermark(
	"This banner text also has lots and lots and lots and lots and lots " +
	"of text in it and will undoubtedly not all fit on one line",
	blueVioletImpact, // Cosmetic appearance of the text
	Justification.Centre, // Justification
	Fitting.Wrap | Fitting.Shrink | Fitting.Grow, // Permitted fitting constraints.
	new Size(0.08f, Sizing.RelativeToAverageSideLength), // Margin
	BannerTextWatermark.BottomLeftToTopRightAngle, // Angle of banner
	Offset.None); // User-defined positional offset.

using var doc = new AspidiftraDocument("C:\\LoremIpsum.pdf");
doc.ApplyWatermark(bannerWatermark);
doc.ApplyWatermark(topPageEdgeWatermark);
doc.ApplyWatermark(rightPageEdgeWatermark);
doc.Save("C:\\Watermarked.pdf");
```
![AspidiftraSample2](/media/watermarkedDocument2.png)
