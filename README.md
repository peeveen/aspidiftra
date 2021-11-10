# aspidiftra
PDF text watermarking library, for use with Aspose PDF.

If you attempt to use this extensively without a valid Aspose license, you will probably hit errors.

# Example usage
```
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
AspidiftraUtil.WatermarkPdf("C:\\LoremIpsum.pdf", watermarks, "C:\\BothWatermarked.pdf");
```

![Aspidiftra](/Media/watermarkedDocument.png?raw=true)

# TODO
* Add a more intelligent mechanism for font size shrinking and growing.
